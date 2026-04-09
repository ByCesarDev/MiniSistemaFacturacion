using System;
using System.Data.SqlClient;

namespace MiniSistemaFacturacion.BusinessLogic
{
    /// <summary>
    /// Clase de utilidad para gestionar transacciones SQL complejas
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class TransaccionHelper
    {
        #region Singleton Pattern

        private static TransaccionHelper _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtiene la instancia única de TransaccionHelper (Singleton)
        /// </summary>
        public static TransaccionHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new TransaccionHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor privado (Singleton pattern)
        /// </summary>
        private TransaccionHelper()
        {
        }

        #endregion

        #region Transaction Management

        /// <summary>
        /// Ejecuta múltiples comandos SQL en una transacción atómica
        /// </summary>
        /// <param name="comandos">Array de comandos SQL a ejecutar</param>
        /// <param name="parametros">Array de parámetros para cada comando</param>
        /// <returns>Array con el número de filas afectadas por cada comando</returns>
        public int[] EjecutarTransaccion(string[] comandos, SqlParameter[][] parametros = null)
        {
            if (comandos == null || comandos.Length == 0)
                throw new ArgumentException("Debe proporcionar al menos un comando");

            try
            {
                using (SqlConnection connection = DataAccess.DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    try
                    {
                        int[] resultados = new int[comandos.Length];

                        for (int i = 0; i < comandos.Length; i++)
                        {
                            using (SqlCommand command = new SqlCommand(comandos[i], connection, transaction))
                            {
                                // Agregar parámetros si existen
                                if (parametros != null && parametros[i] != null)
                                {
                                    command.Parameters.AddRange(parametros[i]);
                                }

                                resultados[i] = command.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return resultados;
                    }
                    catch (Exception ex)
                    {
                        RollbackSeguro(transaction);
                        throw new Exception($"Error en la transacción: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar transacción: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado dentro de una transacción
        /// </summary>
        /// <param name="nombreProcedimiento">Nombre del procedimiento almacenado</param>
        /// <param name="parametros">Parámetros del procedimiento</param>
        /// <returns>Número de filas afectadas</returns>
        public int EjecutarProcedimientoTransaccional(string nombreProcedimiento, SqlParameter[] parametros = null)
        {
            try
            {
                using (SqlConnection connection = DataAccess.DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    try
                    {
                        using (SqlCommand command = new SqlCommand(nombreProcedimiento, connection, transaction))
                        {
                            command.CommandType = System.Data.CommandType.StoredProcedure;

                            if (parametros != null)
                            {
                                command.Parameters.AddRange(parametros);
                            }

                            int resultado = command.ExecuteNonQuery();
                            transaction.Commit();
                            return resultado;
                        }
                    }
                    catch (Exception ex)
                    {
                        RollbackSeguro(transaction);
                        throw new Exception($"Error en el procedimiento transaccional '{nombreProcedimiento}': {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar procedimiento transaccional: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Ejecuta una función que requiere manejo de transacciones manual
        /// </summary>
        /// <param name="accionTransaccional">Función que contiene la lógica transaccional</param>
        /// <returns>Resultado de la ejecución</returns>
        public T EjecutarConTransaccion<T>(Func<SqlConnection, SqlTransaction, T> accionTransaccional)
        {
            try
            {
                using (SqlConnection connection = DataAccess.DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();
                    
                    try
                    {
                        T resultado = accionTransaccional(connection, transaction);
                        transaction.Commit();
                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        RollbackSeguro(transaction);
                        throw new Exception($"Error en la transacción personalizada: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar transacción personalizada: {ex.Message}", ex);
            }
        }

        #endregion

        #region Retry Logic

        /// <summary>
        /// Ejecuta una operación con reintento automático en caso de fallo
        /// </summary>
        /// <param name="accion">Función a ejecutar</param>
        /// <param name="maxReintentos">Número máximo de reintentos</param>
        /// <param name="delayMs">Delay entre reintentos en milisegundos</param>
        /// <returns>Resultado de la ejecución</returns>
        public T EjecutarConReintento<T>(Func<T> accion, int maxReintentos = 3, int delayMs = 1000)
        {
            int intentos = 0;
            Exception ultimaExcepcion = null;

            while (intentos < maxReintentos)
            {
                try
                {
                    return accion();
                }
                catch (Exception ex)
                {
                    ultimaExcepcion = ex;
                    intentos++;

                    // Si no es el último intento, esperar antes de reintentar
                    if (intentos < maxReintentos)
                    {
                        System.Threading.Thread.Sleep(delayMs);
                    }
                }
            }

            throw new Exception($"La operación falló después de {maxReintentos} intentos. Último error: {ultimaExcepcion.Message}", ultimaExcepcion);
        }

        /// <summary>
        /// Ejecuta una operación transaccional con reintento automático
        /// </summary>
        /// <param name="accion">Función transaccional a ejecutar</param>
        /// <param name="maxReintentos">Número máximo de reintentos</param>
        /// <param name="delayMs">Delay entre reintentos en milisegundos</param>
        /// <returns>Resultado de la ejecución</returns>
        public T EjecutarTransaccionConReintento<T>(Func<SqlConnection, SqlTransaction, T> accion, int maxReintentos = 3, int delayMs = 1000)
        {
            return EjecutarConReintento(() => EjecutarConTransaccion(accion), maxReintentos, delayMs);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Verifica si una transacción está activa
        /// </summary>
        /// <param name="transaction">Transacción a verificar</param>
        /// <returns>True si la transacción está activa</returns>
        public bool TransaccionActiva(SqlTransaction transaction)
        {
            if (transaction == null)
                return false;

            return transaction.Connection != null && 
                   transaction.Connection.State == System.Data.ConnectionState.Open;
        }

        /// <summary>
        /// Valida el estado de la conexión antes de usarla en una transacción
        /// </summary>
        /// <param name="connection">Conexión a validar</param>
        /// <exception cref="Exception">Lanza excepción si la conexión no es válida</exception>
        public void ValidarConexion(SqlConnection connection)
        {
            if (connection == null)
                throw new Exception("La conexión no puede ser nula");

            if (connection.State != System.Data.ConnectionState.Open)
                throw new Exception($"La conexión no está abierta. Estado actual: {connection.State}");

            if (connection.Database == null || connection.Database.Length == 0)
                throw new Exception("La conexión no tiene una base de datos asociada");
        }

        /// <summary>
        /// Realiza un rollback seguro de una transacción
        /// </summary>
        /// <param name="transaction">Transacción a hacer rollback</param>
        public void RollbackSeguro(SqlTransaction transaction)
        {
            if (TransaccionActiva(transaction))
            {
                try
                {
                    transaction.Rollback();
                }
                catch
                {
                    // ignorar si ya fue completada
                }
            }
        }

        #endregion

        #region Logging and Monitoring

        /// <summary>
        /// Registra información de una transacción para auditoría
        /// </summary>
        /// <param name="idTransaccion">Identificador de la transacción</param>
        /// <param name="tipoOperacion">Tipo de operación</param>
        /// <param name="detalles">Detalles de la operación</param>
        /// <param name="exito">Si la operación fue exitosa</param>
        public void RegistrarTransaccion(string idTransaccion, string tipoOperacion, string detalles, bool exito)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                                  $"Transacción: {idTransaccion} | " +
                                  $"Operación: {tipoOperacion} | " +
                                  $"Resultado: {(exito ? "EXITO" : "FALLO")} | " +
                                  $"Detalles: {detalles}";

                // Registrar en log del sistema
                System.Diagnostics.Debug.WriteLine(logMessage);

                // Aquí se podría agregar logging a archivo, base de datos, etc.
                // Por ahora solo se registra en debug
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al registrar transacción: {ex.Message}");
            }
        }

        /// <summary>
        /// Genera un identificador único para transacciones
        /// </summary>
        /// <returns>ID de transacción único</returns>
        public string GenerarIdTransaccion()
        {
            return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Crea un parámetro SQL de forma segura
        /// </summary>
        /// <param name="nombre">Nombre del parámetro</param>
        /// <param name="valor">Valor del parámetro</param>
        /// <returns>SqlParameter configurado</returns>
        public SqlParameter CrearParametro(string nombre, object valor)
        {
            return DataAccess.DbHelper.Instance.CreateParameter(nombre, valor);
        }

        /// <summary>
        /// Crea un parámetro SQL con tipo específico
        /// </summary>
        /// <param name="nombre">Nombre del parámetro</param>
        /// <param name="tipo">Tipo de dato SQL</param>
        /// <param name="valor">Valor del parámetro</param>
        /// <returns>SqlParameter configurado</returns>
        public SqlParameter CrearParametro(string nombre, System.Data.SqlDbType tipo, object valor)
        {
            return DataAccess.DbHelper.Instance.CreateParameter(nombre, tipo, valor);
        }

        /// <summary>
        /// Verifica si la base de datos está disponible
        /// </summary>
        /// <returns>True si la base de datos responde</returns>
        public bool VerificarDisponibilidadBaseDatos()
        {
            try
            {
                return DataAccess.DbHelper.Instance.TestConnection();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene información del servidor SQL
        /// </summary>
        /// <returns>Información del servidor</returns>
        public string ObtenerInfoServidor()
        {
            try
            {
                return DataAccess.DbHelper.Instance.GetServerVersion();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        #endregion

        #region Transaction Scopes (for complex operations)

        /// <summary>
        /// Ejecuta una operación compleja que requiere múltiples pasos
        /// </summary>
        /// <param name="operacion">Función que define la operación compleja</param>
        /// <returns>Resultado de la operación</returns>
        public T EjecutarOperacionCompleja<T>(Func<OperacionComplejaContext, T> operacion)
        {
            string idTransaccion = GenerarIdTransaccion();
            
            try
            {
                return EjecutarConTransaccion((connection, transaction) =>
                {
                    var contexto = new OperacionComplejaContext
                    {
                        Connection = connection,
                        Transaction = transaction,
                        IdTransaccion = idTransaccion,
                        Helper = this
                    };

                    return operacion(contexto);
                });
            }
            catch (Exception ex)
            {
                RegistrarTransaccion(idTransaccion, "Operación Compleja", ex.Message, false);
                throw;
            }
        }

        #endregion
    }

    /// <summary>
    /// Contexto para operaciones complejas que requieren transacciones
    /// </summary>
    public class OperacionComplejaContext
    {
        public SqlConnection Connection { get; set; }
        public SqlTransaction Transaction { get; set; }
        public string IdTransaccion { get; set; }
        public TransaccionHelper Helper { get; set; }

        /// <summary>
        /// Ejecuta un comando dentro del contexto de la transacción
        /// </summary>
        /// <param name="comando">Comando SQL</param>
        /// <param name="parametros">Parámetros del comando</param>
        /// <returns>Número de filas afectadas</returns>
        public int EjecutarComando(string comando, SqlParameter[] parametros = null)
        {
            using (SqlCommand command = new SqlCommand(comando, Connection, Transaction))
            {
                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ejecuta una consulta que devuelve un valor escalar
        /// </summary>
        /// <param name="comando">Comando SQL</param>
        /// <param name="parametros">Parámetros del comando</param>
        /// <returns>Valor escalar</returns>
        public object EjecutarEscalar(string comando, SqlParameter[] parametros = null)
        {
            using (SqlCommand command = new SqlCommand(comando, Connection, Transaction))
            {
                if (parametros != null)
                {
                    command.Parameters.AddRange(parametros);
                }

                return command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Registra una acción dentro de la operación compleja
        /// </summary>
        /// <param name="accion">Descripción de la acción</param>
        /// <param name="exito">Si la acción fue exitosa</param>
        /// <param name="detalles">Detalles adicionales</param>
        public void RegistrarAccion(string accion, bool exito, string detalles = "")
        {
            Helper.RegistrarTransaccion(IdTransaccion, accion, detalles, exito);
        }
    }
}
