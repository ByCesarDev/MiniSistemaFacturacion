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
            txtIdCli.Clear();

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
                MessageBox.Show("Complete todos los campos.");
                return false;
            }

            if (!txtTelefonoCli.Text.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' '))
            {
                MessageBox.Show("El teléfono solo puede contener números, +, - y espacios.");
                return false;
            }

            if (!txtCorreoCli.Text.Contains("@") || !txtCorreoCli.Text.Contains("."))
            {
                MessageBox.Show("Ingrese un correo válido.");
                return false;
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

                Cliente cliente = new Cliente
                {
                    ID_Cliente = idClienteSeleccionado,
                    Nombre = txtNombreCli.Text.Trim(),
                    Cedula = txtCedulaCli.Text.Trim(),
                    Direccion = txtDireccionCli.Text.Trim(),
                    Telefono = txtTelefonoCli.Text.Trim(),
                    Email = txtCorreoCli.Text.Trim()
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
                Cliente cliente = new Cliente
                {
                    Nombre = txtNombreCli.Text.Trim(),
                    Cedula = txtCedulaCli.Text.Trim(),
                    Direccion = txtDireccionCli.Text.Trim(),
                    Telefono = txtTelefonoCli.Text.Trim(),
                    Email = txtCorreoCli.Text.Trim()
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
    }
}