using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase para gestionar las cadenas de conexión de la aplicación con persistencia en ProgramData y DPAPI.
    /// Soporta instalaciones bajo Program Files y perfiles de usuario estándar en Windows.
    /// Edited by: Cesar Reyes
    /// Date: 2026-10-11
    /// </summary>
    public static class ConnectionStringManager
    {
        #region Constants

        private const string DEFAULT_CONNECTION_NAME = "MiniSistemaFacturacionDB";
        private const string DEFAULT_CONNECTION_STRING = @"Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;Connect Timeout=30;";
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("MiniSistemaFacturacion_DPAPI_Entropy_2026");

        #endregion

        #region Storage Locations & ProgramData

        /// <summary>
        /// Obtiene la ruta física del archivo de configuración persistente en CommonApplicationData (ProgramData).
        /// </summary>
        public static string GetProgramDataConfigPath()
        {
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MiniSistemaFacturacion");
            return Path.Combine(folder, "database.config");
        }

        private static string ObtenerCadenaDesdeProgramData()
        {
            try
            {
                string filePath = GetProgramDataConfigPath();
                if (!File.Exists(filePath)) return null;

                byte[] bytes = File.ReadAllBytes(filePath);
                if (bytes == null || bytes.Length == 0) return null;

                // Intentar descifrar con DPAPI máquina
                try
                {
                    byte[] descifrado = ProtectedData.Unprotect(bytes, Entropy, DataProtectionScope.LocalMachine);
                    return Encoding.UTF8.GetString(descifrado);
                }
                catch
                {
                    // Fallback para texto plano si no estaba protegido
                    return Encoding.UTF8.GetString(bytes);
                }
            }
            catch
            {
                return null;
            }
        }

        private static bool GuardarEnProgramData(string connectionString, bool protectWithDpapi, out string error)
        {
            error = null;
            try
            {
                string filePath = GetProgramDataConfigPath();
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                byte[] data;
                if (protectWithDpapi)
                {
                    byte[] plain = Encoding.UTF8.GetBytes(connectionString);
                    data = ProtectedData.Protect(plain, Entropy, DataProtectionScope.LocalMachine);
                }
                else
                {
                    data = Encoding.UTF8.GetBytes(connectionString);
                }

                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch (Exception ex)
            {
                error = $"Error al escribir en almacenamiento persistente ({ex.Message})";
                return false;
            }
        }

        #endregion

        #region Connection String Retrieval

        /// <summary>
        /// Obtiene la cadena de conexión priorizando el almacenamiento en ProgramData protegido con DPAPI
        /// y utilizando App.config como respaldo.
        /// </summary>
        /// <returns>Cadena de conexión configurada</returns>
        public static string GetConnectionString()
        {
            try
            {
                // 1. Intentar obtener desde almacenamiento persistente en ProgramData (protegido con DPAPI)
                string programDataConfig = ObtenerCadenaDesdeProgramData();
                if (!string.IsNullOrEmpty(programDataConfig) && IsValidConnectionString(programDataConfig))
                {
                    return programDataConfig;
                }

                // 2. Intentar obtener desde App.config
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[DEFAULT_CONNECTION_NAME];
                if (settings != null && !string.IsNullOrEmpty(settings.ConnectionString))
                {
                    return settings.ConnectionString;
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error de configuración: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener cadena de conexión: {ex.Message}");
            }

            // Retornar cadena por defecto si no se encuentra en configuración
            return DEFAULT_CONNECTION_STRING;
        }

        /// <summary>
        /// Obtiene una cadena de conexión específica por nombre
        /// </summary>
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
        public static string BuildConnectionString(string server, string database, 
            bool useIntegratedSecurity = true, string username = "", string password = "", int connectTimeout = 30)
        {
            if (string.IsNullOrEmpty(server))
                throw new ArgumentException("El nombre del servidor es requerido");
            
            if (string.IsNullOrEmpty(database))
                throw new ArgumentException("El nombre de la base de datos es requerido");

            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder();

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

            builder["Application Name"] = "MiniSistemaFacturacion";
            builder["MultipleActiveResultSets"] = true;
            builder["TrustServerCertificate"] = true;

            return builder.ConnectionString;
        }

        /// <summary>
        /// Construye una cadena de conexión para SQL Server Express
        /// </summary>
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
        public static bool IsValidConnectionString(string connectionString)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                    return false;

                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                
                if (string.IsNullOrEmpty(builder["Server"]?.ToString()) || 
                    string.IsNullOrEmpty(builder["Database"]?.ToString()))
                {
                    return false;
                }

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

        #region Persistence & Security (DPAPI)

        /// <summary>
        /// Guarda la cadena de conexión priorizando el almacenamiento en ProgramData protegido con DPAPI
        /// (apto para ejecuciones bajo Program Files con usuarios estándar) e intentando sincronizar App.config.
        /// </summary>
        public static bool SaveConnectionString(string connectionString, bool protectWithDpapi, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    errorMessage = "La cadena de conexión no puede ser nula o vacía.";
                    return false;
                }

                // 1. Guardar en almacenamiento seguro ProgramData (independiente de permisos de Program Files)
                string errorProgramData;
                if (!GuardarEnProgramData(connectionString, protectWithDpapi, out errorProgramData))
                {
                    errorMessage = errorProgramData;
                    return false;
                }

                // 2. Intentar sincronizar App.config si el usuario tiene privilegios de escritura
                try
                {
                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    ConnectionStringsSection section = config.ConnectionStrings;

                    if (section.ConnectionStrings[DEFAULT_CONNECTION_NAME] != null)
                    {
                        section.ConnectionStrings[DEFAULT_CONNECTION_NAME].ConnectionString = connectionString;
                        section.ConnectionStrings[DEFAULT_CONNECTION_NAME].ProviderName = "System.Data.SqlClient";
                    }
                    else
                    {
                        section.ConnectionStrings.Add(new ConnectionStringSettings(DEFAULT_CONNECTION_NAME, connectionString, "System.Data.SqlClient"));
                    }

                    if (protectWithDpapi)
                    {
                        if (!section.SectionInformation.IsProtected)
                        {
                            section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                        }
                    }
                    else
                    {
                        if (section.SectionInformation.IsProtected)
                        {
                            section.SectionInformation.UnprotectSection();
                        }
                    }

                    config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("connectionStrings");
                }
                catch
                {
                    // Si falla escribir en App.config (por ejemplo bajo C:\Program Files sin elevación de UAC),
                    // no detenemos la operación porque ProgramData ya persistió los datos de forma autoritativa.
                }

                // 3. Actualizar la instancia de DbHelper activa
                DbHelper.Instance.ReloadConnectionString(connectionString);

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al guardar la configuración: {ex.Message}";
                return false;
            }
        }

        #endregion

        #region SQL Identifier Validation & Escaping

        /// <summary>
        /// Valida si un nombre de base de datos u objeto cumple con las reglas de identificadores de SQL Server
        /// previniendo inyección en instrucciones DDL.
        /// </summary>
        public static bool EsIdentificadorSqlValido(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 128)
                return false;

            // Alfanuméricos, guiones bajos, @, #, $ estándar en identificadores de SQL Server
            return Regex.IsMatch(nombre, @"^[a-zA-Z0-9_@#$]+$");
        }

        /// <summary>
        /// Escapa y delimita de forma segura un identificador de base de datos SQL Server.
        /// </summary>
        public static string EscaparIdentificadorSql(string nombre)
        {
            if (!EsIdentificadorSqlValido(nombre))
                throw new ArgumentException($"El identificador '{nombre}' contiene caracteres no permitidos o longitud inválida.");

            return "[" + nombre.Replace("]", "]]") + "]";
        }

        #endregion

        #region Diagnostic & Testing Methods

        /// <summary>
        /// Sanitiza una cadena de conexión para ocultar contraseñas sensibles en logs o interfaces.
        /// </summary>
        public static string SanitizeConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) return string.Empty;
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                if (!string.IsNullOrEmpty(builder.Password))
                {
                    builder.Password = "********";
                }
                return builder.ConnectionString;
            }
            catch
            {
                return "Server=[Oculto];Database=[Oculto];";
            }
        }

        /// <summary>
        /// Prueba si el servidor SQL responde (conectándose a la base de datos master con timeout corto).
        /// </summary>
        public static bool TestServerConnection(string server, bool useIntegratedSecurity, string username, string password, int timeoutSeconds, out string errorMessage)
        {
            errorMessage = null;
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = "master",
                    ConnectTimeout = timeoutSeconds,
                    IntegratedSecurity = useIntegratedSecurity,
                    TrustServerCertificate = true,
                    ApplicationName = "MiniSistemaFacturacion_Setup"
                };

                if (!useIntegratedSecurity)
                {
                    builder.UserID = username;
                    builder.Password = password;
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                errorMessage = $"Error de SQL Server ({sqlEx.Number}): {sqlEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Prueba conectividad completa hacia la base de datos de destino.
        /// Verifica tanto el servidor como la existencia de la base de datos.
        /// </summary>
        public static bool TestDatabaseConnection(string connectionString, out bool databaseExists, out string errorMessage)
        {
            databaseExists = false;
            errorMessage = null;

            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                string targetDb = builder.InitialCatalog;

                // 1. Probar conectividad al servidor usando master
                var serverBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = Math.Min(builder.ConnectTimeout, 10)
                };

                using (var connServer = new System.Data.SqlClient.SqlConnection(serverBuilder.ConnectionString))
                {
                    connServer.Open();

                    using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT DB_ID(@dbName)", connServer))
                    {
                        cmd.Parameters.AddWithValue("@dbName", targetDb);
                        object result = cmd.ExecuteScalar();
                        databaseExists = (result != null && result != DBNull.Value);
                    }
                }

                if (databaseExists)
                {
                    using (var connDb = new System.Data.SqlClient.SqlConnection(connectionString))
                    {
                        connDb.Open();
                    }
                }

                return true;
            }
            catch (System.Data.SqlClient.SqlException sqlEx)
            {
                errorMessage = $"Error de SQL ({sqlEx.Number}): {sqlEx.Message}";
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
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

        public override string ToString()
        {
            return $"Server: {Server}, Database: {Database}, " +
                   $"Auth: {(UseIntegratedSecurity ? "Integrated" : $"User: {Username}")}, " +
                   $"Timeout: {ConnectTimeout}s, MARS: {MultipleActiveResultSets}";
        }
    }
}
