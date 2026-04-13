using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario de gestión de clientes - Panel de control
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public partial class FrmClientes : Form
    {
        private ClienteDAL clienteDAL = new ClienteDAL();
        private List<Cliente> todosLosClientes = new List<Cliente>();
        private Cliente clienteSeleccionado = null;

        // Los controles ahora están definidos en el Designer
        public FrmClientes()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            ConfigurarDataGridView();
            ConfigurarEventos();
            CargarClientes();
        }

        private void ConfigurarFormulario()
        {
            // El formulario ya está configurado por el diseñador
            // Solo configuramos eventos adicionales
        }

        private void ConfigurarDataGridView()
        {
            // Configurar columnas
            dgvClientes.Columns.Clear();

            // Columna ID
            DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn
            {
                Name = "ID_Cliente",
                HeaderText = "ID",
                DataPropertyName = "ID_Cliente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                FillWeight = 5
            };
            dgvClientes.Columns.Add(colId);

            // Columna Nombre
            DataGridViewTextBoxColumn colNombre = new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Nombre/Razón Social",
                DataPropertyName = "Nombre",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 35
            };
            dgvClientes.Columns.Add(colNombre);

            // Columna Cédula
            DataGridViewTextBoxColumn colCedula = new DataGridViewTextBoxColumn
            {
                Name = "Cedula",
                HeaderText = "Cédula",
                DataPropertyName = "Cedula",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                FillWeight = 15
            };
            dgvClientes.Columns.Add(colCedula);

            // Columna Dirección
            DataGridViewTextBoxColumn colDireccion = new DataGridViewTextBoxColumn
            {
                Name = "Direccion",
                HeaderText = "Dirección",
                DataPropertyName = "Direccion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15
            };
            dgvClientes.Columns.Add(colDireccion);

            // Columna Teléfono
            DataGridViewTextBoxColumn colTelefono = new DataGridViewTextBoxColumn
            {
                Name = "Telefono",
                HeaderText = "Teléfono",
                DataPropertyName = "Telefono",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
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
                FillWeight = 25
            };
            dgvClientes.Columns.Add(colEmail);

            // Columna TipoCliente
            DataGridViewTextBoxColumn colTipoCliente = new DataGridViewTextBoxColumn
            {
                Name = "TipoCliente",
                HeaderText = "Tipo",
                DataPropertyName = "TipoCliente",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                FillWeight = 10
            };
            dgvClientes.Columns.Add(colTipoCliente);

            // Columna RNC
            DataGridViewTextBoxColumn colRNC = new DataGridViewTextBoxColumn
            {
                Name = "RNC",
                HeaderText = "RNC",
                DataPropertyName = "RNC",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = 15
            };
            dgvClientes.Columns.Add(colRNC);

            // Columna Estado
            DataGridViewTextBoxColumn colEstado = new DataGridViewTextBoxColumn
            {
                Name = "Estado",
                HeaderText = "Estado",
                DataPropertyName = "Estado",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                FillWeight = 10
            };
            dgvClientes.Columns.Add(colEstado);
        }

        private void ConfigurarEventos()
        {
            // Eventos de botones
            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            // Eventos del DataGridView
            dgvClientes.SelectionChanged += DgvClientes_SelectionChanged;
            dgvClientes.DoubleClick += DgvClientes_DoubleClick;
            dgvClientes.CellFormatting += DgvClientes_CellFormatting;
            dgvClientes.DataBindingComplete += DgvClientes_DataBindingComplete;
            dgvClientes.DataError += DgvClientes_DataError;

            // Evento de búsqueda
            txtBusqueda.TextChanged += TxtBusqueda_TextChanged;
        }

        private void CargarClientes()
        {
            try
            {
                todosLosClientes = clienteDAL.ObtenerTodos();
                dgvClientes.DataSource = null;
                dgvClientes.DataSource = todosLosClientes;
                
                ActualizarBotones();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los clientes: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFormateoRNC()
        {
            try
            {
                // Esperar un momento para que el DataGridView termine de cargar
                this.BeginInvoke((MethodInvoker)delegate
                {
                    foreach (DataGridViewRow row in dgvClientes.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var rncCell = row.Cells["RNC"];
                            if (rncCell.Value == DBNull.Value || rncCell.Value == null || string.IsNullOrEmpty(rncCell.Value?.ToString()))
                            {
                                rncCell.Value = "N/A";
                                rncCell.Style.ForeColor = SystemColors.GrayText;
                                rncCell.Style.Font = new Font(dgvClientes.Font, FontStyle.Italic);
                            }
                            else
                            {
                                rncCell.Style.ForeColor = SystemColors.ControlText;
                                rncCell.Style.Font = new Font(dgvClientes.Font, FontStyle.Regular);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error aplicando formateo RNC: {ex.Message}");
            }
        }

        private void FiltrarClientes()
        {
            try
            {
                string textoBusqueda = txtBusqueda.Text.Trim();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    dgvClientes.DataSource = null;
                    dgvClientes.DataSource = todosLosClientes;
                }
                else
                {
                    var clientesFiltrados = todosLosClientes
                        .Where(c => (!string.IsNullOrEmpty(c.Nombre) && c.Nombre.ToLower().Contains(textoBusqueda.ToLower())) ||
                                   (!string.IsNullOrEmpty(c.Cedula) && c.Cedula.ToLower().Contains(textoBusqueda.ToLower())) ||
                                   (!string.IsNullOrEmpty(c.RNC) && c.RNC.ToLower().Contains(textoBusqueda.ToLower())) ||
                                   (c.ID_Cliente.ToString().Contains(textoBusqueda)))
                        .ToList();

                    dgvClientes.DataSource = null;
                    dgvClientes.DataSource = clientesFiltrados;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar clientes: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarBotones()
        {
            bool haySeleccion = clienteSeleccionado != null;
            btnEditar.Enabled = haySeleccion;
            btnEliminar.Enabled = haySeleccion;
        }

        #region Event Handlers

        private void BtnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frmAgregar = new FrmAgregarCliente())
                {
                    if (frmAgregar.ShowDialog() == DialogResult.OK)
                    {
                        CargarClientes(); // Recargar la lista
                        txtBusqueda.Clear(); // Limpiar búsqueda
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de agregar: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                if (clienteSeleccionado == null)
                {
                    MessageBox.Show("Por favor, seleccione un cliente para editar.", 
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var frmEditar = new FrmEditarCliente(clienteSeleccionado))
                {
                    if (frmEditar.ShowDialog() == DialogResult.OK)
                    {
                        CargarClientes(); // Recargar la lista
                        txtBusqueda.Clear(); // Limpiar búsqueda
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de editar: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (clienteSeleccionado == null)
                {
                    MessageBox.Show("Por favor, seleccione un cliente para eliminar.", 
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al cliente '{clienteSeleccionado.Nombre}'?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar Eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    int filasAfectadas = clienteDAL.Eliminar(clienteSeleccionado.ID_Cliente);
                    
                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Cliente eliminado correctamente.", 
                            "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarClientes();
                        txtBusqueda.Clear();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el cliente.", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar cliente: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvClientes_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvClientes.SelectedRows.Count > 0)
                {
                    DataGridViewRow fila = dgvClientes.SelectedRows[0];
                    clienteSeleccionado = fila.DataBoundItem as Cliente;
                }
                else
                {
                    clienteSeleccionado = null;
                }
                
                ActualizarBotones();
            }
            catch
            {
                clienteSeleccionado = null;
                ActualizarBotones();
            }
        }

        private void DgvClientes_DoubleClick(object sender, EventArgs e)
        {
            BtnEditar_Click(sender, e);
        }

        private void TxtBusqueda_TextChanged(object sender, EventArgs e)
        {
            FiltrarClientes();
        }

        private void DgvClientes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // El formateo se maneja en el evento CellFormatting
        }

        private void DgvClientes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Evitar que se muestre el cuadro de diálogo de error
            e.ThrowException = false;
            
            // Opcional: Registrar el error para depuración
            System.Diagnostics.Debug.WriteLine($"DataGridView DataError: {e.Exception.Message} - Column: {e.ColumnIndex}, Row: {e.RowIndex}");
        }

        private void DgvClientes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Verificar si es la columna RNC
                if (dgvClientes.Columns["RNC"].Index == e.ColumnIndex)
                {
                    // Depuración: mostrar información de la celda
                    string valorOriginal = e.Value?.ToString() ?? "NULL";
                    
                    // Si el valor es DBNull, nulo o vacío, mostrar "N/A"
                    if (e.Value == DBNull.Value || e.Value == null || string.IsNullOrEmpty(e.Value.ToString()))
                    {
                        e.Value = "N/A";
                        e.FormattingApplied = true;
                        e.CellStyle.ForeColor = SystemColors.GrayText;
                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Italic);
                    }
                    else
                    {
                        // Si tiene RNC, mostrarlo normalmente
                        e.FormattingApplied = true;
                        e.CellStyle.ForeColor = SystemColors.ControlText;
                        e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Regular);
                    }
                }

                // También podemos formatear la columna TipoCliente para mejor visualización
                if (dgvClientes.Columns["TipoCliente"].Index == e.ColumnIndex && e.Value != null)
                {
                    string tipoCliente = e.Value.ToString();
                    switch (tipoCliente)
                    {
                        case "CF":
                            e.Value = "Consumidor Final";
                            e.CellStyle.ForeColor = SystemColors.ControlText;
                            break;
                        case "CCF":
                            e.Value = "Crédito Fiscal";
                            e.CellStyle.ForeColor = SystemColors.ControlText;
                            break;
                        default:
                            e.Value = "Desconocido";
                            e.CellStyle.ForeColor = SystemColors.GrayText;
                            break;
                    }
                    e.FormattingApplied = true;
                }

                // Formatear la columna Estado para mejor visualización
                if (dgvClientes.Columns["Estado"].Index == e.ColumnIndex && e.Value != null)
                {
                    bool estado = Convert.ToBoolean(e.Value);
                    e.Value = estado ? "Activo" : "Inactivo";
                    e.CellStyle.ForeColor = estado ? Color.DarkGreen : Color.DarkRed;
                    e.FormattingApplied = true;
                }
            }
            catch
            {
                // Si hay error en el formateo, no aplicar cambios
                e.FormattingApplied = false;
            }
        }

        #endregion
    }
}
