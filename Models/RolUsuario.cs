using System;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Define los roles permitidos en el sistema
    /// </summary>
    public static class RolUsuario
    {
        public const string Administrador = "Administrador";
        public const string Empleado = "Empleado";

        public static readonly string[] RolesValidos = { Administrador, Empleado };

        public static bool EsValido(string rol)
        {
            if (string.IsNullOrWhiteSpace(rol)) return false;
            return rol.Equals(Administrador, StringComparison.OrdinalIgnoreCase) ||
                   rol.Equals(Empleado, StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// Define los estados posibles para una cuenta de usuario
    /// </summary>
    public static class EstadoUsuario
    {
        public const string Activo = "Activo";
        public const string Inactivo = "Inactivo";
        public const string Bloqueado = "Bloqueado";

        public static readonly string[] EstadosValidos = { Activo, Inactivo, Bloqueado };

        public static bool EsValido(string estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return false;
            return estado.Equals(Activo, StringComparison.OrdinalIgnoreCase) ||
                   estado.Equals(Inactivo, StringComparison.OrdinalIgnoreCase) ||
                   estado.Equals(Bloqueado, StringComparison.OrdinalIgnoreCase);
        }
    }
}
