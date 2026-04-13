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
using MiniSistemaFacturacion.BusinessLogic;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para búsqueda de facturas existentes
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public partial class FrmBusquedaFacturas : Form
    {
        #region Properties

        private FacturacionManager _facturacionManager;
        private List<Factura> _facturasEncontradas;
        private Cliente _clienteSeleccionado;

        #endregion

        #region Constructor

        public FrmBusquedaFacturas()
        {
            InitializeComponent();
            _facturacionManager = FacturacionManager.Instance;
            _facturasEncontradas = new List<Factura>();
        }

        #endregion

        #region Form Events

        private void FrmBusquedaFacturas_Load(object sender, EventArgs e)
        {
            try
            {
                // Configurar DataGridView
                ConfigurarDataGridView();
                
                // Establecer fechas por defecto
                dtpFechaDesde.Value = DateTime.Now.AddDays(-30);
                dtpFechaHasta.Value = DateTime.Now;
                
                // Cargar automáticamente las últimas 100 facturas
                try
                {
                    _facturasEncontradas = _facturacionManager.ObtenerUltimasFacturas(100);
                    
                    // Mostrar resultados
                    dgvFacturas.DataSource = null;
                    dgvFacturas.DataSource = _facturasEncontradas;
                    dgvFacturas.Refresh();
                    dgvFacturas.Update();
                    
                    // Actualizar información
                    lblResultados.Text = $"Mostrando las últimas {_facturasEncontradas.Count} facturas";
                }
                catch (Exception ex)
                {
                    lblResultados.Text = "Error al cargar facturas iniciales";
                    MessageBox.Show($"Error al cargar las últimas facturas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                BuscarFacturas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar facturas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void dgvFacturas_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (dgvFacturas.SelectedRows.Count > 0)
                {
                    int idFactura = Convert.ToInt32(dgvFacturas.SelectedRows[0].Cells["ID_Factura"].Value);
                    AbrirVistaPreviaFactura(idFactura);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNumeroFactura_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitir números
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        #endregion

        #region Private Methods

        private void ConfigurarDataGridView()
        {
            try
            {
                // Configurar columnas
                dgvFacturas.AutoGenerateColumns = false;
                dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvFacturas.MultiSelect = false;
                dgvFacturas.ReadOnly = true;
                dgvFacturas.AllowUserToAddRows = false;
                dgvFacturas.AllowUserToDeleteRows = false;
                dgvFacturas.RowHeadersVisible = false;

                // Agregar columnas
                dgvFacturas.Columns.Clear();

                // Columna ID Factura (oculta)
                DataGridViewTextBoxColumn colID = new DataGridViewTextBoxColumn();
                colID.Name = "ID_Factura";
                colID.HeaderText = "ID Factura";
                colID.DataPropertyName = "ID_Factura";
                colID.Visible = false;
                dgvFacturas.Columns.Add(colID);

                // Columna Número Factura
                DataGridViewTextBoxColumn colNumero = new DataGridViewTextBoxColumn();
                colNumero.Name = "NumeroFactura";
                colNumero.HeaderText = "Número Factura";
                colNumero.DataPropertyName = "NumeroFactura";
                colNumero.Width = 120;
                dgvFacturas.Columns.Add(colNumero);

                // Columna Fecha
                DataGridViewTextBoxColumn colFecha = new DataGridViewTextBoxColumn();
                colFecha.Name = "Fecha";
                colFecha.HeaderText = "Fecha";
                colFecha.DataPropertyName = "Fecha";
                colFecha.Width = 100;
                colFecha.DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvFacturas.Columns.Add(colFecha);

                // Columna Cliente (desde la consulta SQL)
                DataGridViewTextBoxColumn colCliente = new DataGridViewTextBoxColumn();
                colCliente.Name = "ClienteNombre";
                colCliente.HeaderText = "Cliente";
                colCliente.DataPropertyName = "ClienteNombre";
                colCliente.Width = 200;
                dgvFacturas.Columns.Add(colCliente);

                // Columna Total Neto
                DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
                colTotal.Name = "TotalNeto";
                colTotal.HeaderText = "Total";
                colTotal.DataPropertyName = "TotalNeto";
                colTotal.Width = 100;
                colTotal.DefaultCellStyle.Format = "C2";
                colTotal.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvFacturas.Columns.Add(colTotal);

                // Columna Estado
                DataGridViewTextBoxColumn colEstado = new DataGridViewTextBoxColumn();
                colEstado.Name = "Estado";
                colEstado.HeaderText = "Estado";
                colEstado.DataPropertyName = "Estado";
                colEstado.Width = 80;
                dgvFacturas.Columns.Add(colEstado);

                DataGridViewTextBoxColumn colTipoComprobante = new DataGridViewTextBoxColumn();
                colTipoComprobante.Name = "TipoComprobante";
                colTipoComprobante.HeaderText = "Tipo Comp.";
                colTipoComprobante.DataPropertyName = "TipoComprobanteDescripcion";
                colTipoComprobante.Width = 90;
                dgvFacturas.Columns.Add(colTipoComprobante);

                // Columna NCF (si existe en el modelo)
                DataGridViewTextBoxColumn colNCF = new DataGridViewTextBoxColumn();
                colNCF.Name = "NCF";
                colNCF.HeaderText = "NCF";
                colNCF.DataPropertyName = "NCF";
                colNCF.Width = 150;
                dgvFacturas.Columns.Add(colNCF);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al configurar DataGridView: {ex.Message}");
            }
        }

        private void BuscarFacturas()
        {
            try
            {
                // Obtener filtros
                string numeroFactura = txtNumeroFactura.Text.Trim();
                string cliente = txtCliente.Text.Trim();
                int? idCliente = _clienteSeleccionado?.ID_Cliente;
                DateTime fechaDesde = dtpFechaDesde.Value.Date;
                DateTime fechaHasta = dtpFechaHasta.Value.Date;

                // Validar fechas
                if (fechaDesde > fechaHasta)
                {
                    MessageBox.Show("La fecha desde no puede ser mayor a la fecha hasta", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Si ambos campos están vacíos, usar rango por defecto (últimos 30 días)
                if (string.IsNullOrEmpty(numeroFactura) && string.IsNullOrEmpty(cliente))
                {
                    fechaDesde = DateTime.Today.AddDays(-30);
                    fechaHasta = DateTime.Today;
                    
                    // Actualizar los DatePicker para mostrar el rango usado
                    dtpFechaDesde.Value = fechaDesde;
                    dtpFechaHasta.Value = fechaHasta;
                }
                
                // Buscar facturas
                _facturasEncontradas = _facturacionManager.BuscarFacturas(numeroFactura, cliente, idCliente, fechaDesde, fechaHasta);

                // Mostrar resultados
                dgvFacturas.DataSource = null;
                dgvFacturas.DataSource = _facturasEncontradas;
                dgvFacturas.Refresh(); // Forzar actualización visual
                dgvFacturas.Update(); // Actualizar controles

                // Actualizar información
                lblResultados.Text = $"Se encontraron {_facturasEncontradas.Count} facturas";

                if (_facturasEncontradas.Count == 0)
                {
                    MessageBox.Show("No se encontraron facturas con los criterios especificados", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar facturas: {ex.Message}");
            }
        }

        private void LimpiarFiltros()
        {
            try
            {
                txtNumeroFactura.Clear();
               // Establecer fechas por defecto
                dtpFechaDesde.Value = DateTime.Now.AddDays(-30);
                dtpFechaHasta.Value = DateTime.Now;

                // Cargar automáticamente las últimas 100 facturas
                try
                {
                    _facturasEncontradas = _facturacionManager.ObtenerUltimasFacturas(100);
                    
                    // Mostrar resultados
                    dgvFacturas.DataSource = null;
                    dgvFacturas.DataSource = _facturasEncontradas;
                    dgvFacturas.Refresh();
                    dgvFacturas.Update();
                    
                    // Actualizar información
                    lblResultados.Text = $"Mostrando las últimas {_facturasEncontradas.Count} facturas";
                }
                catch (Exception ex)
                {
                    lblResultados.Text = "Error al cargar facturas iniciales";
                    MessageBox.Show($"Error al cargar las últimas facturas: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al limpiar filtros: {ex.Message}");
            }
        }

        private void AbrirVistaPreviaFactura(int idFactura)
{
    try
    {
        var facturaCompleta = _facturacionManager.ObtenerFacturaCompleta(idFactura);
        
        if (facturaCompleta == null)
        {
            MessageBox.Show("No se pudo encontrar la factura seleccionada", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // 1. Ocultar este formulario para que el usuario no lo vea más
        this.Hide();

        // 2. Crear la vista previa
        using (FrmVistaPreviaPdf frmVistaPrevia = new FrmVistaPreviaPdf(
            facturaCompleta, 
            facturaCompleta.Cliente, 
            facturaCompleta.Detalles, 
            false 
        ))
        {
            // 3. Mostrar la vista previa
            frmVistaPrevia.ShowDialog();
        }
        
        // 4. Establecer el resultado y cerrar definitivamente
        this.DialogResult = DialogResult.OK;
        this.Close();
    }
    catch (Exception ex)
    {
        this.Show(); // Volver a mostrar si hubo un error
        throw new Exception($"Error al abrir vista previa: {ex.Message}");
    }
}

        #endregion

        #region Designer Generated Code

        private System.ComponentModel.IContainer components = null;
        private GroupBox grpFiltros;
        private Label lblNumeroFactura;
        private TextBox txtNumeroFactura;
        private Label lblCliente;
        private TextBox txtCliente;
        private Button btnBuscarCliente;
        private Label lblFechaDesde;
        private DateTimePicker dtpFechaDesde;
        private Label lblFechaHasta;
        private DateTimePicker dtpFechaHasta;
        private Button btnBuscar;
        private Button btnLimpiar;
        private DataGridView dgvFacturas;
        private Label lblResultados;
        private Button btnCerrar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBusquedaFacturas));
            this.grpFiltros = new System.Windows.Forms.GroupBox();
            this.lblNumeroFactura = new System.Windows.Forms.Label();
            this.txtNumeroFactura = new System.Windows.Forms.TextBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.txtCliente = new System.Windows.Forms.TextBox();
            this.btnBuscarCliente = new System.Windows.Forms.Button();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.dgvFacturas = new System.Windows.Forms.DataGridView();
            this.lblResultados = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.grpFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).BeginInit();
            this.SuspendLayout();
            // 
            // grpFiltros
            // 
            this.grpFiltros.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFiltros.Controls.Add(this.lblNumeroFactura);
            this.grpFiltros.Controls.Add(this.txtNumeroFactura);
            this.grpFiltros.Controls.Add(this.lblCliente);
            this.grpFiltros.Controls.Add(this.txtCliente);
            this.grpFiltros.Controls.Add(this.btnBuscarCliente);
            this.grpFiltros.Controls.Add(this.lblFechaDesde);
            this.grpFiltros.Controls.Add(this.dtpFechaDesde);
            this.grpFiltros.Controls.Add(this.lblFechaHasta);
            this.grpFiltros.Controls.Add(this.dtpFechaHasta);
            this.grpFiltros.Controls.Add(this.btnBuscar);
            this.grpFiltros.Controls.Add(this.btnLimpiar);
            this.grpFiltros.Location = new System.Drawing.Point(12, 12);
            this.grpFiltros.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpFiltros.Name = "grpFiltros";
            this.grpFiltros.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grpFiltros.Size = new System.Drawing.Size(1160, 100);
            this.grpFiltros.TabIndex = 0;
            this.grpFiltros.TabStop = false;
            this.grpFiltros.Text = "Filtros de Búsqueda";
            this.grpFiltros.Enter += new System.EventHandler(this.grpFiltros_Enter);
            // 
            // lblNumeroFactura
            // 
            this.lblNumeroFactura.AutoSize = true;
            this.lblNumeroFactura.Location = new System.Drawing.Point(15, 30);
            this.lblNumeroFactura.Name = "lblNumeroFactura";
            this.lblNumeroFactura.Size = new System.Drawing.Size(106, 16);
            this.lblNumeroFactura.TabIndex = 0;
            this.lblNumeroFactura.Text = "Número Factura:";
            // 
            // txtNumeroFactura
            // 
            this.txtNumeroFactura.Location = new System.Drawing.Point(129, 27);
            this.txtNumeroFactura.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNumeroFactura.Name = "txtNumeroFactura";
            this.txtNumeroFactura.Size = new System.Drawing.Size(151, 22);
            this.txtNumeroFactura.TabIndex = 1;
            this.txtNumeroFactura.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNumeroFactura_KeyPress);
            // 
            // lblCliente
            // 
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(300, 30);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(51, 16);
            this.lblCliente.TabIndex = 2;
            this.lblCliente.Text = "Cliente:";
            // 
            // txtCliente
            // 
            this.txtCliente.Location = new System.Drawing.Point(356, 27);
            this.txtCliente.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.ReadOnly = true;
            this.txtCliente.Size = new System.Drawing.Size(200, 22);
            this.txtCliente.TabIndex = 3;
            this.txtCliente.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCliente_KeyPress);
            // 
            // btnBuscarCliente
            // 
            this.btnBuscarCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarCliente.Location = new System.Drawing.Point(563, 27);
            this.btnBuscarCliente.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBuscarCliente.Name = "btnBuscarCliente";
            this.btnBuscarCliente.Size = new System.Drawing.Size(29, 22);
            this.btnBuscarCliente.TabIndex = 4;
            this.btnBuscarCliente.Text = "🔍 Search";
            this.btnBuscarCliente.UseVisualStyleBackColor = true;
            this.btnBuscarCliente.Click += new System.EventHandler(this.btnBuscarCliente_Click);
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.Location = new System.Drawing.Point(600, 30);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(96, 16);
            this.lblFechaDesde.TabIndex = 5;
            this.lblFechaDesde.Text = "Fecha Desde:";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Location = new System.Drawing.Point(697, 27);
            this.dtpFechaDesde.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(151, 22);
            this.dtpFechaDesde.TabIndex = 6;
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Location = new System.Drawing.Point(917, 30);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(87, 16);
            this.lblFechaHasta.TabIndex = 7;
            this.lblFechaHasta.Text = "Fecha Hasta:";
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Location = new System.Drawing.Point(1008, 27);
            this.dtpFechaHasta.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(151, 22);
            this.dtpFechaHasta.TabIndex = 8;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.Location = new System.Drawing.Point(15, 60);
            this.btnBuscar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(100, 30);
            this.btnBuscar.TabIndex = 9;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Location = new System.Drawing.Point(125, 60);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(100, 30);
            this.btnLimpiar.TabIndex = 10;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // dgvFacturas
            // 
            this.dgvFacturas.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFacturas.ColumnHeadersHeight = 29;
            this.dgvFacturas.Location = new System.Drawing.Point(12, 121);
            this.dgvFacturas.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvFacturas.Name = "dgvFacturas";
            this.dgvFacturas.RowHeadersWidth = 51;
            this.dgvFacturas.Size = new System.Drawing.Size(1160, 400);
            this.dgvFacturas.TabIndex = 1;
            this.dgvFacturas.DoubleClick += new System.EventHandler(this.dgvFacturas_DoubleClick);
            // 
            // lblResultados
            // 
            this.lblResultados.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblResultados.AutoSize = true;
            this.lblResultados.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResultados.Location = new System.Drawing.Point(12, 530);
            this.lblResultados.Name = "lblResultados";
            this.lblResultados.Size = new System.Drawing.Size(205, 18);
            this.lblResultados.TabIndex = 2;
            this.lblResultados.Text = "Listo para buscar facturas";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Location = new System.Drawing.Point(1069, 550);
            this.btnCerrar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 30);
            this.btnCerrar.TabIndex = 3;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmBusquedaFacturas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 592);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.lblResultados);
            this.Controls.Add(this.dgvFacturas);
            this.Controls.Add(this.grpFiltros);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(1199, 630);
            this.Name = "FrmBusquedaFacturas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Buscar Facturas";
            this.Load += new System.EventHandler(this.FrmBusquedaFacturas_Load);
            this.grpFiltros.ResumeLayout(false);
            this.grpFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFacturas)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectorClientes selector = new FrmSelectorClientes();
                
                // Suscribir al evento de cliente seleccionado
                selector.ClienteSeleccionado += (s, cliente) => {
                    _clienteSeleccionado = cliente;
                    txtCliente.Text = cliente.Nombre;
                };
                
                // Mostrar selector como diálogo modal
                var result = selector.ShowDialog();
                
                // Si el usuario seleccionó un cliente, buscar sus facturas automáticamente
                if (result == DialogResult.OK && _clienteSeleccionado != null)
                {
                    // Limpiar otros filtros para enfocarse solo en el cliente
                    txtNumeroFactura.Clear();
                    
                    // Establecer rango de fechas amplio para obtener todas las facturas del cliente
                    dtpFechaDesde.Value = DateTime.Now.AddYears(-5); // 5 años atrás
                    dtpFechaHasta.Value = DateTime.Now;
                    
                    // Buscar facturas del cliente seleccionado
                    BuscarFacturas();
                }
                // Si el usuario canceló, limpiar el cliente seleccionado
                else if (result == DialogResult.Cancel)
                {
                    _clienteSeleccionado = null;
                    txtCliente.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir selector de clientes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Atajo F3 para abrir selector de clientes
            if (e.KeyChar == (char)Keys.F3)
            {
                btnBuscarCliente_Click(null, null);
                e.Handled = true;
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        private void grpFiltros_Enter(object sender, EventArgs e)
        {

        }
    }
}
