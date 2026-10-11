using System;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.Configuration
{
    /// <summary>
    /// Gestiona el estado de la sesión del usuario actualmente autenticado en la aplicación
    /// </summary>
    public static class SesionActual
    {
        private static Usuario _usuarioActual;

        /// <summary>
        /// Usuario autenticado en la sesión activa (sin datos criptográficos)
        /// </summary>
        public static Usuario UsuarioLogueado => _usuarioActual;

        /// <summary>
        /// Indica si hay un usuario autenticado válidamente
        /// </summary>
        public static bool EsAutenticado => _usuarioActual != null;

        /// <summary>
        /// Indica si el usuario autenticado tiene rol de Administrador
        /// </summary>
        public static bool EsAdministrador => EsAutenticado && _usuarioActual.EsAdministrador;

        /// <summary>
        /// Indica si el usuario autenticado tiene rol de Empleado
        /// </summary>
        public static bool EsEmpleado => EsAutenticado && _usuarioActual.EsEmpleado;

        /// <summary>
        /// Nombre completo del usuario en sesión
        /// </summary>
        public static string NombreCompleto => _usuarioActual?.NombreCompleto ?? "Invitado";

        /// <summary>
        /// Nombre de usuario del usuario en sesión
        /// </summary>
        public static string NombreUsuario => _usuarioActual?.NombreUsuario ?? "invitado";

        /// <summary>
        /// Rol del usuario en sesión
        /// </summary>
        public static string Rol => _usuarioActual?.Rol ?? "Sin Rol";

        /// <summary>
        /// Momento exacto en que se inició la sesión
        /// </summary>
        public static DateTime? HoraInicioSesion { get; private set; }

        /// <summary>
        /// Bandera que indica si el usuario solicitó cerrar sesión para volver a la pantalla de Login
        /// </summary>
        public static bool RequiereReinicioSesion { get; set; }

        /// <summary>
        /// Inicializa la sesión con los datos del usuario autenticado
        /// </summary>
        public static void IniciarSesion(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            // Guardamos una copia sin hash ni salt para máxima seguridad en memoria
            _usuarioActual = usuario.ClonarParaVista();
            HoraInicioSesion = DateTime.Now;
            RequiereReinicioSesion = false;
        }

        /// <summary>
        /// Finaliza la sesión actual y limpia la información de memoria
        /// </summary>
        public static void CerrarSesion(bool volverALogin = false)
        {
            _usuarioActual = null;
            HoraInicioSesion = null;
            RequiereReinicioSesion = volverALogin;
        }

        /// <summary>
        /// Valida si el usuario actual tiene permisos de Administrador; de lo contrario lanza excepción de seguridad
        /// </summary>
        public static void ExigirAdministrador()
        {
            if (!EsAdministrador)
            {
                throw new UnauthorizedAccessException("Operación restringida: Se requieren privilegios de Administrador para realizar esta acción.");
            }
        }
    }
}
