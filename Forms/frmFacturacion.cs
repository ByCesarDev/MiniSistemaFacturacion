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


namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmFacturacion : Form
    {
        
        private List<DetalleFactura> listaDetalles = new List<DetalleFactura>();
        private ClienteDAL clienteDAL = new ClienteDAL();
        private ProductoDAL productoDAL = new ProductoDAL();

        
        private int idClienteSeleccionado = -1;
        private const decimal TASA_ITBIS = 0.18m;
        

        public FrmFacturacion()
        {
            InitializeComponent();
        }

        private void FrmFacturacion_Load(object sender, EventArgs e)
        {
            CargarCombos(); 
            ConfigurarGridFactura();
            lblNumeroFactura.Text = FacturacionManager.Instance.GenerarSiguienteNumeroFactura();
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
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ID_Producto", HeaderText = "ID", Width = 50 });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Descripcion", HeaderText = "Producto", Width = 220 });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Cantidad", HeaderText = "Cantidad", Width = 60 });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PrecioUnitarioVenta", HeaderText = "Precio", Width = 90, DefaultCellStyle = { Format = "N2" } });
            dgvDetalle.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Subtotal", HeaderText = "Subtotal", Width = 100, DefaultCellStyle = { Format = "N2" } });
        }

        
        
        private void ActualizarInterfaz()
        {
            dgvDetalle.DataSource = null;
            dgvDetalle.DataSource = listaDetalles;

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

                int id = FacturacionManager.Instance.CrearFacturaCompleta(f, listaDetalles);
                MessageBox.Show($"Factura #{id} guardada con éxito.");
                this.Close();
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void btnVistaPreviaPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbClientes.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar un cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (listaDetalles.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Factura facturaTemp = new Factura
                {
                    NumeroFactura = lblNumeroFactura.Text,
                    ID_Cliente = (int)cmbClientes.SelectedValue,
                    Fecha = dtpFecha.Value,
                    PorcentajeImpuesto = TASA_ITBIS * 100,
                    TotalBruto = listaDetalles.Sum(d => d.Subtotal),
                    ValorImpuesto = listaDetalles.Sum(d => d.Subtotal) * TASA_ITBIS,
                    TotalNeto = listaDetalles.Sum(d => d.Subtotal) * (1 + TASA_ITBIS),
                    SaldoPendiente = listaDetalles.Sum(d => d.Subtotal) * (1 + TASA_ITBIS),
                    Estado = "Pendiente"
                };

                Cliente cliente = (Cliente)cmbClientes.SelectedItem;

                using (FrmVistaPreviaPdf frmVistaPrevia = new FrmVistaPreviaPdf(facturaTemp, cliente, listaDetalles))
                {
                    frmVistaPrevia.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGenerarPdf_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbClientes.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar un cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (listaDetalles.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la factura.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                IPdfTicketService pdfService = new PdfTicketService();

                Factura facturaTemp = new Factura
                {
                    NumeroFactura = lblNumeroFactura.Text,
                    ID_Cliente = (int)cmbClientes.SelectedValue,
                    Fecha = dtpFecha.Value,
                    PorcentajeImpuesto = TASA_ITBIS * 100,
                    TotalBruto = listaDetalles.Sum(d => d.Subtotal),
                    ValorImpuesto = listaDetalles.Sum(d => d.Subtotal) * TASA_ITBIS,
                    TotalNeto = listaDetalles.Sum(d => d.Subtotal) * (1 + TASA_ITBIS),
                    SaldoPendiente = listaDetalles.Sum(d => d.Subtotal) * (1 + TASA_ITBIS),
                    Estado = "Pendiente"
                };

                Cliente cliente = (Cliente)cmbClientes.SelectedItem;

                if (!pdfService.ValidarDatosTicket(facturaTemp, cliente, listaDetalles, out string mensajeError))
                {
                    MessageBox.Show($"Error de validación: {mensajeError}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                byte[] pdfBytes = pdfService.GenerarTicketPdf(facturaTemp, cliente, listaDetalles);

                string ruta = pdfService.ObtenerRutaCompleta(facturaTemp);
                pdfService.GuardarTicketPdf(ruta, pdfBytes);

                MessageBox.Show($"PDF generado exitosamente en:\n{ruta}", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (chkEnviarEmail.Checked && !string.IsNullOrWhiteSpace(cliente.Email))
                {
                    MessageBox.Show("Funcionalidad de email en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (chkImprimirDirecto.Checked)
                {
                    MessageBox.Show("Funcionalidad de impresión directa en desarrollo.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}