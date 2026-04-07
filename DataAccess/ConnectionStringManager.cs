using System;
using System.Configuration;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase para gestionar las cadenas de conexión de la aplicación
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public static class ConnectionStringManager
    {
        #region Constants

        private const string DEFAULT_CONNECTION_NAME = "MiniSistemaFacturacionDB";
        private const string DEFAULT_CONNECTION_STRING = @"Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;Connect Timeout=30;";

        #endregion

        #region Connection String Methods

        /// <summary>
        /// Obtiene la cadena de conexión desde el archivo de configuración
        /// </summary>
        /// <returns>Cadena de conexión configurada</returns>
        public static string GetConnectionString()
        {
            try
            {
                // Intentar obtener desde App.config
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[DEFAULT_CONNECTION_NAME];
                
                if (settings != null && !string.IsNullOrEmpty(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                // Log error si hay problema con el archivo de configuración
                System.Diagnostics.Debug.WriteLine($"Error de configuración: {ex.Message}");
            }

            // Retornar cadena por defecto si no se encuentra en configuración
            return DEFAULT_CONNECTION_STRING;
        }

        /// <summary>
        /// Obtiene una cadena de conexión específica por nombre
        /// </summary>
        /// <param name="connectionName">Nombre de la conexión</param>
        /// <returns>Cadena de conexión específica</returns>
        public static string GetConnectionString(string connectionName)
        {
            try
            {
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[connectionName];
                
                if (settings != null && !string.IsNullOrEmpty(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de configuración para '{connectionName}': {ex.Message}");
            }

            throw new ArgumentException($"No se encontró la cadena de conexión '{connectionName}'");
        }

        #endregion

        #region Builder Methods

        /// <summary>
        /// Construye una cadena de conexión para SQL Server
        /// </summary>
        /// <param name="server">Nombre del servidor</param>
        /// <param name="database">Nombre de la base de datos</param>
        /// <param name="useIntegratedSecurity">Usar seguridad integrada</param>
        /// <param name="username">Usuario (si no usa seguridad integrada)</param>
        /// <param name="password">Contraseña (si no usa seguridad integrada)</param>
        /// <param name="connectTimeout">Timeout de conexión (segundos)</param>
        /// <returns>Cadena de conexión construida</returns>
        public static string BuildConnectionString(string server, string database, 
            bool useIntegratedSecurity = true, string username = "", string password = "", int connectTimeout = 30)
        {
            if (string.IsNullOrEmpty(server))
                throw new ArgumentException("El nombre del servidor es requerido");
            
            if (string.IsNullOrEmpty(database))
                throw new ArgumentException("El nombre de la base de datos es requerido");

            System.Data.SqlClient.SqlConnectionStringBuilder builder = 
                new System.Data.SqlClient.SqlConnectionStringBuilder();

            builder["Server"] = server;
            builder["Database"] = database;
            builder["Connect Timeout"] = connectTimeout;
            
            if (useIntegratedSecurity)
            {
                builder["Integrated Security"] = true;
            }
            else
            {
                if (string.IsNullOrEmpty(username))
                    throw new ArgumentException("El usuario es requerido cuando no se usa seguridad integrada");
                
                builder["Integrated Security"] = false;
                builder["User ID"] = username;
                builder["Password"] = password;
            }

            // Configuraciones adicionales recomendadas
            builder["Application Name"] = "MiniSistemaFacturacion";
            builder["MultipleActiveResultSets"] = true;
            builder["TrustServerCertificate"] = true;

            return builder.ConnectionString;
        }

        /// <summary>
        /// Construye una cadena de conexión para SQL Server Express
        /// </summary>
        /// <param name="instanceName">Nombre de la instancia (default: SQLEXPRESS)</param>
        /// <param name="database">Nombre de la base de datos</param>
        /// <param name="useIntegratedSecurity">Usar seguridad integrada</param>
        /// <param name="username">Usuario (opcional)</param>
        /// <param name="password">Contraseña (opcional)</param>
        /// <returns>Cadena de conexión para SQL Server Express</returns>
        public static string BuildExpressConnectionString(string instanceName = "SQLEXPRESS", 
            string database = "MiniSistemaFacturacion", bool useIntegratedSecurity = true, 
            string username = "", string password = "")
        {
            string server = $@"localhost\{instanceName}";
            return BuildConnectionString(server, database, useIntegratedSecurity, username, password);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida si una cadena de conexión es válida
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a validar</param>
        /// <returns>True si la cadena es válida</returns>
        public static bool IsValidConnectionString(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                    return false;

                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                
                // Validar componentes esenciales
                if (string.IsNullOrEmpty(builder["Server"]?.ToString()) || 
                    string.IsNullOrEmpty(builder["Database"]?.ToString()))
                {
                    return false;
                }

                // Si no usa seguridad integrada, validar usuario y contraseña
                if (!Convert.ToBoolean(builder["Integrated Security"] ?? false))
                {
                    if (string.IsNullOrEmpty(builder["User ID"]?.ToString()))
                        return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extrae información de una cadena de conexión
        /// </summary>
        /// <param name="connectionString">Cadena de conexión</param>
        /// <returns>Objeto con información extraída</returns>
        public static ConnectionInfo ExtractConnectionInfo(string connectionString)
        {
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                
                return new ConnectionInfo
                {
                    Server = builder["Server"]?.ToString(),
                    Database = builder["Database"]?.ToString(),
                    UseIntegratedSecurity = Convert.ToBoolean(builder["Integrated Security"] ?? false),
                    Username = builder["User ID"]?.ToString(),
                    ConnectTimeout = Convert.ToInt32(builder["Connect Timeout"] ?? 30),
                    ApplicationName = builder["Application Name"]?.ToString(),
                    MultipleActiveResultSets = Convert.ToBoolean(builder["MultipleActiveResultSets"] ?? false)
                };
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error al analizar la cadena de conexión: {ex.Message}", ex);
            }
        }

        #endregion

        #region Environment Detection

        /// <summary>
        /// Determina si estamos en entorno de desarrollo
        /// </summary>
        /// <returns>True si es entorno de desarrollo</returns>
        public static bool IsDevelopmentEnvironment()
        {
            #if DEBUG
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Obtiene la cadena de conexión apropiada según el entorno
        /// </summary>
        /// <returns>Cadena de conexión del entorno actual</returns>
        public static string GetEnvironmentConnectionString()
        {
            if (IsDevelopmentEnvironment())
            {
                // En desarrollo, intentar usar SQL Server Express local
                return BuildExpressConnectionString();
            }
            else
            {
                // En producción, usar la configuración del archivo
                return GetConnectionString();
            }
        }

        #endregion
    }

    /// <summary>
    /// Clase para almacenar información extraída de una cadena de conexión
    /// </summary>
    public class ConnectionInfo
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string Username { get; set; }
        public int ConnectTimeout { get; set; }
        public string ApplicationName { get; set; }
        public bool MultipleActiveResultSets { get; set; }

        /// <summary>
        /// Obtiene una representación segura de la información (sin contraseñas)
        /// </summary>
        /// <returns>Información de conexión segura</returns>
        public override string ToString()
        {
            return $"Server: {Server}, Database: {Database}, " +
                   $"Auth: {(UseIntegratedSecurity ? "Integrated" : $"User: {Username}")}, " +
                   $"Timeout: {ConnectTimeout}s, MARS: {MultipleActiveResultSets}";
        }
    }
}
