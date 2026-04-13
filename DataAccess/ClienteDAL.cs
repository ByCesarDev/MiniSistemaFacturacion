using System;
using System.Collections.Generic;
using System.Data;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;
using System.Data.SqlClient;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase de Acceso a Datos para la entidad Cliente
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class ClienteDAL
    {
        #region CRUD Operations

        /// <summary>
        /// Inserta un nuevo cliente en la base de datos
        /// </summary>
        /// <param name="cliente">Cliente a insertar</param>
        /// <returns>ID del cliente insertado</returns>
        public int Insertar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentException("El cliente no puede ser nulo");

            if (!cliente.IsValid())
                throw new ArgumentException($"Cliente inválido: {cliente.GetValidationError()}");

            try
            {
                string query = @"
                    INSERT INTO Clientes (Nombre, Cedula, Direccion, Telefono, Email, TipoCliente, RNC)
                    VALUES (@Nombre, @Cedula, @Direccion, @Telefono, @Email, @TipoCliente, @RNC);
                    SELECT SCOPE_IDENTITY();";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Nombre", cliente.Nombre),
                    DbHelper.Instance.CreateParameter("@Cedula", cliente.Cedula),
                    DbHelper.Instance.CreateParameter("@Direccion", cliente.Direccion ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Email", cliente.Email ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@TipoCliente", cliente.TipoCliente),
                    DbHelper.Instance.CreateParameter("@RNC", cliente.RNC ?? (object)DBNull.Value)
                };

                object result = DbHelper.Instance.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar cliente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza un cliente existente
        /// </summary>
        /// <param name="cliente">Cliente con datos actualizados</param>
        /// <returns>Número de filas afectadas</returns>
        public int Actualizar(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentException("El cliente no puede ser nulo");

            if (cliente.ID_Cliente <= 0)
                throw new ArgumentException("El ID del cliente es requerido");

            if (!cliente.IsValid())
                throw new ArgumentException($"Cliente inválido: {cliente.GetValidationError()}");

            try
            {
                string query = @"
                    UPDATE Clientes 
                    SET Nombre = @Nombre, 
                        Cedula = @Cedula, 
                        Direccion = @Direccion, 
                        Telefono = @Telefono, 
                        Email = @Email,
                        TipoCliente = @TipoCliente,
                        RNC = @RNC
                    WHERE ID_Cliente = @ID_Cliente";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Cliente", cliente.ID_Cliente),
                    DbHelper.Instance.CreateParameter("@Nombre", cliente.Nombre),
                    DbHelper.Instance.CreateParameter("@Cedula", cliente.Cedula),
                    DbHelper.Instance.CreateParameter("@Direccion", cliente.Direccion ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Email", cliente.Email ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@TipoCliente", cliente.TipoCliente),
                    DbHelper.Instance.CreateParameter("@RNC", cliente.RNC ?? (object)DBNull.Value)
                };

                return DbHelper.Instance.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar cliente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina un cliente por su ID
        /// </summary>
        /// <param name="idCliente">ID del cliente a eliminar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Eliminar(int idCliente)
        {
            if (idCliente <= 0)
                throw new ArgumentException("El ID del cliente es requerido");

            try
            {
                string query = "DELETE FROM Clientes WHERE ID_Cliente = @ID_Cliente";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Cliente", idCliente);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar cliente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Desactiva un cliente (borrado lógico)
        /// </summary>
        /// <param name="idCliente">ID del cliente a desactivar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Desactivar(int idCliente)
        {
            if (idCliente <= 0)
                throw new ArgumentException("El ID del cliente es requerido");

            try
            {
                string query = "UPDATE Clientes SET Estado = 0 WHERE ID_Cliente = @ID_Cliente";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Cliente", idCliente);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar cliente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Activa un cliente
        /// </summary>
        /// <param name="idCliente">ID del cliente a activar</param>
        /// <returns>Número de filas afectadas</returns>
        public int Activar(int idCliente)
        {
            if (idCliente <= 0)
                throw new ArgumentException("El ID del cliente es requerido");

            try
            {
                string query = "UPDATE Clientes SET Estado = 1 WHERE ID_Cliente = @ID_Cliente";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Cliente", idCliente);

                return DbHelper.Instance.ExecuteNonQuery(query, new SqlParameter[] { parameter });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al activar cliente: {ex.Message}", ex);
            }
        }

        #endregion

        #region Query Operations

        /// <summary>
        /// Obtiene un cliente por su ID
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>Cliente encontrado o null</returns>
        public Cliente ObtenerPorId(int idCliente)
        {
            if (idCliente <= 0)
                return null;

            try
            {
                string query = @"
                    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                           TipoCliente, RNC, FechaCreacion, Estado
                    FROM Clientes
                    WHERE ID_Cliente = @ID_Cliente";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@ID_Cliente", idCliente);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                if (dt.Rows.Count > 0)
                {
                    return MapearCliente(dt.Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cliente: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene un cliente por su cédula
        /// </summary>
        /// <param name="cedula">Cédula del cliente</param>
        /// <returns>Cliente encontrado o null</returns>
        public Cliente ObtenerPorCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return null;

            try
            {
                string query = @"
                    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                           FechaCreacion, Estado
                    FROM Clientes
                    WHERE Cedula = @Cedula";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@Cedula", cedula);
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                if (dt.Rows.Count > 0)
                {
                    return MapearCliente(dt.Rows[0]);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener cliente por cédula: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        /// <returns>Lista de todos los clientes</returns>
        public List<Cliente> ObtenerTodos()
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                // Verificar si las columnas TipoCliente y RNC existen
                bool tieneTipoCliente = ColumnExists("Clientes", "TipoCliente");
                bool tieneRNC = ColumnExists("Clientes", "RNC");

                string query;
                if (tieneTipoCliente && tieneRNC)
                {
                    query = @"
                        SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                               TipoCliente, RNC, FechaCreacion, Estado
                        FROM Clientes
                        ORDER BY ID_Cliente";
                }
                else
                {
                    query = @"
                        SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                               FechaCreacion, Estado
                        FROM Clientes
                        ORDER BY ID_Cliente";
                }

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row, tieneTipoCliente, tieneRNC));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los clientes: {ex.Message}", ex);
            }

            return clientes;
        }

        /// <summary>
        /// Obtiene solo los clientes activos
        /// </summary>
        /// <returns>Lista de clientes activos</returns>
        public List<Cliente> ObtenerActivos()
        {
            List<Cliente> clientes = new List<Cliente>();

            try
            {
                string query = @"
                    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                           FechaCreacion, Estado
                    FROM Clientes
                    WHERE Estado = 1
                    ORDER BY ID_Cliente";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener clientes activos: {ex.Message}", ex);
            }

            return clientes;
        }

        /// <summary>
        /// Busca clientes por nombre o cédula
        /// </summary>
        /// <param name="terminoBusqueda">Término de búsqueda</param>
        /// <returns>Lista de clientes que coinciden con la búsqueda</returns>
        public List<Cliente> Buscar(string terminoBusqueda)
        {
            List<Cliente> clientes = new List<Cliente>();

            if (string.IsNullOrWhiteSpace(terminoBusqueda))
                return clientes;

            try
            {
                string query = @"
                    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                           FechaCreacion, Estado
                    FROM Clientes
                    WHERE (Nombre LIKE @TerminoBusqueda OR Cedula LIKE @TerminoBusqueda)
                    ORDER BY ID_Cliente";

                SqlParameter parameter = DbHelper.Instance.CreateParameter("@TerminoBusqueda", $"%{terminoBusqueda}%");
                DataTable dt = DbHelper.Instance.ExecuteQuery(query, new SqlParameter[] { parameter });

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar clientes: {ex.Message}", ex);
            }

            return clientes;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Verifica si un cliente existe por su ID
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existe</returns>
        public bool Existe(int idCliente)
        {
            if (idCliente <= 0)
                return false;

            try
            {
                string query = "SELECT COUNT(*) FROM Clientes WHERE ID_Cliente = @ID_Cliente";
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
        /// Verifica si un cliente existe por su cédula
        /// </summary>
        /// <param name="cedula">Cédula del cliente</param>
        /// <returns>True si existe</returns>
        public bool ExistePorCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            try
            {
                string query = "SELECT COUNT(*) FROM Clientes WHERE Cedula = @Cedula";
                SqlParameter parameter = DbHelper.Instance.CreateParameter("@Cedula", cedula);

                int count = Convert.ToInt32(DbHelper.Instance.ExecuteScalar(query, new SqlParameter[] { parameter }));
                return count > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica si un cliente está activo
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si está activo</returns>
        public bool EstaActivo(int idCliente)
        {
            if (idCliente <= 0)
                return false;

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

        #endregion

        #region Search Methods

        /// <summary>
        /// Busca clientes por nombre y/o RNC
        /// </summary>
        /// <param name="nombre">Nombre del cliente (opcional)</param>
        /// <param name="rnc">RNC/Cédula del cliente (opcional)</param>
        /// <returns>Lista de clientes que coinciden con los criterios</returns>
        public List<Cliente> BuscarClientes(string nombre = null, string rnc = null)
        {
            try
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                string query = "SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, FechaCreacion, Estado FROM Clientes WHERE Estado = 1";

                if (!string.IsNullOrWhiteSpace(nombre))
                {
                    query += " AND Nombre LIKE @Nombre";
                    parameters.Add(DbHelper.Instance.CreateParameter("@Nombre", $"%{nombre}%"));
                }

                if (!string.IsNullOrWhiteSpace(rnc))
                {
                    query += " AND Cedula LIKE @Cedula";
                    parameters.Add(DbHelper.Instance.CreateParameter("@Cedula", $"%{rnc}%"));
                }

                query += " ORDER BY Nombre";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parameters.ToArray());
                List<Cliente> clientes = new List<Cliente>();

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
                }

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar clientes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene los últimos N clientes registrados
        /// </summary>
        /// <param name="cantidad">Número de clientes a obtener</param>
        /// <returns>Lista de los últimos clientes</returns>
        public List<Cliente> ObtenerUltimosClientes(int cantidad = 100)
        {
            try
            {
                string query = @"
                    SELECT TOP (@Cantidad) ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, FechaCreacion, Estado 
                    FROM Clientes 
                    WHERE Estado = 1 
                    ORDER BY FechaCreacion DESC";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Cantidad", cantidad)
                };

                DataTable dt = DbHelper.Instance.ExecuteQuery(query, parameters);
                List<Cliente> clientes = new List<Cliente>();

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
                }

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener últimos clientes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca clientes por nombre (búsqueda parcial)
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre a buscar</param>
        /// <returns>Lista de clientes que coinciden</returns>
        public List<Cliente> BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return new List<Cliente>();

            return BuscarClientes(nombre, null);
        }

        /// <summary>
        /// Busca clientes por RNC/Cédula (búsqueda exacta o parcial)
        /// </summary>
        /// <param name="rnc">RNC/Cédula a buscar</param>
        /// <returns>Lista de clientes que coinciden</returns>
        public List<Cliente> BuscarPorRNC(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc))
                return new List<Cliente>();

            return BuscarClientes(null, rnc);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Obtiene todos los clientes activos
        /// </summary>
        /// <returns>Lista de clientes activos</returns>
        public List<Cliente> ObtenerClientesActivos()
        {
            try
            {
                string query = "SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, FechaCreacion, Estado FROM Clientes WHERE Estado = 1 ORDER BY Nombre";
                
                DataTable dt = DbHelper.Instance.ExecuteQuery(query);
                List<Cliente> clientes = new List<Cliente>();

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
                }

                return clientes;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener clientes activos: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene el número total de clientes
        /// </summary>
        /// <returns>Número total de clientes</returns>
        public int ObtenerTotalClientes()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Clientes";
                object result = DbHelper.Instance.ExecuteScalar(query);
                return Convert.ToInt32(result ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Obtiene el número de clientes activos
        /// </summary>
        /// <returns>Número de clientes activos</returns>
        public int ObtenerTotalClientesActivos()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Clientes WHERE Estado = 1";
                object result = DbHelper.Instance.ExecuteScalar(query);
                return Convert.ToInt32(result ?? 0);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Mapea un DataRow a un objeto Cliente
        /// </summary>
        /// <param name="row">DataRow con los datos del cliente</param>
        /// <returns>Objeto Cliente mapeado</returns>
        private Cliente MapearCliente(DataRow row)
        {
            return MapearCliente(row, true, true);
        }

        private Cliente MapearCliente(DataRow row, bool tieneTipoCliente, bool tieneRNC)
        {
            Cliente cliente = new Cliente
            {
                ID_Cliente = Convert.ToInt32(row["ID_Cliente"]),
                Nombre = row["Nombre"].ToString(),
                Cedula = row["Cedula"].ToString(),
                Direccion = row["Direccion"] != DBNull.Value ? row["Direccion"].ToString() : null,
                Telefono = row["Telefono"] != DBNull.Value ? row["Telefono"].ToString() : null,
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                Estado = Convert.ToBoolean(row["Estado"])
            };

            // Asignar TipoCliente y RNC solo si las columnas existen
            if (tieneTipoCliente && row.Table.Columns.Contains("TipoCliente"))
            {
                cliente.TipoCliente = row["TipoCliente"] != DBNull.Value ? row["TipoCliente"].ToString() : "CF";
            }
            else
            {
                cliente.TipoCliente = "CF"; // Valor por defecto
            }

            if (tieneRNC && row.Table.Columns.Contains("RNC"))
            {
                cliente.RNC = row["RNC"] != DBNull.Value ? row["RNC"].ToString() : null;
            }

            return cliente;
        }

        private bool ColumnExists(string tableName, string columnName)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@TableName", tableName),
                    DbHelper.Instance.CreateParameter("@ColumnName", columnName)
                };

                object result = DbHelper.Instance.ExecuteScalar(query, parameters);
                return Convert.ToInt32(result) > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Stored Procedures

        /// <summary>
        /// Inserta un cliente usando procedimiento almacenado
        /// </summary>
        /// <param name="cliente">Cliente a insertar</param>
        /// <returns>ID del cliente insertado</returns>
        public int InsertarConProcedimiento(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentException("El cliente no puede ser nulo");

            if (!cliente.IsValid())
                throw new ArgumentException($"Cliente inválido: {cliente.GetValidationError()}");

            try
            {
                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Nombre", cliente.Nombre),
                    DbHelper.Instance.CreateParameter("@Cedula", cliente.Cedula),
                    DbHelper.Instance.CreateParameter("@Direccion", cliente.Direccion ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Email", cliente.Email ?? (object)DBNull.Value)
                };

                DataTable dt = DbHelper.Instance.ExecuteStoredProcedure("sp_InsertarCliente", parameters);

                if (dt.Rows.Count > 0)
                {
                    return Convert.ToInt32(dt.Rows[0]["ID_Cliente"]);
                }

                throw new Exception("No se pudo obtener el ID del cliente insertado");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar cliente con procedimiento: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
