using System;
using System.Data;
using System.Data.SqlClient;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.DataAccess
{
    public class FacturacionDAL
    {
        // Inserta la cabecera y nos devuelve el ID (Identity) que generó SQL
        public int InsertarCabecera(Factura factura, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO Facturas 
                 (NumeroFactura, Fecha, ID_Cliente, TotalBruto, PorcentajeImpuesto, 
                  ValorImpuesto, TotalNeto, SaldoPendiente, Estado, TipoVenta, FechaCreacion, NCF, TipoComprobante) 
                 VALUES 
                 (@Num, @Fecha, @IdCli, @Bruto, @Porc, @ValImp, @Neto, @Saldo, @Estado, @TipoVenta, GETDATE(), @NCF, @TipoComprobante);
                 SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@Num", factura.NumeroFactura);
                cmd.Parameters.AddWithValue("@Fecha", factura.Fecha);
                cmd.Parameters.AddWithValue("@IdCli", factura.ID_Cliente);
                cmd.Parameters.AddWithValue("@Bruto", factura.TotalBruto);
                cmd.Parameters.AddWithValue("@Porc", factura.PorcentajeImpuesto);
                cmd.Parameters.AddWithValue("@ValImp", factura.ValorImpuesto);
                cmd.Parameters.AddWithValue("@Neto", factura.TotalNeto);
                cmd.Parameters.AddWithValue("@Saldo", factura.SaldoPendiente);
                cmd.Parameters.AddWithValue("@Estado", factura.Estado);
                cmd.Parameters.AddWithValue("@TipoVenta", (object)factura.TipoVenta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NCF", (object)factura.NCF ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoComprobante", (object)factura.TipoComprobante ?? DBNull.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Inserta cada renglón del detalle
        public void InsertarDetalle(DetalleFactura detalle, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"INSERT INTO DetallesFactura 
                             (ID_Factura, ID_Producto, Cantidad, PrecioUnitarioVenta, Subtotal) 
                             VALUES (@IdFac, @IdProd, @Cant, @Precio, @Sub);";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@IdFac", detalle.ID_Factura);
                cmd.Parameters.AddWithValue("@IdProd", detalle.ID_Producto);
                cmd.Parameters.AddWithValue("@Cant", detalle.Cantidad);
                cmd.Parameters.AddWithValue("@Precio", detalle.PrecioUnitarioVenta);
                cmd.Parameters.AddWithValue("@Sub", detalle.Subtotal);

                cmd.ExecuteNonQuery();
            }
        }

        // Elimina todos los detalles de una factura
        public void EliminarDetallesFactura(int idFactura, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"DELETE FROM DetallesFactura WHERE ID_Factura = @IdFactura";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@IdFactura", idFactura);
                cmd.ExecuteNonQuery();
            }
        }

        // Actualiza la cabecera de una factura existente
        public void ActualizarCabecera(Factura factura, SqlConnection connection, SqlTransaction transaction)
        {
            string query = @"UPDATE Facturas 
                 SET ID_Cliente = @IdCli, 
                     TotalBruto = @Bruto, 
                     PorcentajeImpuesto = @Porc, 
                     ValorImpuesto = @ValImp, 
                     TotalNeto = @Neto, 
                     SaldoPendiente = @Saldo, 
                     Estado = @Estado,
                     TipoVenta = @TipoVenta,
                     NCF = @NCF,
                     TipoComprobante = @TipoComprobante
                 WHERE ID_Factura = @IdFactura";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@IdCli", factura.ID_Cliente);
                cmd.Parameters.AddWithValue("@Bruto", factura.TotalBruto);
                cmd.Parameters.AddWithValue("@Porc", factura.PorcentajeImpuesto);
                cmd.Parameters.AddWithValue("@ValImp", factura.ValorImpuesto);
                cmd.Parameters.AddWithValue("@Neto", factura.TotalNeto);
                cmd.Parameters.AddWithValue("@Saldo", factura.SaldoPendiente);
                cmd.Parameters.AddWithValue("@Estado", factura.Estado);
                cmd.Parameters.AddWithValue("@TipoVenta", (object)factura.TipoVenta ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IdFactura", factura.ID_Factura);
                cmd.Parameters.AddWithValue("@NCF", (object)factura.NCF ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TipoComprobante", (object)factura.TipoComprobante ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        // Borra todos los registros de DetalleFactura, Pagos y Facturas
        public void BorrarTodosLosRegistros()
        {
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;Connect Timeout=30;";
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                transaction = connection.BeginTransaction();

                // 1. Borrar todos los detalles de facturas
                string queryDetalles = "DELETE FROM DetallesFactura";
                using (SqlCommand cmdDetalles = new SqlCommand(queryDetalles, connection, transaction))
                {
                    int detallesBorrados = cmdDetalles.ExecuteNonQuery();
                    Console.WriteLine($"DetallesFactura borrados: {detallesBorrados}");
                }

                // 2. Borrar todos los pagos (si existe la tabla Pagos)
                string queryPagos = "DELETE FROM Pagos";
                try
                {
                    using (SqlCommand cmdPagos = new SqlCommand(queryPagos, connection, transaction))
                    {
                        int pagosBorrados = cmdPagos.ExecuteNonQuery();
                        Console.WriteLine($"Pagos borrados: {pagosBorrados}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"La tabla Pagos no existe o no se pudo borrar: {ex.Message}");
                }

                // 3. Borrar todas las facturas
                string queryFacturas = "DELETE FROM Facturas";
                using (SqlCommand cmdFacturas = new SqlCommand(queryFacturas, connection, transaction))
                {
                    int facturasBorradas = cmdFacturas.ExecuteNonQuery();
                    Console.WriteLine($"Facturas borradas: {facturasBorradas}");
                }

                // Confirmar la transacción
                transaction.Commit();
                Console.WriteLine("Todos los registros han sido borrados exitosamente.");
            }
            catch (Exception ex)
            {
                // Revertir la transacción si hay error
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        Console.WriteLine($"Error al hacer rollback: {rollbackEx.Message}");
                    }
                }
                
                Console.WriteLine($"Error al borrar registros: {ex.Message}");
                throw new Exception($"Error al borrar todos los registros: {ex.Message}", ex);
            }
            finally
            {
                // Cerrar la conexión manualmente
                if (connection != null)
                {
                    try
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                        connection.Dispose();
                    }
                    catch (Exception closeEx)
                    {
                        Console.WriteLine($"Error al cerrar la conexión: {closeEx.Message}");
                    }
                }
            }
        }
    }
}