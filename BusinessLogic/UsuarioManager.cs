using System;
using System.Collections.Generic;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.BusinessLogic
{
    /// <summary>
    /// Capa de lógica de negocio para la gestión de usuarios, seguridad y control de acceso
    /// </summary>
    public class UsuarioManager
    {
        private readonly UsuarioDAL _usuarioDAL;
        private const int MaxIntentosFallidos = 5;
        private const int MinutosBloqueo = 15;

        public UsuarioManager()
        {
            _usuarioDAL = new UsuarioDAL();
        }

        public UsuarioManager(UsuarioDAL dal)
        {
            _usuarioDAL = dal ?? new UsuarioDAL();
        }

        #region Autenticación

        /// <summary>
        /// Realiza la autenticación de un usuario validando credenciales, estado y bloqueos temporales
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <param name="password">Contraseña en texto claro</param>
        /// <param name="mensajeError">Mensaje explicativo en caso de fallo</param>
        /// <returns>Usuario autenticado si fue exitoso, null de lo contrario</returns>
        public Usuario Autenticar(string nombreUsuario, string password, out string mensajeError)
        {
            mensajeError = null;

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
            {
                mensajeError = "Por favor ingrese su usuario y contraseña.";
                return null;
            }

            string userTrim = nombreUsuario.Trim();

            // 1. Buscar usuario en base de datos
            Usuario usuario = _usuarioDAL.ObtenerPorNombreUsuario(userTrim);

            if (usuario == null)
            {
                _usuarioDAL.RegistrarAuditoria(null, userTrim, "IntentoFallido", "Usuario no encontrado.");
                mensajeError = "Usuario o contraseña incorrectos.";
                return null;
            }

            // 2. Verificar si la cuenta está inactiva
            if (string.Equals(usuario.Estado, EstadoUsuario.Inactivo, StringComparison.OrdinalIgnoreCase))
            {
                _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, userTrim, "IntentoFallido", "Cuenta inactiva.");
                mensajeError = "Su cuenta se encuentra inactiva. Contacte al administrador del sistema.";
                return null;
            }

            // 3. Verificar si está bloqueada temporalmente
            if (usuario.EstaBloqueado)
            {
                if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta.Value > DateTime.Now)
                {
                    TimeSpan restante = usuario.BloqueadoHasta.Value - DateTime.Now;
                    int minutos = (int)Math.Ceiling(restante.TotalMinutes);
                    _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, userTrim, "IntentoFallido", $"Cuenta bloqueada temporalmente. Restan {minutos} min.");
                    mensajeError = $"La cuenta está bloqueada temporalmente por intentos fallidos. Intente de nuevo en {minutos} minuto(s).";
                    return null;
                }
            }

            // 4. Verificar contraseña con PBKDF2-HMAC-SHA256
            bool passwordCorrecta = PasswordHasher.VerificarPassword(
                password,
                usuario.PasswordSalt,
                usuario.PasswordHash,
                usuario.HashIterations
            );

            if (!passwordCorrecta)
            {
                int nuevosIntentos = usuario.IntentosFallidos + 1;
                DateTime? bloqueadoHasta = null;
                string nuevoEstado = usuario.Estado;

                if (nuevosIntentos >= MaxIntentosFallidos)
                {
                    bloqueadoHasta = DateTime.Now.AddMinutes(MinutosBloqueo);
                    nuevoEstado = EstadoUsuario.Bloqueado;
                    mensajeError = $"Ha superado el número máximo de intentos ({MaxIntentosFallidos}). La cuenta ha sido bloqueada por {MinutosBloqueo} minutos.";
                    _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, userTrim, "BloqueoCuenta", $"Bloqueado por {MinutosBloqueo} min tras {nuevosIntentos} intentos.");
                }
                else
                {
                    int restantes = MaxIntentosFallidos - nuevosIntentos;
                    mensajeError = $"Usuario o contraseña incorrectos. Intentos restantes: {restantes}.";
                    _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, userTrim, "IntentoFallido", $"Contraseña incorrecta. Intento {nuevosIntentos} de {MaxIntentosFallidos}.");
                }

                _usuarioDAL.RegistrarIntentoFallido(usuario.ID_Usuario, nuevosIntentos, bloqueadoHasta, nuevoEstado);
                return null;
            }

            // 5. Autenticación exitosa
            _usuarioDAL.ActualizarAccesoExitoso(usuario.ID_Usuario);
            _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, userTrim, "InicioSesionExitoso", $"Inicio de sesión exitoso. Rol: {usuario.Rol}");

            // Inicializar sesión en memoria
            SesionActual.IniciarSesion(usuario);

            return usuario.ClonarParaVista();
        }

        #endregion

        #region Gestión de Usuarios (CRUD y Administración)

        /// <summary>
        /// Lista todos los usuarios registrados (con hashes omitidos para seguridad)
        /// </summary>
        public List<Usuario> ListarUsuarios()
        {
            return _usuarioDAL.ListarTodos();
        }

        /// <summary>
        /// Obtiene un usuario específico por su ID
        /// </summary>
        public Usuario ObtenerPorId(int idUsuario)
        {
            Usuario u = _usuarioDAL.ObtenerPorId(idUsuario);
            return u?.ClonarParaVista();
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema con su contraseña hasheada
        /// </summary>
        public int CrearUsuario(Usuario usuario, string password)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            string errorValidacion = usuario.GetValidationError();
            if (!string.IsNullOrEmpty(errorValidacion))
                throw new ArgumentException(errorValidacion);

            if (!PasswordHasher.EsPasswordSegura(password, out string errorPassword))
                throw new ArgumentException(errorPassword);

            if (_usuarioDAL.ExisteNombreUsuario(usuario.NombreUsuario))
                throw new InvalidOperationException($"El nombre de usuario '{usuario.NombreUsuario}' ya está en uso. Por favor elija otro.");

            // Hashing criptográfico
            string salt = PasswordHasher.GenerarSalt();
            string hash = PasswordHasher.HashearPassword(password, salt, PasswordHasher.DefaultIterations);

            usuario.PasswordSalt = salt;
            usuario.PasswordHash = hash;
            usuario.HashIterations = PasswordHasher.DefaultIterations;
            usuario.HashAlgorithm = PasswordHasher.AlgorithmName;
            usuario.Estado = EstadoUsuario.Activo;
            usuario.IntentosFallidos = 0;
            usuario.BloqueadoHasta = null;

            int nuevoId = _usuarioDAL.Insertar(usuario);
            _usuarioDAL.RegistrarAuditoria(nuevoId, usuario.NombreUsuario, "UsuarioCreado", $"Usuario creado con rol {usuario.Rol}. Operador: {SesionActual.NombreUsuario}");

            return nuevoId;
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente, protegiendo al último administrador activo
        /// </summary>
        public void ActualizarUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            if (usuario.ID_Usuario <= 0)
                throw new ArgumentException("ID de usuario inválido.");

            string errorValidacion = usuario.GetValidationError();
            if (!string.IsNullOrEmpty(errorValidacion))
                throw new ArgumentException(errorValidacion);

            Usuario actual = _usuarioDAL.ObtenerPorId(usuario.ID_Usuario);
            if (actual == null)
                throw new InvalidOperationException("El usuario especificado no existe.");

            // Protección crítica: Si es el último administrador activo, no permitir cambiarle el rol a Empleado ni desactivarlo
            if (actual.EsAdministrador && actual.EstaActivo)
            {
                int totalAdmins = _usuarioDAL.ContarAdministradoresActivos();
                if (totalAdmins <= 1)
                {
                    if (!usuario.EsAdministrador)
                        throw new InvalidOperationException("No se puede cambiar el rol del único Administrador activo del sistema.");

                    if (!usuario.EstaActivo)
                        throw new InvalidOperationException("No se puede desactivar o bloquear al único Administrador activo del sistema.");
                }
            }

            bool ok = _usuarioDAL.Actualizar(usuario);
            if (!ok)
                throw new Exception("No se pudo actualizar el usuario.");

            _usuarioDAL.RegistrarAuditoria(usuario.ID_Usuario, usuario.NombreUsuario, "UsuarioModificado", $"Datos modificados. Nuevo rol: {usuario.Rol}, Estado: {usuario.Estado}. Operador: {SesionActual.NombreUsuario}");
        }

        /// <summary>
        /// Cambia o restablece la contraseña de un usuario
        /// </summary>
        public void CambiarPassword(int idUsuario, string nuevaPassword, string passwordActual = null, bool esAdminRestableciendo = false)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("ID de usuario inválido.");

            if (!PasswordHasher.EsPasswordSegura(nuevaPassword, out string errorPass))
                throw new ArgumentException(errorPass);

            Usuario usuario = _usuarioDAL.ObtenerPorId(idUsuario);
            if (usuario == null)
                throw new InvalidOperationException("El usuario no existe.");

            // Si no es un administrador restableciendo la clave, verificar la clave actual
            if (!esAdminRestableciendo)
            {
                if (string.IsNullOrEmpty(passwordActual))
                    throw new ArgumentException("Debe ingresar la contraseña actual para verificar su identidad.");

                bool valida = PasswordHasher.VerificarPassword(
                    passwordActual,
                    usuario.PasswordSalt,
                    usuario.PasswordHash,
                    usuario.HashIterations
                );

                if (!valida)
                    throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");
            }

            string nuevoSalt = PasswordHasher.GenerarSalt();
            string nuevoHash = PasswordHasher.HashearPassword(nuevaPassword, nuevoSalt, PasswordHasher.DefaultIterations);

            bool ok = _usuarioDAL.ActualizarPassword(idUsuario, nuevoHash, nuevoSalt, PasswordHasher.DefaultIterations, PasswordHasher.AlgorithmName);
            if (!ok)
                throw new Exception("Error al actualizar la contraseña.");

            string detalle = esAdminRestableciendo 
                ? $"Contraseña restablecida por el Administrador: {SesionActual.NombreUsuario}"
                : "Contraseña cambiada por el propio usuario.";

            _usuarioDAL.RegistrarAuditoria(idUsuario, usuario.NombreUsuario, "ContrasenaCambiada", detalle);
        }

        /// <summary>
        /// Desactiva una cuenta de usuario, impidiendo que sea el último administrador activo
        /// </summary>
        public void DesactivarUsuario(int idUsuario)
        {
            Usuario actual = _usuarioDAL.ObtenerPorId(idUsuario);
            if (actual == null)
                throw new InvalidOperationException("El usuario especificado no existe.");

            if (actual.EsAdministrador && actual.EstaActivo)
            {
                int totalAdmins = _usuarioDAL.ContarAdministradoresActivos();
                if (totalAdmins <= 1)
                    throw new InvalidOperationException("Acción denegada: No se puede desactivar al único Administrador activo del sistema.");
            }

            bool ok = _usuarioDAL.Desactivar(idUsuario);
            if (!ok)
                throw new Exception("No se pudo desactivar el usuario.");

            _usuarioDAL.RegistrarAuditoria(idUsuario, actual.NombreUsuario, "UsuarioDesactivado", $"Cuenta desactivada por: {SesionActual.NombreUsuario}");
        }

        /// <summary>
        /// Reactiva una cuenta de usuario previamente desactivada o bloqueada
        /// </summary>
        public void ActivarUsuario(int idUsuario)
        {
            Usuario actual = _usuarioDAL.ObtenerPorId(idUsuario);
            if (actual == null)
                throw new InvalidOperationException("El usuario especificado no existe.");

            bool ok = _usuarioDAL.Activar(idUsuario);
            if (!ok)
                throw new Exception("No se pudo activar el usuario.");

            _usuarioDAL.RegistrarAuditoria(idUsuario, actual.NombreUsuario, "UsuarioActivado", $"Cuenta activada por: {SesionActual.NombreUsuario}");
        }

        #endregion

        #region Inicialización y Asistente de Primer Arranque

        /// <summary>
        /// Verifica si existe al menos un Administrador activo en el sistema
        /// </summary>
        public bool ExisteAdministradorActivo()
        {
            return _usuarioDAL.ContarAdministradoresActivos() > 0;
        }

        /// <summary>
        /// Registra el primer administrador del sistema. Sólo se permite cuando la base de datos no tiene administradores activos.
        /// Evita la necesidad de contraseñas fijas o credenciales predeterminadas.
        /// </summary>
        public int CrearPrimerAdministrador(string nombreCompleto, string nombreUsuario, string password)
        {
            if (ExisteAdministradorActivo())
                throw new InvalidOperationException("Ya existe un Administrador activo registrado en el sistema. Debe iniciar sesión normalmente.");

            var admin = new Usuario
            {
                NombreCompleto = nombreCompleto,
                NombreUsuario = nombreUsuario,
                Rol = RolUsuario.Administrador,
                Estado = EstadoUsuario.Activo
            };

            return CrearUsuario(admin, password);
        }

        #endregion
    }
}
