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
    /// Clase que gestiona la lógica de negocio para el proceso de facturación
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class FacturacionManager
    {
        #region Singleton Pattern

        private static FacturacionManager _instance;
        private static readonly object _lock = new object();

        private FacturacionDAL facturacionDAL = new FacturacionDAL();

        /// <summary>
        /// Obtiene la instancia única de FacturacionManager (Singleton)
        /// </summary>
        public static FacturacionManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new FacturacionManager();
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
        private FacturacionManager()
        {
        }

        #endregion

        #region Factura Creation Methods

        /// <summary>
        /// Crea una nueva factura con sus detalles en una transacción
        /// </summary>
        /// <param name="factura">Factura a crear</param>
        /// <param name="detalles">Lista de detalles de la factura</param>
        /// <returns>ID de la factura creada</returns>
        public int CrearFacturaCompleta(Factura factura, List<DetalleFactura> detalles)
        {
            if (factura == null)
                throw new ArgumentException("La factura no puede ser nula");

            if (detalles == null || detalles.Count == 0)
                throw new ArgumentException("La factura debe tener al menos un detalle");
            

         

            try
            {
                // Generar NCF antes de guardar
                factura.NCF = Configuration.EmpresaConfig.Instance.GenerarSiguienteNCF(factura.TipoComprobante);
                
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                   
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    else if (connection.State == ConnectionState.Broken)
                    {
                        connection.Close();
                        connection.Open();
                    }

                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // 1. Insertar la factura
                        int idFactura = facturacionDAL.InsertarCabecera(factura, connection, transaction);
                        
                        // 2. Insertar los detalles
                        foreach (var detalle in detalles)
                        {
                            detalle.ID_Factura = idFactura;
                            if (!detalle.IsValid())
                                throw new Exception("Detalle inválido: " + detalle.GetValidationError());

                            facturacionDAL.InsertarDetalle(detalle, connection, transaction);
                        }

                        // 3. Actualizar stock de productos
                        ActualizarStockProductos(connection, transaction, detalles);

                        // 4. Calcular y actualizar totales de la factura
                        CalcularYActualizarTotalesFactura(connection, transaction, idFactura, detalles);

                        transaction.Commit();
                        return idFactura;
                    }
                    catch (Exception ex)
                    {
                        TransaccionHelper.Instance.RollbackSeguro(transaction);
                        throw new Exception($"Error al crear factura: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el proceso de facturación: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Crea una factura básica (sin detalles)
        /// </summary>
        /// <param name="factura">Factura a crear</param>
        /// <returns>ID de la factura creada</returns>
        public int CrearFacturaBasica(Factura factura)
        {
            if (factura == null)
                throw new ArgumentException("La factura no puede ser nula");

            if (!factura.IsValid())
                throw new ArgumentException($"Factura inválida: {factura.GetValidationError()}");

            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    return InsertarFactura(connection, null, factura);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear factura básica: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Agrega un detalle a una factura existente
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="detalle">Detalle a agregar</param>
        /// <returns>ID del detalle creado</returns>
        public int AgregarDetalleFactura(int idFactura, DetalleFactura detalle)
        {
            if (detalle == null)
                throw new ArgumentException("El detalle no puede ser nulo");

            if (!detalle.IsValid())
                throw new ArgumentException($"Detalle inválido: {detalle.GetValidationError()}");

            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    SqlTransaction transaction = connection.BeginTransaction();

                    try
                    {
                        // 1. Insertar el detalle
                        detalle.ID_Factura = idFactura;
                        int idDetalle = InsertarDetalleFactura(connection, transaction, detalle);

                        // 2. Actualizar stock del producto
                        ActualizarStockProducto(connection, transaction, detalle.ID_Producto, detalle.Cantidad);

                        // 3. Recalcular totales de la factura
                        List<DetalleFactura> todosLosDetalles = ObtenerDetallesFactura(connection, transaction, idFactura);
                        CalcularYActualizarTotalesFactura(connection, transaction, idFactura, todosLosDetalles);

                        transaction.Commit();
                        return idDetalle;
                    }
                    catch (Exception ex)
                    {
                        TransaccionHelper.Instance.RollbackSeguro(transaction);
                        throw new Exception($"Error al agregar detalle: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en el proceso de agregar detalle: {ex.Message}", ex);
            }
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida una factura completa con sus detalles
        /// </summary>
        /// <param name="factura">Factura a validar</param>
        /// <param name="detalles">Detalles de la factura</param>
        private void ValidarFacturaCompleta(Factura factura, List<DetalleFactura> detalles)
        {
            // Validar factura
            if (!factura.IsValid())
                throw new ArgumentException($"Factura inválida: {factura.GetValidationError()}");

            // Validar que el cliente exista y esté activo
            if (!ClienteExisteYActivo(factura.ID_Cliente))
                throw new ArgumentException("El cliente no existe o no está activo");

            // Validar detalles
            foreach (var detalle in detalles)
            {
                if (!detalle.IsValid())
                    throw new ArgumentException($"Detalle inválido: {detalle.GetValidationError()}");

                // Validar que el producto exista y esté activo
                if (!ProductoExisteYActivo(detalle.ID_Producto))
                    throw new ArgumentException($"El producto {detalle.ID_Producto} no existe o no está activo");

                // Validar stock suficiente
                if (!TieneStockSuficiente(detalle.ID_Producto, detalle.Cantidad))
                    throw new ArgumentException($"Stock insuficiente para el producto {detalle.ID_Producto}");
            }

            // Validar que no haya duplicados de productos
            var productosDuplicados = detalles.GroupBy(d => d.ID_Producto)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => g.Key);

            if (productosDuplicados.Any())
            {
                throw new ArgumentException("No se puede agregar el mismo producto más de una vez en una factura");
            }
        }

        /// <summary>
        /// Verifica si un cliente existe y está activo
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existe y está activo</returns>
        private bool ClienteExisteYActivo(int idCliente)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Clientes WHERE ID_Cliente = @ID_Cliente AND Estado = 1";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Cliente", idCliente);

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter }));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un producto existe y está activo
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <returns>True si existe y está activo</returns>
        private bool ProductoExisteYActivo(int idProducto)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Productos WHERE ID_Producto = @ID_Producto AND Estado = 1";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter }));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si hay stock suficiente para un producto
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidadRequerida">Cantidad requerida</param>
        /// <returns>True si hay stock suficiente</returns>
        private bool TieneStockSuficiente(int idProducto, int cantidadRequerida)
        {
            try
            {
                string query = "SELECT Stock FROM Productos WHERE ID_Producto = @ID_Producto AND Estado = 1";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);

                object result = DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter });
                
                if (result != null && result != DBNull.Value)
                {
                    int stock = Convert.ToInt32(result);
                    return stock >= cantidadRequerida;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Database Operations

        /// <summary>
        /// Inserta una factura en la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual (puede ser null)</param>
        /// <param name="factura">Factura a insertar</param>
        /// <returns>ID de la factura insertada</returns>
        private int InsertarFactura(SqlConnection connection, SqlTransaction transaction, Factura factura)
        {
            string query = @"
                INSERT INTO Facturas (NumeroFactura, Fecha, ID_Cliente, TotalBruto, PorcentajeImpuesto, 
                                   ValorImpuesto, TotalNeto, SaldoPendiente, Estado)
                VALUES (@NumeroFactura, @Fecha, @ID_Cliente, @TotalBruto, @PorcentajeImpuesto, 
                        @ValorImpuesto, @TotalNeto, @SaldoPendiente, @Estado);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@NumeroFactura", factura.NumeroFactura);
                command.Parameters.AddWithValue("@Fecha", factura.Fecha);
                command.Parameters.AddWithValue("@ID_Cliente", factura.ID_Cliente);
                command.Parameters.AddWithValue("@TotalBruto", factura.TotalBruto);
                command.Parameters.AddWithValue("@PorcentajeImpuesto", factura.PorcentajeImpuesto);
                command.Parameters.AddWithValue("@ValorImpuesto", factura.ValorImpuesto);
                command.Parameters.AddWithValue("@TotalNeto", factura.TotalNeto);
                command.Parameters.AddWithValue("@SaldoPendiente", factura.SaldoPendiente);
                command.Parameters.AddWithValue("@Estado", factura.Estado);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Inserta un detalle de factura en la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="detalle">Detalle a insertar</param>
        /// <returns>ID del detalle insertado</returns>
        private int InsertarDetalleFactura(SqlConnection connection, SqlTransaction transaction, DetalleFactura detalle)
        {
            string query = @"
                INSERT INTO DetallesFactura (ID_Factura, ID_Producto, Cantidad, PrecioUnitarioVenta, Subtotal)
                VALUES (@ID_Factura, @ID_Producto, @Cantidad, @PrecioUnitarioVenta, @Subtotal);
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Factura", detalle.ID_Factura);
                command.Parameters.AddWithValue("@ID_Producto", detalle.ID_Producto);
                command.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                command.Parameters.AddWithValue("@PrecioUnitarioVenta", detalle.PrecioUnitarioVenta);
                command.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Actualiza el stock de múltiples productos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="detalles">Lista de detalles con productos a actualizar</param>
        private void ActualizarStockProductos(SqlConnection connection, SqlTransaction transaction, List<DetalleFactura> detalles)
        {
            foreach (var detalle in detalles)
            {
                ActualizarStockProducto(connection, transaction, detalle.ID_Producto, detalle.Cantidad);
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto específico
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidadVendida">Cantidad vendida</param>
        private void ActualizarStockProducto(SqlConnection connection, SqlTransaction transaction, int idProducto, int cantidadVendida)
        {
            string query = "UPDATE Productos SET Stock = Stock - @CantidadVendida WHERE ID_Producto = @ID_Producto";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@CantidadVendida", cantidadVendida);
                command.Parameters.AddWithValue("@ID_Producto", idProducto);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new Exception($"No se encontró el producto {idProducto} para actualizar stock");
            }
        }

        /// <summary>
        /// Calcula y actualiza los totales de una factura
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="detalles">Lista de detalles de la factura</param>
        private void CalcularYActualizarTotalesFactura(SqlConnection connection, SqlTransaction transaction, int idFactura, List<DetalleFactura> detalles)
        {
            // Calcular totales
            decimal totalBruto = detalles.Sum(d => d.Subtotal);
            decimal valorImpuesto = totalBruto * 15.0m / 100.0m; // 15% de impuesto
            decimal totalNeto = totalBruto + valorImpuesto;

            string query = @"
                UPDATE Facturas 
                SET TotalBruto = @TotalBruto, 
                    ValorImpuesto = @ValorImpuesto, 
                    TotalNeto = @TotalNeto, 
                    SaldoPendiente = @TotalNeto
                WHERE ID_Factura = @ID_Factura";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@TotalBruto", totalBruto);
                command.Parameters.AddWithValue("@ValorImpuesto", valorImpuesto);
                command.Parameters.AddWithValue("@TotalNeto", totalNeto);
                command.Parameters.AddWithValue("@ID_Factura", idFactura);

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Obtiene todos los detalles de una factura
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <param name="transaction">Transacción actual</param>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Lista de detalles de la factura</returns>
        private List<DetalleFactura> ObtenerDetallesFactura(SqlConnection connection, SqlTransaction transaction, int idFactura)
        {
            List<DetalleFactura> detalles = new List<DetalleFactura>();

            string query = @"
                SELECT ID_Detalle, ID_Factura, ID_Producto, Cantidad, PrecioUnitarioVenta, Subtotal
                FROM DetallesFactura
                WHERE ID_Factura = @ID_Factura";

            using (SqlCommand command = new SqlCommand(query, connection, transaction))
            {
                command.Parameters.AddWithValue("@ID_Factura", idFactura);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DetalleFactura detalle = new DetalleFactura
                        {
                            ID_Detalle = Convert.ToInt32(reader["ID_Detalle"]),
                            ID_Factura = Convert.ToInt32(reader["ID_Factura"]),
                            ID_Producto = Convert.ToInt32(reader["ID_Producto"]),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            PrecioUnitarioVenta = Convert.ToDecimal(reader["PrecioUnitarioVenta"]),
                            Subtotal = Convert.ToDecimal(reader["Subtotal"])
                        };

                        detalles.Add(detalle);
                    }
                }
            }

            return detalles;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Genera el siguiente número de factura
        /// </summary>
        /// <returns>Siguiente número de factura</returns>
        public string GenerarSiguienteNumeroFactura()
        {
            try
            {
                string query = "SELECT TOP 1 NumeroFactura FROM Facturas ORDER BY ID_Factura DESC";
                object result = DbHelper.Instance.ExecuteScalar(query);

                if (result != null && result != DBNull.Value)
                {
                    return Factura.GenerarSiguienteNumero(result.ToString());
                }

                return "FAC-2026-001";
            }
            catch
            {
                return "FAC-2026-001";
            }
        }

        /// <summary>
        /// Verifica si un número de factura ya existe
        /// </summary>
        /// <param name="numeroFactura">Número de factura a verificar</param>
        /// <returns>True si ya existe</returns>
        public bool ExisteNumeroFactura(string numeroFactura)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Facturas WHERE NumeroFactura = @NumeroFactura";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@NumeroFactura", numeroFactura);

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter }));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene una factura por su ID
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Factura encontrada o null</returns>
        public Factura ObtenerFactura(int idFactura)
        {
            try
            {
                string query = @"
                    SELECT f.*, c.Nombre, c.Cedula, c.Direccion, c.Telefono, c.Email
                    FROM Facturas f
                    LEFT JOIN Clientes c ON f.ID_Cliente = c.ID_Cliente
                    WHERE f.ID_Factura = @ID_Factura";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Factura", idFactura);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    Factura factura = new Factura
                    {
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        NumeroFactura = row["NumeroFactura"].ToString(),
                        Fecha = Convert.ToDateTime(row["Fecha"]),
                        ID_Cliente = Convert.ToInt32(row["ID_Cliente"]),
                        TotalBruto = Convert.ToDecimal(row["TotalBruto"]),
                        PorcentajeImpuesto = Convert.ToDecimal(row["PorcentajeImpuesto"]),
                        ValorImpuesto = Convert.ToDecimal(row["ValorImpuesto"]),
                        TotalNeto = Convert.ToDecimal(row["TotalNeto"]),
                        SaldoPendiente = Convert.ToDecimal(row["SaldoPendiente"]),
                        Estado = row["Estado"].ToString(),
                        FechaCreacion = Convert.ToDateTime(row["FechaCreacion"])
                    };

                    // Agregar información del cliente
                    if (row["Nombre"] != DBNull.Value)
                    {
                        factura.Cliente = new Cliente
                        {
                            ID_Cliente = Convert.ToInt32(row["ID_Cliente"]),
                            Nombre = row["Nombre"].ToString(),
                            Cedula = row["Cedula"].ToString(),
                            Direccion = row["Direccion"] != DBNull.Value ? row["Direccion"].ToString() : null,
                            Telefono = row["Telefono"] != DBNull.Value ? row["Telefono"].ToString() : null,
                            Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null
                        };
                    }

                    return factura;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener factura: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una factura completa con todos sus detalles
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Factura completa con cliente y detalles</returns>
        public Factura ObtenerFacturaCompleta(int idFactura)
        {
            try
            {
                // Obtener cabecera de la factura
                Factura factura = ObtenerFactura(idFactura);
                if (factura == null)
                    return null;

                // Obtener detalles de la factura
                factura.Detalles = ObtenerDetallesFacturaCompleta(idFactura);

                return factura;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener factura completa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los detalles completos de una factura con información de productos
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <returns>Lista de detalles completos</returns>
        private List<DetalleFactura> ObtenerDetallesFacturaCompleta(int idFactura)
        {
            List<DetalleFactura> detalles = new List<DetalleFactura>();

            try
            {
                string query = @"
                    SELECT df.ID_Detalle, df.ID_Factura, df.ID_Producto, df.Cantidad, 
                           df.PrecioUnitarioVenta, df.Subtotal, p.Codigo, p.Descripcion
                    FROM DetallesFactura df
                    LEFT JOIN Productos p ON df.ID_Producto = p.ID_Producto
                    WHERE df.ID_Factura = @ID_Factura
                    ORDER BY df.ID_Detalle";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Factura", idFactura);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                foreach (DataRow row in dt.Rows)
                {
                    DetalleFactura detalle = new DetalleFactura
                    {
                        ID_Detalle = Convert.ToInt32(row["ID_Detalle"]),
                        ID_Factura = Convert.ToInt32(row["ID_Factura"]),
                        ID_Producto = Convert.ToInt32(row["ID_Producto"]),
                        Cantidad = Convert.ToInt32(row["Cantidad"]),
                        PrecioUnitarioVenta = Convert.ToDecimal(row["PrecioUnitarioVenta"]),
                        Subtotal = Convert.ToDecimal(row["Subtotal"])
                    };

                    // Agregar descripción del producto si está disponible
                    if (row["Descripcion"] != DBNull.Value)
                    {
                        detalle.Descripcion = row["Descripcion"].ToString();
                    }

                    detalles.Add(detalle);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener detalles de factura: {ex.Message}", ex);
            }

            return detalles;
        }

        #endregion
    }
}
