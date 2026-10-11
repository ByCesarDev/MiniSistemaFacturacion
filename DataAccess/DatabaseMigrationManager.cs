using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace MiniSistemaFacturacion.DataAccess
{
    /// <summary>
    /// Resultado de la verificación o ejecución de migraciones de base de datos.
    /// </summary>
    public class MigrationResult
    {
        public bool Exito { get; set; }
        public int MigracionesAplicadas { get; set; }
        public string Mensaje { get; set; }
        public List<string> DetalleMigraciones { get; set; } = new List<string>();
        public string RutaBackup { get; set; }
        public bool BaseDatosNueva { get; set; }
    }

    /// <summary>
    /// Representa el estado de salud y esquema de la base de datos.
    /// </summary>
    public class DatabaseHealthStatus
    {
        public bool ServidorAccesible { get; set; }
        public bool BaseDatosExiste { get; set; }
        public bool EsquemaAlDia { get; set; }
        public int VersionActual { get; set; }
        public int TotalMigracionesPendientes { get; set; }
        public List<string> MigracionesPendientes { get; set; } = new List<string>();
        public string MensajeError { get; set; }
        public bool RequiereInicializacionCompleta => ServidorAccesible && (!BaseDatosExiste || VersionActual == 0);
    }

    /// <summary>
    /// Gestor de migraciones versionadas e inicialización segura de base de datos.
    /// Implementa exclusión mutua distribuida con sp_getapplock, respaldo previo y cero datos de prueba.
    /// </summary>
    public class DatabaseMigrationManager
    {
        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _serverName;

        public DatabaseMigrationManager(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            var builder = new SqlConnectionStringBuilder(_connectionString);
            _databaseName = builder.InitialCatalog;
            _serverName = builder.DataSource;

            if (!ConnectionStringManager.EsIdentificadorSqlValido(_databaseName))
            {
                throw new ArgumentException($"El nombre de base de datos '{_databaseName}' contiene caracteres inválidos.");
            }
        }

        #region Verificación de Estado

        /// <summary>
        /// Evalúa el estado del servidor, base de datos, integridad estructural y migraciones pendientes sin aplicar cambios.
        /// </summary>
        public DatabaseHealthStatus EvaluarEstado()
        {
            var status = new DatabaseHealthStatus();

            try
            {
                // 1. Probar conectividad al servidor SQL (vía master con timeout controlado)
                var serverBuilder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = 8
                };

                using (var conn = new SqlConnection(serverBuilder.ConnectionString))
                {
                    conn.Open();
                    status.ServidorAccesible = true;

                    // 2. Verificar existencia de la base de datos
                    using (var cmd = new SqlCommand("SELECT DB_ID(@dbName)", conn))
                    {
                        cmd.Parameters.AddWithValue("@dbName", _databaseName);
                        object result = cmd.ExecuteScalar();
                        status.BaseDatosExiste = (result != null && result != DBNull.Value);
                    }
                }

                if (!status.BaseDatosExiste)
                {
                    status.EsquemaAlDia = false;
                    status.VersionActual = 0;
                    status.TotalMigracionesPendientes = GetMigracionesDisponibles().Count;
                    status.MigracionesPendientes = GetMigracionesDisponibles().Select(m => m.Id).ToList();
                    return status;
                }

                // 3. Si existe la base de datos, comprobar versiones aplicadas en __SchemaVersions
                using (var connDb = new SqlConnection(_connectionString))
                {
                    connDb.Open();

                    bool existeTablaVersiones = false;
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM sys.tables WHERE name = '__SchemaVersions'", connDb))
                    {
                        existeTablaVersiones = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }

                    var versionesAplicadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    if (existeTablaVersiones)
                    {
                        using (var cmd = new SqlCommand("SELECT NombreScript FROM [dbo].[__SchemaVersions] WHERE Exito = 1", connDb))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                versionesAplicadas.Add(reader.GetString(0));
                            }
                        }
                    }

                    var todas = GetMigracionesDisponibles();
                    var pendientes = todas.Where(m => !versionesAplicadas.Contains(m.Id)).ToList();

                    status.VersionActual = versionesAplicadas.Count;
                    status.TotalMigracionesPendientes = pendientes.Count;
                    status.MigracionesPendientes = pendientes.Select(p => p.Id).ToList();
                    status.EsquemaAlDia = (pendientes.Count == 0);

                    // 4. Verificación de Integridad Estructural Real (no confiar ciegamente en __SchemaVersions)
                    if (status.EsquemaAlDia)
                    {
                        string sqlChequeo = @"
                            SELECT 
                                (SELECT COUNT(*) FROM sys.tables WHERE name IN ('Clientes', 'Productos', 'ConfiguracionEmpresa', 'SecuenciasNCF', 'Facturas', 'DetallesFactura', 'Pagos', 'Usuarios', 'AuditoriaUsuarios')) AS TotalTablas,
                                (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name IN ('PasswordHash', 'PasswordSalt', 'HashIterations', 'HashAlgorithm')) AS TotalColsUsuarios,
                                (SELECT COUNT(*) FROM sys.procedures WHERE name = 'sp_ExisteAdministradorActivo') AS TotalProcedimientos;";

                        using (var cmdCheck = new SqlCommand(sqlChequeo, connDb))
                        using (var reader = cmdCheck.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int tablas = Convert.ToInt32(reader["TotalTablas"]);
                                int cols = Convert.ToInt32(reader["TotalColsUsuarios"]);
                                int procs = Convert.ToInt32(reader["TotalProcedimientos"]);

                                if (tablas < 9 || cols < 4 || procs < 1)
                                {
                                    status.EsquemaAlDia = false;
                                    status.MensajeError = $"Discrepancia de integridad estructural detectada (Tablas: {tablas}/9, Columnas seguridad: {cols}/4, Procedimientos: {procs}/1). Se requiere reparar o aplicar las migraciones.";
                                    status.TotalMigracionesPendientes = todas.Count;
                                    status.MigracionesPendientes = todas.Select(t => t.Id).ToList();
                                }
                            }
                        }
                    }
                }

                return status;
            }
            catch (SqlException sqlEx)
            {
                status.ServidorAccesible = false;
                status.MensajeError = DiagnosticarFalloSql(sqlEx, _serverName);
                return status;
            }
            catch (Exception ex)
            {
                status.ServidorAccesible = false;
                status.MensajeError = ex.Message;
                return status;
            }
        }

        private static string DiagnosticarFalloSql(SqlException sqlEx, string serverName)
        {
            string host = serverName ?? "localhost";
            int port = 1433;
            string instancia = "";
            bool tieneInstancia = false;

            if (host.Contains(","))
            {
                var partes = host.Split(',');
                host = partes[0].Trim();
                int.TryParse(partes[1].Trim(), out port);
            }
            else if (host.Contains(@"\"))
            {
                var partes = host.Split('\\');
                host = partes[0].Trim();
                instancia = partes[1].Trim();
                tieneInstancia = true;
            }

            bool esLocal = string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(host, ".", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(host, "(local)", StringComparison.OrdinalIgnoreCase);

            if (!esLocal)
            {
                bool tcpResponde = ProbarSocketTcp(host, port, 2500);

                if (!tcpResponde)
                {
                    return $"No se pudo establecer comunicación TCP con '{host}:{port}'.\n\n" +
                           "Diagnóstico de Red Local (LAN):\n" +
                           "1. Verifique que el equipo servidor esté encendido y conectado a la misma red local.\n" +
                           "2. Verifique que el protocolo TCP/IP esté HABILITADO en 'SQL Server Configuration Manager' en el servidor.\n" +
                           "3. Verifique que el Firewall de Windows en el servidor permita tráfico entrante en el puerto TCP 1433 (Perfil Red Privada).\n" +
                           (tieneInstancia ? $"4. Al utilizar instancia con nombre ({instancia}), compruebe que el servicio 'SQL Server Browser' (UDP 1434) esté iniciado en el servidor.\n" : "") +
                           $"Detalle técnico ({sqlEx.Number}): {sqlEx.Message}";
                }

                if (sqlEx.Number == 18456)
                {
                    return $"Conexión TCP al servidor establecida con éxito en el puerto {port}, pero el inicio de sesión fue rechazado (Error 18456).\n\n" +
                           "Diagnóstico de Credenciales:\n" +
                           "1. Si utiliza Autenticación SQL, verifique que el usuario y la contraseña sean correctos.\n" +
                           "2. Verifique que SQL Server en el servidor tenga configurado el 'Modo de Autenticación Mixto' (SQL Server y Windows).\n" +
                           "3. Si utiliza Autenticación de Windows en red, compruebe que ambos equipos pertenezcan al mismo dominio o compartan cuentas válidas.\n" +
                           $"Detalle técnico: {sqlEx.Message}";
                }
            }
            else
            {
                if (sqlEx.Number == 26 || sqlEx.Number == 53 || sqlEx.Number == 40)
                {
                    return $"No se encontró la instancia local de SQL Server '{serverName}'.\n\n" +
                           "Diagnóstico Local:\n" +
                           "1. Verifique que el servicio 'SQL Server (SQLEXPRESS)' esté en ejecución (services.msc).\n" +
                           "2. Verifique el nombre exacto de la instancia instalada.\n" +
                           $"Detalle técnico: {sqlEx.Message}";
                }
            }

            return $"Error al conectar con SQL Server ({sqlEx.Number}): {sqlEx.Message}";
        }

        private static bool ProbarSocketTcp(string host, int puerto, int timeoutMs)
        {
            try
            {
                using (var client = new System.Net.Sockets.TcpClient())
                {
                    var result = client.BeginConnect(host, puerto, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeoutMs);
                    if (!success) return false;
                    client.EndConnect(result);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Ejecución de Migraciones

        /// <summary>
        /// Crea la base de datos si no existe y aplica todas las migraciones pendientes.
        /// Exige bloqueo exclusivo previo a nivel de sesión y respaldo preventivo obligatorio verificado.
        /// Si el respaldo falla en una base existente con datos, LA MIGRACIÓN SE CANCELA INMEDIATAMENTE.
        /// </summary>
        /// <param name="crearBackupPrevio">True para realizar respaldo obligatorio antes de modificar tablas existentes.</param>
        public MigrationResult AplicarMigraciones(bool crearBackupPrevio = true)
        {
            var result = new MigrationResult();

            try
            {
                // 1. Validar nombre y escapar identificador de base de datos de forma segura
                string dbEscapada = ConnectionStringManager.EscaparIdentificadorSql(_databaseName);

                // 2. Asegurar la existencia física de la base de datos en master
                var serverBuilder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master",
                    ConnectTimeout = 15
                };

                using (var connMaster = new SqlConnection(serverBuilder.ConnectionString))
                {
                    connMaster.Open();

                    bool existeBd = false;
                    using (var cmdCheck = new SqlCommand("SELECT DB_ID(@dbName)", connMaster))
                    {
                        cmdCheck.Parameters.AddWithValue("@dbName", _databaseName);
                        object val = cmdCheck.ExecuteScalar();
                        existeBd = (val != null && val != DBNull.Value);
                    }

                    if (!existeBd)
                    {
                        using (var cmdCreate = new SqlCommand($"CREATE DATABASE {dbEscapada}", connMaster))
                        {
                            cmdCreate.ExecuteNonQuery();
                        }
                        result.BaseDatosNueva = true;
                        result.DetalleMigraciones.Add($"Base de datos {_databaseName} creada exitosamente.");
                    }
                }

                // 3. Conectarse a la base de datos destino y ejecutar proceso bajo bloqueo distribuido
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // 3.1 Adquirir bloqueo exclusivo distribuido a nivel de SESIÓN antes de cualquier evaluación
                    if (!AdquirirBloqueoSesion(conn, 30000))
                    {
                        result.Exito = false;
                        result.Mensaje = "No se pudo adquirir el bloqueo de migración: otra instancia podría estar actualizando la base de datos en este momento.";
                        return result;
                    }

                    try
                    {
                        // 3.2 Asegurar tabla __SchemaVersions dentro del bloqueo
                        AsegurarTablaVersiones(conn);

                        // 3.3 Re-consultar migraciones aplicadas bajo el bloqueo exclusivo
                        var aplicadas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        using (var cmdVersiones = new SqlCommand("SELECT NombreScript FROM [dbo].[__SchemaVersions] WHERE Exito = 1", conn))
                        using (var reader = cmdVersiones.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                aplicadas.Add(reader.GetString(0));
                            }
                        }

                        var pendientes = GetMigracionesDisponibles()
                            .Where(m => !aplicadas.Contains(m.Id))
                            .OrderBy(m => m.Orden)
                            .ToList();

                        if (pendientes.Count == 0)
                        {
                            result.Exito = true;
                            result.Mensaje = "La base de datos ya se encuentra en la versión más reciente.";
                            return result;
                        }

                        // 3.4 Respaldo Preventivo OBLIGATORIO si la base es existente y contiene tablas
                        if (!result.BaseDatosNueva && crearBackupPrevio)
                        {
                            string rutaBackup;
                            string errorBackup;
                            if (!CrearBackupPreventivoObligatorio(conn, out rutaBackup, out errorBackup))
                            {
                                result.Exito = false;
                                result.Mensaje = $"No se pudo completar el respaldo preventivo obligatorio de la base de datos existente:\n{errorBackup}\n\nLa migración ha sido CANCELADA para proteger la información.";
                                result.DetalleMigraciones.Add($"CANCELADO: Fallo en respaldo preventivo ({errorBackup}).");
                                return result;
                            }

                            if (!string.IsNullOrEmpty(rutaBackup))
                            {
                                result.RutaBackup = rutaBackup;
                                result.DetalleMigraciones.Add($"Respaldo preventivo verificado en: {rutaBackup}");
                            }
                        }

                        // 3.5 Ejecución secuencial transaccional de cada migración pendiente
                        foreach (var migracion in pendientes)
                        {
                            using (var trans = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                            {
                                try
                                {
                                    // Ejecutar cada lote SQL de la migración
                                    foreach (var loteSql in migracion.LotesSql)
                                    {
                                        using (var cmd = new SqlCommand(loteSql, conn, trans))
                                        {
                                            cmd.CommandTimeout = 120;
                                            cmd.ExecuteNonQuery();
                                        }
                                    }

                                    // Registrar migración completada en __SchemaVersions
                                    using (var cmdReg = new SqlCommand(@"
                                        INSERT INTO [dbo].[__SchemaVersions] 
                                        ([Version], [NombreScript], [FechaAplicacion], [Exito], [Descripcion])
                                        VALUES (@ver, @nombre, GETDATE(), 1, @desc)", conn, trans))
                                    {
                                        cmdReg.Parameters.AddWithValue("@ver", migracion.Version);
                                        cmdReg.Parameters.AddWithValue("@nombre", migracion.Id);
                                        cmdReg.Parameters.AddWithValue("@desc", migracion.Descripcion ?? (object)DBNull.Value);
                                        cmdReg.ExecuteNonQuery();
                                    }

                                    trans.Commit();
                                    result.MigracionesAplicadas++;
                                    result.DetalleMigraciones.Add($"Migración {migracion.Id} ({migracion.Version}) aplicada con éxito.");
                                }
                                catch (Exception exMigracion)
                                {
                                    try { trans.Rollback(); } catch { }

                                    result.Exito = false;
                                    result.Mensaje = $"Error al aplicar migración {migracion.Id}: {exMigracion.Message}";
                                    result.DetalleMigraciones.Add($"FALLO en {migracion.Id}: {exMigracion.Message}");
                                    return result;
                                }
                            }
                        }

                        result.Exito = true;
                        result.Mensaje = $"Se aplicaron {result.MigracionesAplicadas} migraciones correctamente.";
                        return result;
                    }
                    finally
                    {
                        // 3.6 Liberar siempre el bloqueo de migración de sesión al finalizar
                        LiberarBloqueoSesion(conn);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Exito = false;
                result.Mensaje = $"Error general en la migración: {ex.Message}";
                return result;
            }
        }

        #endregion

        #region Métodos Auxiliares de Soporte

        private void AsegurarTablaVersiones(SqlConnection conn)
        {
            string sql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__SchemaVersions')
                BEGIN
                    CREATE TABLE [dbo].[__SchemaVersions](
                        [Id] [int] IDENTITY(1,1) NOT NULL,
                        [Version] [varchar](20) NOT NULL,
                        [NombreScript] [varchar](150) NOT NULL,
                        [FechaAplicacion] [datetime] NOT NULL CONSTRAINT [DF_SchemaVersions_Fecha] DEFAULT (GETDATE()),
                        [Exito] [bit] NOT NULL CONSTRAINT [DF_SchemaVersions_Exito] DEFAULT (1),
                        [Descripcion] [nvarchar](255) NULL,
                        CONSTRAINT [PK_SchemaVersions] PRIMARY KEY CLUSTERED ([Id] ASC),
                        CONSTRAINT [UQ_SchemaVersions_Nombre] UNIQUE NONCLUSTERED ([NombreScript] ASC)
                    );
                END";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private bool AdquirirBloqueoSesion(SqlConnection conn, int timeoutMs = 30000)
        {
            using (var cmd = new SqlCommand("sp_getapplock", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Resource", "MiniSistemaFacturacion_MigrationLock");
                cmd.Parameters.AddWithValue("@LockMode", "Exclusive");
                cmd.Parameters.AddWithValue("@LockOwner", "Session");
                cmd.Parameters.AddWithValue("@LockTimeout", timeoutMs);

                var returnParam = cmd.Parameters.Add("@Result", SqlDbType.Int);
                returnParam.Direction = ParameterDirection.ReturnValue;

                cmd.ExecuteNonQuery();
                int result = (int)returnParam.Value;
                return result >= 0; // 0: concedido inmediatamente, 1: concedido tras espera
            }
        }

        private void LiberarBloqueoSesion(SqlConnection conn)
        {
            try
            {
                using (var cmd = new SqlCommand("sp_releaseapplock", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Resource", "MiniSistemaFacturacion_MigrationLock");
                    cmd.Parameters.AddWithValue("@LockOwner", "Session");
                    cmd.ExecuteNonQuery();
                }
            }
            catch { }
        }

        /// <summary>
        /// Crea una copia de seguridad obligatoria y verifica su integridad con RESTORE VERIFYONLY.
        /// Si la base no tiene tablas con datos, no es necesario respaldar.
        /// Si la base tiene tablas pero el respaldo falla, devuelve false con el error para abortar la migración.
        /// </summary>
        private bool CrearBackupPreventivoObligatorio(SqlConnection conn, out string rutaBackup, out string errorMensaje)
        {
            rutaBackup = null;
            errorMensaje = null;

            try
            {
                // 1. Verificar si hay tablas de usuario en la base de datos
                using (var cmdCount = new SqlCommand("SELECT COUNT(*) FROM sys.tables WHERE name <> '__SchemaVersions'", conn))
                {
                    int tablas = Convert.ToInt32(cmdCount.ExecuteScalar());
                    if (tablas == 0) return true; // Base vacía, no requiere respaldo previo
                }

                // 2. Obtener la ruta predeterminada de backup configurada en SQL Server (donde el servicio tiene permisos garantizados)
                string dirBackups = null;
                try
                {
                    using (var cmdPath = new SqlCommand("SELECT CAST(SERVERPROPERTY('InstanceDefaultBackupPath') AS NVARCHAR(512))", conn))
                    {
                        object p = cmdPath.ExecuteScalar();
                        if (p != null && p != DBNull.Value)
                        {
                            dirBackups = p.ToString();
                        }
                    }
                }
                catch { }

                // Fallback a ProgramData si el servidor no reportó ruta predeterminada
                if (string.IsNullOrWhiteSpace(dirBackups) || !Directory.Exists(dirBackups))
                {
                    dirBackups = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "MiniSistemaFacturacion", "Backups");
                }

                if (!Directory.Exists(dirBackups))
                {
                    Directory.CreateDirectory(dirBackups);
                }

                string nombreArchivo = $"{_databaseName}_PreMigration_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                rutaBackup = Path.Combine(dirBackups, nombreArchivo);

                string dbEscapada = ConnectionStringManager.EscaparIdentificadorSql(_databaseName);
                string sqlBackup = $"BACKUP DATABASE {dbEscapada} TO DISK = @rutaBackup WITH FORMAT, INIT, NAME = 'Respaldo Pre-Migracion';";

                using (var cmdBackup = new SqlCommand(sqlBackup, conn))
                {
                    cmdBackup.CommandTimeout = 300;
                    cmdBackup.Parameters.AddWithValue("@rutaBackup", rutaBackup);
                    cmdBackup.ExecuteNonQuery();
                }

                // 3. Verificación de integridad con RESTORE VERIFYONLY
                string sqlVerify = "RESTORE VERIFYONLY FROM DISK = @rutaBackup;";
                using (var cmdVerify = new SqlCommand(sqlVerify, conn))
                {
                    cmdVerify.CommandTimeout = 120;
                    cmdVerify.Parameters.AddWithValue("@rutaBackup", rutaBackup);
                    cmdVerify.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMensaje = ex.Message;
                rutaBackup = null;
                return false;
            }
        }

        #endregion

        #region Definición de Migraciones Oficiales

        private class MigracionDefinicion
        {
            public int Orden { get; set; }
            public string Id { get; set; }
            public string Version { get; set; }
            public string Descripcion { get; set; }
            public List<string> LotesSql { get; set; } = new List<string>();
        }

        private List<MigracionDefinicion> GetMigracionesDisponibles()
        {
            return new List<MigracionDefinicion>
            {
                // -------------------------------------------------------------
                // M001: Esquema Base (Tablas de negocio, relaciones, índices)
                // -------------------------------------------------------------
                new MigracionDefinicion
                {
                    Orden = 1,
                    Id = "M001_EsquemaBase",
                    Version = "1.0.0",
                    Descripcion = "Creación segura de tablas base (Clientes, Productos, Facturas, Pagos, Configuración).",
                    LotesSql = new List<string>
                    {
                        // 1. Clientes
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
                        BEGIN
                            CREATE TABLE [dbo].[Clientes](
                                [ID_Cliente] [int] IDENTITY(1,1) NOT NULL,
                                [Nombre] [nvarchar](100) NOT NULL,
                                [Cedula] [nvarchar](20) NOT NULL,
                                [Direccion] [nvarchar](200) NULL,
                                [Telefono] [nvarchar](25) NULL,
                                [Email] [nvarchar](100) NULL,
                                [TipoCliente] [varchar](3) NOT NULL CONSTRAINT [DF_Clientes_Tipo] DEFAULT ('CF'),
                                [RNC] [nvarchar](20) NULL,
                                [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_Clientes_Fecha] DEFAULT (GETDATE()),
                                [Estado] [bit] NOT NULL CONSTRAINT [DF_Clientes_Estado] DEFAULT (1),
                                CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([ID_Cliente] ASC),
                                CONSTRAINT [UQ_Clientes_Cedula] UNIQUE NONCLUSTERED ([Cedula] ASC)
                            );
                        END",

                        // 2. Productos
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Productos')
                        BEGIN
                            CREATE TABLE [dbo].[Productos](
                                [ID_Producto] [int] IDENTITY(1,1) NOT NULL,
                                [Codigo] [nvarchar](50) NOT NULL,
                                [Descripcion] [nvarchar](200) NOT NULL,
                                [PrecioUnitario] [decimal](18,2) NOT NULL,
                                [Stock] [int] NOT NULL CONSTRAINT [DF_Productos_Stock] DEFAULT (0),
                                [StockMinimo] [int] NOT NULL CONSTRAINT [DF_Productos_StockMin] DEFAULT (5),
                                [Categoria] [nvarchar](50) NULL,
                                [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_Productos_Fecha] DEFAULT (GETDATE()),
                                [Estado] [bit] NOT NULL CONSTRAINT [DF_Productos_Estado] DEFAULT (1),
                                CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([ID_Producto] ASC),
                                CONSTRAINT [UQ_Productos_Codigo] UNIQUE NONCLUSTERED ([Codigo] ASC)
                            );
                        END",

                        // 3. Configuración de Empresa (Estructura base)
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionEmpresa')
                        BEGIN
                            CREATE TABLE [dbo].[ConfiguracionEmpresa](
                                [ID] [int] IDENTITY(1,1) NOT NULL,
                                [Nombre] [nvarchar](200) NOT NULL,
                                [Direccion] [nvarchar](500) NULL,
                                [Telefono] [nvarchar](50) NULL,
                                [Email] [nvarchar](100) NULL,
                                [RNC] [nvarchar](20) NOT NULL,
                                [NCFActual] [nvarchar](20) NULL,
                                [NCFConsumidorFinal] [nvarchar](20) NULL,
                                [RutaPdfTickets] [nvarchar](250) NULL,
                                [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_ConfiguracionEmpresa_FCreacion] DEFAULT (GETDATE()),
                                [FechaActualizacion] [datetime] NULL,
                                [Activo] [bit] NOT NULL CONSTRAINT [DF_ConfiguracionEmpresa_Activo] DEFAULT (1),
                                CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY CLUSTERED ([ID] ASC)
                            );

                            INSERT INTO [dbo].[ConfiguracionEmpresa] 
                            ([Nombre], [RNC], [Direccion], [Telefono], [Email], [Activo])
                            VALUES ('Mi Empresa Comercial', '000000000', 'Dirección Principal', '809-000-0000', 'info@empresa.com', 1);
                        END",

                        // 4. Secuencias NCF
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SecuenciasNCF')
                        BEGIN
                            CREATE TABLE [dbo].[SecuenciasNCF](
                                [TipoComprobante] [varchar](2) NOT NULL,
                                [Prefijo] [varchar](10) NOT NULL,
                                [UltimoNumero] [int] NOT NULL CONSTRAINT [DF_NCF_UltNum] DEFAULT (0),
                                CONSTRAINT [PK_SecuenciasNCF] PRIMARY KEY CLUSTERED ([TipoComprobante] ASC)
                            );
                        END",

                        // 5. Facturas
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Facturas')
                        BEGIN
                            CREATE TABLE [dbo].[Facturas](
                                [ID_Factura] [int] IDENTITY(1,1) NOT NULL,
                                [NumeroFactura] [nvarchar](20) NOT NULL,
                                [NCF] [nvarchar](19) NULL,
                                [ID_Cliente] [int] NOT NULL,
                                [Fecha] [datetime] NOT NULL CONSTRAINT [DF_Facturas_Fecha] DEFAULT (GETDATE()),
                                [TotalBruto] [decimal](18,2) NOT NULL CONSTRAINT [DF_Facturas_TotalBruto] DEFAULT (0),
                                [PorcentajeImpuesto] [decimal](5,2) NOT NULL CONSTRAINT [DF_Facturas_ITBIS] DEFAULT (18.00),
                                [ValorImpuesto] [decimal](18,2) NOT NULL CONSTRAINT [DF_Facturas_ValImp] DEFAULT (0),
                                [TotalNeto] [decimal](18,2) NOT NULL CONSTRAINT [DF_Facturas_TotNet] DEFAULT (0),
                                [SaldoPendiente] [decimal](18,2) NOT NULL CONSTRAINT [DF_Facturas_Saldo] DEFAULT (0),
                                [Estado] [nvarchar](20) NOT NULL CONSTRAINT [DF_Facturas_Estado] DEFAULT ('Pendiente'),
                                [TipoComprobante] [nvarchar](10) NULL,
                                [TipoVenta] [varchar](20) NULL,
                                [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_Facturas_FCreacion] DEFAULT (GETDATE()),
                                CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([ID_Factura] ASC),
                                CONSTRAINT [UQ_Facturas_Numero] UNIQUE NONCLUSTERED ([NumeroFactura] ASC),
                                CONSTRAINT [FK_Facturas_Clientes] FOREIGN KEY ([ID_Cliente]) REFERENCES [dbo].[Clientes] ([ID_Cliente])
                            );
                        END",

                        // 6. Detalles Factura
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DetallesFactura')
                        BEGIN
                            CREATE TABLE [dbo].[DetallesFactura](
                                [ID_Detalle] [int] IDENTITY(1,1) NOT NULL,
                                [ID_Factura] [int] NOT NULL,
                                [ID_Producto] [int] NOT NULL,
                                [Cantidad] [int] NOT NULL,
                                [PrecioUnitarioVenta] [decimal](18,2) NOT NULL,
                                [Subtotal] [decimal](18,2) NOT NULL,
                                CONSTRAINT [PK_DetallesFactura] PRIMARY KEY CLUSTERED ([ID_Detalle] ASC),
                                CONSTRAINT [FK_DetallesFactura_Facturas] FOREIGN KEY ([ID_Factura]) REFERENCES [dbo].[Facturas] ([ID_Factura]) ON DELETE CASCADE,
                                CONSTRAINT [FK_DetallesFactura_Productos] FOREIGN KEY ([ID_Producto]) REFERENCES [dbo].[Productos] ([ID_Producto])
                            );
                        END",

                        // 7. Pagos
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Pagos')
                        BEGIN
                            CREATE TABLE [dbo].[Pagos](
                                [ID_Pago] [int] IDENTITY(1,1) NOT NULL,
                                [ID_Factura] [int] NOT NULL,
                                [FechaPago] [datetime] NOT NULL CONSTRAINT [DF_Pagos_Fecha] DEFAULT (GETDATE()),
                                [MontoPagado] [decimal](18,2) NOT NULL,
                                [FormaPago] [nvarchar](50) NOT NULL CONSTRAINT [DF_Pagos_Forma] DEFAULT ('Efectivo'),
                                [Referencia] [nvarchar](100) NULL,
                                [Observaciones] [nvarchar](500) NULL,
                                [Estado] [bit] NOT NULL CONSTRAINT [DF_Pagos_Estado] DEFAULT (1),
                                CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED ([ID_Pago] ASC),
                                CONSTRAINT [FK_Pagos_Facturas] FOREIGN KEY ([ID_Factura]) REFERENCES [dbo].[Facturas] ([ID_Factura])
                            );
                        END"
                    }
                },

                // -------------------------------------------------------------
                // M002: Procedimientos Almacenados Idempotentes
                // -------------------------------------------------------------
                new MigracionDefinicion
                {
                    Orden = 2,
                    Id = "M002_ProcedimientosAlmacenados",
                    Version = "1.1.0",
                    Descripcion = "Procedimientos almacenados para gestión de clientes, facturación y reportes.",
                    LotesSql = new List<string>
                    {
                        // sp_InsertarCliente
                        @"IF OBJECT_ID('dbo.sp_InsertarCliente', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_InsertarCliente;",
                        @"CREATE PROCEDURE dbo.sp_InsertarCliente
                            @Nombre NVARCHAR(100),
                            @Cedula NVARCHAR(20),
                            @Direccion NVARCHAR(200) = NULL,
                            @Telefono NVARCHAR(25) = NULL,
                            @Email NVARCHAR(100) = NULL,
                            @TipoCliente VARCHAR(3) = 'CF',
                            @RNC VARCHAR(20) = NULL
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            IF @TipoCliente = 'CCF' AND (@RNC IS NULL OR @RNC = '')
                            BEGIN
                                RAISERROR('Los clientes de Crédito Fiscal deben tener RNC', 16, 1);
                                RETURN;
                            END
                            IF @TipoCliente = 'CF' AND @RNC IS NOT NULL AND @RNC != ''
                            BEGIN
                                SET @RNC = NULL;
                            END
                            INSERT INTO Clientes (Nombre, Cedula, Direccion, Telefono, Email, TipoCliente, RNC)
                            VALUES (@Nombre, @Cedula, @Direccion, @Telefono, @Email, @TipoCliente, @RNC);
                            SELECT SCOPE_IDENTITY() AS ID_Cliente;
                        END",

                        // sp_ActualizarCliente
                        @"IF OBJECT_ID('dbo.sp_ActualizarCliente', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ActualizarCliente;",
                        @"CREATE PROCEDURE dbo.sp_ActualizarCliente
                            @ID_Cliente INT,
                            @Nombre NVARCHAR(100),
                            @Cedula NVARCHAR(20),
                            @Direccion NVARCHAR(200) = NULL,
                            @Telefono NVARCHAR(25) = NULL,
                            @Email NVARCHAR(100) = NULL,
                            @TipoCliente VARCHAR(3) = 'CF',
                            @RNC VARCHAR(20) = NULL
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            IF @TipoCliente = 'CCF' AND (@RNC IS NULL OR @RNC = '')
                            BEGIN
                                RAISERROR('Los clientes de Crédito Fiscal deben tener RNC', 16, 1);
                                RETURN;
                            END
                            IF @TipoCliente = 'CF' AND @RNC IS NOT NULL AND @RNC != ''
                            BEGIN
                                SET @RNC = NULL;
                            END
                            UPDATE Clientes
                            SET Nombre = @Nombre,
                                Cedula = @Cedula,
                                Direccion = @Direccion,
                                Telefono = @Telefono,
                                Email = @Email,
                                TipoCliente = @TipoCliente,
                                RNC = @RNC
                            WHERE ID_Cliente = @ID_Cliente;
                        END",

                        // sp_ObtenerSiguienteNCF
                        @"IF OBJECT_ID('dbo.sp_ObtenerSiguienteNCF', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ObtenerSiguienteNCF;",
                        @"CREATE PROCEDURE dbo.sp_ObtenerSiguienteNCF
                            @TipoComprobante VARCHAR(2),
                            @NCF VARCHAR(20) OUTPUT
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            DECLARE @Prefijo VARCHAR(10);
                            DECLARE @Ultimo INT;

                            SELECT @Prefijo = Prefijo, @Ultimo = UltimoNumero
                            FROM SecuenciasNCF
                            WHERE TipoComprobante = @TipoComprobante;

                            IF @Prefijo IS NULL
                            BEGIN
                                RAISERROR('No hay secuencia configurada para este comprobante', 16, 1);
                                RETURN;
                            END

                            SET @Ultimo = @Ultimo + 1;

                            UPDATE SecuenciasNCF
                            SET UltimoNumero = @Ultimo
                            WHERE TipoComprobante = @TipoComprobante;

                            SET @NCF = @Prefijo + RIGHT('00000000' + CAST(@Ultimo AS VARCHAR(8)), 8);
                        END"
                    }
                },

                // -------------------------------------------------------------
                // M003: Seguridad Criptográfica PBKDF2 y Auditoría
                // -------------------------------------------------------------
                new MigracionDefinicion
                {
                    Orden = 3,
                    Id = "M003_SeguridadYAuditoria",
                    Version = "1.2.0",
                    Descripcion = "Tabla de Usuarios con PBKDF2-HMAC-SHA256, Auditoría de eventos y compatibilidad incremental.",
                    LotesSql = new List<string>
                    {
                        // 1. Tabla Usuarios (creación si no existe)
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
                        BEGIN
                            CREATE TABLE [dbo].[Usuarios](
                                [ID_Usuario] [int] IDENTITY(1,1) NOT NULL,
                                [NombreCompleto] [nvarchar](150) NOT NULL,
                                [NombreUsuario] [nvarchar](50) NOT NULL,
                                [PasswordHash] [nvarchar](256) NOT NULL,
                                [PasswordSalt] [nvarchar](128) NOT NULL,
                                [HashIterations] [int] NOT NULL CONSTRAINT [DF_Usuarios_HashIterations] DEFAULT (100000),
                                [HashAlgorithm] [nvarchar](50) NOT NULL CONSTRAINT [DF_Usuarios_HashAlgorithm] DEFAULT ('PBKDF2-SHA256'),
                                [Rol] [nvarchar](20) NOT NULL,
                                [Estado] [nvarchar](20) NOT NULL CONSTRAINT [DF_Usuarios_Estado] DEFAULT ('Activo'),
                                [IntentosFallidos] [int] NOT NULL CONSTRAINT [DF_Usuarios_IntentosFallidos] DEFAULT (0),
                                [BloqueadoHasta] [datetime] NULL,
                                [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_Usuarios_FechaCreacion] DEFAULT (GETDATE()),
                                [UltimoAcceso] [datetime] NULL,
                                CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([ID_Usuario] ASC),
                                CONSTRAINT [UQ_Usuarios_NombreUsuario] UNIQUE NONCLUSTERED ([NombreUsuario] ASC),
                                CONSTRAINT [CK_Usuarios_Rol] CHECK ([Rol] IN ('Administrador', 'Empleado')),
                                CONSTRAINT [CK_Usuarios_Estado] CHECK ([Estado] IN ('Activo', 'Inactivo', 'Bloqueado'))
                            );
                        END",

                        // 2. Columnas incrementales si la tabla Usuarios ya existía
                        @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
                        BEGIN
                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'HashIterations')
                                ALTER TABLE [dbo].[Usuarios] ADD [HashIterations] [int] NOT NULL CONSTRAINT [DF_Usuarios_HashIterations_Alt] DEFAULT (100000);

                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'HashAlgorithm')
                                ALTER TABLE [dbo].[Usuarios] ADD [HashAlgorithm] [nvarchar](50) NOT NULL CONSTRAINT [DF_Usuarios_HashAlgorithm_Alt] DEFAULT ('PBKDF2-SHA256');

                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'IntentosFallidos')
                                ALTER TABLE [dbo].[Usuarios] ADD [IntentosFallidos] [int] NOT NULL CONSTRAINT [DF_Usuarios_IntentosFallidos_Alt] DEFAULT (0);

                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'BloqueadoHasta')
                                ALTER TABLE [dbo].[Usuarios] ADD [BloqueadoHasta] [datetime] NULL;

                            IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Usuarios') AND name = 'UltimoAcceso')
                                ALTER TABLE [dbo].[Usuarios] ADD [UltimoAcceso] [datetime] NULL;
                        END",

                        // 3. Índices de optimización de Usuarios
                        @"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_NombreUsuario' AND object_id = OBJECT_ID('Usuarios'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX [IX_Usuarios_NombreUsuario]
                            ON [dbo].[Usuarios] ([NombreUsuario])
                            INCLUDE ([ID_Usuario], [NombreCompleto], [PasswordHash], [PasswordSalt], [Rol], [Estado], [IntentosFallidos], [BloqueadoHasta]);
                        END",

                        @"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_Rol_Estado' AND object_id = OBJECT_ID('Usuarios'))
                        BEGIN
                            CREATE NONCLUSTERED INDEX [IX_Usuarios_Rol_Estado]
                            ON [dbo].[Usuarios] ([Rol], [Estado]);
                        END",

                        // 4. Tabla AuditoriaUsuarios
                        @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditoriaUsuarios')
                        BEGIN
                            CREATE TABLE [dbo].[AuditoriaUsuarios](
                                [ID_Auditoria] [int] IDENTITY(1,1) NOT NULL,
                                [ID_Usuario] [int] NULL,
                                [NombreUsuario] [nvarchar](50) NOT NULL,
                                [Accion] [nvarchar](100) NOT NULL,
                                [Detalles] [nvarchar](255) NULL,
                                [Fecha] [datetime] NOT NULL CONSTRAINT [DF_AuditoriaUsuarios_Fecha] DEFAULT (GETDATE()),
                                [Equipo] [nvarchar](100) NULL,
                                CONSTRAINT [PK_AuditoriaUsuarios] PRIMARY KEY CLUSTERED ([ID_Auditoria] ASC),
                                CONSTRAINT [FK_AuditoriaUsuarios_Usuarios] FOREIGN KEY ([ID_Usuario]) 
                                    REFERENCES [dbo].[Usuarios] ([ID_Usuario]) ON DELETE SET NULL
                            );
                        END",

                        // 5. Procedimiento sp_ExisteAdministradorActivo
                        @"IF OBJECT_ID('dbo.sp_ExisteAdministradorActivo', 'P') IS NOT NULL DROP PROCEDURE dbo.sp_ExisteAdministradorActivo;",
                        @"CREATE PROCEDURE dbo.sp_ExisteAdministradorActivo
                        AS
                        BEGIN
                            SET NOCOUNT ON;
                            IF EXISTS (
                                SELECT 1 
                                FROM dbo.Usuarios 
                                WHERE Rol = 'Administrador' 
                                  AND Estado = 'Activo'
                            )
                                SELECT CAST(1 AS BIT);
                            ELSE
                                SELECT CAST(0 AS BIT);
                        END"
                    }
                }
            };
        }

        #endregion
    }
}
