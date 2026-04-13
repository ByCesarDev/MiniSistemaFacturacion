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
    // Delegado para comunicación desacoplada
    public delegate void ClienteSeleccionadoEventHandler(object sender, Cliente cliente);

    public partial class FrmSelectorClientes : Form
    {
        #region Events

        public event ClienteSeleccionadoEventHandler ClienteSeleccionado;

        #endregion

        #region Properties

        private List<Cliente> _clientes;
        private List<Cliente> _clientesFiltrados;
        private ClienteDAL _clienteDAL;

        #endregion

        #region Constructor

        public FrmSelectorClientes()
        {
            InitializeComponent();
            _clienteDAL = new ClienteDAL();
            _clientes = new List<Cliente>();
            _clientesFiltrados = new List<Cliente>();
        }

        #endregion

        #region Form Events

        private void FrmSelectorClientes_Load(object sender, EventArgs e)
        {
            try
            {
                CargarClientes();
                ConfigurarDataGridView();
                txtBusqueda.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FiltrarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al filtrar clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClientes_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                SeleccionarClienteActual();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvClientes_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SeleccionarClienteActual();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al manejar tecla: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            try
            {
                SeleccionarClienteActual();
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

        private void CargarClientes()
        {
            try
            {
                _clientes = _clienteDAL.ObtenerTodos();
                _clientesFiltrados = new List<Cliente>(_clientes);
                
                // Mostrar todos los clientes inicialmente
                dgvClientes.DataSource = null;
                dgvClientes.DataSource = _clientesFiltrados;
                
                lblResultados.Text = $"{_clientesFiltrados.Count} clientes encontrados";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar clientes: {ex.Message}", ex);
            }
        }

        private void ConfigurarDataGridView()
        {
            try
            {
                // Configurar propiedades básicas
                dgvClientes.AutoGenerateColumns = false;
                dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvClientes.MultiSelect = false;
                dgvClientes.ReadOnly = true;
                dgvClientes.AllowUserToAddRows = false;
                dgvClientes.AllowUserToDeleteRows = false;
                dgvClientes.RowHeadersVisible = false;
                dgvClientes.AllowUserToOrderColumns = false;

                // Limpiar columnas existentes
                dgvClientes.Columns.Clear();

                // Agregar columnas
                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "ID_Cliente",
                    HeaderText = "ID",
                    DataPropertyName = "ID_Cliente",
                    Width = 60,
                    ReadOnly = true
                });

                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Nombre",
                    HeaderText = "Nombre",
                    DataPropertyName = "Nombre",
                    Width = 250,
                    ReadOnly = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                });

                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Cedula",
                    HeaderText = "Cédula/RNC",
                    DataPropertyName = "Cedula",
                    Width = 120,
                    ReadOnly = true
                });

                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Telefono",
                    HeaderText = "Teléfono",
                    DataPropertyName = "Telefono",
                    Width = 100,
                    ReadOnly = true
                });

                dgvClientes.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Direccion",
                    HeaderText = "Dirección",
                    DataPropertyName = "Direccion",
                    Width = 200,
                    ReadOnly = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al configurar DataGridView: {ex.Message}", ex);
            }
        }

        private void FiltrarClientes()
        {
            try
            {
                string textoBusqueda = txtBusqueda.Text.Trim().ToLower();

                if (string.IsNullOrEmpty(textoBusqueda))
                {
                    _clientesFiltrados = new List<Cliente>(_clientes);
                }
                else
                {
                    _clientesFiltrados = _clientes.Where(c =>
                        c.Nombre.ToLower().Contains(textoBusqueda) ||
                        c.Cedula.ToLower().Contains(textoBusqueda) ||
                        c.Telefono.ToLower().Contains(textoBusqueda) ||
                        (c.Direccion != null && c.Direccion.ToLower().Contains(textoBusqueda))
                    ).ToList();
                }

                dgvClientes.DataSource = null;
                dgvClientes.DataSource = _clientesFiltrados;

                lblResultados.Text = $"{_clientesFiltrados.Count} clientes encontrados";

                // Seleccionar primera fila si hay resultados
                if (_clientesFiltrados.Count > 0)
                {
                    dgvClientes.Rows[0].Selected = true;
                    dgvClientes.FirstDisplayedScrollingRowIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al filtrar clientes: {ex.Message}", ex);
            }
        }

        private void SeleccionarClienteActual()
        {
            try
            {
                if (dgvClientes.SelectedRows.Count > 0)
                {
                    int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["ID_Cliente"].Value);
                    Cliente clienteSeleccionado = _clientesFiltrados.FirstOrDefault(c => c.ID_Cliente == idCliente);

                    if (clienteSeleccionado != null)
                    {
                        // Disparar evento para notificar al formulario padre
                        ClienteSeleccionado?.Invoke(this, clienteSeleccionado);
                        
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo encontrar el cliente seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un cliente de la lista", "Selección", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al seleccionar cliente: {ex.Message}", ex);
            }
        }

        #endregion

        #region Designer Generated Code

        private TextBox txtBusqueda;
        private DataGridView dgvClientes;
        private Label lblResultados;
        private Button btnSeleccionar;
        private Button btnCancelar;
        private Label lblTitulo;
        private Label lblInstrucciones;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectorClientes));
            this.txtBusqueda = new System.Windows.Forms.TextBox();
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.lblResultados = new System.Windows.Forms.Label();
            this.btnSeleccionar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblInstrucciones = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBusqueda
            // 
            this.txtBusqueda.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBusqueda.Location = new System.Drawing.Point(16, 68);
            this.txtBusqueda.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtBusqueda.Name = "txtBusqueda";
            this.txtBusqueda.Size = new System.Drawing.Size(745, 26);
            this.txtBusqueda.TabIndex = 0;
            this.txtBusqueda.TextChanged += new System.EventHandler(this.txtBusqueda_TextChanged);
            // 
            // dgvClientes
            // 
            this.dgvClientes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClientes.ColumnHeadersHeight = 29;
            this.dgvClientes.Location = new System.Drawing.Point(16, 103);
            this.dgvClientes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.RowHeadersWidth = 51;
            this.dgvClientes.Size = new System.Drawing.Size(747, 394);
            this.dgvClientes.TabIndex = 1;
            this.dgvClientes.DoubleClick += new System.EventHandler(this.dgvClientes_DoubleClick);
            this.dgvClientes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvClientes_KeyDown);
            // 
            // lblResultados
            // 
            this.lblResultados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultados.AutoSize = true;
            this.lblResultados.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultados.Location = new System.Drawing.Point(16, 505);
            this.lblResultados.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResultados.Name = "lblResultados";
            this.lblResultados.Size = new System.Drawing.Size(151, 17);
            this.lblResultados.TabIndex = 2;
            this.lblResultados.Text = "0 clientes encontrados";
            // 
            // btnSeleccionar
            // 
            this.btnSeleccionar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSeleccionar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSeleccionar.Location = new System.Drawing.Point(532, 535);
            this.btnSeleccionar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSeleccionar.Name = "btnSeleccionar";
            this.btnSeleccionar.Size = new System.Drawing.Size(113, 37);
            this.btnSeleccionar.TabIndex = 2;
            this.btnSeleccionar.Text = "Seleccionar";
            this.btnSeleccionar.UseVisualStyleBackColor = true;
            this.btnSeleccionar.Click += new System.EventHandler(this.btnSeleccionar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(653, 535);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(109, 37);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(16, 11);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(200, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Seleccionar Cliente";
            // 
            // lblInstrucciones
            // 
            this.lblInstrucciones.AutoSize = true;
            this.lblInstrucciones.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstrucciones.Location = new System.Drawing.Point(16, 39);
            this.lblInstrucciones.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInstrucciones.Name = "lblInstrucciones";
            this.lblInstrucciones.Size = new System.Drawing.Size(489, 17);
            this.lblInstrucciones.TabIndex = 1;
            this.lblInstrucciones.Text = "Escriba para buscar o use las flechas para navegar. Enter para seleccionar.";
            // 
            // FrmSelectorClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 587);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSeleccionar);
            this.Controls.Add(this.lblResultados);
            this.Controls.Add(this.dgvClientes);
            this.Controls.Add(this.txtBusqueda);
            this.Controls.Add(this.lblInstrucciones);
            this.Controls.Add(this.lblTitulo);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MinimumSize = new System.Drawing.Size(661, 481);
            this.Name = "FrmSelectorClientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Seleccionar Cliente";
            this.Load += new System.EventHandler(this.FrmSelectorClientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
