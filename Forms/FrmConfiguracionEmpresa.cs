using System;
using System.IO;
using System.Windows.Forms;
using System.Configuration;
using EmpresaConfig = MiniSistemaFacturacion.Configuration.EmpresaConfig;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para configurar los datos de la empresa y parámetros fiscales
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public partial class FrmConfiguracionEmpresa : Form
    {
        #region Constructor

        public FrmConfiguracionEmpresa()
        {
            InitializeComponent();
        }

        #endregion

        #region Eventos del Formulario

        private void FrmConfiguracionEmpresa_Load(object sender, EventArgs e)
        {
            CargarConfiguracionActual();
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Carga la configuración actual en los controles del formulario
        /// </summary>
        private void CargarConfiguracionActual()
        {
            try
            {
                var config = EmpresaConfig.Instance;

                // Datos de la empresa
                txtNombre.Text = config.Nombre ?? string.Empty;
                txtDireccion.Text = config.Direccion ?? string.Empty;
                txtTelefono.Text = config.Telefono ?? string.Empty;
                txtEmail.Text = config.Email ?? string.Empty;
                txtRNC.Text = config.RNC ?? string.Empty;

                // Configuración fiscal
                txtNCFActual.Text = config.NCFActual ?? string.Empty;
                txtNCFConsumidorFinal.Text = config.NCFConsumidorFinal ?? string.Empty;
                txtRutaPdfTickets.Text = config.RutaPdfTickets ?? string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuración: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Valida los datos ingresados antes de guardar
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        private bool ValidarDatos()
        {
            // Validar nombre de empresa
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre de la empresa es obligatorio.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return false;
            }

            // Validar RNC
            if (string.IsNullOrWhiteSpace(txtRNC.Text))
            {
                MessageBox.Show("El RNC de la empresa es obligatorio.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRNC.Focus();
                return false;
            }

            // Validar formato del RNC (solo números, 9 dígitos)
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtRNC.Text, @"^\d{9}$"))
            {
                MessageBox.Show("El RNC debe contener exactamente 9 dígitos numéricos.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRNC.Focus();
                return false;
            }

            // Validar NCF Actual
            if (string.IsNullOrWhiteSpace(txtNCFActual.Text))
            {
                MessageBox.Show("El NCF Actual es obligatorio.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNCFActual.Focus();
                return false;
            }

            // Validar formato del NCF (11 dígitos)
            if (!EmpresaConfig.Instance.ValidarNCF(txtNCFActual.Text))
            {
                MessageBox.Show("El NCF Actual debe contener exactamente 11 dígitos numéricos.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNCFActual.Focus();
                return false;
            }

            // Validar NCF Consumidor Final
            if (string.IsNullOrWhiteSpace(txtNCFConsumidorFinal.Text))
            {
                MessageBox.Show("El NCF Consumidor Final es obligatorio.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNCFConsumidorFinal.Focus();
                return false;
            }

            // Validar formato del NCF Consumidor Final
            if (!EmpresaConfig.Instance.ValidarNCF(txtNCFConsumidorFinal.Text))
            {
                MessageBox.Show("El NCF Consumidor Final debe contener exactamente 11 dígitos numéricos.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNCFConsumidorFinal.Focus();
                return false;
            }

            // Validar ruta PDF
            if (string.IsNullOrWhiteSpace(txtRutaPdfTickets.Text))
            {
                MessageBox.Show("La ruta para los PDF Tickets es obligatoria.", 
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar que la ruta exista o se pueda crear
            try
            {
                if (!Directory.Exists(txtRutaPdfTickets.Text))
                {
                    var resultado = MessageBox.Show(
                        "La ruta especificada no existe. ¿Desea crearla?", 
                        "Ruta no encontrada", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        Directory.CreateDirectory(txtRutaPdfTickets.Text);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se puede acceder a la ruta especificada: {ex.Message}", 
                    "Error de Ruta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Validar email si se proporciona
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("El formato del email no es válido.", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtEmail.Focus();
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Guarda la configuración en el archivo App.config
        /// </summary>
        private void GuardarConfiguracion()
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
                var appSettings = config.AppSettings.Settings;

                // Actualizar o agregar valores
                UpdateOrCreateSetting(appSettings, config, "EmpresaNombre", txtNombre.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "EmpresaDireccion", txtDireccion.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "EmpresaTelefono", txtTelefono.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "EmpresaEmail", txtEmail.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "EmpresaRNC", txtRNC.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "NCFActual", txtNCFActual.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "NCFConsumidorFinal", txtNCFConsumidorFinal.Text.Trim());
                UpdateOrCreateSetting(appSettings, config, "RutaPdfTickets", txtRutaPdfTickets.Text.Trim());

                // Guardar cambios
                config.Save(System.Configuration.ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                MessageBox.Show("Configuración guardada exitosamente.", 
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualiza o crea una configuración en el App.config
        /// </summary>
        private void UpdateOrCreateSetting(KeyValueConfigurationCollection appSettings, 
            System.Configuration.Configuration config, string key, string value)
        {
            if (appSettings[key] == null)
            {
                appSettings.Add(key, value);
            }
            else
            {
                appSettings[key].Value = value;
            }
        }

        /// <summary>
        /// Restablece los valores por defecto
        /// </summary>
        private void RestablecerValoresPorDefecto()
        {
            var resultado = MessageBox.Show(
                "¿Está seguro de restablecer todos los valores a los valores por defecto?", 
                "Confirmar Restablecimiento", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                txtNombre.Text = "Mi Empresa S.A.";
                txtDireccion.Text = "Dirección no configurada";
                txtTelefono.Text = "Teléfono no configurado";
                txtEmail.Text = "email@empresa.com";
                txtRNC.Text = "123456789";
                txtNCFActual.Text = "01010000001";
                txtNCFConsumidorFinal.Text = "B0100000001";
                txtRutaPdfTickets.Text = "./TicketsPDF/";
            }
        }

        #endregion

        #region Eventos de Controles

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidarDatos())
            {
                GuardarConfiguracion();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnRestablecer_Click(object sender, EventArgs e)
        {
            RestablecerValoresPorDefecto();
        }

        private void btnExaminarRuta_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Seleccionar carpeta para PDF Tickets";
                folderDialog.ShowNewFolderButton = true;
                folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                // Si ya hay una ruta, establecerla como ruta inicial
                if (!string.IsNullOrWhiteSpace(txtRutaPdfTickets.Text) && 
                    Directory.Exists(txtRutaPdfTickets.Text))
                {
                    folderDialog.SelectedPath = txtRutaPdfTickets.Text;
                }

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtRutaPdfTickets.Text = folderDialog.SelectedPath;
                }
            }
        }

        #endregion
    }
}
