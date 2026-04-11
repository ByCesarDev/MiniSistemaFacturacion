using System;
using System.Configuration;

namespace MiniSistemaFacturacion.Configuration
{
    /// <summary>
    /// Clase para manejar la configuración de la empresa y datos fiscales
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public class EmpresaConfig
    {
        #region Properties

        /// <summary>
        /// Nombre de la empresa
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Dirección de la empresa
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// Teléfono de la empresa
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Email de la empresa
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// RNC de la empresa
        /// </summary>
        public string RNC { get; set; }

        /// <summary>
        /// NCF actual para facturas fiscales
        /// </summary>
        public string NCFActual { get; set; }

        /// <summary>
        /// NCF de inicio para consumidor final
        /// </summary>
        public string NCFConsumidorFinal { get; set; }

        /// <summary>
        /// Ruta donde se guardarán los PDFs
        /// </summary>
        public string RutaPdfTickets { get; set; }

        #endregion

        #region Singleton

        private static EmpresaConfig _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtiene la instancia única de EmpresaConfig (Singleton)
        /// </summary>
        public static EmpresaConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EmpresaConfig();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor privado que carga la configuración desde App.config
        /// </summary>
        private EmpresaConfig()
        {
            CargarConfiguracion();
        }

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Carga la configuración desde el archivo App.config
        /// </summary>
        private void CargarConfiguracion()
        {
            try
            {
                Nombre = GetConfigValue("EmpresaNombre", "Mi Empresa S.A.");
                Direccion = GetConfigValue("EmpresaDireccion", "Dirección no configurada");
                Telefono = GetConfigValue("EmpresaTelefono", "Teléfono no configurado");
                Email = GetConfigValue("EmpresaEmail", "email@empresa.com");
                RNC = GetConfigValue("EmpresaRNC", "123456789");
                NCFActual = GetConfigValue("NCFActual", "01010000001");
                NCFConsumidorFinal = GetConfigValue("NCFConsumidorFinal", "B0100000001");
                RutaPdfTickets = GetConfigValue("RutaPdfTickets", "./TicketsPDF/");
            }
            catch (Exception ex)
            {
                // Si hay error al cargar configuración, usar valores por defecto
                SetValoresPorDefecto();
            }
        }

        /// <summary>
        /// Obtiene un valor de configuración del App.config
        /// </summary>
        /// <param name="key">Clave de configuración</param>
        /// <param name="defaultValue">Valor por defecto si no existe</param>
        /// <returns>Valor de configuración</returns>
        private string GetConfigValue(string key, string defaultValue)
        {
            try
            {
                return ConfigurationManager.AppSettings[key] ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Establece valores por defecto en caso de error
        /// </summary>
        private void SetValoresPorDefecto()
        {
            Nombre = "Mi Empresa S.A.";
            Direccion = "Dirección no configurada";
            Telefono = "Teléfono no configurado";
            Email = "email@empresa.com";
            RNC = "123456789";
            NCFActual = "01010000001";
            NCFConsumidorFinal = "B0100000001";
            RutaPdfTickets = "./TicketsPDF/";
        }

        #endregion

        #region NCF Management

        /// <summary>
        /// Genera el siguiente NCF según el tipo de comprobante
        /// </summary>
        /// <param name="tipoComprobante">Tipo de comprobante (01, 02, 03, 14, 15, 16)</param>
        /// <returns>Siguiente NCF disponible</returns>
        public string GenerarSiguienteNCF(string tipoComprobante)
        {
            try
            {
                string prefijo = ObtenerPrefijoNCF(tipoComprobante);
                
                // Para consumidor final (02), usar secuencia B
                if (tipoComprobante == "02")
                {
                    return GenerarSiguienteNCFConsumidorFinal();
                }

                // Para otros tipos, usar secuencia normal
                string ncfActual = GetConfigValue($"NCF{tipoComprobante}", $"{prefijo}000000001");
                return IncrementarNCF(ncfActual);
            }
            catch
            {
                // Si hay error, retornar NCF por defecto
                return tipoComprobante == "02" ? "B0100000001" : "01010000001";
            }
        }

        /// <summary>
        /// Obtiene el prefijo NCF según el tipo de comprobante
        /// </summary>
        /// <param name="tipoComprobante">Tipo de comprobante</param>
        /// <returns>Prefijo NCF</returns>
        private string ObtenerPrefijoNCF(string tipoComprobante)
        {
            switch (tipoComprobante)
            {
                case "01": return "01010000001"; // Crédito Fiscal
                case "02": return "B0100000001";  // Consumidor Final
                case "03": return "13010000001"; // Regímenes Especiales
                case "14": return "11010000001"; // Gubernamental
                case "15": return "31010000001"; // Nota de Débito
                case "16": return "32010000001"; // Nota de Crédito
                default: return "01010000001";   // Por defecto Crédito Fiscal
            }
        }

        /// <summary>
        /// Genera el siguiente NCF para consumidor final
        /// </summary>
        /// <returns>Siguiente NCF de consumidor final</returns>
        private string GenerarSiguienteNCFConsumidorFinal()
        {
            string ncfActual = NCFConsumidorFinal;
            string siguiente = IncrementarNCF(ncfActual);
            NCFConsumidorFinal = siguiente;
            return siguiente;
        }

        /// <summary>
        /// Incrementa el número secuencial de un NCF
        /// </summary>
        /// <param name="ncfActual">NCF actual</param>
        /// <returns>NCF incrementado</returns>
        private string IncrementarNCF(string ncfActual)
        {
            if (string.IsNullOrWhiteSpace(ncfActual) || ncfActual.Length < 9)
                return ncfActual;

            try
            {
                // Extraer el número secuencial (últimos 8 dígitos)
                string prefijo = ncfActual.Substring(0, ncfActual.Length - 8);
                string numeroStr = ncfActual.Substring(ncfActual.Length - 8);
                
                if (int.TryParse(numeroStr, out int numero))
                {
                    numero++;
                    return prefijo + numero.ToString("D8");
                }
            }
            catch
            {
                // Si hay error, retornar el mismo NCF
            }

            return ncfActual;
        }

        /// <summary>
        /// Valida si un NCF tiene formato válido
        /// </summary>
        /// <param name="ncf">NCF a validar</param>
        /// <returns>True si es válido</returns>
        public bool ValidarNCF(string ncf)
        {
            if (string.IsNullOrWhiteSpace(ncf))
                return false;

            // Validar longitud (11 caracteres para NCF estándar)
            if (ncf.Length != 11)
                return false;

            // Validar que sea numérico
            return long.TryParse(ncf, out _);
        }

        /// <summary>
        /// Obtiene la descripción del tipo de comprobante
        /// </summary>
        /// <param name="tipoComprobante">Tipo de comprobante</param>
        /// <returns>Descripción del tipo</returns>
        public string GetDescripcionTipoComprobante(string tipoComprobante)
        {
            switch (tipoComprobante)
            {
                case "01": return "Crédito Fiscal";
                case "02": return "Consumidor Final";
                case "03": return "Regímenes Especiales";
                case "14": return "Gubernamental";
                case "15": return "Nota de Débito";
                case "16": return "Nota de Crédito";
                default: return "No Especificado";
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Formatea el RNC para mostrar
        /// </summary>
        /// <returns>RNC formateado</returns>
        public string GetFormattedRNC()
        {
            if (string.IsNullOrWhiteSpace(RNC))
                return string.Empty;

            // Formato para RNC de 9 dígitos: XXX-XXXXXXX
            if (RNC.Length == 9 && RNC.All(char.IsDigit))
            {
                return $"{RNC.Substring(0, 3)}-{RNC.Substring(3, 6)}";
            }

            return RNC;
        }

        /// <summary>
        /// Verifica si la configuración está completa
        /// </summary>
        /// <returns>True si la configuración es válida</returns>
        public bool IsConfiguracionValida()
        {
            return !string.IsNullOrWhiteSpace(Nombre) &&
                   !string.IsNullOrWhiteSpace(RNC) &&
                   !string.IsNullOrWhiteSpace(NCFActual) &&
                   !string.IsNullOrWhiteSpace(RutaPdfTickets);
        }

        /// <summary>
        /// Obtiene información completa de la empresa
        /// </summary>
        /// <returns>String con información de la empresa</returns>
        public string GetFullInfo()
        {
            string info = $"{Nombre}\n";
            info += $"{Direccion}\n";
            info += $"Tel: {Telefono}\n";
            info += $"Email: {Email}\n";
            info += $"RNC: {GetFormattedRNC()}";
            
            return info;
        }

        #endregion
    }
}
