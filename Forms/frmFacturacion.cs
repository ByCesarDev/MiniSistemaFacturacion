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
            // Si el error es por un valor nulo (DBNull) en una columna numérica
            if (e.Exception is ArgumentException && e.Context.HasFlag(DataGridViewDataErrorContexts.Parsing))
            {
                // Evitamos que salte el diálogo de error y cancelamos el evento
                e.ThrowException = false;
                e.Cancel = true;
            }
        }

        // Constructor para editar factura existente
        public FrmFacturacion(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            InitializeComponent();
            
            // Almacenar datos para usarlos en el Load
            _facturaEdicion = factura;
            _clienteEdicion = cliente;
            _detallesEdicion = detalles;
            
            // Establecer modo edición
            _esEdicion = true;
            _idFacturaExistente = factura.ID_Factura;
        }

        private void CargarDatosFactura()
        {
            // Establecer datos de la factura
            lblNumeroFactura.Text = _facturaEdicion.NumeroFactura;
            
            // Buscar y seleccionar el cliente
            if (_clienteEdicion != null)
            {
                // Intentar usar SelectedValue primero
                cmbClientes.SelectedValue = _clienteEdicion.ID_Cliente;
                
                // Si SelectedValue no funciona, buscar por índice
                if (cmbClientes.SelectedValue == null || (int)cmbClientes.SelectedValue != _clienteEdicion.ID_Cliente)
                {
                    for (int i = 0; i < cmbClientes.Items.Count; i++)
                    {
                        Cliente item = (Cliente)cmbClientes.Items[i];
                        if (item.ID_Cliente == _clienteEdicion.ID_Cliente)
                        {
                            cmbClientes.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                // Verificación final
                if (cmbClientes.SelectedItem == null)
                {
                    MessageBox.Show($"No se encontró el cliente '{_clienteEdicion.Nombre}' (ID: {_clienteEdicion.ID_Cliente}) en la lista. Por favor, seleccione un cliente manualmente.", 
                                  "Cliente no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            
            // Cargar detalles
            listaDetalles = new List<DetalleFactura>(_detallesEdicion);
            
            // Actualizar interfaz
            ActualizarInterfaz();
            
            // IMPORTANTE: Asegurar que los controles estén habilitados para edición
            HabilitarControles(true);
        }

        private void FrmFacturacion_Load(object sender, EventArgs e)
        {
            CargarCombos(); 
            ConfigurarGridFactura();
            
            // Si estamos en modo edición, cargar los datos
            if (_esEdicion)
            {
                CargarDatosFactura();
            }
            else
            {
                // Solo generar número si es factura nueva
                lblNumeroFactura.Text = FacturacionManager.Instance.GenerarSiguienteNumeroFactura();
            }
        }

        private void CargarCombos()
        {
            // Lógica para cargar Clientes en el ComboBox
            cmbClientes.DataSource = clienteDAL.ObtenerTodos();
            cmbClientes.DisplayMember = "Nombre";
            cmbClientes.ValueMember = "ID_Cliente";
            cmbClientes.SelectedIndex = -1; 

            // Lógica para cargar Productos
            cmbProductos.DataSource = productoDAL.ObtenerTodos();
            cmbProductos.DisplayMember = "Descripcion";
            cmbProductos.ValueMember = "ID_Producto";
            cmbProductos.SelectedIndex = -1;
        }



        private void ConfigurarGridFactura()
        {
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.Columns.Clear();

            // 1. Permitir edición en el grid
            dgvDetalle.ReadOnly = false;

            // Columna ID (No editable)
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ID_Producto",
                HeaderText = "ID",
                Width = 50,
                ReadOnly = true
            });

            // Columna Producto (No editable)
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descripcion",
                HeaderText = "Producto",
                Width = 220,
                ReadOnly = true
            });

            // Columna CANTIDAD (ESTA SÍ ES EDITABLE)
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "Cantidad",
                Width = 60,
                ReadOnly = false
            });

            // Columna Precio (No editable)
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioUnitarioVenta",
                HeaderText = "Precio",
                Width = 90,
                DefaultCellStyle = { Format = "N2" },
                ReadOnly = true
            });

            // Columna Subtotal (No editable)
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Subtotal",
                HeaderText = "Subtotal",
                Width = 100,
                DefaultCellStyle = { Format = "N2" },
                ReadOnly = true
            });
        }





        private void ActualizarInterfaz()
        {
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = listaDetalles;

            ActualizarTotales();
        }

        private void ActualizarTotales()
        {
            decimal bruto = listaDetalles.Sum(d => d.Subtotal);
            decimal itbis = bruto * TASA_ITBIS;
            decimal neto = bruto + itbis;

            txtSubtotal.Text = bruto.ToString("N2");
            txtIVA.Text = itbis.ToString("N2");
            txtTotalNeto.Text = neto.ToString("N2");
            txtSaldoPendiente.Text = neto.ToString("N2");
        }
        

        private void btnGuardarFactura_Click_1(object sender, EventArgs e)
        {
            try
            {
               
                if (cmbClientes.SelectedIndex == -1)
                    throw new Exception("Debe seleccionar un cliente de la lista.");

                if (listaDetalles.Count == 0)
                    throw new Exception("No puede guardar una factura sin productos.");

                Factura f = new Factura
                {
                    ID_Cliente = (int)cmbClientes.SelectedValue, 
                    NumeroFactura = lblNumeroFactura.Text,
                    Fecha = DateTime.Now,
                    PorcentajeImpuesto = TASA_ITBIS * 100,
                    Estado = "Pendiente"
                };

                int id;
                if (_esEdicion)
                {
                    // Actualizar factura existente - mantener NCF original
                    f.ID_Factura = _idFacturaExistente;
                    f.NCF = _facturaEdicion.NCF; // Mantener el NCF original
                    f.TipoComprobante = _facturaEdicion.TipoComprobante; // Mantener tipo de comprobante
                    FacturacionManager.Instance.ActualizarFacturaCompleta(f, listaDetalles);
                    id = _idFacturaExistente;
                }
                else
                {
                    // Crear nueva factura
                    id = FacturacionManager.Instance.CrearFacturaCompleta(f, listaDetalles);
                }
                
                // Obtener factura completa para PDF
                Factura facturaCompleta = FacturacionManager.Instance.ObtenerFactura(id);
                Cliente cliente = (Cliente)cmbClientes.SelectedItem;

                // 1. Deshabilitar antes de mostrar vista previa
                HabilitarControles(false);

                // Abrir vista previa automáticamente
                using (FrmVistaPreviaPdf frmVistaPrevia = new FrmVistaPreviaPdf(facturaCompleta, cliente, listaDetalles, true))
                {
                    var result = frmVistaPrevia.ShowDialog();

                    if (result == DialogResult.Retry)
                    {
                        // Esto es lo que permite que vuelvas a editar la cantidad y el cliente
                        HabilitarControles(true);
                    }
                    else if (result == DialogResult.OK)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
    
        }

        private void btnAnadir_Click_1(object sender, EventArgs e)
        {
            if (cmbProductos.SelectedIndex == -1) return;

            
            Producto prod = (Producto)cmbProductos.SelectedItem;
            int cant = (int)numCantidad.Value;

           
            DetalleFactura nuevoItem = new DetalleFactura
            {
                ID_Producto = prod.ID_Producto,
                Descripcion = prod.Descripcion,
                Cantidad = cant,
                PrecioUnitarioVenta = prod.PrecioUnitario,
                Subtotal = cant * prod.PrecioUnitario
            };

            listaDetalles.Add(nuevoItem);


            ActualizarInterfaz();

            cmbProductos.SelectedIndex = -1;
            numCantidad.Value = 1;

            cmbProductos.Focus();
        }

        private void btnEliminarItem_Click_1(object sender, EventArgs e)
        {
            if (dgvDetalle.SelectedRows.Count > 0)
            {
                
                int idProducto = Convert.ToInt32(dgvDetalle.SelectedRows[0].Cells[0].Value);

                
                var itemAEliminar = listaDetalles.FirstOrDefault(d => d.ID_Producto == idProducto);

                if (itemAEliminar != null)
                {
                    listaDetalles.Remove(itemAEliminar);

                    ActualizarInterfaz();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fila completa en la tabla para eliminar.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void HabilitarControles(bool habilitar)
        {
            cmbClientes.Enabled = habilitar;
            cmbProductos.Enabled = habilitar;
            numCantidad.Enabled = habilitar;
            btnAnadir.Enabled = habilitar;
            btnEliminarItem.Enabled = habilitar;
            dgvDetalle.ReadOnly = !habilitar;
            btnGuardarFactura.Enabled = habilitar;
        }

        private void dgvDetalle_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDetalle.Columns[e.ColumnIndex].HeaderText == "Cantidad")
            {
                try
                {
                    var item = listaDetalles[e.RowIndex];
                    var valorCelda = dgvDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    // Permitir valores vacíos - establecer cantidad a 0 si está vacío
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

                    // Actualizar solo la fila modificada para mantener el foco
                    dgvDetalle.Refresh();
                    ActualizarTotales();
                }
                catch
                {
                    // Error silencioso para no interrumpir al usuario mientras escribe
                }
            }
        }

        private void dgvDetalle_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvDetalle.IsCurrentCellDirty)
            {
                // Esto fuerza a que el CellValueChanged se dispare inmediatamente
                dgvDetalle.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}