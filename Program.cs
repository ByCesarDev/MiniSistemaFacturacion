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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
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
