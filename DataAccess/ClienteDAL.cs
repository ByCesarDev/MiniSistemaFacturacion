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
                    INSERT INTO Clientes (Nombre, Cedula, Direccion, Telefono, Email)
                    VALUES (@Nombre, @Cedula, @Direccion, @Telefono, @Email);
                    SELECT SCOPE_IDENTITY();";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@Nombre", cliente.Nombre),
                    DbHelper.Instance.CreateParameter("@Cedula", cliente.Cedula),
                    DbHelper.Instance.CreateParameter("@Direccion", cliente.Direccion ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Email", cliente.Email ?? (object)DBNull.Value)
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
                        Email = @Email
                    WHERE ID_Cliente = @ID_Cliente";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    DbHelper.Instance.CreateParameter("@ID_Cliente", cliente.ID_Cliente),
                    DbHelper.Instance.CreateParameter("@Nombre", cliente.Nombre),
                    DbHelper.Instance.CreateParameter("@Cedula", cliente.Cedula),
                    DbHelper.Instance.CreateParameter("@Direccion", cliente.Direccion ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Telefono", cliente.Telefono ?? (object)DBNull.Value),
                    DbHelper.Instance.CreateParameter("@Email", cliente.Email ?? (object)DBNull.Value)
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
                           FechaCreacion, Estado
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
                string query = @"
                    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email, 
                           FechaCreacion, Estado
                    FROM Clientes
                    ORDER BY Nombre";

                DataTable dt = DbHelper.Instance.ExecuteQuery(query);

                foreach (DataRow row in dt.Rows)
                {
                    clientes.Add(MapearCliente(row));
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
                    ORDER BY Nombre";

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
                    ORDER BY Nombre";

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

        #region Utility Methods

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

            return cliente;
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
