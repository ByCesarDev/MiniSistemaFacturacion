using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmClientes : Form
    {
        int idClienteSeleccionado = 0;
        int estadoClienteSeleccionado = 1;

        ClienteDAL clienteDAL = new ClienteDAL();
        public FrmClientes()
        {
            InitializeComponent();
        }

        private void CargarClientes()
        {
            dgvClientes.DataSource = clienteDAL.ObtenerTodos();
        }
        
        private void LimpiarCampos()
        {
            txtNombreCli.Clear();
            txtCedulaCli.Clear();
            txtDireccionCli.Clear();
            txtTelefonoCli.Clear();
            txtCorreoCli.Clear();
            txtRNC.Clear();
            txtIdCli.Clear();
            
            // Establecer valores por defecto
            cmbTipoCliente.SelectedIndex = 0; // CF - Consumidor Final por defecto
            txtRNC.Enabled = false; // RNC deshabilitado para CF

            idClienteSeleccionado = 0;
            estadoClienteSeleccionado = 1;
        }

        private bool ValidarCampos()
        {
            if (txtNombreCli.Text.Trim() == "" ||
                txtCedulaCli.Text.Trim() == "" ||
                txtDireccionCli.Text.Trim() == "" ||
                txtTelefonoCli.Text.Trim() == "" ||
                txtCorreoCli.Text.Trim() == "")
            {
                MessageBox.Show("Complete todos los campos obligatorios.");
                return false;
            }

            // Validar teléfono
            if (!txtTelefonoCli.Text.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' '))
            {
                MessageBox.Show("El teléfono solo puede contener números, +, - y espacios.");
                return false;
            }

            // Validar correo
            if (!txtCorreoCli.Text.Contains("@") || !txtCorreoCli.Text.Contains("."))
            {
                MessageBox.Show("Ingrese un correo válido.");
                return false;
            }

            // Validar tipo de cliente
            if (cmbTipoCliente.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un tipo de cliente.");
                return false;
            }

            // Validar RNC para Crédito Fiscal
            string tipoCliente = cmbTipoCliente.SelectedItem.ToString();
            if (tipoCliente.StartsWith("CCF"))
            {
                string rnc = txtRNC.Text.Trim();
                if (string.IsNullOrWhiteSpace(rnc))
                {
                    MessageBox.Show("El RNC es obligatorio para clientes de Crédito Fiscal.");
                    return false;
                }

                // Validar formato y dígito verificador del RNC
                string mensajeErrorRNC = RNCValidator.GetValidationMessage(rnc);
                if (!string.IsNullOrEmpty(mensajeErrorRNC))
                {
                    MessageBox.Show($"RNC inválido: {mensajeErrorRNC}", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private void txtBuscarCli_TextChanged(object sender, EventArgs e)
        {
            if (txtBuscarCli.Text == "")
            {
                CargarClientes();
            }
            else
            {
                dgvClientes.DataSource = clienteDAL.Buscar(txtBuscarCli.Text);
            }
        }

        private void dgvClientes_DoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvClientes.Rows[e.RowIndex];

                idClienteSeleccionado = Convert.ToInt32(fila.Cells["ID_Cliente"].Value);
                estadoClienteSeleccionado = Convert.ToInt32(fila.Cells["Estado"].Value);

                txtIdCli.Text = fila.Cells["ID_Cliente"].Value.ToString();
                txtNombreCli.Text = fila.Cells["Nombre"].Value.ToString();
                txtCedulaCli.Text = fila.Cells["Cedula"].Value.ToString();
                txtDireccionCli.Text = fila.Cells["Direccion"].Value.ToString();
                txtTelefonoCli.Text = fila.Cells["Telefono"].Value.ToString();
                txtCorreoCli.Text = fila.Cells["Email"].Value.ToString();
                
                // Cargar TipoCliente y RNC si existen en el DataGridView
                if (dgvClientes.Columns["TipoCliente"] != null && fila.Cells["TipoCliente"].Value != null)
                {
                    string tipoCliente = fila.Cells["TipoCliente"].Value.ToString();
                    cmbTipoCliente.SelectedItem = tipoCliente == "CF" ? "CF - Consumidor Final" : "CCF - Crédito Fiscal";
                }
                
                if (dgvClientes.Columns["RNC"] != null && fila.Cells["RNC"].Value != null)
                {
                    txtRNC.Text = fila.Cells["RNC"].Value.ToString();
                }
            }
        }

        private void btnEditarCli_Click(object sender, EventArgs e)
        {
            try
            {
                if (idClienteSeleccionado == 0)
                {
                    MessageBox.Show("Seleccione un cliente para editar.");
                    return;
                }

                if (!ValidarCampos()) return;

                var confirm = MessageBox.Show("¿Está seguro de actualizar este cliente?",
                             "Confirmar",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes) return;

                // Obtener tipo cliente del ComboBox
                string tipoClienteSeleccion = cmbTipoCliente.SelectedItem.ToString();
                string tipoCliente = tipoClienteSeleccion.StartsWith("CF") ? "CF" : "CCF";
                string rnc = txtRNC.Text.Trim();

                Cliente cliente = new Cliente
                {
                    ID_Cliente = idClienteSeleccionado,
                    Nombre = txtNombreCli.Text.Trim(),
                    Cedula = txtCedulaCli.Text.Trim(),
                    Direccion = txtDireccionCli.Text.Trim(),
                    Telefono = txtTelefonoCli.Text.Trim(),
                    Email = txtCorreoCli.Text.Trim(),
                    TipoCliente = tipoCliente,
                    RNC = tipoCliente == "CF" ? null : rnc
                };
                
                int actualizado = clienteDAL.Actualizar(cliente);

                if (actualizado > 0)
                {
                    MessageBox.Show("Cliente actualizado correctamente.");
                    CargarClientes();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("No se pudo actualizar el cliente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un cliente.");
                return;
            }

            var confirm = MessageBox.Show("¿Desea cambiar el estado de este cliente?",
                             "Confirmar",
                             MessageBoxButtons.YesNo);

            if (confirm != DialogResult.Yes) return;

            int cambiado = estadoClienteSeleccionado == 1 ? clienteDAL.Desactivar(idClienteSeleccionado) : clienteDAL.Activar(idClienteSeleccionado);

            if (cambiado > 0)
            {
                MessageBox.Show("Estado actualizado.");
                CargarClientes();
                LimpiarCampos();
            }
        }

        private void btnNuevoCli_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                // Obtener tipo cliente del ComboBox
                string tipoClienteSeleccion = cmbTipoCliente.SelectedItem.ToString();
                string tipoCliente = tipoClienteSeleccion.StartsWith("CF") ? "CF" : "CCF";
                string rnc = txtRNC.Text.Trim();

                Cliente cliente = new Cliente
                {
                    Nombre = txtNombreCli.Text.Trim(),
                    Cedula = txtCedulaCli.Text.Trim(),
                    Direccion = txtDireccionCli.Text.Trim(),
                    Telefono = txtTelefonoCli.Text.Trim(),
                    Email = txtCorreoCli.Text.Trim(),
                    TipoCliente = tipoCliente,
                    RNC = tipoCliente == "CF" ? null : rnc
                };
                
                int insertado = clienteDAL.Insertar(cliente);

                if (insertado > 0)
                {
                    MessageBox.Show("Cliente agregado correctamente.");
                    CargarClientes();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("No se pudo agregar el cliente.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar: " + ex.Message);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void cmbTipoCliente_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbTipoCliente.SelectedItem != null)
            {
                string seleccion = cmbTipoCliente.SelectedItem.ToString();
                
                // Habilitar RNC solo para Crédito Fiscal
                if (seleccion.StartsWith("CCF"))
                {
                    txtRNC.Enabled = true;
                    txtRNC.BackColor = SystemColors.Window;
                    label9.ForeColor = SystemColors.ControlText;
                }
                else
                {
                    txtRNC.Enabled = false;
                    txtRNC.Clear();
                    txtRNC.BackColor = SystemColors.Control;
                    label9.ForeColor = SystemColors.GrayText;
                }
            }
        }
    }
}
