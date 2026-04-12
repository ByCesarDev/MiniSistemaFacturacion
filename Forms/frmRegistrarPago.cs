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
    public partial class frmRegistrarPago : Form
    {
        private int _idFactura;
        private decimal _saldoPendiente;

        public frmRegistrarPago(int idFactura, string numFactura, decimal saldo)
        {
            InitializeComponent();
            _idFactura = idFactura;
            _saldoPendiente = saldo;

            
            lblInfoFactura.Text = $"Factura: {numFactura} | Saldo Actual: ${saldo:N2}";

            
            cmbFormaPago.Items.AddRange(new string[] { "Efectivo", "Tarjeta Credito", "Tarjeta Debito", "Cheque", "Transferencia" });
            cmbFormaPago.SelectedIndex = 0;
            txtReferencia.Enabled = false;
        }


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
    
                if (!decimal.TryParse(txtMonto.Text, out decimal monto))
                    throw new Exception("Por favor, ingrese un monto válido.");

                if (monto <= 0)
                    throw new Exception("El monto debe ser mayor a cero.");

                if (monto > _saldoPendiente)
                    throw new Exception("El pago no puede ser mayor al saldo pendiente.");

      
                if (cmbFormaPago.Text != "Efectivo" && string.IsNullOrWhiteSpace(txtReferencia.Text))
                    throw new Exception("Debe ingresar el número de referencia para pagos con " + cmbFormaPago.Text);

               
                Pago nuevoPago = new Pago
                {
                    ID_Factura = _idFactura,
                    FechaPago = dtpFecha.Value,
                    MontoPagado = monto,
                    FormaPago = cmbFormaPago.Text,
                    Referencia = txtReferencia.Text,
                    Estado = true
                };

 
                PagoManager.Instance.RegistrarPago(nuevoPago);

                MessageBox.Show("¡Pago procesado con éxito!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK; 
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al procesar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbFormaPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool necesitaReferencia = cmbFormaPago.Text != "Efectivo";
            txtReferencia.Enabled = necesitaReferencia;

            if (!necesitaReferencia)
            {
                txtReferencia.Clear();
            }
        }

       
    }
}