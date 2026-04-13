using System;
using System.Text.RegularExpressions;

namespace MiniSistemaFacturacion.Utils
{
    /// <summary>
    /// Utilidad para validación de RNC (Registro Nacional de Contribuyentes)
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public static class RNCValidator
    {
        /// <summary>
        /// Valida si un RNC tiene el formato correcto
        /// </summary>
        /// <param name="rnc">RNC a validar</param>
        /// <returns>True si el RNC es válido</returns>
        public static bool IsValidFormat(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc))
                return false;

            // Limpiar el RNC (quitar guiones, espacios)
            string rncLimpio = rnc.Replace("-", "").Replace(" ", "").Trim();

            // El RNC debe tener entre 9 y 11 dígitos para El Salvador
            if (rncLimpio.Length < 9 || rncLimpio.Length > 11)
                return false;

            // Verificar que todos los caracteres sean dígitos
            if (!Regex.IsMatch(rncLimpio, @"^\d+$"))
                return false;

            return true;
        }

        /// <summary>
        /// Valida el dígito verificador del RNC (algoritmo de módulo 10)
        /// </summary>
        /// <param name="rnc">RNC a validar</param>
        /// <returns>True si el dígito verificador es correcto</returns>
        public static bool IsValidCheckDigit(string rnc)
        {
            if (!IsValidFormat(rnc))
                return false;

            string rncLimpio = rnc.Replace("-", "").Replace(" ", "").Trim();

            try
            {
                // Para RNC de 9 dígitos (personas naturales)
                if (rncLimpio.Length == 9)
                {
                    return ValidateNaturalPersonRNC(rncLimpio);
                }
                // Para RNC de 14 dígitos (personas jurídicas)
                else if (rncLimpio.Length == 14)
                {
                    return ValidateLegalEntityRNC(rncLimpio);
                }
                // Para otros formatos, solo validar formato básico
                else
                {
                    return true; // Aceptar otros formatos válidos
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida RNC de persona natural (9 dígitos)
        /// </summary>
        private static bool ValidateNaturalPersonRNC(string rnc)
        {
            if (rnc.Length != 9)
                return false;

            // Algoritmo de validación para RNC de persona natural
            int[] multiplicadores = { 9, 8, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < 8; i++)
            {
                suma += int.Parse(rnc[i].ToString()) * multiplicadores[i];
            }

            int modulo = suma % 11;
            int resultado = 11 - modulo;

            if (resultado == 10)
                resultado = 0;
            else if (resultado == 11)
                resultado = 1;

            return resultado == int.Parse(rnc[8].ToString());
        }

        /// <summary>
        /// Valida RNC de persona jurídica (14 dígitos)
        /// </summary>
        private static bool ValidateLegalEntityRNC(string rnc)
        {
            if (rnc.Length != 14)
                return false;

            // Algoritmo de validación para RNC de persona jurídica
            int[] multiplicadores = { 7, 6, 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int suma = 0;

            for (int i = 0; i < 12; i++)
            {
                suma += int.Parse(rnc[i].ToString()) * multiplicadores[i];
            }

            int modulo = suma % 11;
            int resultado = 11 - modulo;

            if (resultado == 10)
                resultado = 0;
            else if (resultado == 11)
                resultado = 1;

            return resultado == int.Parse(rnc[12].ToString());
        }

        /// <summary>
        /// Formatea un RNC agregando guiones según el formato estándar
        /// </summary>
        /// <param name="rnc">RNC a formatear</param>
        /// <returns>RNC formateado</returns>
        public static string FormatRNC(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc))
                return string.Empty;

            string rncLimpio = rnc.Replace("-", "").Replace(" ", "").Trim();

            if (rncLimpio.Length == 9)
            {
                return $"{rncLimpio.Substring(0, 3)}-{rncLimpio.Substring(3, 3)}-{rncLimpio.Substring(3, 3)}-{rncLimpio.Substring(6, 3)}";
            }
            else if (rncLimpio.Length == 14)
            {
                return $"{rncLimpio.Substring(0, 3)}-{rncLimpio.Substring(3, 3)}-{rncLimpio.Substring(6, 8)}-{rncLimpio.Substring(14, 1)}";
            }

            return rncLimpio;
        }

        /// <summary>
        /// Obtiene un mensaje de error descriptivo para un RNC inválido
        /// </summary>
        /// <param name="rnc">RNC a validar</param>
        /// <returns>Mensaje de error o string vacío si es válido</returns>
        public static string GetValidationMessage(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc))
                return "El RNC es requerido para clientes de Crédito Fiscal.";

            string rncLimpio = rnc.Replace("-", "").Replace(" ", "").Trim();

            if (rncLimpio.Length < 9 || rncLimpio.Length > 14)
                return "El RNC debe tener entre 9 y 14 dígitos.";

            if (!Regex.IsMatch(rncLimpio, @"^\d+$"))
                return "El RNC solo puede contener números.";

            if (!IsValidCheckDigit(rnc))
                return "El dígito verificador del RNC es incorrecto.";

            return string.Empty;
        }

        /// <summary>
        /// Valida completamente un RNC (formato y dígito verificador)
        /// </summary>
        /// <param name="rnc">RNC a validar</param>
        /// <returns>True si el RNC es completamente válido</returns>
        public static bool IsValid(string rnc)
        {
            return IsValidFormat(rnc) && IsValidCheckDigit(rnc);
        }
    }
}
