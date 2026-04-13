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
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Services;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmFacturacion : Form
    {
        private List<DetalleFactura> listaDetalles = new List<DetalleFactura>();
        private ClienteDAL clienteDAL = new ClienteDAL();
        private ProductoDAL productoDAL = new ProductoDAL();

        private const decimal TASA_ITBIS = 0.18m;
        private bool _esEdicion = false;
        private int _idFacturaExistente = 0;

        // Campo para almacenar el cliente seleccionado
        private Cliente _clienteSeleccionado;

        // Campo para almacenar el producto seleccionado
        private Producto _productoSeleccionado;

        // Campos para almacenar datos de edición
        private Factura _facturaEdicion;
        private Cliente _clienteEdicion;
        private List<DetalleFactura> _detallesEdicion;

        public FrmFacturacion()
        {
            InitializeComponent();
            dgvDetalle.DataError += dgvDetalle_DataError;
        }

        private void dgvDetalle_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is ArgumentException && e.Context.HasFlag(DataGridViewDataErrorContexts.Parsing))
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
        }

        // Constructor para editar factura existente
        public FrmFacturacion(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            InitializeComponent();
            dgvDetalle.DataError += dgvDetalle_DataError;
            
            // Configurar modo edición
            _esEdicion = true;
            _idFacturaExistente = factura.ID_Factura;
            _facturaEdicion = factura;
            _clienteEdicion = cliente;
            _detallesEdicion = detalles;
            
            // Cargar datos en el formulario
            CargarDatosFactura(factura, cliente, detalles);
        }

        private void FrmFacturacion_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurarGridFactura();
                CargarTiposComprobante();
                CargarFormasPago();

                if (!_esEdicion)
                {
                    lblNumeroFactura.Text = FacturacionManager.Instance.GenerarSiguienteNumeroFactura();
                    dtpFecha.Value = DateTime.Now;
                    cmbTipoComprobante.SelectedIndex = 1; // Consumidor Final por defecto
                    cmbFormaPago.SelectedIndex = 0; // Efectivo por defecto
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar formulario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarGridFactura()
        {
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.Columns.Clear();
            
            // Columna Descripción
            var colDescripcion = new DataGridViewTextBoxColumn();
            colDescripcion.Name = "Descripcion";
            colDescripcion.HeaderText = "Descripción";
            colDescripcion.DataPropertyName = "Descripcion";
            colDescripcion.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colDescripcion.ReadOnly = true;
            dgvDetalle.Columns.Add(colDescripcion);

            // Columna Cantidad
            var colCantidad = new DataGridViewTextBoxColumn();
            colCantidad.Name = "Cantidad";
            colCantidad.HeaderText = "Cantidad";
            colCantidad.DataPropertyName = "Cantidad";
            colCantidad.Width = 80;
            colCantidad.ReadOnly = false;
            dgvDetalle.Columns.Add(colCantidad);

            // Columna Precio Unitario
            var colPrecio = new DataGridViewTextBoxColumn();
            colPrecio.Name = "PrecioUnitarioVenta";
            colPrecio.HeaderText = "Precio Unitario";
            colPrecio.DataPropertyName = "PrecioUnitarioVenta";
            colPrecio.Width = 100;
            colPrecio.ReadOnly = true;
            dgvDetalle.Columns.Add(colPrecio);

            // Columna Subtotal
            var colSubtotal = new DataGridViewTextBoxColumn();
            colSubtotal.Name = "Subtotal";
            colSubtotal.HeaderText = "Subtotal";
            colSubtotal.DataPropertyName = "Subtotal";
            colSubtotal.Width = 100;
            colSubtotal.ReadOnly = true;
            dgvDetalle.Columns.Add(colSubtotal);
        }

        
        
        private void CargarTiposComprobante()
        {
            var tipos = new[]
            {
                new { Value = "01", Text = "01 - Factura con Valor Fiscal" },
                new { Value = "02", Text = "02 - Factura para Consumidor Final" },
                new { Value = "03", Text = "03 - Nota de Débito" },
                new { Value = "04", Text = "04 - Nota de Crédito" }
            };

            cmbTipoComprobante.DataSource = tipos;
            cmbTipoComprobante.DisplayMember = "Text";
            cmbTipoComprobante.ValueMember = "Value";
        }

        private void CargarFormasPago()
        {
            var formas = new[]
            {
                new { Value = "EFECTIVO", Text = "Efectivo" },
                new { Value = "TARJETA", Text = "Tarjeta de Crédito/Débito" },
                new { Value = "TRANSFERENCIA", Text = "Transferencia Bancaria" },
                new { Value = "CHEQUE", Text = "Cheque" }
            };

            cmbFormaPago.DataSource = formas;
            cmbFormaPago.DisplayMember = "Text";
            cmbFormaPago.ValueMember = "Value";
        }

        private void CargarDatosFactura(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            try
            {
                lblNumeroFactura.Text = factura.NumeroFactura;
                dtpFecha.Value = factura.Fecha;
                txtClientes.Text = cliente.Nombre;
                _clienteSeleccionado = cliente;
                cmbTipoComprobante.SelectedValue = factura.TipoComprobante;
                cmbFormaPago.SelectedValue = factura.FormaPago;
                chkCredito.Checked = factura.Credito;

                listaDetalles.Clear();
                listaDetalles.AddRange(detalles);
                dgvDetalle.DataSource = null;
                dgvDetalle.DataSource = listaDetalles;

                ActualizarTotales();

                btnGuardarFactura.Text = "Actualizar";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos de factura: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarFactura_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarFormulario())
                    return;

                Factura f = new Factura
                {
                    NumeroFactura = lblNumeroFactura.Text,
                    Fecha = dtpFecha.Value,
                    ID_Cliente = _clienteSeleccionado.ID_Cliente,
                    TipoComprobante = cmbTipoComprobante.SelectedValue.ToString(),
                    FormaPago = cmbFormaPago.SelectedValue.ToString(),
                    Credito = chkCredito.Checked,
                    TotalBruto = decimal.Parse(txtSubtotal.Text),
                    ValorImpuesto = decimal.Parse(txtIVA.Text),
                    TotalNeto = decimal.Parse(txtTotalNeto.Text),
                    SaldoPendiente = chkCredito.Checked ? decimal.Parse(txtTotalNeto.Text) : 0
                };

                int id;
                if (_esEdicion)
                {
                    f.ID_Factura = _idFacturaExistente;
                    f.NCF = _facturaEdicion.NCF;
                    f.TipoComprobante = _facturaEdicion.TipoComprobante;

                    FacturacionManager.Instance.ActualizarFacturaCompleta(f, listaDetalles);
                    id = _idFacturaExistente;
                }
                else
                {
                    id = FacturacionManager.Instance.CrearFacturaCompleta(f, listaDetalles);
                }

                Factura facturaCompleta = FacturacionManager.Instance.ObtenerFactura(id);
                Cliente cliente = _clienteSeleccionado;

                HabilitarControles(false);

                using (FrmVistaPreviaPdf frmVistaPrevia = new FrmVistaPreviaPdf(facturaCompleta, cliente, listaDetalles, true))
                {
                    var result = frmVistaPrevia.ShowDialog();

                    if (result == DialogResult.Retry)
                    {
                        HabilitarControles(true);
                    }
                    else if (result == DialogResult.OK || result == DialogResult.Cancel)
                    {
                        HabilitarControles(true);
                        LimpiarFormularioFacturacion();
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Error al convertir los valores numéricos.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private bool ValidarFormulario()
        {
            if (_clienteSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnBuscarCliente.Focus();
                return false;
            }

            if (listaDetalles.Count == 0)
            {
                MessageBox.Show("Por favor, agregue al menos un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnBuscarProducto.Focus();
                return false;
            }

            return true;
        }

        private void btnAnadir_Click_1(object sender, EventArgs e)
        {
            if (_productoSeleccionado == null)
            {
                MessageBox.Show("Por favor, seleccione un producto.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnBuscarProducto.Focus();
                return;
            }

            int cant = (int)numCantidad.Value;

            DetalleFactura nuevoItem = new DetalleFactura
            {
                ID_Producto = _productoSeleccionado.ID_Producto,
                Descripcion = _productoSeleccionado.Descripcion,
                Cantidad = cant,
                PrecioUnitarioVenta = _productoSeleccionado.PrecioUnitario,
                Subtotal = cant * _productoSeleccionado.PrecioUnitario
            };

            listaDetalles.Add(nuevoItem);
            ActualizarInterfaz();

            txtProductos.Text = "";
            _productoSeleccionado = null;
            numCantidad.Value = 1;
            btnBuscarProducto.Focus();
        }

        private void ActualizarInterfaz()
        {
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = listaDetalles;
            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal subtotal = listaDetalles.Sum(d => d.Subtotal);
            decimal itbis = subtotal * TASA_ITBIS;
            decimal total = subtotal + itbis;

            txtSubtotal.Text = subtotal.ToString("F2");
            txtIVA.Text = itbis.ToString("F2");
            txtTotalNeto.Text = total.ToString("F2");
            txtSaldoPendiente.Text = chkCredito.Checked ? total.ToString("F2") : "0.00";
        }

        private void LimpiarFormularioFacturacion()
        {
            // Limpiar cliente
            txtClientes.Text = "";
            _clienteSeleccionado = null;

            // Limpiar productos
            txtProductos.Text = "";
            _productoSeleccionado = null;

            // Reiniciar cantidad
            numCantidad.Value = 1;

            listaDetalles.Clear();
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = listaDetalles;

            chkEnviarEmail.Checked = false;
            chkImprimirDirecto.Checked = false;
            chkCredito.Checked = false;

            if (cmbTipoComprobante.Items.Count > 1)
                cmbTipoComprobante.SelectedIndex = 1;
            else
                cmbTipoComprobante.SelectedIndex = -1;

            cmbFormaPago.Enabled = true;
            lblFormaPago.Enabled = true;

            if (cmbFormaPago.Items.Count > 0)
                cmbFormaPago.SelectedIndex = 0;
            else
                cmbFormaPago.SelectedIndex = -1;

            txtSubtotal.Text = "0.00";
            txtIVA.Text = "0.00";
            txtTotalNeto.Text = "0.00";
            txtSaldoPendiente.Text = "0.00";

            _esEdicion = false;
            _idFacturaExistente = 0;
            _facturaEdicion = null;
            _clienteEdicion = null;
            _detallesEdicion = null;

            lblNumeroFactura.Text = FacturacionManager.Instance.GenerarSiguienteNumeroFactura();
            btnGuardarFactura.Text = "Pagar";
            btnBuscarCliente.Focus();
        }

        private void btnEliminarItem_Click_1(object sender, EventArgs e)
        {
            if (dgvDetalle.SelectedRows.Count > 0)
            {
                int rowIndex = dgvDetalle.SelectedRows[0].Index;
                
                if (rowIndex >= 0 && rowIndex < listaDetalles.Count)
                {
                    var itemAEliminar = listaDetalles[rowIndex];

                    if (itemAEliminar != null)
                    {
                        listaDetalles.Remove(itemAEliminar);
                        ActualizarInterfaz();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el producto a eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("La selección no es válida. Por favor, seleccione una fila válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fila completa en la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HabilitarControles(bool habilitar)
        {
            btnBuscarCliente.Enabled = habilitar;
            txtProductos.Enabled = habilitar;
            btnBuscarProducto.Enabled = habilitar;
            numCantidad.Enabled = habilitar;
            btnAnadir.Enabled = habilitar;
            btnEliminarItem.Enabled = habilitar;
            dgvDetalle.ReadOnly = !habilitar;
            btnGuardarFactura.Enabled = habilitar;
            chkCredito.Enabled = habilitar;
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDetalle.Columns[e.ColumnIndex].HeaderText == "Cantidad")
            {
                try
                {
                    var item = listaDetalles[e.RowIndex];
                    var valorCelda = dgvDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    if (valorCelda == null || string.IsNullOrWhiteSpace(valorCelda.ToString()))
                    {
                        item.Cantidad = 0;
                        item.Subtotal = 0;
                    }
                    else if (int.TryParse(valorCelda.ToString(), out int nuevaCantidad))
                    {
                        item.Cantidad = nuevaCantidad;
                        item.Subtotal = item.Cantidad * item.PrecioUnitarioVenta;
                    }

                    dgvDetalle.Refresh();
                    ActualizarTotales();
                }
                catch
                {
                    // Error silencioso mientras el usuario escribe
                }
            }
        }

        private void dgvDetalle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDetalle.IsCurrentCellDirty)
            {
                dgvDetalle.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label10_Click(object sender, EventArgs e)
        {
        }

        private void chkCredito_CheckedChanged(object sender, EventArgs e)
        {
            // Actualizar saldo pendiente cuando cambia el estado de crédito
            if (chkCredito.Checked)
            {
                txtSaldoPendiente.Text = txtTotalNeto.Text;
            }
            else
            {
                txtSaldoPendiente.Text = "0.00";
            }
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frmBusqueda = new FrmBusquedaClientes())
                {
                    if (frmBusqueda.ShowDialog() == DialogResult.OK)
                    {
                        var clienteSeleccionado = frmBusqueda.ClienteSeleccionado;
                        if (clienteSeleccionado != null)
                        {
                            // Asignar el cliente seleccionado
                            _clienteSeleccionado = clienteSeleccionado;
                            txtClientes.Text = clienteSeleccionado.GetIdentificadorCompleto();
                            
                            // Auto-seleccionar tipo de comprobante según tipo de cliente
                            SeleccionarTipoComprobanteSegunCliente(clienteSeleccionado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarTipoComprobanteSegunCliente(Cliente cliente)
        {
            try
            {
                if (cliente == null) return;

                // Auto-seleccionar tipo de comprobante según tipo de cliente
                if (cliente.EsCreditoFiscal())
                {
                    // Cliente de Crédito Fiscal -> Factura con Valor Fiscal (índice 0)
                    if (cmbTipoComprobante.Items.Count > 0)
                    {
                        cmbTipoComprobante.SelectedIndex = 0;
                    }
                }
                else
                {
                    // Consumidor Final -> Factura para Consumidor Final (índice 1)
                    if (cmbTipoComprobante.Items.Count > 1)
                    {
                        cmbTipoComprobante.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar tipo de comprobante: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frmBusqueda = new FrmBusquedaProductos())
                {
                    if (frmBusqueda.ShowDialog() == DialogResult.OK)
                    {
                        var productoSeleccionado = frmBusqueda.ProductoSeleccionado;
                        if (productoSeleccionado != null)
                        {
                            // Asignar el producto seleccionado
                            _productoSeleccionado = productoSeleccionado;
                            txtProductos.Text = productoSeleccionado.Descripcion;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
