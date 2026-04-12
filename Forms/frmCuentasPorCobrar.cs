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

namespace MiniSistemaFacturacion.Forms
{
    public partial class frmCuentasPorCobrar : Form
    {
        public frmCuentasPorCobrar()
        {
            InitializeComponent();
        }

        private void frmCuentasPorCobrar_Load(object sender, EventArgs e)
        {
            CargarFacturas();
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
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Error en búsqueda: " + ex.Message);
            }
        }


        private void btnRegistrarPago_Click_1(object sender, EventArgs e)
        {
            if (dgvPendientes.SelectedRows.Count > 0)
            {
                int idFactura = Convert.ToInt32(dgvPendientes.SelectedRows[0].Cells["ID_Factura"].Value);
                string numFactura = dgvPendientes.SelectedRows[0].Cells["NumeroFactura"].Value.ToString();
                decimal saldoActual = Convert.ToDecimal(dgvPendientes.SelectedRows[0].Cells["SaldoPendiente"].Value);

                // Abrimos el formulario modal pasando los datos
                using (var frmPago = new frmRegistrarPago(idFactura, numFactura, saldoActual))
                {
                    if (frmPago.ShowDialog() == DialogResult.OK)
                    {
                        txtBusqueda.Clear();
                        CargarFacturas();
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione una factura de la lista.");
            }


        }

        private void txtBusqueda_TextChanged(object sender, EventArgs e)
        {
            CargarFacturas(txtBusqueda.Text.Trim());
        }
    }
}