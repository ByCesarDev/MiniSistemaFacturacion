using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmReportes : Form
    {
        // Componentes del Dashboard (definidos en Designer)
        // private Panel chartFacturasPeriodo;
        // private Panel chartProductosMasVendidos;
        // private Panel panelInventario;
        // private Panel panelCuentasPorCobrar;
        // private DataGridView dgvClientesDeuda;
        // private TableLayoutPanel dashboardLayout;

        public FrmReportes()
        {
            InitializeComponent();
            
            // Inicializar Dashboard
            InicializarDashboard();
        }

        private void FrmReportes_Load(object sender, EventArgs e)
        {
            // Configuración adicional si es necesaria
        }

        #region Dashboard Methods

        private void InicializarDashboard()
        {
            try
            {
                // Validar que los componentes del Designer no sean nulos
                if (chartFacturasPanel == null || chartProductosPanel == null || 
                    panelInventario == null || panelCuentasPorCobrar == null || 
                    dgvClientesDeuda == null)
                {
                    MessageBox.Show("Error: Los componentes del dashboard no están inicializados correctamente.", 
                        "Error de Componentes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Los componentes ya están creados en el Designer
                // Solo necesitamos poblarlos con los datos
                
                // Crear componentes
                CrearGraficoFacturasPeriodo();
                CrearGraficoProductosMasVendidos();
                CrearIndicadoresRapidos();
                CrearListaClientesDeuda();

                // Cargar datos iniciales
                ActualizarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar dashboard: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CrearGraficoFacturasPeriodo()
        {
            // Usar el panel del Designer
            chartFacturasPanel.Controls.Clear();

            var lblTitulo = new Label
            {
                Text = "Ventas (7 días)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 25,
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White
            };

            var panelDatos = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Crear barras visuales simples (más pequeñas)
            var flowPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoScroll = true
            };

            // Datos de ejemplo
            var valores = new[] { 15000, 22000, 18000, 25000, 30000, 28000, 35000 };
            var fechas = Enumerable.Range(0, 7).Select(i => DateTime.Now.AddDays(-6 + i).ToString("dd/MM")).ToArray();

            for (int i = 0; i < valores.Length; i++)
            {
                var panelBarra = new Panel
                {
                    Width = 40,
                    Height = 100,
                    Margin = new Padding(2, 0, 2, 0)
                };

                var barra = new Panel
                {
                    Height = (int)(valores[i] / 35000.0 * 60), // Normalizar altura más pequeña
                    Width = 25,
                    BackColor = Color.FromArgb(52, 152, 219),
                    Dock = DockStyle.Bottom
                };

                var lblFecha = new Label
                {
                    Text = fechas[i],
                    Font = new Font("Segoe UI", 6F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Bottom,
                    Height = 15
                };

                var lblValor = new Label
                {
                    Text = $"{(valores[i] / 1000)}K",
                    Font = new Font("Segoe UI", 6F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Bottom,
                    Height = 12,
                    ForeColor = Color.Gray
                };

                panelBarra.Controls.Add(barra);
                panelBarra.Controls.Add(lblValor);
                panelBarra.Controls.Add(lblFecha);
                flowPanel.Controls.Add(panelBarra);
            }

            panelDatos.Controls.Add(flowPanel);
            chartFacturasPanel.Controls.Add(panelDatos);
            chartFacturasPanel.Controls.Add(lblTitulo);
        }

        private void CrearGraficoProductosMasVendidos()
        {
            // Usar el panel del Designer
            chartProductosPanel.Controls.Clear();

            var lblTitulo = new Label
            {
                Text = "Top 5 Productos",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Height = 25,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White
            };

            var panelDatos = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            // Datos de ejemplo
            var productos = new[] { "Laptop HP", "Mouse", "Teclado", "Monitor", "Impresora" };
            var cantidades = new[] { 45, 120, 85, 32, 67 };

            for (int i = 0; i < productos.Length; i++)
            {
                var panelItem = new Panel
                {
                    Height = 25,
                    Dock = DockStyle.Top,
                    Margin = new Padding(0, 1, 0, 1)
                };

                var lblProducto = new Label
                {
                    Text = productos[i],
                    Font = new Font("Segoe UI", 7F),
                    Dock = DockStyle.Left,
                    Width = 80,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                var panelBarra = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5, 2, 5, 2)
                };

                var barra = new Panel
                {
                    Width = (int)(cantidades[i] / 120.0 * 100), // Normalizar ancho más pequeño
                    Height = 20,
                    BackColor = Color.FromArgb(46, 204, 113),
                    Dock = DockStyle.Left
                };

                var lblCantidad = new Label
                {
                    Text = cantidades[i].ToString(),
                    Font = new Font("Segoe UI", 7F, FontStyle.Bold),
                    Dock = DockStyle.Right,
                    Width = 30,
                    TextAlign = ContentAlignment.MiddleRight
                };

                panelBarra.Controls.Add(barra);
                panelItem.Controls.Add(lblCantidad);
                panelItem.Controls.Add(panelBarra);
                panelItem.Controls.Add(lblProducto);
                panelDatos.Controls.Add(panelItem);
            }

            chartProductosPanel.Controls.Add(panelDatos);
            chartProductosPanel.Controls.Add(lblTitulo);
        }

        private void CrearIndicadoresRapidos()
        {
            // Usar los paneles del Designer
            panelInventario.Controls.Clear();
            panelCuentasPorCobrar.Controls.Clear();

            // Panel para Valor del Inventario
            var lblInventarioTitulo = new Label
            {
                Text = "Inventario",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 20
            };

            var lblInventarioValor = new Label
            {
                Text = "RD$2.5M",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelInventario.Controls.Add(lblInventarioValor);
            panelInventario.Controls.Add(lblInventarioTitulo);

            // Panel para Cuentas por Cobrar
            var lblCuentasTitulo = new Label
            {
                Text = "Cuentas x Cobrar",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 20
            };

            var lblCuentasValor = new Label
            {
                Text = "RD$125K",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelCuentasPorCobrar.Controls.Add(lblCuentasValor);
            panelCuentasPorCobrar.Controls.Add(lblCuentasTitulo);
        }

        private void CrearListaClientesDeuda()
        {
            // Usar el DataGridView del Designer
            dgvClientesDeuda.Columns.Clear();

            // Configurar columnas
            dgvClientesDeuda.Columns.Add("Cliente", "Cliente");
            dgvClientesDeuda.Columns.Add("Deuda", "Deuda");
            dgvClientesDeuda.Columns.Add("FechaVencimiento", "Vencimiento");

            // Estilo de columnas
            dgvClientesDeuda.Columns["Deuda"].DefaultCellStyle.Format = "C";
            dgvClientesDeuda.Columns["Deuda"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvClientesDeuda.Columns["FechaVencimiento"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        public void ActualizarDashboard()
        {
            try
            {
                // Actualizar gráfico de facturas
                ActualizarGraficoFacturas();

                // Actualizar gráfico de productos
                ActualizarGraficoProductos();

                // Actualizar indicadores
                ActualizarIndicadores();

                // Actualizar lista de clientes con deuda
                ActualizarClientesDeuda();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar dashboard: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarGraficoFacturas()
        {
            // Los datos ya están incrustados en el método CrearGraficoFacturasPeriodo
            // Para actualizar dinámicamente, podríamos recorrer los controles y actualizar valores
            // Por ahora, el dashboard muestra los datos de ejemplo estáticos
        }

        private void ActualizarGraficoProductos()
        {
            // Los datos ya están incrustados en el método CrearGraficoProductosMasVendidos
            // Para actualizar dinámicamente, podríamos recorrer los controles y actualizar valores
            // Por ahora, el dashboard muestra los datos de ejemplo estáticos
        }

        private void ActualizarIndicadores()
        {
            try
            {
                // Datos de ejemplo para indicadores
                double valorInventario = 2500000.50;
                double totalCuentas = 125000.75;

                // Validar que los paneles no sean nulos
                if (panelInventario != null && panelInventario.Controls.Count > 0)
                {
                    var lblInventarioValor = panelInventario.Controls[0] as Label;
                    if (lblInventarioValor != null)
                    {
                        lblInventarioValor.Text = $"RD${valorInventario:N2}";
                    }
                }

                if (panelCuentasPorCobrar != null && panelCuentasPorCobrar.Controls.Count > 0)
                {
                    var lblCuentasValor = panelCuentasPorCobrar.Controls[0] as Label;
                    if (lblCuentasValor != null)
                    {
                        lblCuentasValor.Text = $"RD${totalCuentas:N2}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar indicadores: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ActualizarClientesDeuda()
        {
            try
            {
                // Datos de ejemplo para clientes con deuda
                var clientesDeuda = new[]
                {
                    new { Cliente = "Juan Pérez", Deuda = 15000, FechaVencimiento = DateTime.Now.AddDays(5) },
                    new { Cliente = "María García", Deuda = 8500, FechaVencimiento = DateTime.Now.AddDays(10) },
                    new { Cliente = "Carlos Rodríguez", Deuda = 22000, FechaVencimiento = DateTime.Now.AddDays(2) },
                    new { Cliente = "Ana Martínez", Deuda = 12000, FechaVencimiento = DateTime.Now.AddDays(15) },
                    new { Cliente = "Luis Sánchez", Deuda = 5000, FechaVencimiento = DateTime.Now.AddDays(20) }
                };

                dgvClientesDeuda.Rows.Clear();
                foreach (var cliente in clientesDeuda)
                {
                    // Validar valores nulos antes de agregar
                    string clienteNombre = cliente.Cliente ?? "N/A";
                    decimal deudaValor = cliente.Deuda >= 0 ? cliente.Deuda : 0;
                    string fechaVencimiento = cliente.FechaVencimiento != null ? 
                        cliente.FechaVencimiento.ToString("dd/MM/yyyy") : 
                        DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");

                    dgvClientesDeuda.Rows.Add(clienteNombre, deudaValor, fechaVencimiento);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar clientes con deuda: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
