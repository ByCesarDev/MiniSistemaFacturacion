using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MiniSistemaFacturacion.Utils
{
    /// <summary>
    /// Utilidad para detectar si un control está en modo de diseño
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public static class DesignModeHelper
    {
        /// <summary>
        /// Verifica si el control está en modo de diseño
        /// </summary>
        /// <param name="control">Control a verificar</param>
        /// <returns>True si está en modo de diseño</returns>
        public static bool IsInDesignMode(Control control)
        {
            if (control == null)
                return false;

            // Método 1: Usar la propiedad DesignMode si está disponible
            try
            {
                if (control.Site != null && control.Site.DesignMode)
                    return true;
            }
            catch
            {
                // Si falla, continuar con otros métodos
            }

            // Método 2: Verificar si el LicenseContext está en modo diseño
            try
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                    return true;
            }
            catch
            {
                // Si falla, continuar con otros métodos
            }

            // Método 3: Verificar si estamos en tiempo de diseño mediante el proceso actual
            try
            {
                // Si el proceso actual es devenv.exe (Visual Studio), estamos en modo diseño
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (processName.Equals("devenv", StringComparison.OrdinalIgnoreCase) ||
                    processName.Equals("SharpDevelop", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            catch
            {
                // Si falla, asumir que no estamos en modo diseño
            }

            return false;
        }
    }
}
