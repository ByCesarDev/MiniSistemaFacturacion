using System;
using System.Windows.Forms;

namespace MiniSistemaFacturacion
{
    static class Program
    {
        /// <summary>
        /// Punto de inicio principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                // Inicializar la aplicación
                Application.Run(new Forms.frmMain());
            }
            catch (Exception ex)
            {
                // Manejo de errores críticos
                MessageBox.Show($"Error crítico al iniciar la aplicación: {ex.Message}", 
                              "Error de Inicio", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Error);
            }
        }
    }
}
