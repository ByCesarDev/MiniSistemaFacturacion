using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSistemaFacturacion.BusinessLogic;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    public partial class frmCuentasPorCobrar : Form
    {
        public frmCuentasPorCobrar()
        {
            InitializeComponent();
        }

        private DateTime _ultimoDobleClick = DateTime.MinValue;

        private void frmCuentasPorCobrar_Load(object sender, EventArgs e)
        {
            UITheme.AplicarTemaFormulario(this);
            UITheme.EstilizarDataGridView(dgvPendientes);
            UITheme.EstilizarBotonExito(btnRegistrarPago);
            UITheme.EstilizarBotonCancelar(btnCerrar);
            UITheme.EstilizarTextBox(txtBusqueda);

            dgvPendientes.RowHeadersVisible = false;
            dgvPendientes.MultiSelect = false;
            dgvPendientes.Cursor = Cursors.Hand;
            dgvPendientes.CellDoubleClick += dgvPendientes_CellDoubleClick;
            dgvPendientes.DoubleClick += dgvPendientes_DoubleClick;
            dgvPendientes.KeyDown += dgvPendientes_KeyDown;

            CargarFacturas();
        }

        private void dgvPendientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                EjecutarDobleClickSeguro();
            }
        }

        private void dgvPendientes_DoubleClick(object sender, EventArgs e)
        {
            EjecutarDobleClickSeguro();
        }

        private void dgvPendientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                EjecutarDobleClickSeguro();
            }
        }

        private void EjecutarDobleClickSeguro()
        {
            if ((DateTime.Now - _ultimoDobleClick).TotalMilliseconds < 500)
                return;

            _ultimoDobleClick = DateTime.Now;
            AbrirRegistroPago();
        }

        private void CargarFacturas(string filtro = "")
        {
            try
            {
                string sql = @"SELECT f.ID_Factura, f.NumeroFactura, c.Nombre as Cliente, 
                               f.Fecha, f.TotalNeto, f.SaldoPendiente, f.Estado 
                               FROM Facturas f 
                               INNER JOIN Clientes c ON f.ID_Cliente = c.ID_Cliente
                               WHERE f.SaldoPendiente > 0 AND f.Estado <> 'Anulada'";

                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    sql += $" AND (c.Nombre LIKE '%{filtro}%' OR f.NumeroFactura LIKE '%{filtro}%')";
                }

                dgvPendientes.DataSource = DbHelper.Instance.ExecuteQuery(sql);

                // Configurar apariencia de columnas
                if (dgvPendientes.Columns.Contains("ID_Factura"))
                    dgvPendientes.Columns["ID_Factura"].Visible = false;

                if (dgvPendientes.Columns.Contains("NumeroFactura"))
                {
                    dgvPendientes.Columns["NumeroFactura"].HeaderText = "N° Factura";
                    dgvPendientes.Columns["NumeroFactura"].Width = 120;
                }

                if (dgvPendientes.Columns.Contains("Cliente"))
                {
                    dgvPendientes.Columns["Cliente"].HeaderText = "Cliente";
                    dgvPendientes.Columns["Cliente"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (dgvPendientes.Columns.Contains("Fecha"))
                {
                    dgvPendientes.Columns["Fecha"].HeaderText = "Fecha";
                    dgvPendientes.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvPendientes.Columns["Fecha"].Width = 100;
                }

                if (dgvPendientes.Columns.Contains("TotalNeto"))
                {
                    dgvPendientes.Columns["TotalNeto"].HeaderText = "Total Neto";
                    dgvPendientes.Columns["TotalNeto"].DefaultCellStyle.Format = "N2";
                    dgvPendientes.Columns["TotalNeto"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPendientes.Columns["TotalNeto"].Width = 120;
                }

                if (dgvPendientes.Columns.Contains("SaldoPendiente"))
                {
                    dgvPendientes.Columns["SaldoPendiente"].HeaderText = "Saldo Pendiente";
                    dgvPendientes.Columns["SaldoPendiente"].DefaultCellStyle.Format = "N2";
                    dgvPendientes.Columns["SaldoPendiente"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvPendientes.Columns["SaldoPendiente"].DefaultCellStyle.Font = UITheme.FontBodyBold;
                    dgvPendientes.Columns["SaldoPendiente"].DefaultCellStyle.ForeColor = Color.FromArgb(180, 50, 0);
                    dgvPendientes.Columns["SaldoPendiente"].Width = 130;
                }

                if (dgvPendientes.Columns.Contains("Estado"))
                {
                    dgvPendientes.Columns["Estado"].HeaderText = "Estado";
                    dgvPendientes.Columns["Estado"].Width = 90;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en búsqueda: " + ex.Message);
            }
        }

        private void btnRegistrarPago_Click_1(object sender, EventArgs e)
        {
            AbrirRegistroPago();
        }

        private void AbrirRegistroPago()
        {
            DataGridViewRow fila = null;
            if (dgvPendientes.SelectedRows.Count > 0)
            {
                fila = dgvPendientes.SelectedRows[0];
            }
            else if (dgvPendientes.CurrentRow != null)
            {
                fila = dgvPendientes.CurrentRow;
            }

            if (fila != null && fila.Cells["ID_Factura"].Value != null && fila.Cells["ID_Factura"].Value != DBNull.Value)
            {
                int idFactura = Convert.ToInt32(fila.Cells["ID_Factura"].Value);
                string numFactura = fila.Cells["NumeroFactura"].Value?.ToString() ?? "";
                decimal saldoActual = Convert.ToDecimal(fila.Cells["SaldoPendiente"].Value);

                // Abrimos el formulario modal pasando los datos
                using (var frmPago = new frmRegistrarPago(idFactura, numFactura, saldoActual))
                {
                    if (frmPago.ShowDialog(this) == DialogResult.OK)
                    {
                        txtBusqueda.Clear();
                        CargarFacturas();
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una factura de la lista para registrar el abono o pago.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            CargarFacturas(txtBusqueda.Text.Trim());
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}