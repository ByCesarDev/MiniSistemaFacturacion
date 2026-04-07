using System;
using System.Windows.Forms;

namespace MiniSistemaFacturacion
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Created by: Cesar Reyes
        /// Date: 2026-04-07
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
