using System;
using System.Drawing;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para agregar nuevos clientes
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public partial class FrmAgregarCliente : FormularioClienteBase
    {
        public FrmAgregarCliente()
        {
            InitializeComponent();
            this.Text = "Agregar Nuevo Cliente";
            
            // Configurar eventos de botones
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            
            // Configurar evento Load
            this.Load += FrmAgregarCliente_Load;
        }

        private void FrmAgregarCliente_Load(object sender, EventArgs e)
        {
            CargarTiposCliente();
            ConfigurarValidaciones();
        }

        #region Implementación de Métodos Virtuales

        protected override void CargarDatosCliente()
        {
            // No aplica para formulario de agregar nuevo cliente
        }

        protected override void CargarTiposCliente()
        {
            cmbTipoCliente.Items.Clear();
            cmbTipoCliente.Items.Add("CF - Consumidor Final");
            cmbTipoCliente.Items.Add("CCF - Crédito Fiscal");
            cmbTipoCliente.SelectedIndex = 0; // Seleccionar Consumidor Final por defecto
        }

        protected override void ConfigurarValidaciones()
        {
            // Configurar validaciones adicionales que no se pueden hacer en el diseñador
            ValidadorCliente.ConfigurarValidacionRNC(txtRNC, cmbTipoCliente);
        }

        protected override Cliente CrearClienteDesdeControles()
        {
            string tipoCliente = "CF"; // Por defecto

            if (cmbTipoCliente.SelectedItem != null)
            {
                string selected = cmbTipoCliente.SelectedItem.ToString();
                tipoCliente = selected.Contains("CCF") ? "CCF" : "CF";
            }

            Cliente cliente = new Cliente
            {
                Nombre = txtNombre.Text.Trim(),
                Cedula = txtCedula.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                TipoCliente = tipoCliente,
                RNC = (tipoCliente == "CCF") ? txtRNC.Text.Trim() : null,
                Estado = true
            };

            return cliente;
        }

        protected override void GuardarCliente()
        {
            Cliente cliente = CrearClienteDesdeControles();
            int resultado = clienteDAL.Insertar(cliente);

            if (resultado <= 0)
            {
                throw new Exception("No se pudo agregar el cliente. Verifique los datos e intente nuevamente.");
            }
        }

        #endregion

        #region Configuración Adicional

        private void ConfigurarEventosAdicionales()
        {
            // Configurar validaciones adicionales que no se pueden hacer en el diseñador
            ValidadorCliente.ConfigurarValidacionRNC(txtRNC, cmbTipoCliente);
        }

        #endregion

        #region Validaciones Adicionales

        protected override bool ValidarFormulario()
        {
            // Validaciones específicas para agregar
            try
            {
                // Verificar si ya existe un cliente con la misma cédula
                Cliente clienteExistente = clienteDAL.ObtenerPorCedula(txtCedula.Text.Trim());
                if (clienteExistente != null)
                {
                    MessageBox.Show("Ya existe un cliente con esta cédula. Por favor, verifique los datos.", 
                        "Cliente Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCedula.Focus();
                    return false;
                }

                // Verificar si ya existe un cliente con el mismo RNC (para CCF)
                string tipoCliente = ValidadorCliente.ObtenerTipoCliente(cmbTipoCliente);
                if (tipoCliente == "CCF" && !string.IsNullOrWhiteSpace(txtRNC.Text.Trim()))
                {
                    Cliente clienteRNCExistente = clienteDAL.ObtenerPorRNC(txtRNC.Text.Trim());
                    if (clienteRNCExistente != null)
                    {
                        MessageBox.Show("Ya existe un cliente con este RNC. Por favor, verifique los datos.", 
                            "RNC Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtRNC.Focus();
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar duplicados: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        #endregion
    }
}
