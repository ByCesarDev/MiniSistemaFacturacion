using System;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Modelo representativo de la entidad Usuario en el sistema
    /// </summary>
    public class Usuario
    {
        public int ID_Usuario { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }

        // Credenciales criptográficas (solo para capas internas DAL / Seguridad)
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int HashIterations { get; set; } = 100000;
        public string HashAlgorithm { get; set; } = "PBKDF2-SHA256";

        public string Rol { get; set; } = RolUsuario.Empleado;
        public string Estado { get; set; } = EstadoUsuario.Activo;
        public int IntentosFallidos { get; set; } = 0;
        public DateTime? BloqueadoHasta { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? UltimoAcceso { get; set; }

        #region Propiedades Calculadas

        public bool EsAdministrador => string.Equals(Rol, RolUsuario.Administrador, StringComparison.OrdinalIgnoreCase);

        public bool EsEmpleado => string.Equals(Rol, RolUsuario.Empleado, StringComparison.OrdinalIgnoreCase);

        public bool EstaActivo => string.Equals(Estado, EstadoUsuario.Activo, StringComparison.OrdinalIgnoreCase);

        public bool EstaBloqueado
        {
            get
            {
                if (string.Equals(Estado, EstadoUsuario.Bloqueado, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (BloqueadoHasta.HasValue && BloqueadoHasta.Value > DateTime.Now)
                    return true;

                return false;
            }
        }

        #endregion

        #region Constructores

        public Usuario()
        {
        }

        public Usuario(string nombreCompleto, string nombreUsuario, string rol = RolUsuario.Empleado)
        {
            NombreCompleto = nombreCompleto;
            NombreUsuario = nombreUsuario;
            Rol = rol;
            Estado = EstadoUsuario.Activo;
            FechaCreacion = DateTime.Now;
        }

        #endregion

        #region Métodos de Validación y Seguridad

        public bool IsValid()
        {
            return string.IsNullOrEmpty(GetValidationError());
        }

        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
                return "El nombre completo es obligatorio.";

            if (NombreCompleto.Trim().Length < 3)
                return "El nombre completo debe tener al menos 3 caracteres.";

            if (string.IsNullOrWhiteSpace(NombreUsuario))
                return "El nombre de usuario es obligatorio.";

            if (NombreUsuario.Trim().Length < 3)
                return "El nombre de usuario debe tener al menos 3 caracteres.";

            if (NombreUsuario.Trim().Length > 50)
                return "El nombre de usuario no puede exceder los 50 caracteres.";

            if (!RolUsuario.EsValido(Rol))
                return $"El rol especificado '{Rol}' no es válido.";

            if (!EstadoUsuario.EsValido(Estado))
                return $"El estado especificado '{Estado}' no es válido.";

            return null;
        }

        /// <summary>
        /// Crea una copia del usuario sin información sensible (Hash ni Salt) para uso seguro en vistas y formularios
        /// </summary>
        public Usuario ClonarParaVista()
        {
            return new Usuario
            {
                ID_Usuario = this.ID_Usuario,
                NombreCompleto = this.NombreCompleto,
                NombreUsuario = this.NombreUsuario,
                PasswordHash = null,
                PasswordSalt = null,
                HashIterations = this.HashIterations,
                HashAlgorithm = this.HashAlgorithm,
                Rol = this.Rol,
                Estado = this.Estado,
                IntentosFallidos = this.IntentosFallidos,
                BloqueadoHasta = this.BloqueadoHasta,
                FechaCreacion = this.FechaCreacion,
                UltimoAcceso = this.UltimoAcceso
            };
        }

        public override string ToString()
        {
            return $"{NombreCompleto} ({NombreUsuario}) - {Rol}";
        }

        #endregion
    }
}
