using System;
using System.Collections.Generic;
using System.Data;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;
using System.Data.SqlClient;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase de Acceso a Datos para la entidad Producto
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class ProductoDAL
    {
        #region CRUD Operations

        /// <summary>
        /// Inserta un nuevo producto en la base de datos
        /// </summary>
        /// <param name="producto">Producto a insertar</param>
        /// <returns>ID del producto insertado</returns>
        public int Insertar(Producto producto)
        {
            if (producto == null)
                throw new ArgumentException("El producto no puede ser nulo");

            if (!producto.IsValid())
                throw new ArgumentException($"Producto inválido: {producto.GetValidationError()}");

            try
            {
                string query = @"
                    INSERT INTO Productos (Codigo, Descripcion, PrecioUnitario, Stock, StockMinimo, Categoria)
                    VALUES (@Codigo, @Descripcion, @PrecioUnitario, @Stock, @StockMinimo, @Categoria);
                    SELECT SCOPE_IDENTITY();";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Codigo", producto.Codigo),
                    DbHelper.Instance.CreateParameter("@Descripcion", producto.Descripcion),
                    DbHelper.Instance.CreateParameter("@PrecioUnitario", producto.PrecioUnitario),
                    DbHelper.Instance.CreateParameter("@Stock", producto.Stock),
                    DbHelper.Instance.CreateParameter("@StockMinimo", producto.StockMinimo),
                    DbHelper.Instance.CreateParameter("@Categoria", producto.Categoria ?? (object)DBNull.Value)
                };

                object result = DbHelper.Instance.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="producto">Producto con datos actualizados</param>
        /// <returns>Número de filas afectadas</returns>
        public int Actualizar(Producto producto)
        {
            if (producto == null)
                throw new ArgumentException("El producto no puede ser nulo");

            if (producto.ID_Producto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            if (!producto.IsValid())
                throw new ArgumentException($"Producto inválido: {producto.GetValidationError()}");

            try
            {
                string query = @"
                    UPDATE Productos 
                    SET Codigo = @Codigo, 
                        Descripcion = @Descripcion, 
                        PrecioUnitario = @PrecioUnitario, 
                        Stock = @Stock, 
                        StockMinimo = @StockMinimo, 
                        Categoria = @Categoria
                    WHERE ID_Producto = @ID_Producto";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Producto", producto.ID_Producto),
                    DbHelper.Instance.CreateParameter("@Codigo", producto.Codigo),
                    DbHelper.Instance.CreateParameter("@Descripcion", producto.Descripcion),
                    DbHelper.Instance.CreateParameter("@PrecioUnitario", producto.PrecioUnitario),
                    DbHelper.Instance.CreateParameter("@Stock", producto.Stock),
                    DbHelper.Instance.CreateParameter("@StockMinimo", producto.StockMinimo),
                    DbHelper.Instance.CreateParameter("@Categoria", producto.Categoria ?? (object)DBNull.Value)
                };

                return DbHelper.Instance.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un producto por su ID
        /// </summary>
        /// <param name="idProducto">ID del producto a eliminar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Eliminar(int idProducto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            try
            {
                string query = "DELETE FROM Productos WHERE ID_Producto = @ID_Producto";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desactiva un producto (borrado lógico)
        /// </summary>
        /// <param name="idProducto">ID del producto a desactivar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Desactivar(int idProducto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            try
            {
                string query = "UPDATE Productos SET Estado = 0 WHERE ID_Producto = @ID_Producto";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Activa un producto
        /// </summary>
        /// <param name="idProducto">ID del producto a activar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Activar(int idProducto)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            try
            {
                string query = "UPDATE Productos SET Estado = 1 WHERE ID_Producto = @ID_Producto";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al activar producto: {ex.Message}", ex);
            }
        }

        #endregion

        #region Stock Management

        /// <summary>
        /// Actualiza el stock de un producto
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="nuevoStock">Nuevo stock</param>
        /// <returns>Número de filas afectadas</returns>
        public int ActualizarStock(int idProducto, int nuevoStock)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            if (nuevoStock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            try
            {
                string query = "UPDATE Productos SET Stock = @Stock WHERE ID_Producto = @ID_Producto";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Producto", idProducto),
                    DbHelper.Instance.CreateParameter("@Stock", nuevoStock)
                };

                return DbHelper.Instance.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar stock: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Reduce el stock de un producto
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidad">Cantidad a reducir</param>
        /// <returns>Número de filas afectadas</returns>
        public int ReducirStock(int idProducto, int cantidad)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero");

            try
            {
                string query = @"
                    UPDATE Productos 
                    SET Stock = Stock - @Cantidad 
                    WHERE ID_Producto = @ID_Producto AND Stock >= @Cantidad";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Producto", idProducto),
                    DbHelper.Instance.CreateParameter("@Cantidad", cantidad)
                };

                int filasAfectadas = DbHelper.Instance.ExecuteNonQuery(query, parameters);

                if (filasAfectadas == 0)
                    throw new Exception("Stock insuficiente o producto no encontrado");

                return filasAfectadas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al reducir stock: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Agrega stock a un producto
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidad">Cantidad a agregar</param>
        /// <returns>Número de filas afectadas</returns>
        public int AgregarStock(int idProducto, int cantidad)
        {
            if (idProducto <= 0)
                throw new ArgumentException("El ID del producto es requerido");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero");

            try
            {
                string query = @"
                    UPDATE Productos 
                    SET Stock = Stock + @Cantidad 
                    WHERE ID_Producto = @ID_Producto";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Producto", idProducto),
                    DbHelper.Instance.CreateParameter("@Cantidad", cantidad)
                };

                return DbHelper.Instance.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al agregar stock: {ex.Message}", ex);
            }
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <returns>Producto encontrado o null</returns>
        public Producto ObtenerPorId(int idProducto)
        {
            if (idProducto <= 0)
                return null;

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE ID_Producto = @ID_Producto";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Producto", idProducto);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                if (dt.Rows.Count > 0)
                {
                    return MapearProducto(dt.Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener producto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un producto por su código
        /// </summary>
        /// <param name="codigo">Código del producto</param>
        /// <returns>Producto encontrado o null</returns>
        public Producto ObtenerPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE Codigo = @Codigo";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@Codigo", codigo);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                if (dt.Rows.Count > 0)
                {
                    return MapearProducto(dt.Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener producto por código: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        /// <returns>Lista de todos los productos</returns>
        public List<Producto> ObtenerTodos()
        {
            List<Producto> productos = new List<Producto>();

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    ORDER BY Descripcion";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los productos: {ex.Message}", ex);
            }

            return productos;
        }

        /// <summary>
        /// Obtiene solo los productos activos
        /// </summary>
        /// <returns>Lista de productos activos</returns>
        public List<Producto> ObtenerActivos()
        {
            List<Producto> productos = new List<Producto>();

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE Estado = 1
                    ORDER BY Descripcion";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos activos: {ex.Message}", ex);
            }

            return productos;
        }

        /// <summary>
        /// Obtiene productos con bajo stock
        /// </summary>
        /// <returns>Lista de productos con stock bajo o crítico</returns>
        public List<Producto> ObtenerConBajoStock()
        {
            List<Producto> productos = new List<Producto>();

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE Estado = 1 AND Stock <= StockMinimo
                    ORDER BY Stock, Descripcion";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos con bajo stock: {ex.Message}", ex);
            }

            return productos;
        }

        /// <summary>
        /// Obtiene productos sin stock
        /// </summary>
        /// <returns>Lista de productos sin stock</returns>
        public List<Producto> ObtenerSinStock()
        {
            List<Producto> productos = new List<Producto>();

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE Estado = 1 AND Stock = 0
                    ORDER BY Descripcion";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos sin stock: {ex.Message}", ex);
            }

            return productos;
        }

        /// <summary>
        /// Busca productos por descripción o código
        /// </summary>
        /// <param name="terminoBusqueda">Término de búsqueda</param>
        /// <returns>Lista de productos que coinciden con la búsqueda</returns>
        public List<Producto> Buscar(string terminoBusqueda)
        {
            List<Producto> productos = new List<Producto>();

            if (string.IsNullOrWhiteSpace(terminoBusqueda))
                return productos;

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE (Descripcion LIKE @TerminoBusqueda OR Codigo LIKE @TerminoBusqueda)
                    ORDER BY Descripcion";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@TerminoBusqueda", $"%{terminoBusqueda}%");
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar productos: {ex.Message}", ex);
            }

            return productos;
        }

        /// <summary>
        /// Obtiene productos por categoría
        /// </summary>
        /// <param name="categoria">Categoría a filtrar</param>
        /// <returns>Lista de productos de la categoría</returns>
        public List<Producto> ObtenerPorCategoria(string categoria)
        {
            List<Producto> productos = new List<Producto>();

            if (string.IsNullOrWhiteSpace(categoria))
                return productos;

            try
            {
                string query = @"
                    SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, 
                           StockMinimo, Categoria, Estado, FechaCreacion
                    FROM Productos
                    WHERE Categoria = @Categoria AND Estado = 1
                    ORDER BY Descripcion";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@Categoria", categoria);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos por categoría: {ex.Message}", ex);
            }

            return productos;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Verifica si un producto existe por su ID
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <returns>True si existe</returns>
        public bool Existe(int idProducto)
        {
            if (idProducto <= 0)
                return false;

            try
            {
                string query = "SELECT COUNT(*) FROM Productos WHERE ID_Producto = @ID_Producto";
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
        /// Verifica si un producto existe por su código
        /// </summary>
        /// <param name="codigo">Código del producto</param>
        /// <returns>True si existe</returns>
        public bool ExistePorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            try
            {
                string query = "SELECT COUNT(*) FROM Productos WHERE Codigo = @Codigo";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@Codigo", codigo);

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter }));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un producto está activo
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <returns>True si está activo</returns>
        public bool EstaActivo(int idProducto)
        {
            if (idProducto <= 0)
                return false;

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
        /// Verifica si hay stock suficiente para una cantidad específica
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidadRequerida">Cantidad requerida</param>
        /// <returns>True si hay stock suficiente</returns>
        public bool TieneStockSuficiente(int idProducto, int cantidadRequerida)
        {
            if (idProducto <= 0 || cantidadRequerida <= 0)
                return false;

            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM Productos 
                    WHERE ID_Producto = @ID_Producto AND Estado = 1 AND Stock >= @CantidadRequerida";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Producto", idProducto),
                    DbHelper.Instance.CreateParameter("@CantidadRequerida", cantidadRequerida)
                };

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, parameters));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Obtiene el número total de productos
        /// </summary>
        /// <returns>Número total de productos</returns>
        public int ObtenerTotalProductos()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Productos";
                object result = DbHelper.Instance.ExecuteScalar(query);
                return Convert.ToInt32(result ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el número de productos activos
        /// </summary>
        /// <returns>Número de productos activos</returns>
        public int ObtenerTotalProductosActivos()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Productos WHERE Estado = 1";
                object result = DbHelper.Instance.ExecuteScalar(query);
                return Convert.ToInt32(result ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        /// <returns>Lista de productos activos</returns>
        public List<Producto> ObtenerProductosActivos()
        {
            try
            {
                string query = "SELECT ID_Producto, Codigo, Descripcion, PrecioUnitario, Stock, StockMinimo, Categoria, FechaCreacion, Estado FROM Productos WHERE Estado = 1 ORDER BY Descripcion";
                
                DataTable dt = DbHelper.Instance.ExecuteQuery(query);
                List<Producto> productos = new List<Producto>();

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }

                return productos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener productos activos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todas las categorías de productos
        /// </summary>
        /// <returns>Lista de categorías únicas</returns>
        public List<string> ObtenerCategorias()
        {
            List<string> categorias = new List<string>();

            try
            {
                string query = @"
                    SELECT DISTINCT Categoria 
                    FROM Productos 
                    WHERE Categoria IS NOT NULL AND Categoria != '' AND Estado = 1
                    ORDER BY Categoria";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    categorias.Add(row["Categoria"].ToString());
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener categorías: {ex.Message}", ex);
            }

            return categorias;
        }

        /// <summary>
        /// Mapea un DataRow a un objeto Producto
        /// </summary>
        /// <param name="row">DataRow con los datos del producto</param>
        /// <returns>Objeto Producto mapeado</returns>
        private Producto MapearProducto(DataRow row)
        {
            Producto producto = new Producto
            {
                ID_Producto = Convert.ToInt32(row["ID_Producto"]),
                Codigo = row["Codigo"].ToString(),
                Descripcion = row["Descripcion"].ToString(),
                PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                Stock = row["Stock"] != DBNull.Value ? Convert.ToInt32(row["Stock"]) : 0,
                StockMinimo = row["StockMinimo"] != DBNull.Value ? Convert.ToInt32(row["StockMinimo"]) : 0,
                Categoria = row["Categoria"] != DBNull.Value ? row["Categoria"].ToString() : null,
                Estado = Convert.ToBoolean(row["Estado"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"])
            };

            // Depuración: mostrar información del producto mapeado
            System.Diagnostics.Debug.WriteLine($"Producto mapeado: {producto.Descripcion} - Stock: {producto.Stock} - StockMinimo: {producto.StockMinimo}");

            return producto;
        }

        #endregion

        #region Stored Procedures

        /// <summary>
        /// Inserta un producto usando procedimiento almacenado
        /// </summary>
        /// <param name="producto">Producto a insertar</param>
        /// <returns>ID del producto insertado</returns>
        public int InsertarConProcedimiento(Producto producto)
        {
            if (producto == null)
                throw new ArgumentException("El producto no puede ser nulo");

            if (!producto.IsValid())
                throw new ArgumentException($"Producto inválido: {producto.GetValidationError()}");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Codigo", producto.Codigo),
                    DbHelper.Instance.CreateParameter("@Descripcion", producto.Descripcion),
                    DbHelper.Instance.CreateParameter("@PrecioUnitario", producto.PrecioUnitario),
                    DbHelper.Instance.CreateParameter("@Stock", producto.Stock),
                    DbHelper.Instance.CreateParameter("@StockMinimo", producto.StockMinimo),
                    DbHelper.Instance.CreateParameter("@Categoria", producto.Categoria ?? (object)DBNull.Value)
                };

                DataTable dt = DbHelper.Instance.ExecuteStoredProcedure("sp_InsertarProducto", parameters);

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["ID_Producto"]);
                }

                throw new Exception("No se pudo obtener el ID del producto insertado");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar producto con procedimiento: {ex.Message}", ex);
            }
        }

        #endregion

        #region Search Methods

        /// <summary>
        /// Busca productos por descripción, código y/o categoría
        /// </summary>
        /// <param name="descripcion">Descripción del producto (opcional)</param>
        /// <param name="codigo">Código del producto (opcional)</param>
        /// <param name="categoria">Categoría del producto (opcional)</param>
        /// <returns>Lista de productos que coinciden con los criterios</returns>
        public List<Producto> BuscarProductos(string descripcion = "", string codigo = "", string categoria = "")
        {
            try
            {
                List<string> condiciones = new List<string>();
                List<SqlParameter> parametros = new List<SqlParameter>();

                if (!string.IsNullOrWhiteSpace(descripcion))
                {
                    condiciones.Add("p.Descripcion LIKE @Descripcion");
                    parametros.Add(new SqlParameter("@Descripcion", $"%{descripcion}%"));
                }

                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    condiciones.Add("p.Codigo LIKE @Codigo");
                    parametros.Add(new SqlParameter("@Codigo", $"%{codigo}%"));
                }

                if (!string.IsNullOrWhiteSpace(categoria))
                {
                    condiciones.Add("p.Categoria LIKE @Categoria");
                    parametros.Add(new SqlParameter("@Categoria", $"%{categoria}%"));
                }

                // Siempre filtrar por productos activos
                condiciones.Add("p.Estado = 1");

                string whereClause = condiciones.Count > 0 ? "WHERE " + string.Join(" AND ", condiciones) : "WHERE p.Estado = 1";

                string query = $@"
                    SELECT p.ID_Producto, p.Codigo, p.Descripcion, p.PrecioUnitario, p.Stock, p.StockMinimo, 
                           p.Categoria, p.FechaCreacion, p.Estado
                    FROM Productos p
                    {whereClause}
                    ORDER BY p.Descripcion";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parametros.ToArray());
                List<Producto> productos = new List<Producto>();

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }

                return productos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar productos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los últimos N productos agregados
        /// </summary>
        /// <param name="cantidad">Número de productos a obtener</param>
        /// <returns>Lista de los últimos productos</returns>
        public List<Producto> ObtenerUltimosProductos(int cantidad = 100)
        {
            try
            {
                string query = @"
                    SELECT TOP (@Cantidad) 
                           p.ID_Producto, p.Codigo, p.Descripcion, p.PrecioUnitario, p.Stock, p.StockMinimo, 
                           p.Categoria, p.FechaCreacion, p.Estado
                    FROM Productos p
                    WHERE p.Estado = 1
                    ORDER BY p.FechaCreacion DESC";

                SqlParameter[] parametros = { new SqlParameter("@Cantidad", cantidad) };
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parametros);
                List<Producto> productos = new List<Producto>();

                foreach (DataRow row in dt.Rows)
                {
                    productos.Add(MapearProducto(row));
                }

                return productos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener últimos productos: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
