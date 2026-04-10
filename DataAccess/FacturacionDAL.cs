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
                              ValorImpuesto, TotalNeto, SaldoPendiente, Estado, FechaCreacion) 
                             VALUES 
                             (@Num, @Fecha, @IdCli, @Bruto, @Porc, @ValImp, @Neto, @Saldo, @Estado, GETDATE());
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
    }
}