using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Capa de acceso a datos para la entidad Usuario y Auditoría
    /// </summary>
    public class UsuarioDAL
    {
        private readonly DbHelper _db = DbHelper.Instance;

        #region Consultas y Búsqueda

        /// <summary>
        /// Obtiene un usuario completo (incluyendo credenciales criptográficas) por su nombre de usuario.
        /// Uso restringido para autenticación.
        /// </summary>
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario))
                return null;

            string query = @"
                SELECT ID_Usuario, NombreCompleto, NombreUsuario, PasswordHash, PasswordSalt,
                       HashIterations, HashAlgorithm, Rol, Estado, IntentosFallidos,
                       BloqueadoHasta, FechaCreacion, UltimoAcceso
                FROM Usuarios
                WHERE NombreUsuario = @NombreUsuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@NombreUsuario", nombreUsuario.Trim())
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return MapearUsuarioDesdeDataRow(dt.Rows[0]);
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        public Usuario ObtenerPorId(int idUsuario)
        {
            string query = @"
                SELECT ID_Usuario, NombreCompleto, NombreUsuario, PasswordHash, PasswordSalt,
                       HashIterations, HashAlgorithm, Rol, Estado, IntentosFallidos,
                       BloqueadoHasta, FechaCreacion, UltimoAcceso
                FROM Usuarios
                WHERE ID_Usuario = @ID_Usuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@ID_Usuario", idUsuario)
            };

            DataTable dt = _db.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0)
                return null;

            return MapearUsuarioDesdeDataRow(dt.Rows[0]);
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios sin información sensible (Hash y Salt en blanco)
        /// </summary>
        public List<Usuario> ListarTodos()
        {
            string query = @"
                SELECT ID_Usuario, NombreCompleto, NombreUsuario, '' AS PasswordHash, '' AS PasswordSalt,
                       HashIterations, HashAlgorithm, Rol, Estado, IntentosFallidos,
                       BloqueadoHasta, FechaCreacion, UltimoAcceso
                FROM Usuarios
                ORDER BY NombreCompleto ASC";

            DataTable dt = _db.ExecuteQuery(query);
            var lista = new List<Usuario>();

            foreach (DataRow row in dt.Rows)
            {
                lista.Add(MapearUsuarioDesdeDataRow(row));
            }

            return lista;
        }

        /// <summary>
        /// Verifica si ya existe un nombre de usuario registrado, opcionalmente excluyendo un ID
        /// </summary>
        public bool ExisteNombreUsuario(string nombreUsuario, int idExcluir = 0)
        {
            string query = @"
                SELECT COUNT(1) 
                FROM Usuarios 
                WHERE NombreUsuario = @NombreUsuario AND ID_Usuario != @ID_Excluir";

            SqlParameter[] parameters = {
                _db.CreateParameter("@NombreUsuario", nombreUsuario.Trim()),
                _db.CreateParameter("@ID_Excluir", idExcluir)
            };

            object result = _db.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// Cuenta la cantidad de usuarios con rol Administrador en estado Activo
        /// </summary>
        public int ContarAdministradoresActivos()
        {
            string query = @"
                SELECT COUNT(1) 
                FROM Usuarios 
                WHERE Rol = 'Administrador' AND Estado = 'Activo'";

            object result = _db.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Cuenta el total de usuarios en la base de datos
        /// </summary>
        public int ContarTotalUsuarios()
        {
            string query = "SELECT COUNT(1) FROM Usuarios";
            object result = _db.ExecuteScalar(query);
            return Convert.ToInt32(result);
        }

        #endregion

        #region Operaciones de Escritura y Modificación

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos
        /// </summary>
        public int Insertar(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            string query = @"
                INSERT INTO Usuarios (
                    NombreCompleto, NombreUsuario, PasswordHash, PasswordSalt,
                    HashIterations, HashAlgorithm, Rol, Estado, IntentosFallidos,
                    BloqueadoHasta, FechaCreacion, UltimoAcceso
                )
                VALUES (
                    @NombreCompleto, @NombreUsuario, @PasswordHash, @PasswordSalt,
                    @HashIterations, @HashAlgorithm, @Rol, @Estado, @IntentosFallidos,
                    @BloqueadoHasta, GETDATE(), NULL
                );
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = {
                _db.CreateParameter("@NombreCompleto", usuario.NombreCompleto.Trim()),
                _db.CreateParameter("@NombreUsuario", usuario.NombreUsuario.Trim()),
                _db.CreateParameter("@PasswordHash", usuario.PasswordHash),
                _db.CreateParameter("@PasswordSalt", usuario.PasswordSalt),
                _db.CreateParameter("@HashIterations", usuario.HashIterations),
                _db.CreateParameter("@HashAlgorithm", usuario.HashAlgorithm ?? "PBKDF2-SHA256"),
                _db.CreateParameter("@Rol", usuario.Rol),
                _db.CreateParameter("@Estado", usuario.Estado),
                _db.CreateParameter("@IntentosFallidos", usuario.IntentosFallidos),
                _db.CreateParameter("@BloqueadoHasta", usuario.BloqueadoHasta.HasValue ? (object)usuario.BloqueadoHasta.Value : DBNull.Value)
            };

            object result = _db.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Actualiza los datos generales (Nombre, Rol, Estado) de un usuario
        /// </summary>
        public bool Actualizar(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            string query = @"
                UPDATE Usuarios
                SET NombreCompleto = @NombreCompleto,
                    Rol = @Rol,
                    Estado = @Estado
                WHERE ID_Usuario = @ID_Usuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@NombreCompleto", usuario.NombreCompleto.Trim()),
                _db.CreateParameter("@Rol", usuario.Rol),
                _db.CreateParameter("@Estado", usuario.Estado),
                _db.CreateParameter("@ID_Usuario", usuario.ID_Usuario)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Actualiza las credenciales criptográficas de un usuario
        /// </summary>
        public bool ActualizarPassword(int idUsuario, string passwordHash, string passwordSalt, int iteraciones, string algoritmo)
        {
            string query = @"
                UPDATE Usuarios
                SET PasswordHash = @PasswordHash,
                    PasswordSalt = @PasswordSalt,
                    HashIterations = @HashIterations,
                    HashAlgorithm = @HashAlgorithm,
                    IntentosFallidos = 0,
                    BloqueadoHasta = NULL
                WHERE ID_Usuario = @ID_Usuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@PasswordHash", passwordHash),
                _db.CreateParameter("@PasswordSalt", passwordSalt),
                _db.CreateParameter("@HashIterations", iteraciones),
                _db.CreateParameter("@HashAlgorithm", algoritmo),
                _db.CreateParameter("@ID_Usuario", idUsuario)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Actualiza la fecha de último acceso y resetea los contadores de bloqueo tras un inicio exitoso
        /// </summary>
        public void ActualizarAccesoExitoso(int idUsuario)
        {
            string query = @"
                UPDATE Usuarios
                SET UltimoAcceso = GETDATE(),
                    IntentosFallidos = 0,
                    BloqueadoHasta = NULL,
                    Estado = CASE WHEN Estado = 'Bloqueado' THEN 'Activo' ELSE Estado END
                WHERE ID_Usuario = @ID_Usuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@ID_Usuario", idUsuario)
            };

            _db.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Registra un intento fallido y actualiza el estado de bloqueo si supera el límite
        /// </summary>
        public void RegistrarIntentoFallido(int idUsuario, int intentos, DateTime? bloqueadoHasta, string nuevoEstado)
        {
            string query = @"
                UPDATE Usuarios
                SET IntentosFallidos = @IntentosFallidos,
                    BloqueadoHasta = @BloqueadoHasta,
                    Estado = @Estado
                WHERE ID_Usuario = @ID_Usuario";

            SqlParameter[] parameters = {
                _db.CreateParameter("@IntentosFallidos", intentos),
                _db.CreateParameter("@BloqueadoHasta", bloqueadoHasta.HasValue ? (object)bloqueadoHasta.Value : DBNull.Value),
                _db.CreateParameter("@Estado", nuevoEstado),
                _db.CreateParameter("@ID_Usuario", idUsuario)
            };

            _db.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Cambia el estado de un usuario a Inactivo
        /// </summary>
        public bool Desactivar(int idUsuario)
        {
            string query = "UPDATE Usuarios SET Estado = 'Inactivo' WHERE ID_Usuario = @ID_Usuario";
            SqlParameter[] parameters = { _db.CreateParameter("@ID_Usuario", idUsuario) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        /// <summary>
        /// Cambia el estado de un usuario a Activo y limpia posibles bloqueos
        /// </summary>
        public bool Activar(int idUsuario)
        {
            string query = @"
                UPDATE Usuarios 
                SET Estado = 'Activo', 
                    IntentosFallidos = 0, 
                    BloqueadoHasta = NULL 
                WHERE ID_Usuario = @ID_Usuario";
            SqlParameter[] parameters = { _db.CreateParameter("@ID_Usuario", idUsuario) };
            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        #endregion

        #region Auditoría

        /// <summary>
        /// Inserta un registro de auditoría de seguridad
        /// </summary>
        public void RegistrarAuditoria(int? idUsuario, string nombreUsuario, string accion, string detalles)
        {
            try
            {
                string equipo = Environment.MachineName;
                string query = @"
                    INSERT INTO AuditoriaUsuarios (ID_Usuario, NombreUsuario, Accion, Detalles, Fecha, Equipo)
                    VALUES (@ID_Usuario, @NombreUsuario, @Accion, @Detalles, GETDATE(), @Equipo)";

                SqlParameter[] parameters = {
                    _db.CreateParameter("@ID_Usuario", idUsuario.HasValue ? (object)idUsuario.Value : DBNull.Value),
                    _db.CreateParameter("@NombreUsuario", nombreUsuario ?? "Desconocido"),
                    _db.CreateParameter("@Accion", accion),
                    _db.CreateParameter("@Detalles", detalles ?? (object)DBNull.Value),
                    _db.CreateParameter("@Equipo", equipo)
                };

                _db.ExecuteNonQuery(query, parameters);
            }
            catch
            {
                // La auditoría no debe interrumpir el flujo principal de la aplicación en caso de error puntual
            }
        }

        #endregion

        #region Helpers

        private Usuario MapearUsuarioDesdeDataRow(DataRow row)
        {
            return new Usuario
            {
                ID_Usuario = Convert.ToInt32(row["ID_Usuario"]),
                NombreCompleto = Convert.ToString(row["NombreCompleto"]),
                NombreUsuario = Convert.ToString(row["NombreUsuario"]),
                PasswordHash = Convert.ToString(row["PasswordHash"]),
                PasswordSalt = Convert.ToString(row["PasswordSalt"]),
                HashIterations = row["HashIterations"] != DBNull.Value ? Convert.ToInt32(row["HashIterations"]) : 100000,
                HashAlgorithm = row["HashAlgorithm"] != DBNull.Value ? Convert.ToString(row["HashAlgorithm"]) : "PBKDF2-SHA256",
                Rol = Convert.ToString(row["Rol"]),
                Estado = Convert.ToString(row["Estado"]),
                IntentosFallidos = row["IntentosFallidos"] != DBNull.Value ? Convert.ToInt32(row["IntentosFallidos"]) : 0,
                BloqueadoHasta = row["BloqueadoHasta"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["BloqueadoHasta"]) : null,
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                UltimoAcceso = row["UltimoAcceso"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["UltimoAcceso"]) : null
            };
        }

        #endregion
    }
}
