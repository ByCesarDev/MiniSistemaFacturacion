using System;
using System.Drawing;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para editar clientes existentes
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public partial class FrmEditarCliente : FormularioClienteBase
    {
        public FrmEditarCliente()
        {
            InitializeComponent();
            this.Text = "Editar Cliente";
            
            // Configurar eventos de botones
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            
            // Configurar evento Load
            this.Load += FrmEditarCliente_Load;
        }

        private void FrmEditarCliente_Load(object sender, EventArgs e)
        {
            CargarTiposCliente();
            ConfigurarValidaciones();
            
            if (clienteActual != null)
            {
                CargarDatosCliente();
            }
        }

        /// <summary>
        /// Constructor que recibe el cliente a editar
        /// </summary>
        /// <param name="cliente">Cliente existente a editar</param>
        public FrmEditarCliente(Cliente cliente) : this()
        {
            this.ClienteActual = cliente;
        }

        /// <summary>
        /// Constructor que recibe el ID del cliente a editar
        /// </summary>
        /// <param name="idCliente">ID del cliente existente</param>
        public FrmEditarCliente(int idCliente) : this()
        {
            try
            {
                Cliente cliente = clienteDAL.ObtenerPorId(idCliente);
                if (cliente != null)
                {
                    this.ClienteActual = cliente;
                }
                else
                {
                    throw new Exception($"No se encontró el cliente con ID: {idCliente}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el cliente: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
            }
        }

        #region Implementación de Métodos Virtuales

        protected override void CargarDatosCliente()
        {
            if (clienteActual == null) return;

            txtNombre.Text = clienteActual.Nombre;
            txtCedula.Text = clienteActual.Cedula;
            txtDireccion.Text = clienteActual.Direccion;
            txtTelefono.Text = clienteActual.Telefono;
            txtEmail.Text = clienteActual.Email;
            
            // Configurar tipo cliente
            string tipoClienteDisplay = clienteActual.TipoCliente == "CF" ? "CF - Consumidor Final" : "CCF - Crédito Fiscal";
            cmbTipoCliente.SelectedItem = tipoClienteDisplay;
            
            // Configurar RNC si aplica
            if (clienteActual.TipoCliente == "CCF")
            {
                txtRNC.Text = clienteActual.RNC;
            }
        }

        protected override void CargarTiposCliente()
        {
            cmbTipoCliente.Items.Clear();
            cmbTipoCliente.Items.Add("CF - Consumidor Final");
            cmbTipoCliente.Items.Add("CCF - Crédito Fiscal");
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

            // Mantener el ID original para la actualización
            if (clienteActual != null)
            {
                cliente.ID_Cliente = clienteActual.ID_Cliente;
            }

            return cliente;
        }

        protected override void GuardarCliente()
        {
            Cliente cliente = CrearClienteDesdeControles();
            int resultado = clienteDAL.Actualizar(cliente);

            if (resultado <= 0)
            {
                throw new Exception("No se pudo actualizar el cliente. Verifique los datos e intente nuevamente.");
            }
        }

        #endregion

        #region Validaciones Adicionales

        protected override bool ValidarFormulario()
        {
            // Validaciones específicas para editar
            try
            {
                // Verificar si existe otro cliente con la misma cédula (excluyendo el actual)
                Cliente clienteExistente = clienteDAL.ObtenerPorCedula(txtCedula.Text.Trim());
                if (clienteExistente != null && clienteExistente.ID_Cliente != this.ClienteActual.ID_Cliente)
                {
                    MessageBox.Show("Ya existe otro cliente con esta cédula. Por favor, verifique los datos.", 
                        "Cliente Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCedula.Focus();
                    return false;
                }

                // Verificar si existe otro cliente con el mismo RNC (para CCF)
                string tipoCliente = ValidadorCliente.ObtenerTipoCliente(cmbTipoCliente);
                if (tipoCliente == "CCF" && !string.IsNullOrWhiteSpace(txtRNC.Text.Trim()))
                {
                    Cliente clienteRNCExistente = clienteDAL.ObtenerPorRNC(txtRNC.Text.Trim());
                    if (clienteRNCExistente != null && clienteRNCExistente.ID_Cliente != this.ClienteActual.ID_Cliente)
                    {
                        MessageBox.Show("Ya existe otro cliente con este RNC. Por favor, verifique los datos.", 
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

        #region Sobrecargas

        #endregion
    }
}
