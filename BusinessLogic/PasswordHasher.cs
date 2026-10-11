using System;
using System.Security.Cryptography;
using System.Text;

namespace MiniSistemaFacturacion.BusinessLogic
{
    /// <summary>
    /// Servicio criptográfico para hashing y verificación segura de contraseñas
    /// Utiliza PBKDF2-HMAC-SHA256 con Salt aleatorio y comparación en tiempo constante.
    /// </summary>
    public static class PasswordHasher
    {
        public const int DefaultIterations = 100000;
        public const int SaltByteSize = 32;       // 256 bits
        public const int HashByteSize = 32;       // 256 bits
        public const string AlgorithmName = "PBKDF2-SHA256";

        /// <summary>
        /// Genera un salt criptográficamente seguro codificado en Base64
        /// </summary>
        public static string GenerarSalt(int byteSize = SaltByteSize)
        {
            byte[] saltBytes = new byte[byteSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Calcula el hash PBKDF2-HMAC-SHA256 de una contraseña a partir de su salt e iteraciones
        /// </summary>
        public static string HashearPassword(string password, string saltBase64, int iteraciones = DefaultIterations)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía.", nameof(password));

            if (string.IsNullOrEmpty(saltBase64))
                throw new ArgumentException("El salt no puede estar vacío.", nameof(saltBase64));

            byte[] saltBytes = Convert.FromBase64String(saltBase64);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iteraciones, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = deriveBytes.GetBytes(HashByteSize);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verifica si una contraseña ingresada coincide con el hash almacenado, usando comparación en tiempo constante
        /// </summary>
        public static bool VerificarPassword(string passwordIngresada, string saltBase64, string hashAlmacenadoBase64, int iteraciones = DefaultIterations)
        {
            if (string.IsNullOrEmpty(passwordIngresada) || string.IsNullOrEmpty(saltBase64) || string.IsNullOrEmpty(hashAlmacenadoBase64))
                return false;

            try
            {
                byte[] saltBytes = Convert.FromBase64String(saltBase64);
                byte[] hashEsperado = Convert.FromBase64String(hashAlmacenadoBase64);

                byte[] hashCalculado;
                using (var deriveBytes = new Rfc2898DeriveBytes(passwordIngresada, saltBytes, iteraciones, HashAlgorithmName.SHA256))
                {
                    hashCalculado = deriveBytes.GetBytes(hashEsperado.Length);
                }

                return CompararTiempoConstante(hashCalculado, hashEsperado);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Comparación segura en tiempo constante de dos arreglos de bytes para mitigar ataques de temporización (timing attacks)
        /// </summary>
        public static bool CompararTiempoConstante(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }

        /// <summary>
        /// Valida las políticas mínimas de seguridad de una contraseña
        /// </summary>
        public static bool EsPasswordSegura(string password, out string mensajeError)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                mensajeError = "La contraseña no puede estar vacía.";
                return false;
            }

            if (password.Length < 6)
            {
                mensajeError = "La contraseña debe tener al menos 6 caracteres.";
                return false;
            }

            mensajeError = null;
            return true;
        }
    }
}
