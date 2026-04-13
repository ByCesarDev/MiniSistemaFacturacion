using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para búsqueda de clientes existentes
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public partial class FrmBusquedaClientes : Form
    {
        #region Properties

        private ClienteDAL _clienteDAL;
        private BindingList<Cliente> _clientesEncontrados;
        public Cliente ClienteSeleccionado { get; private set; }

        #endregion

        #region Constructor

        public FrmBusquedaClientes()
        {
            InitializeComponent();
            _clienteDAL = new ClienteDAL();
            _clientesEncontrados = new BindingList<Cliente>();
        }

        #endregion

        #region Form Events

        private void FrmBusquedaClientes_Load(object sender, EventArgs e)
        {
            try
            {
                // Configurar DataGridView
                ConfigurarDataGridView();
                
                // Cargar automáticamente los últimos 100 clientes
                try
                {
                    _clientesEncontrados = new BindingList<Cliente>(_clienteDAL.ObtenerUltimosClientes(100));
                    
                    // Mostrar resultados
                    dgvClientes.DataSource = _clientesEncontrados;
                    dgvClientes.Refresh();
                    dgvClientes.Update();
                    
                    // Actualizar información
                    lblResultados.Text = $"Mostrando los últimos {_clientesEncontrados.Count} clientes";
                }
                catch (Exception ex)
                {
                    lblResultados.Text = "Error al cargar clientes iniciales";
                    MessageBox.Show($"Error al cargar los últimos clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                BuscarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            try
            {
                LimpiarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar filtros: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClientes_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvClientes.SelectedRows.Count > 0)
                {
                    int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["ID_Cliente"].Value);
                    SeleccionarCliente(idCliente);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNombre_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir búsqueda automática al presionar Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BuscarClientes();
            }
        }

        private void txtRNC_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir búsqueda automática al presionar Enter
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BuscarClientes();
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvClientes.SelectedRows.Count > 0)
                {
                    int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["ID_Cliente"].Value);
                    SeleccionarCliente(idCliente);
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un cliente de la lista.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Private Methods

        private void ConfigurarDataGridView()
        {
            try
            {
                // Configurar columnas
                dgvClientes.AutoGenerateColumns = false;
                dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvClientes.MultiSelect = false;
                dgvClientes.ReadOnly = true;
                dgvClientes.AllowUserToAddRows = false;
                dgvClientes.AllowUserToDeleteRows = false;
                dgvClientes.RowHeadersVisible = false;

                // Agregar columnas
                dgvClientes.Columns.Clear();

                // Columna ID Cliente (oculta)
                DataGridViewTextBoxColumn colID = new DataGridViewTextBoxColumn
                {
                    Name = "ID_Cliente",
                    HeaderText = "ID",
                    DataPropertyName = "ID_Cliente",
                    Visible = false
                };
                dgvClientes.Columns.Add(colID);

                // Columna Nombre
                DataGridViewTextBoxColumn colNombre = new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Nombre",
                    DataPropertyName = "Nombre",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 40
                };
                dgvClientes.Columns.Add(colNombre);

                // Columna Cédula
                DataGridViewTextBoxColumn colCedula = new DataGridViewTextBoxColumn
                {
                    Name = "Cedula",
                    HeaderText = "Cédula",
                    DataPropertyName = "Cedula",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 15
                };
                dgvClientes.Columns.Add(colCedula);

                // Columna Tipo Cliente
                DataGridViewTextBoxColumn colTipoCliente = new DataGridViewTextBoxColumn
                {
                    Name = "TipoCliente",
                    HeaderText = "Tipo",
                    DataPropertyName = "TipoCliente",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 10
                };
                dgvClientes.Columns.Add(colTipoCliente);

                // Columna RNC (mostrar solo si tiene RNC)
                DataGridViewTextBoxColumn colRNC = new DataGridViewTextBoxColumn
                {
                    Name = "RNC",
                    HeaderText = "RNC",
                    DataPropertyName = "RNC",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 15
                };
                dgvClientes.Columns.Add(colRNC);

                // Columna Teléfono
                DataGridViewTextBoxColumn colTelefono = new DataGridViewTextBoxColumn
                {
                    Name = "Telefono",
                    HeaderText = "Teléfono",
                    DataPropertyName = "Telefono",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 15
                };
                dgvClientes.Columns.Add(colTelefono);

                // Columna Email
                DataGridViewTextBoxColumn colEmail = new DataGridViewTextBoxColumn
                {
                    Name = "Email",
                    HeaderText = "Email",
                    DataPropertyName = "Email",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    FillWeight = 20
                };
                dgvClientes.Columns.Add(colEmail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar DataGridView: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuscarClientes()
        {
            try
            {
                // Obtener criterios de búsqueda
                string nombre = txtNombre.Text.Trim();
                string cedula = txtRNC.Text.Trim();

                // Validar que al menos un criterio esté lleno
                if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(cedula))
                {
                    MessageBox.Show("Por favor, ingrese al menos un criterio de búsqueda (nombre o RNC/Cédula).", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Buscar clientes
                _clientesEncontrados = new BindingList<Cliente>(_clienteDAL.BuscarClientes(nombre, cedula));

                // Mostrar resultados
                dgvClientes.DataSource = _clientesEncontrados;
                dgvClientes.Refresh();
                dgvClientes.Update();

                // Actualizar información
                lblResultados.Text = $"Se encontraron {_clientesEncontrados.Count} clientes";

                if (_clientesEncontrados.Count == 0)
                {
                    MessageBox.Show("No se encontraron clientes con los criterios especificados.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFiltros()
        {
            try
            {
                txtNombre.Clear();
                txtRNC.Clear();
                
                // Recargar los últimos 100 clientes
                _clientesEncontrados = new BindingList<Cliente>(_clienteDAL.ObtenerUltimosClientes(100));
                
                dgvClientes.DataSource = _clientesEncontrados;
                dgvClientes.Refresh();
                dgvClientes.Update();
                
                lblResultados.Text = $"Mostrando los últimos {_clientesEncontrados.Count} clientes";
                
                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar filtros: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarCliente(int idCliente)
        {
            try
            {
                // Buscar el cliente en la lista
                ClienteSeleccionado = _clientesEncontrados.FirstOrDefault(c => c.ID_Cliente == idCliente);
                
                if (ClienteSeleccionado != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("No se pudo encontrar el cliente seleccionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
