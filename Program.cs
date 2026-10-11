using System;
using System.Windows.Forms;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.Forms;

namespace MiniSistemaFacturacion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación con control de autenticación y sesiones.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
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
    }
}
