using System;
using System.Windows.Forms;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Forms;

namespace MiniSistemaFacturacion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación con control de conectividad, migraciones y autenticación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Soporte para ejecución en modo configuración (--config, --setup)
                if (args != null && args.Length > 0)
                {
                    bool modoConfig = false;
                    bool modoInitConfig = false;
                    string serverArg = null;
                    string databaseArg = null;
                    bool? authWindowsArg = null;
                    string userArg = null;

                    for (int i = 0; i < args.Length; i++)
                    {
                        string a = args[i];
                        if (a.Equals("--config", StringComparison.OrdinalIgnoreCase) ||
                            a.Equals("--setup", StringComparison.OrdinalIgnoreCase) ||
                            a.Equals("-c", StringComparison.OrdinalIgnoreCase))
                        {
                            modoConfig = true;
                        }
                        else if (a.Equals("--init-config", StringComparison.OrdinalIgnoreCase))
                        {
                            modoInitConfig = true;
                        }
                        else if (a.Equals("--server", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                        {
                            serverArg = args[++i];
                        }
                        else if (a.Equals("--database", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                        {
                            databaseArg = args[++i];
                        }
                        else if (a.Equals("--auth", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                        {
                            string authVal = args[++i];
                            authWindowsArg = authVal.Equals("windows", StringComparison.OrdinalIgnoreCase);
                        }
                        else if (a.Equals("--user", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                        {
                            userArg = args[++i];
                        }
                    }

                    if (modoInitConfig)
                    {
                        string srv = string.IsNullOrEmpty(serverArg) ? @"localhost\SQLEXPRESS" : serverArg;
                        string db = string.IsNullOrEmpty(databaseArg) ? "MiniSistemaFacturacion" : databaseArg;
                        bool integrated = !authWindowsArg.HasValue || authWindowsArg.Value;
                        string usr = integrated ? "" : (userArg ?? "");

                        string connStr = ConnectionStringManager.BuildConnectionString(srv, db, integrated, usr, "", 15);
                        string err;
                        bool saved = ConnectionStringManager.SaveConnectionString(connStr, !integrated, out err);
                        Environment.Exit(saved ? 0 : 1);
                        return;
                    }

                    if (modoConfig)
                    {
                        using (var frmConfig = new FrmConfigurarConexion(serverArg, databaseArg, authWindowsArg, userArg))
                        {
                            frmConfig.ShowDialog();
                        }
                        return;
                    }
                }

                // 0. Barrera de Infraestructura: Verificar conectividad y esquema de base de datos
                if (!AsegurarConexionYEsquemaValido())
                {
                    // Si el usuario canceló la configuración o no se logró conexión, salir ordenadamente
                    return;
                }

                while (true)
                {
                    // 1. Barrera de autenticación: El sistema exige iniciar sesión antes de abrir cualquier módulo
                    using (var frmLogin = new FrmLogin())
                    {
                        DialogResult loginResult = frmLogin.ShowDialog();

                        // Si el usuario canceló, cerró el diálogo o no completó la autenticación, salir limpiamente
                        if (loginResult != DialogResult.OK || !SesionActual.EsAutenticado)
                        {
                            break;
                        }
                    }

                    // 2. Iniciar el formulario principal con la sesión activa
                    using (var frmMain = new FrmMain())
                    {
                        Application.Run(frmMain);
                    }

                    // 3. Evaluar si el usuario cerró sesión voluntariamente para volver a la pantalla de login
                    if (!SesionActual.RequiereReinicioSesion)
                    {
                        break;
                    }

                    // Limpiar la bandera antes de mostrar de nuevo el login
                    SesionActual.RequiereReinicioSesion = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error crítico al ejecutar la aplicación: {ex.Message}\n\nStackTrace: {ex.StackTrace}",
                              "Error del Sistema",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Comprueba silenciosamente si el servidor SQL y la base de datos están listos con todas las migraciones.
        /// Si detecta fallas de conexión o esquema pendiente, abre FrmConfigurarConexion.
        /// </summary>
        private static bool AsegurarConexionYEsquemaValido()
        {
            while (true)
            {
                string connectionString = ConnectionStringManager.GetConnectionString();
                bool requiereConfigurador = false;

                if (string.IsNullOrWhiteSpace(connectionString) || !ConnectionStringManager.IsValidConnectionString(connectionString))
                {
                    requiereConfigurador = true;
                }
                else
                {
                    try
                    {
                        var migrador = new DatabaseMigrationManager(connectionString);
                        var status = migrador.EvaluarEstado();

                        if (status.ServidorAccesible && (!status.BaseDatosExiste || !status.EsquemaAlDia))
                        {
                            bool esAdmin = false;
                            try
                            {
                                using (var identity = System.Security.Principal.WindowsIdentity.GetCurrent())
                                {
                                    var principal = new System.Security.Principal.WindowsPrincipal(identity);
                                    esAdmin = principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                                }
                            }
                            catch { }

                            if (!esAdmin)
                            {
                                string motivo = !status.BaseDatosExiste
                                    ? "La base de datos del sistema aún no ha sido creada o inicializada en el servidor."
                                    : "La base de datos requiere una actualización de esquema de versiones pendientes.";

                                MessageBox.Show(
                                    $"{motivo}\n\n" +
                                    "Por seguridad e integridad contable, la inicialización y migración de esquemas deben " +
                                    "ser aplicadas por el Administrador del sistema mediante el acceso directo 'Configurar Base de Datos'.\n\n" +
                                    "La aplicación se cerrará.",
                                    "Acción Administrativa Requerida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                                return false;
                            }
                        }

                        if (!status.ServidorAccesible || !status.BaseDatosExiste || !status.EsquemaAlDia)
                        {
                            requiereConfigurador = true;
                        }
                    }
                    catch
                    {
                        requiereConfigurador = true;
                    }
                }

                // Si todo está perfectamente accesible y al día, continuar de inmediato
                if (!requiereConfigurador)
                {
                    return true;
                }

                // Si falla la infraestructura o requiere migraciones, abrir el asistente corporativo
                using (var frmConfig = new FrmConfigurarConexion())
                {
                    DialogResult resultado = frmConfig.ShowDialog();
                    if (resultado != DialogResult.OK)
                    {
                        // El usuario canceló o cerró la ventana de configuración
                        return false;
                    }
                }
            }
        }
    }
}
