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
    public partial class FrmBusquedaProductos : Form
    {
        private ProductoDAL productoDAL = new ProductoDAL();
        private Producto _productoSeleccionado;

        public Producto ProductoSeleccionado
        {
            get { return _productoSeleccionado; }
            private set { _productoSeleccionado = value; }
        }

        public FrmBusquedaProductos()
        {
            InitializeComponent();
            CargarProductosIniciales();
        }

        private void CargarProductosIniciales()
        {
            try
            {
                var productos = productoDAL.ObtenerUltimosProductos(100);
                ConfigurarDataGridView();
                dgvProductos.DataSource = productos;
                dgvProductos.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos iniciales: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDataGridView()
        {
            // Limpiar columnas existentes
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.Columns.Clear();

            // Crear columnas manualmente
            dgvProductos.Columns.Add("ID_Producto", "ID");
            dgvProductos.Columns.Add("Codigo", "Código");
            dgvProductos.Columns.Add("Descripcion", "Descripción");
            dgvProductos.Columns.Add("PrecioUnitario", "Precio");
            dgvProductos.Columns.Add("Stock", "Stock");
            dgvProductos.Columns.Add("Categoria", "Categoría");

            // Configurar DataPropertyName para cada columna
            dgvProductos.Columns["ID_Producto"].DataPropertyName = "ID_Producto";
            dgvProductos.Columns["Codigo"].DataPropertyName = "Codigo";
            dgvProductos.Columns["Descripcion"].DataPropertyName = "Descripcion";
            dgvProductos.Columns["PrecioUnitario"].DataPropertyName = "PrecioUnitario";
            dgvProductos.Columns["Stock"].DataPropertyName = "Stock";
            dgvProductos.Columns["Categoria"].DataPropertyName = "Categoria";

            // Configurar columnas como de solo lectura
            foreach (DataGridViewColumn column in dgvProductos.Columns)
            {
                column.ReadOnly = true;
            }

            // Configurar anchos y visibilidad
            dgvProductos.Columns["ID_Producto"].Visible = false; // Ocultar ID
            dgvProductos.Columns["Codigo"].Width = 80;
            dgvProductos.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProductos.Columns["PrecioUnitario"].Width = 80;
            dgvProductos.Columns["Stock"].Width = 60;
            dgvProductos.Columns["Categoria"].Width = 100;

            // Formato de moneda para precio
            dgvProductos.Columns["PrecioUnitario"].DefaultCellStyle.Format = "C2";
            dgvProductos.Columns["PrecioUnitario"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // Alineación para stock
            dgvProductos.Columns["Stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                string descripcion = txtDescripcion.Text.Trim();
                string codigo = txtCodigo.Text.Trim();
                string categoria = txtCategoria.Text.Trim();

                var productos = productoDAL.BuscarProductos(descripcion, codigo, categoria);
                ConfigurarDataGridView();
                dgvProductos.DataSource = productos;
                dgvProductos.Refresh();

                lblResultados.Text = $"Se encontraron {productos.Count} productos";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDescripcion.Text = "";
            txtCodigo.Text = "";
            txtCategoria.Text = "";
            CargarProductosIniciales();
            lblResultados.Text = "";
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarProductoActual();
        }

        private void dgvProductos_DoubleClick(object sender, EventArgs e)
        {
            SeleccionarProductoActual();
        }

        private void SeleccionarProductoActual()
        {
            // Intentar obtener la fila seleccionada
            DataGridViewRow selectedRow = null;
            
            // Si hay filas seleccionadas, usar la primera
            if (dgvProductos.SelectedRows.Count > 0)
            {
                selectedRow = dgvProductos.SelectedRows[0];
            }
            // Si no hay filas seleccionadas pero hay celda seleccionada, obtener su fila
            else if (dgvProductos.CurrentRow != null)
            {
                selectedRow = dgvProductos.CurrentRow;
            }
            
            if (selectedRow != null && selectedRow.DataBoundItem != null)
            {
                try
                {
                    var productoSeleccionado = (Producto)selectedRow.DataBoundItem;
                    ProductoSeleccionado = productoSeleccionado;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al seleccionar producto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un producto de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void FrmBusquedaProductos_Load(object sender, EventArgs e)
        {
            lblResultados.Text = "";
        }

        private void dgvProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridViewColumn column = dgvProductos.Columns[e.ColumnIndex];
                
                // Formato de moneda para PrecioUnitario
                if (column.DataPropertyName == "PrecioUnitario")
                {
                    if (decimal.TryParse(e.Value.ToString(), out decimal precio))
                    {
                        e.Value = precio.ToString("C2");
                        e.FormattingApplied = true;
                    }
                }
                // Cambio de color para Stock bajo (sin cambiar el valor, solo el estilo)
                else if (column.DataPropertyName == "Stock")
                {
                    if (int.TryParse(e.Value.ToString(), out int stock))
                    {
                        // SOLO cambiar el estilo, NO modificar el valor
                        if (stock <= 10)
                        {
                            e.CellStyle.ForeColor = Color.Red;
                            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);
                        }
                        else if (stock <= 20)
                        {
                            e.CellStyle.ForeColor = Color.Orange;
                        }
                        else
                        {
                            e.CellStyle.ForeColor = SystemColors.WindowText; // Color normal
                            e.CellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Regular);
                        }
                        // NO establecer FormattingApplied = true para Stock
                        // Esto permite que el DataGridView muestre el valor original
                    }
                }
            }
        }

        private void dgvProductos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Silenciar errores de formato y otros errores del DataGridView
            // Evitar que aparezcan cuadros de diálogo de error
            e.Cancel = true;
            e.ThrowException = false;
        }

        private void txtDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }

        private void txtCategoria_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(null, null);
            }
        }
    }
}
