using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.Utils
{
    /// <summary>
    /// Clase de utilidad para validación de clientes
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public static class ValidadorCliente
    {
        /// <summary>
        /// Valida todos los campos de un cliente
        /// </summary>
        /// <param name="cliente">Cliente a validar</param>
        /// <param name="mensajeError">Mensaje de error si hay validación fallida</param>
        /// <returns>True si el cliente es válido</returns>
        public static bool ValidarCampos(Cliente cliente, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Validar nombre
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
            {
                mensajeError = "El nombre del cliente es obligatorio.";
                return false;
            }

            if (cliente.Nombre.Trim().Length < 3)
            {
                mensajeError = "El nombre del cliente debe tener al menos 3 caracteres.";
                return false;
            }

            // Validar cédula
            if (string.IsNullOrWhiteSpace(cliente.Cedula))
            {
                mensajeError = "La cédula del cliente es obligatoria.";
                return false;
            }

            if (!ValidarCedula(cliente.Cedula))
            {
                mensajeError = "El formato de la cédula no es válido.";
                return false;
            }

            // Validar dirección
            if (string.IsNullOrWhiteSpace(cliente.Direccion))
            {
                mensajeError = "La dirección del cliente es obligatoria.";
                return false;
            }

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(cliente.Telefono))
            {
                mensajeError = "El teléfono del cliente es obligatorio.";
                return false;
            }

            if (!ValidarTelefono(cliente.Telefono))
            {
                mensajeError = "El formato del teléfono no es válido. Solo permite números, +, - y espacios.";
                return false;
            }

            // Validar correo
            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                mensajeError = "El correo electrónico del cliente es obligatorio.";
                return false;
            }

            if (!ValidarCorreo(cliente.Email))
            {
                mensajeError = "El formato del correo electrónico no es válido.";
                return false;
            }

            // Validar tipo de cliente
            if (string.IsNullOrWhiteSpace(cliente.TipoCliente))
            {
                mensajeError = "El tipo de cliente es obligatorio.";
                return false;
            }

            if (cliente.TipoCliente != "CF" && cliente.TipoCliente != "CCF")
            {
                mensajeError = "El tipo de cliente debe ser 'CF' (Consumidor Final) o 'CCF' (Crédito Fiscal).";
                return false;
            }

            // Validar RNC para Crédito Fiscal
            if (cliente.TipoCliente == "CCF")
            {
                if (string.IsNullOrWhiteSpace(cliente.RNC))
                {
                    mensajeError = "El RNC es obligatorio para clientes de Crédito Fiscal.";
                    return false;
                }

                string mensajeErrorRNC = RNCValidator.GetValidationMessage(cliente.RNC);
                if (!string.IsNullOrEmpty(mensajeErrorRNC))
                {
                    mensajeError = $"RNC inválido: {mensajeErrorRNC}";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valida el formato de una cédula
        /// </summary>
        /// <param name="cedula">Cédula a validar</param>
        /// <returns>True si la cédula tiene formato válido</returns>
        public static bool ValidarCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return false;

            // Limpiar cédula (quitar guiones, espacios)
            string cedulaLimpia = cedula.Replace("-", "").Replace(" ", "").Trim();

            // La cédula debe tener entre 7 y 10 dígitos
            if (cedulaLimpia.Length < 7 || cedulaLimpia.Length > 10)
                return false;

            // Verificar que todos los caracteres sean dígitos
            return Regex.IsMatch(cedulaLimpia, @"^\d+$");
        }

        /// <summary>
        /// Valida el formato de un teléfono
        /// </summary>
        /// <param name="telefono">Teléfono a validar</param>
        /// <returns>True si el teléfono tiene formato válido</returns>
        public static bool ValidarTelefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                return false;

            // Permite números, +, - y espacios
            return Regex.IsMatch(telefono, @"^[0-9+\-\s]+$");
        }

        /// <summary>
        /// Valida el formato de un correo electrónico
        /// </summary>
        /// <param name="correo">Correo a validar</param>
        /// <returns>True si el correo tiene formato válido</returns>
        public static bool ValidarCorreo(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
                return false;

            try
            {
                var mail = new System.Net.Mail.MailAddress(correo);
                return mail.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Configura los eventos de validación para los controles de un formulario
        /// </summary>
        /// <param name="txtRNC">TextBox del RNC</param>
        /// <param name="cmbTipoCliente">ComboBox de tipo de cliente</param>
        public static void ConfigurarValidacionRNC(TextBox txtRNC, ComboBox cmbTipoCliente)
        {
            // Evento cuando cambia el tipo de cliente
            cmbTipoCliente.SelectedValueChanged += (sender, e) =>
            {
                if (cmbTipoCliente.SelectedItem != null)
                {
                    string seleccion = cmbTipoCliente.SelectedItem.ToString();
                    
                    // Habilitar RNC solo para Crédito Fiscal
                    if (seleccion.StartsWith("CCF"))
                    {
                        txtRNC.Enabled = true;
                        txtRNC.BackColor = System.Drawing.SystemColors.Window;
                        txtRNC.Clear();
                    }
                    else
                    {
                        txtRNC.Enabled = false;
                        txtRNC.Clear();
                        txtRNC.BackColor = System.Drawing.SystemColors.Control;
                    }
                }
            };

            // Validación en tiempo real del RNC
            txtRNC.TextChanged += (sender, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtRNC.Text) && cmbTipoCliente.SelectedItem != null)
                {
                    string seleccion = cmbTipoCliente.SelectedItem.ToString();
                    if (seleccion.StartsWith("CCF"))
                    {
                        string mensajeError = RNCValidator.GetValidationMessage(txtRNC.Text);
                        if (!string.IsNullOrEmpty(mensajeError))
                        {
                            txtRNC.BackColor = System.Drawing.Color.LightPink;
                            toolTip.SetToolTip(txtRNC, mensajeError);
                        }
                        else
                        {
                            txtRNC.BackColor = System.Drawing.Color.LightGreen;
                            toolTip.SetToolTip(txtRNC, "RNC válido");
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Obtiene el tipo de cliente desde un ComboBox
        /// </summary>
        /// <param name="cmbTipoCliente">ComboBox con tipos de cliente</param>
        /// <returns>Tipo de cliente ("CF" o "CCF")</returns>
        public static string ObtenerTipoCliente(ComboBox cmbTipoCliente)
        {
            if (cmbTipoCliente.SelectedItem == null)
                return "CF";

            string seleccion = cmbTipoCliente.SelectedItem.ToString();
            return seleccion.StartsWith("CCF") ? "CCF" : "CF";
        }

        /// <summary>
        /// Obtiene el RNC formateado y validado
        /// </summary>
        /// <param name="txtRNC">TextBox del RNC</param>
        /// <param name="tipoCliente">Tipo de cliente</param>
        /// <returns>RNC formateado o null si no aplica</returns>
        public static string ObtenerRNC(TextBox txtRNC, string tipoCliente)
        {
            if (tipoCliente == "CCF" && !string.IsNullOrWhiteSpace(txtRNC.Text))
            {
                return txtRNC.Text.Trim();
            }
            return null;
        }

        private static ToolTip toolTip = new ToolTip();

        /// <summary>
        /// Limpia los mensajes de validación
        /// </summary>
        public static void LimpiarValidacion()
        {
            toolTip.RemoveAll();
        }
    }
}
