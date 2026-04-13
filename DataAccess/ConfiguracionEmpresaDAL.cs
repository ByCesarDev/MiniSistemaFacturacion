using System;
using System.Data;
using System.Data.SqlClient;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Clase de acceso a datos para Configuración de Empresa
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public class ConfiguracionEmpresaDAL
    {
        #region Obtener Configuración

        /// <summary>
        /// Obtiene la configuración activa de la empresa
        /// </summary>
        /// <returns>DataTable con la configuración o null si no existe</returns>
        public DataTable ObtenerConfiguracionActiva()
        {
            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    string query = @"
                        SELECT ID, Nombre, Direccion, Telefono, Email, RNC, 
                               NCFActual, NCFConsumidorFinal, RutaPdfTickets,
                               FechaCreacion, FechaActualizacion, Activo
                        FROM ConfiguracionEmpresa 
                        WHERE Activo = 1";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt.Rows.Count > 0 ? dt : null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener configuración activa: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene la configuración como un objeto con propiedades
        /// </summary>
        /// <returns>Objeto anónimo con la configuración o null si no existe</returns>
        public dynamic ObtenerConfiguracionComoObjeto()
        {
            try
            {
                DataTable dt = ObtenerConfiguracionActiva();
                if (dt == null || dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];
                return new
                {
                    ID = Convert.ToInt32(row["ID"]),
                    Nombre = row["Nombre"].ToString(),
                    Direccion = row["Direccion"].ToString(),
                    Telefono = row["Telefono"].ToString(),
                    Email = row["Email"].ToString(),
                    RNC = row["RNC"].ToString(),
                    NCFActual = row["NCFActual"].ToString(),
                    NCFConsumidorFinal = row["NCFConsumidorFinal"].ToString(),
                    RutaPdfTickets = row["RutaPdfTickets"].ToString(),
                    FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                    FechaActualizacion = Convert.ToDateTime(row["FechaActualizacion"]),
                    Activo = Convert.ToBoolean(row["Activo"])
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener configuración como objeto: {ex.Message}", ex);
            }
        }

        #endregion

        #region Guardar/Actualizar Configuración

        /// <summary>
        /// Inserta una nueva configuración de empresa
        /// </summary>
        /// <param name="nombre">Nombre de la empresa</param>
        /// <param name="direccion">Dirección</param>
        /// <param name="telefono">Teléfono</param>
        /// <param name="email">Email</param>
        /// <param name="rnc">RNC</param>
        /// <param name="ncfActual">NCF Actual</param>
        /// <param name="ncfConsumidorFinal">NCF Consumidor Final</param>
        /// <param name="rutaPdfTickets">Ruta PDF Tickets</param>
        /// <returns>ID de la configuración insertada</returns>
        public int InsertarConfiguracion(string nombre, string direccion, string telefono, 
            string email, string rnc, string ncfActual, string ncfConsumidorFinal, string rutaPdfTickets)
        {
            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    // Primero desactivar todas las configuraciones existentes
                    DesactivarTodasLasConfiguraciones(connection);

                    string query = @"
                        INSERT INTO ConfiguracionEmpresa 
                        (Nombre, Direccion, Telefono, Email, RNC, NCFActual, NCFConsumidorFinal, 
                         RutaPdfTickets, FechaCreacion, FechaActualizacion, Activo)
                        VALUES 
                        (@Nombre, @Direccion, @Telefono, @Email, @RNC, @NCFActual, @NCFConsumidorFinal, 
                         @RutaPdfTickets, GETDATE(), GETDATE(), 1);
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", nombre ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", direccion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RNC", rnc ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NCFActual", ncfActual ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NCFConsumidorFinal", ncfConsumidorFinal ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RutaPdfTickets", rutaPdfTickets ?? (object)DBNull.Value);

                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar configuración: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza una configuración existente
        /// </summary>
        /// <param name="id">ID de la configuración</param>
        /// <param name="nombre">Nombre de la empresa</param>
        /// <param name="direccion">Dirección</param>
        /// <param name="telefono">Teléfono</param>
        /// <param name="email">Email</param>
        /// <param name="rnc">RNC</param>
        /// <param name="ncfActual">NCF Actual</param>
        /// <param name="ncfConsumidorFinal">NCF Consumidor Final</param>
        /// <param name="rutaPdfTickets">Ruta PDF Tickets</param>
        /// <returns>True si se actualizó correctamente</returns>
        public bool ActualizarConfiguracion(int id, string nombre, string direccion, string telefono, 
            string email, string rnc, string ncfActual, string ncfConsumidorFinal, string rutaPdfTickets)
        {
            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    string query = @"
                        UPDATE ConfiguracionEmpresa 
                        SET Nombre = @Nombre, 
                            Direccion = @Direccion, 
                            Telefono = @Telefono, 
                            Email = @Email, 
                            RNC = @RNC, 
                            NCFActual = @NCFActual, 
                            NCFConsumidorFinal = @NCFConsumidorFinal, 
                            RutaPdfTickets = @RutaPdfTickets,
                            FechaActualizacion = GETDATE()
                        WHERE ID = @ID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Nombre", nombre ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Direccion", direccion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Telefono", telefono ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RNC", rnc ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NCFActual", ncfActual ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@NCFConsumidorFinal", ncfConsumidorFinal ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@RutaPdfTickets", rutaPdfTickets ?? (object)DBNull.Value);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar configuración: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Guarda o actualiza la configuración (Upsert)
        /// </summary>
        /// <param name="nombre">Nombre de la empresa</param>
        /// <param name="direccion">Dirección</param>
        /// <param name="telefono">Teléfono</param>
        /// <param name="email">Email</param>
        /// <param name="rnc">RNC</param>
        /// <param name="ncfActual">NCF Actual</param>
        /// <param name="ncfConsumidorFinal">NCF Consumidor Final</param>
        /// <param name="rutaPdfTickets">Ruta PDF Tickets</param>
        /// <returns>ID de la configuración guardada</returns>
        public int GuardarConfiguracion(string nombre, string direccion, string telefono, 
            string email, string rnc, string ncfActual, string ncfConsumidorFinal, string rutaPdfTickets)
        {
            try
            {
                // Verificar si ya existe una configuración activa
                DataTable dt = ObtenerConfiguracionActiva();
                
                if (dt != null && dt.Rows.Count > 0)
                {
                    // Actualizar la configuración existente
                    int id = Convert.ToInt32(dt.Rows[0]["ID"]);
                    bool actualizado = ActualizarConfiguracion(id, nombre, direccion, telefono, 
                        email, rnc, ncfActual, ncfConsumidorFinal, rutaPdfTickets);
                    
                    if (actualizado)
                        return id;
                    else
                        throw new Exception("No se pudo actualizar la configuración existente.");
                }
                else
                {
                    // Insertar nueva configuración
                    return InsertarConfiguracion(nombre, direccion, telefono, 
                        email, rnc, ncfActual, ncfConsumidorFinal, rutaPdfTickets);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar configuración: {ex.Message}", ex);
            }
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Desactiva todas las configuraciones existentes
        /// </summary>
        /// <param name="connection">Conexión SQL abierta</param>
        private void DesactivarTodasLasConfiguraciones(SqlConnection connection)
        {
            try
            {
                string query = "UPDATE ConfiguracionEmpresa SET Activo = 0 WHERE Activo = 1";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar configuraciones: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si existe una configuración activa
        /// </summary>
        /// <returns>True si existe configuración activa</returns>
        public bool ExisteConfiguracionActiva()
        {
            try
            {
                DataTable dt = ObtenerConfiguracionActiva();
                return dt != null && dt.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de configuración: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una configuración por ID (desactivación lógica)
        /// </summary>
        /// <param name="id">ID de la configuración</param>
        /// <returns>True si se eliminó correctamente</returns>
        public bool EliminarConfiguracion(int id)
        {
            try
            {
                using (SqlConnection connection = DbHelper.Instance.GetConnection())
                {
                    string query = "UPDATE ConfiguracionEmpresa SET Activo = 0 WHERE ID = @ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ID", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar configuración: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
