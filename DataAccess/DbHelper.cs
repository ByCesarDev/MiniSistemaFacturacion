using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase base para la gestión de conexiones a la base de datos SQL Server
    /// Implementa el patrón Singleton para optimizar el uso de conexiones
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class DbHelper
    {
        #region Singleton Pattern

        private static DbHelper _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtiene la instancia única de DbHelper (Singleton)
        /// </summary>
        public static DbHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DbHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Connection Management

        private string _connectionString;

        /// <summary>
        /// Constructor privado (Singleton pattern)
        /// </summary>
        private DbHelper()
        {
            _connectionString = GetConnectionString();
        }

        /// <summary>
        /// Obtiene la cadena de conexión desde el archivo de configuración
        /// </summary>
        /// <returns>Cadena de conexión a SQL Server</returns>
        private string GetConnectionString()
        {
            try
            {
                // Intentar obtener desde App.config
                string connectionString = ConfigurationManager.ConnectionStrings["MiniSistemaFacturacionDB"]?.ConnectionString;

                if (string.IsNullOrEmpty(connectionString))
                {
                    // Cadena de conexión por defecto para desarrollo
                    connectionString = @"Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;Connect Timeout=30;";
                }

                return connectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la cadena de conexión: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Crea una nueva conexión a la base de datos
        /// </summary>
        /// <returns>SqlConnection abierta</returns>
        public SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al establecer conexión con la base de datos: " + ex.Message, ex);
            }
        }

        #endregion

        #region Execute Methods

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un conjunto de resultados
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros SQL (opcional)</param>
        /// <returns>DataTable con los resultados</returns>
        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar consulta: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ejecuta una consulta SQL que devuelve un valor escalar
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parámetros SQL (opcional)</param>
        /// <returns>Valor escalar</returns>
        public object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar consulta escalar: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ejecuta un comando SQL que no devuelve resultados (INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Comando SQL</param>
        /// <param name="parameters">Parámetros SQL (opcional)</param>
        /// <returns>Número de filas afectadas</returns>
        public int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar comando: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve un conjunto de resultados
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento (opcional)</param>
        /// <returns>DataTable con los resultados</returns>
        public DataTable ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar procedimiento almacenado: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado que no devuelve resultados
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento almacenado</param>
        /// <param name="parameters">Parámetros del procedimiento (opcional)</param>
        /// <returns>Número de filas afectadas</returns>
        public int ExecuteStoredProcedureNonQuery(string procedureName, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            using (SqlCommand command = new SqlCommand(procedureName, connection))
            {
                try
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al ejecutar procedimiento almacenado: " + ex.Message, ex);
                }
            }
        }

        #endregion

        #region Transaction Management

        /// <summary>
        /// Ejecuta múltiples comandos dentro de una transacción
        /// </summary>
        /// <param name="queries">Array de comandos SQL</param>
        /// <param name="parametersArray">Array de parámetros para cada comando</param>
        /// <returns>Array con el número de filas afectadas por cada comando</returns>
        public int[] ExecuteTransaction(string[] queries, SqlParameter[][] parametersArray = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    int[] results = new int[queries.Length];

                    for (int i = 0; i < queries.Length; i++)
                    {
                        using (SqlCommand command = new SqlCommand(queries[i], connection, transaction))
                        {
                            if (parametersArray != null && parametersArray[i] != null)
                            {
                                command.Parameters.AddRange(parametersArray[i]);
                            }

                            results[i] = command.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return results;
                }
                catch (Exception ex)
                {
                    BusinessLogic.TransaccionHelper.Instance.RollbackSeguro(transaction);
                    throw new Exception("Error en la transacción: " + ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado dentro de una transacción
        /// </summary>
        /// <param name="procedureName">Nombre del procedimiento</param>
        /// <param name="parameters">Parámetros del procedimiento</param>
        /// <returns>Número de filas afectadas</returns>
        public int ExecuteStoredProcedureTransaction(string procedureName, SqlParameter[] parameters = null)
        {
            using (SqlConnection connection = GetConnection())
            {
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand(procedureName, connection, transaction))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        int result = command.ExecuteNonQuery();
                        transaction.Commit();
                        return result;
                    }
                }
                catch (Exception ex)
                {
                    BusinessLogic.TransaccionHelper.Instance.RollbackSeguro(transaction);
                    throw new Exception("Error en la transacción del procedimiento almacenado: " + ex.Message, ex);
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Crea un parámetro SQL
        /// </summary>
        /// <param name="name">Nombre del parámetro</param>
        /// <param name="value">Valor del parámetro</param>
        /// <returns>SqlParameter configurado</returns>
        public SqlParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }

        /// <summary>
        /// Crea un parámetro SQL con tipo específico
        /// </summary>
        /// <param name="name">Nombre del parámetro</param>
        /// <param name="sqlDbType">Tipo de dato SQL</param>
        /// <param name="value">Valor del parámetro</param>
        /// <returns>SqlParameter configurado</returns>
        public SqlParameter CreateParameter(string name, SqlDbType sqlDbType, object value)
        {
            SqlParameter parameter = new SqlParameter(name, sqlDbType);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        /// <summary>
        /// Verifica si la conexión a la base de datos está activa
        /// </summary>
        /// <returns>True si la conexión es exitosa</returns>
        public bool TestConnection()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    return connection.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene información de la versión de SQL Server
        /// </summary>
        /// <returns>Versión del servidor SQL</returns>
        public string GetServerVersion()
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    return connection.ServerVersion;
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        #endregion

        #region Connection String Management

        /// <summary>
        /// Actualiza la cadena de conexión en tiempo de ejecución
        /// </summary>
        /// <param name="newConnectionString">Nueva cadena de conexión</param>
        public void UpdateConnectionString(string newConnectionString)
        {
            if (string.IsNullOrEmpty(newConnectionString))
            {
                throw new ArgumentException("La cadena de conexión no puede ser nula o vacía");
            }

            try
            {
                // Probar la nueva conexión antes de actualizar
                using (SqlConnection testConnection = new SqlConnection(newConnectionString))
                {
                    testConnection.Open();
                }

                // Si la conexión es exitosa, actualizar
                _connectionString = newConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("La nueva cadena de conexión no es válida: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión actual (sin contraseña para logging)
        /// </summary>
        /// <returns>Cadena de conexión segura</returns>
        public string GetSafeConnectionString()
        {
            string safeConnectionString = _connectionString;
            
            // Ocultar información sensible para logging
            if (safeConnectionString.Contains("Password="))
            {
                int startIndex = safeConnectionString.IndexOf("Password=");
                int endIndex = safeConnectionString.IndexOf(";", startIndex);
                if (endIndex == -1) endIndex = safeConnectionString.Length;
                
                safeConnectionString = safeConnectionString.Substring(0, startIndex) + 
                                    "Password=***" + 
                                    safeConnectionString.Substring(endIndex);
            }

            return safeConnectionString;
        }

        #endregion
    }
}
