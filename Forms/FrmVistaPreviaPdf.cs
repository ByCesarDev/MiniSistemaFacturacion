using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Services;

namespace MiniSistemaFacturacion.Forms
{
    /// <summary>
    /// Formulario para mostrar vista previa de tickets PDF
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public partial class FrmVistaPreviaPdf : Form
    {
        #region Properties

        private Factura _factura;
        private Cliente _cliente;
        private System.Collections.Generic.List<DetalleFactura> _detalles;
        private IPdfTicketService _pdfService;
        private byte[] _pdfBytes;

        #endregion

        #region Constructor

        public FrmVistaPreviaPdf(Factura factura, Cliente cliente, System.Collections.Generic.List<DetalleFactura> detalles)
        {
            InitializeComponent();
            _factura = factura;
            _cliente = cliente;
            _detalles = detalles;
            _pdfService = new PdfTicketService();
        }

        #endregion

        #region Form Events

        private void FrmVistaPreviaPdf_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = $"Vista Previa - Factura {_factura.NumeroFactura}";
                CargarVistaPrevia();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Carga la vista previa del PDF
        /// </summary>
        private void CargarVistaPrevia()
        {
            try
            {
                // Validar datos
                if (!_pdfService.ValidarDatosTicket(_factura, _cliente, _detalles, out string mensajeError))
                {
                    throw new Exception(mensajeError);
                }

                // Generar PDF para vista previa
                _pdfBytes = _pdfService.GenerarVistaPrevia(_factura, _cliente, _detalles);

                // Mostrar información en el PictureBox (ya que no podemos mostrar PDF directamente)
                pictureBoxVistaPrevia.Image = null;
                pictureBoxVistaPrevia.BackColor = Color.LightGray;
                
                // Crear un texto informativo
                using (var font = new Font("Arial", 12, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.Black))
                using (var bitmap = new Bitmap(pictureBoxVistaPrevia.Width, pictureBoxVistaPrevia.Height))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.Clear(Color.White);
                    
                    string mensaje = $"VISTA PREVIA - FACTURA #{_factura.NumeroFactura}\n\n";
                    mensaje += $"Cliente: {_cliente.Nombre}\n";
                    mensaje += $"Total: ${_factura.TotalNeto:F2}\n\n";
                    mensaje += "PDF generado correctamente.\n";
                    mensaje += "Use 'Guardar PDF' para ver el documento completo.";
                    
                    var size = graphics.MeasureString(mensaje, font);
                    var x = (bitmap.Width - size.Width) / 2;
                    var y = (bitmap.Height - size.Height) / 2;
                    
                    graphics.DrawString(mensaje, font, brush, x, y);
                    pictureBoxVistaPrevia.Image = bitmap;
                }

                // Mostrar información
                lblInfo.Text = $"Factura: {_factura.NumeroFactura} | Cliente: {_cliente.Nombre} | Total: ${_factura.TotalNeto:F2} | PDF Listo para guardar";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Button Events

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pdfBytes == null)
                {
                    MessageBox.Show("No hay PDF para guardar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener ruta donde guardar
                string ruta = _pdfService.ObtenerRutaCompleta(_factura);
                
                // Mostrar diálogo para guardar
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "Archivos PDF (*.pdf)|*.pdf";
                    saveDialog.FileName = Path.GetFileName(ruta);
                    saveDialog.Title = "Guardar Ticket PDF";

                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        _pdfService.GuardarTicketPdf(saveDialog.FileName, _pdfBytes);
                        MessageBox.Show($"Ticket guardado exitosamente en:\n{saveDialog.FileName}", 
                                      "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pdfBytes == null)
                {
                    MessageBox.Show("No hay PDF para enviar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_cliente.Email))
                {
                    MessageBox.Show("El cliente no tiene correo electrónico configurado", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Por ahora mostramos mensaje, en producción se implementaría envío real
                MessageBox.Show($"Funcionalidad de email en desarrollo.\nSe enviaría a: {_cliente.Email}", 
                              "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al preparar email: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (_pdfBytes == null)
                {
                    MessageBox.Show("No hay PDF para imprimir", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mostrar diálogo de impresión
                using (PrintDialog printDialog = new PrintDialog())
                {
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Por ahora mostramos mensaje, en producción se implementaría impresión real
                        MessageBox.Show("Funcionalidad de impresión en desarrollo.\nSe imprimiría en: " + printDialog.PrinterSettings.PrinterName, 
                                      "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al preparar impresión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Designer Generated Code

        private System.ComponentModel.IContainer components = null;
        private PictureBox pictureBoxVistaPrevia;
        private Label lblInfo;
        private Button btnGuardar;
        private Button btnEmail;
        private Button btnImprimir;
        private Button btnCancelar;

        private void InitializeComponent()
        {
            this.pictureBoxVistaPrevia = new PictureBox();
            this.lblInfo = new Label();
            this.btnGuardar = new Button();
            this.btnEmail = new Button();
            this.btnImprimir = new Button();
            this.btnCancelar = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVistaPrevia)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxVistaPrevia
            // 
            this.pictureBoxVistaPrevia.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxVistaPrevia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxVistaPrevia.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxVistaPrevia.Name = "pictureBoxVistaPrevia";
            this.pictureBoxVistaPrevia.Size = new System.Drawing.Size(760, 450);
            this.pictureBoxVistaPrevia.TabIndex = 0;
            this.pictureBoxVistaPrevia.TabStop = false;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(12, 468);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(44, 15);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Información";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Location = new System.Drawing.Point(12, 495);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 35);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar PDF";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnEmail
            // 
            this.btnEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEmail.Location = new System.Drawing.Point(118, 495);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(100, 35);
            this.btnEmail.TabIndex = 3;
            this.btnEmail.Text = "Enviar Email";
            this.btnEmail.UseVisualStyleBackColor = true;
            this.btnEmail.Click += new System.EventHandler(this.btnEmail_Click);
            // 
            // btnImprimir
            // 
            this.btnImprimir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImprimir.Location = new System.Drawing.Point(224, 495);
            this.btnImprimir.Name = "btnImprimir";
            this.btnImprimir.Size = new System.Drawing.Size(100, 35);
            this.btnImprimir.TabIndex = 4;
            this.btnImprimir.Text = "Imprimir";
            this.btnImprimir.UseVisualStyleBackColor = true;
            this.btnImprimir.Click += new System.EventHandler(this.btnImprimir_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(672, 495);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 35);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FrmVistaPreviaPdf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 542);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnImprimir);
            this.Controls.Add(this.btnEmail);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.pictureBoxVistaPrevia);
            this.MinimumSize = new System.Drawing.Size(800, 580);
            this.Name = "FrmVistaPreviaPdf";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vista Previa PDF";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVistaPrevia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
