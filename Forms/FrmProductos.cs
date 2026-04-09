using System;
using System.Drawing;
using System.Windows.Forms;
using PruebaProyectoFinalProgIII.Clases;

namespace PruebaProyectoFinalProgIII.Presentacion
{
    public partial class FrmProductos : Form
    {
        ProductoDAL productoDAL = new ProductoDAL();
        int idProductoSeleccionado = 0;
        bool estadoProductoSeleccionado = true;
        bool modoNuevo = false;
        bool modoEdicion = false;

        public FrmProductos()
        {
            InitializeComponent();
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarProductos();
            ReiniciarFormulario();
        }

        private void ConfigurarGrid()
        {
            dgvProductos.AutoGenerateColumns = true;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.MultiSelect = false;
            dgvProductos.ReadOnly = true;
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
        }

        private void FormatearGrid()
        {
            if (dgvProductos.Columns.Count > 0)
            {
                if (dgvProductos.Columns["ID_Producto"] != null)
                    dgvProductos.Columns["ID_Producto"].Visible = false;

                if (dgvProductos.Columns["FechaCreacion"] != null)
                    dgvProductos.Columns["FechaCreacion"].Visible = false;

                if (dgvProductos.Columns["Codigo"] != null)
                    dgvProductos.Columns["Codigo"].HeaderText = "Código";

                if (dgvProductos.Columns["Descripcion"] != null)
                    dgvProductos.Columns["Descripcion"].HeaderText = "Descripción";

                if (dgvProductos.Columns["PrecioUnitario"] != null)
                    dgvProductos.Columns["PrecioUnitario"].HeaderText = "Precio";

                if (dgvProductos.Columns["Stock"] != null)
                    dgvProductos.Columns["Stock"].HeaderText = "Stock";

                if (dgvProductos.Columns["StockMinimo"] != null)
                    dgvProductos.Columns["StockMinimo"].HeaderText = "Stock Mínimo";

                if (dgvProductos.Columns["Categoria"] != null)
                    dgvProductos.Columns["Categoria"].HeaderText = "Categoría";

                if (dgvProductos.Columns["Estado"] != null)
                    dgvProductos.Columns["Estado"].HeaderText = "Estado";

                if (dgvProductos.Columns["PrecioUnitario"] != null)
                    dgvProductos.Columns["PrecioUnitario"].DefaultCellStyle.Format = "N2";
            }

            dgvProductos.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvProductos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvProductos.DefaultCellStyle.SelectionBackColor = Color.RoyalBlue;
            dgvProductos.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvProductos.RowTemplate.Height = 28;
        }

        private void CargarProductos()
        {
            dgvProductos.DataSource = null;
            dgvProductos.DataSource = productoDAL.ListarProductos();
            FormatearGrid();
        }

        private void LimpiarCampos()
        {
            txtCodigo.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            txtStockMinimo.Clear();
            txtCategoria.Clear();
        }

        private void BloquearCampos()
        {
            txtCodigo.ReadOnly = true;
            txtDescripcion.ReadOnly = true;
            txtPrecio.ReadOnly = true;
            txtStock.ReadOnly = true;
            txtStockMinimo.ReadOnly = true;
            txtCategoria.ReadOnly = true;
        }

        private void DesbloquearCampos()
        {
            txtCodigo.ReadOnly = false;
            txtDescripcion.ReadOnly = false;
            txtPrecio.ReadOnly = false;
            txtStock.ReadOnly = false;
            txtStockMinimo.ReadOnly = false;
            txtCategoria.ReadOnly = false;
        }

        private void ReiniciarFormulario()
        {
            LimpiarCampos();
            BloquearCampos();

            idProductoSeleccionado = 0;
            estadoProductoSeleccionado = true;
            modoNuevo = false;
            modoEdicion = false;

            btnGuardar.Enabled = false;
            btnEditar.Enabled = false;
            btnEstado.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = false;

            btnEstado.Text = "Inactivar";

            dgvProductos.ClearSelection();
            txtCodigo.Focus();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("El código es obligatorio.");
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("La descripción es obligatoria.");
                txtDescripcion.Focus();
                return false;
            }

            if (!decimal.TryParse(txtPrecio.Text.Trim(), out decimal precio) || precio <= 0)
            {
                MessageBox.Show("Ingrese un precio válido mayor que cero.");
                txtPrecio.Focus();
                return false;
            }

            if (!int.TryParse(txtStock.Text.Trim(), out int stock) || stock < 0)
            {
                MessageBox.Show("Ingrese un stock válido.");
                txtStock.Focus();
                return false;
            }

            if (!int.TryParse(txtStockMinimo.Text.Trim(), out int stockMinimo) || stockMinimo < 0)
            {
                MessageBox.Show("Ingrese un stock mínimo válido.");
                txtStockMinimo.Focus();
                return false;
            }

            return true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
            DesbloquearCampos();

            idProductoSeleccionado = 0;
            estadoProductoSeleccionado = true;
            modoNuevo = true;
            modoEdicion = false;

            btnGuardar.Enabled = true;
            btnEditar.Enabled = false;
            btnEstado.Enabled = false;
            btnEliminar.Enabled = false;
            btnCancelar.Enabled = true;

            btnEstado.Text = "Inactivar";
            txtCodigo.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto para editar.");
                return;
            }

            modoNuevo = false;
            modoEdicion = true;

            DesbloquearCampos();

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtCodigo.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!modoNuevo && !modoEdicion)
            {
                MessageBox.Show("Primero pulse Nuevo o seleccione un producto y luego Editar.");
                return;
            }

            if (!ValidarCampos())
                return;

            try
            {
                if (modoNuevo)
                {
                    Producto producto = new Producto
                    {
                        Codigo = txtCodigo.Text.Trim(),
                        Descripcion = txtDescripcion.Text.Trim(),
                        PrecioUnitario = decimal.Parse(txtPrecio.Text.Trim()),
                        Stock = int.Parse(txtStock.Text.Trim()),
                        StockMinimo = int.Parse(txtStockMinimo.Text.Trim()),
                        Categoria = txtCategoria.Text.Trim(),
                        Estado = true,
                        FechaCreacion = DateTime.Now
                    };

                    int idGenerado = productoDAL.InsertarProducto(producto);

                    if (idGenerado <= 0)
                    {
                        MessageBox.Show("No se pudo guardar el producto.");
                        return;
                    }

                    MessageBox.Show("Producto guardado correctamente.");
                }
                else if (modoEdicion)
                {
                    Producto producto = new Producto
                    {
                        ID_Producto = idProductoSeleccionado,
                        Codigo = txtCodigo.Text.Trim(),
                        Descripcion = txtDescripcion.Text.Trim(),
                        PrecioUnitario = decimal.Parse(txtPrecio.Text.Trim()),
                        Stock = int.Parse(txtStock.Text.Trim()),
                        StockMinimo = int.Parse(txtStockMinimo.Text.Trim()),
                        Categoria = txtCategoria.Text.Trim(),
                        Estado = estadoProductoSeleccionado
                    };

                    bool actualizado = productoDAL.ActualizarProducto(producto);

                    if (!actualizado)
                    {
                        MessageBox.Show("No se pudo actualizar el producto.");
                        return;
                    }

                    MessageBox.Show("Producto actualizado correctamente.");
                }

                CargarProductos();
                ReiniciarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            bool nuevoEstado = !estadoProductoSeleccionado;
            string accion = nuevoEstado ? "activar" : "inactivar";

            DialogResult resultado = MessageBox.Show(
                "¿Seguro que desea " + accion + " este producto?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    bool cambiado = productoDAL.CambiarEstadoProducto(idProductoSeleccionado, nuevoEstado);

                    if (!cambiado)
                    {
                        MessageBox.Show("No se pudo cambiar el estado del producto.");
                        return;
                    }

                    MessageBox.Show("Estado del producto actualizado correctamente.");
                    CargarProductos();
                    ReiniciarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cambiar estado: " + ex.Message);
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (idProductoSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
                return;
            }

            try
            {
                if (productoDAL.TieneStock(idProductoSeleccionado))
                {
                    MessageBox.Show("No se puede eliminar el producto porque tiene stock disponible.");
                    return;
                }

                if (productoDAL.TieneMovimientos(idProductoSeleccionado))
                {
                    MessageBox.Show("No se puede eliminar el producto porque ya tiene movimientos en facturas.");
                    return;
                }

                DialogResult resultado = MessageBox.Show(
                    "¿Seguro que desea eliminar este producto?",
                    "Confirmación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.Yes)
                {
                    bool eliminado = productoDAL.EliminarProducto(idProductoSeleccionado);

                    if (!eliminado)
                    {
                        MessageBox.Show("No se pudo eliminar el producto.");
                        return;
                    }

                    MessageBox.Show("Producto eliminado correctamente.");
                    CargarProductos();
                    ReiniciarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscarProductos();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            BuscarProductos();
        }

        private void BuscarProductos()
        {
            string texto = txtBuscar.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                CargarProductos();
            }
            else
            {
                dgvProductos.DataSource = null;
                dgvProductos.DataSource = productoDAL.BuscarProductos(texto);
                FormatearGrid();
            }
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

            if (fila.Cells["ID_Producto"] == null || fila.Cells["ID_Producto"].Value == null)
                return;

            idProductoSeleccionado = Convert.ToInt32(fila.Cells["ID_Producto"].Value);
            txtCodigo.Text = fila.Cells["Codigo"].Value?.ToString() ?? "";
            txtDescripcion.Text = fila.Cells["Descripcion"].Value?.ToString() ?? "";
            txtPrecio.Text = fila.Cells["PrecioUnitario"].Value?.ToString() ?? "";
            txtStock.Text = fila.Cells["Stock"].Value?.ToString() ?? "";
            txtStockMinimo.Text = fila.Cells["StockMinimo"].Value?.ToString() ?? "";
            txtCategoria.Text = fila.Cells["Categoria"].Value?.ToString() ?? "";

            estadoProductoSeleccionado = Convert.ToBoolean(fila.Cells["Estado"].Value);

            if (estadoProductoSeleccionado)
                btnEstado.Text = "Inactivar";
            else
                btnEstado.Text = "Activar";

            BloquearCampos();

            modoNuevo = false;
            modoEdicion = false;

            btnGuardar.Enabled = false;
            btnEditar.Enabled = true;
            btnEstado.Enabled = true;
            btnEliminar.Enabled = true;
            btnCancelar.Enabled = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ReiniciarFormulario();
        }
    }
}