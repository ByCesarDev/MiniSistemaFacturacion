using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;
using System.Data;

namespace MiniSistemaFacturacion.BusinessLogic
{
    /// <summary>
    /// Clase que gestiona la lógica de negocio para el proceso de pagos
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class PagoManager
    {
        #region Singleton Pattern

        private static PagoManager _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtiene la instancia única de PagoManager (Singleton)
        /// </summary>
        public static PagoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new PagoManager();
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
        private PagoManager()
        {
        }

        #endregion

        #region Payment Registration Methods

        /// <summary>
        /// Registra un pago para una factura en una transacción
        /// </summary>
        /// <param name="pago">Pago a registrar</param>
        /// <returns>ID del pago registrado</returns>
        public int RegistrarPago(Pago pago)
        {
            if (pago == null)
                throw new ArgumentException("El pago no puede ser nulo");

            // Validar pago
            if (!pago.IsValid())
                throw new ArgumentException($"Pago inválido: {pago.GetValidationError()}");

            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // 1. Validar que la factura exista y esté en estado válido
                        Factura factura = ValidarFacturaParaPago(connection, transaction, pago.ID_Factura);

                        // 2. Validar que el monto no exceda el saldo pendiente
                        if (pago.MontoPagado > factura.SaldoPendiente)
                            throw new ArgumentException($"El monto del pago (${pago.MontoPagado:F2}) excede el saldo pendiente (${factura.SaldoPendiente:F2})");

                        // 3. Validar referencia si es requerida
                        if (pago.RequiereReferencia() && !pago.IsValidReferencia())
                            throw new ArgumentException($"Referencia inválida: {pago.GetReferenciaError()}");

                        // 4. Insertar el pago
                        int idPago = InsertarPago(connection, transaction, pago);

                        // 5. Actualizar saldo pendiente de la factura
                        decimal nuevoSaldo = factura.SaldoPendiente - pago.MontoPagado;
                        string nuevoEstado = DeterminarNuevoEstadoFactura(factura.Estado, nuevoSaldo);
                        ActualizarSaldoFactura(connection, transaction, pago.ID_Factura, nuevoSaldo, nuevoEstado);

                        transaction.Commit();
                        return idPago;
                    }
                    catch (Exception ex)
                    {
                        TransaccionHelper.Instance.RollbackSeguro(transaction);
                        throw new Exception($"Error al registrar pago: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el proceso de pago: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Registra múltiples pagos para diferentes facturas en una transacción
        /// </summary>
        /// <param name="pagos">Lista de pagos a registrar</param>
        /// <returns>Lista de IDs de pagos registrados</returns>
        public List<int> RegistrarPagosMasivos(List<Pago> pagos)
        {
            if (pagos == null || pagos.Count == 0)
                throw new ArgumentException("La lista de pagos no puede estar vacía");

            List<int> idsPagos = new List<int>();

            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        foreach (var pago in pagos)
                        {
                            if (!pago.IsValid())
                                throw new ArgumentException($"Pago inválido: {pago.GetValidationError()}");

                            // Validar factura
                            Factura factura = ValidarFacturaParaPago(connection, transaction, pago.ID_Factura);

                            // Validar monto
                            if (pago.MontoPagado > factura.SaldoPendiente)
                                throw new ArgumentException($"Monto excedido para factura {pago.ID_Factura}");

                            // Validar referencia si es requerida
                            if (pago.RequiereReferencia() && !pago.IsValidReferencia())
                                throw new ArgumentException($"Referencia inválida para factura {pago.ID_Factura}");

                            // Insertar pago
                            int idPago = InsertarPago(connection, transaction, pago);
                            idsPagos.Add(idPago);

                            // Actualizar saldo
                            decimal nuevoSaldo = factura.SaldoPendiente - pago.MontoPagado;
                            string nuevoEstado = DeterminarNuevoEstadoFactura(factura.Estado, nuevoSaldo);
                            ActualizarSaldoFactura(connection, transaction, pago.ID_Factura, nuevoSaldo, nuevoEstado);
                        }

                        transaction.Commit();
                        return idsPagos;
                    }
                    catch (Exception ex)
                    {
                        TransaccionHelper.Instance.RollbackSeguro(transaction);
                        throw new Exception($"Error al registrar pagos masivos: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el proceso de pagos masivos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Anula un pago (lo marca como inactivo y devuelve el saldo a la factura)
        /// </summary>
        /// <param name="idPago">ID del pago a anular</param>
        /// <returns>True si se anuló correctamente</returns>
        public bool AnularPago(int idPago)
        {
            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // 1. Obtener información del pago
                        Pago pago = ObtenerPagoParaAnular(connection, transaction, idPago);
                        if (pago == null)
                            throw new ArgumentException("El pago no existe o ya está anulado");

                        // 2. Obtener la factura actualizada
                        Factura factura = ObtenerFacturaActualizada(connection, transaction, pago.ID_Factura);
                        if (factura == null)
                            throw new ArgumentException("La factura asociada no existe");

                        // 3. Desactivar el pago
                        DesactivarPago(connection, transaction, idPago);

                        // 4. Devolver el monto al saldo pendiente de la factura
                        decimal nuevoSaldo = factura.SaldoPendiente + pago.MontoPagado;
                        string nuevoEstado = DeterminarNuevoEstadoFactura(factura.Estado, nuevoSaldo);
                        ActualizarSaldoFactura(connection, transaction, pago.ID_Factura, nuevoSaldo, nuevoEstado);

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        TransaccionHelper.Instance.RollbackSeguro(transaction);
                        throw new Exception($"Error al anular pago: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el proceso de anulación: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que una factura exista y esté en estado válido para recibir pagos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Factura encontrada</returns>
        private Factura ValidarFacturaParaPago(SqlConnection connection, SqlTransaction transaction, int idFactura)
        {
            string query = @"
                SELECT ID_Factura, NumeroFactura, Estado, SaldoPendiente, TotalNeto
                FROM Facturas
                WHERE ID_Factura = @ID_Factura";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Factura", idFactura);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Factura factura = new Factura
                        {
                            ID_Factura = Convert.ToInt32(reader["ID_Factura"]),
                            NumeroFactura = reader["NumeroFactura"].ToString(),
                            Estado = reader["Estado"].ToString(),
                            SaldoPendiente = Convert.ToDecimal(reader["SaldoPendiente"]),
                            TotalNeto = Convert.ToDecimal(reader["TotalNeto"])
                        };

                        // Validar estado
                        if (factura.Estado == "Anulada")
                            throw new ArgumentException("No se puede pagar una factura anulada");

                        if (factura.Estado == "Pagada")
                            throw new ArgumentException("La factura ya está pagada");

                        return factura;
                    }
                    else
                    {
                        throw new ArgumentException("La factura no existe");
                    }
                }
            }
        }

        /// <summary>
        /// Determina el nuevo estado de una factura basado en el saldo
        /// </summary>
        /// <param name="estadoActual">Estado actual de la factura</param>
        /// <param name="nuevoSaldo">Nuevo saldo pendiente</param>
        /// <returns>Nuevo estado de la factura</returns>
        private string DeterminarNuevoEstadoFactura(string estadoActual, decimal nuevoSaldo)
        {
            if (nuevoSaldo < 0)
                throw new ArgumentException("El saldo no puede ser negativo");

            if (nuevoSaldo == 0)
                return "Pagada";

            return "Parcial";
        }

        #endregion

        #region Database Operations

        /// <summary>
        /// Inserta un pago en la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="pago">Pago a insertar</param>
        /// <returns>ID del pago insertado</returns>
        private int InsertarPago(SqlConnection connection, SqlTransaction transaction, Pago pago)
        {
            string query = @"
                INSERT INTO Pagos (ID_Factura, FechaPago, MontoPagado, FormaPago, Referencia, Observaciones, Estado)
                VALUES (@ID_Factura, @FechaPago, @MontoPagado, @FormaPago, @Referencia, @Observaciones, @Estado);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Factura", pago.ID_Factura);
                command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                command.Parameters.AddWithValue("@MontoPagado", pago.MontoPagado);
                command.Parameters.AddWithValue("@FormaPago", pago.FormaPago);
                command.Parameters.AddWithValue("@Referencia", (object)pago.Referencia ?? DBNull.Value);
                command.Parameters.AddWithValue("@Observaciones", (object)pago.Observaciones ?? DBNull.Value);
                command.Parameters.AddWithValue("@Estado", pago.Estado);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Actualiza el saldo pendiente y estado de una factura
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="nuevoSaldo">Nuevo saldo pendiente</param>
        /// <param name="nuevoEstado">Nuevo estado</param>
        private void ActualizarSaldoFactura(SqlConnection connection, SqlTransaction transaction, int idFactura, decimal nuevoSaldo, string nuevoEstado)
        {
            string query = @"
                UPDATE Facturas 
                SET SaldoPendiente = @NuevoSaldo, Estado = @NuevoEstado
                WHERE ID_Factura = @ID_Factura";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@NuevoSaldo", nuevoSaldo);
                command.Parameters.AddWithValue("@NuevoEstado", nuevoEstado);
                command.Parameters.AddWithValue("@ID_Factura", idFactura);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception($"No se encontró la factura {idFactura} para actualizar");
            }
        }

        /// <summary>
        /// Obtiene información de un pago para anular
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idPago">ID del pago</param>
        /// <returns>Pago encontrado o null</returns>
        private Pago ObtenerPagoParaAnular(SqlConnection connection, SqlTransaction transaction, int idPago)
        {
            string query = @"
                SELECT ID_Pago, ID_Factura, FechaPago, MontoPagado, FormaPago, Referencia, Observaciones, Estado
                FROM Pagos
                WHERE ID_Pago = @ID_Pago AND Estado = 1";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Pago", idPago);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Pago
                        {
                            ID_Pago = Convert.ToInt32(reader["ID_Pago"]),
                            ID_Factura = Convert.ToInt32(reader["ID_Factura"]),
                            FechaPago = Convert.ToDateTime(reader["FechaPago"]),
                            MontoPagado = Convert.ToDecimal(reader["MontoPagado"]),
                            FormaPago = reader["FormaPago"].ToString(),
                            Referencia = reader["Referencia"] != DBNull.Value ? reader["Referencia"].ToString() : null,
                            Observaciones = reader["Observaciones"] != DBNull.Value ? reader["Observaciones"].ToString() : null,
                            Estado = Convert.ToBoolean(reader["Estado"])
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene la información actualizada de una factura
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Factura encontrada o null</returns>
        private Factura ObtenerFacturaActualizada(SqlConnection connection, SqlTransaction transaction, int idFactura)
        {
            string query = @"
                SELECT ID_Factura, NumeroFactura, Estado, SaldoPendiente, TotalNeto
                FROM Facturas
                WHERE ID_Factura = @ID_Factura";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Factura", idFactura);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Factura
                        {
                            ID_Factura = Convert.ToInt32(reader["ID_Factura"]),
                            NumeroFactura = reader["NumeroFactura"].ToString(),
                            Estado = reader["Estado"].ToString(),
                            SaldoPendiente = Convert.ToDecimal(reader["SaldoPendiente"]),
                            TotalNeto = Convert.ToDecimal(reader["TotalNeto"])
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Desactiva un pago (lo marca como inactivo)
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idPago">ID del pago a desactivar</param>
        private void DesactivarPago(SqlConnection connection, SqlTransaction transaction, int idPago)
        {
            string query = "UPDATE Pagos SET Estado = 0 WHERE ID_Pago = @ID_Pago";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Pago", idPago);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception($"No se encontró el pago {idPago} para desactivar");
            }
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Obtiene todos los pagos de una factura
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Lista de pagos de la factura</returns>
        public List<Pago> ObtenerPagosPorFactura(int idFactura)
        {
            List<Pago> pagos = new List<Pago>();

            try
            {
                string query = @"
                    SELECT p.ID_Pago, p.ID_Factura, p.FechaPago, p.MontoPagado, p.FormaPago, 
                           p.Referencia, p.Observaciones, p.Estado,
                           f.NumeroFactura, f.Estado AS EstadoFactura
                    FROM Pagos p
                    LEFT JOIN Facturas f ON p.ID_Factura = f.ID_Factura
                    WHERE p.ID_Factura = @ID_Factura
                    ORDER BY p.FechaPago DESC";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Factura", idFactura);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                foreach (DataRow row in dt.Rows)
                {
                    Pago pago = new Pago
                    {
                        ID_Pago = Convert.ToInt32(row["ID_Pago"]),
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        FechaPago = Convert.ToDateTime(row["FechaPago"]),
                        MontoPagado = Convert.ToDecimal(row["MontoPagado"]),
                        FormaPago = row["FormaPago"].ToString(),
                        Referencia = row["Referencia"] != DBNull.Value ? row["Referencia"].ToString() : null,
                        Observaciones = row["Observaciones"] != DBNull.Value ? row["Observaciones"].ToString() : null,
                        Estado = Convert.ToBoolean(row["Estado"])
                    };

                    // Agregar información de la factura
                    pago.Factura = new Factura
                    {
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        NumeroFactura = row["NumeroFactura"].ToString(),
                        Estado = row["EstadoFactura"].ToString()
                    };

                    pagos.Add(pago);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener pagos de la factura: {ex.Message}", ex);
            }

            return pagos;
        }

        /// <summary>
        /// Obtiene el total de pagos de una factura
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Total de pagos activos</returns>
        public decimal ObtenerTotalPagosFactura(int idFactura)
        {
            try
            {
                string query = @"
                    SELECT ISNULL(SUM(MontoPagado), 0) 
                    FROM Pagos 
                    WHERE ID_Factura = @ID_Factura AND Estado = 1";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Factura", idFactura);
                object result = DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter });

                return Convert.ToDecimal(result ?? 0);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener total de pagos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene pagos por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Lista de pagos en el rango de fechas</returns>
        public List<Pago> ObtenerPagosPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            List<Pago> pagos = new List<Pago>();

            try
            {
                string query = @"
                    SELECT p.ID_Pago, p.ID_Factura, p.FechaPago, p.MontoPagado, p.FormaPago, 
                           p.Referencia, p.Observaciones, p.Estado,
                           f.NumeroFactura, c.Nombre AS NombreCliente
                    FROM Pagos p
                    LEFT JOIN Facturas f ON p.ID_Factura = f.ID_Factura
                    LEFT JOIN Clientes c ON f.ID_Cliente = c.ID_Cliente
                    WHERE p.FechaPago BETWEEN @FechaInicio AND @FechaFin AND p.Estado = 1
                    ORDER BY p.FechaPago DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@FechaInicio", fechaInicio),
                    DbHelper.Instance.CreateParameter("@FechaFin", fechaFin)
                };

                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    Pago pago = new Pago
                    {
                        ID_Pago = Convert.ToInt32(row["ID_Pago"]),
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        FechaPago = Convert.ToDateTime(row["FechaPago"]),
                        MontoPagado = Convert.ToDecimal(row["MontoPagado"]),
                        FormaPago = row["FormaPago"].ToString(),
                        Referencia = row["Referencia"] != DBNull.Value ? row["Referencia"].ToString() : null,
                        Observaciones = row["Observaciones"] != DBNull.Value ? row["Observaciones"].ToString() : null,
                        Estado = Convert.ToBoolean(row["Estado"])
                    };

                    // Agregar información de la factura
                    pago.Factura = new Factura
                    {
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        NumeroFactura = row["NumeroFactura"].ToString()
                    };

                    pagos.Add(pago);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener pagos por fecha: {ex.Message}", ex);
            }

            return pagos;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Genera un resumen de pagos por forma de pago
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio</param>
        /// <param name="fechaFin">Fecha de fin</param>
        /// <returns>Dictionary con forma de pago como clave y total como valor</returns>
        public Dictionary<string, decimal> ObtenerResumenPagosPorForma(DateTime fechaInicio, DateTime fechaFin)
        {
            Dictionary<string, decimal> resumen = new Dictionary<string, decimal>();

            try
            {
                string query = @"
                    SELECT FormaPago, ISNULL(SUM(MontoPagado), 0) AS Total
                    FROM Pagos
                    WHERE FechaPago BETWEEN @FechaInicio AND @FechaFin AND Estado = 1
                    GROUP BY FormaPago
                    ORDER BY Total DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@FechaInicio", fechaInicio),
                    DbHelper.Instance.CreateParameter("@FechaFin", fechaFin)
                };

                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parameters);

                foreach (DataRow row in dt.Rows)
                {
                    string formaPago = row["FormaPago"].ToString();
                    decimal total = Convert.ToDecimal(row["Total"]);
                    resumen[formaPago] = total;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener resumen de pagos: {ex.Message}", ex);
            }

            return resumen;
        }

        #endregion
    }
}
