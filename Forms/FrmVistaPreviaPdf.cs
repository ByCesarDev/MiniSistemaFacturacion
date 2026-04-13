using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Services;
using MiniSistemaFacturacion.BusinessLogic;

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
        private Button btnEditar;
        private bool _esDesdeGuardado;

        #endregion

        #region Constructor

        public FrmVistaPreviaPdf(Factura factura, Cliente cliente, System.Collections.Generic.List<DetalleFactura> detalles)
        {
            InitializeComponent();
            _factura = factura;
            _cliente = cliente;
            _detalles = detalles;
            _pdfService = new PdfTicketService();
            _esDesdeGuardado = false;
        }

        public FrmVistaPreviaPdf(Factura factura, Cliente cliente, System.Collections.Generic.List<DetalleFactura> detalles, bool esDesdeGuardado)
        {
            InitializeComponent();
            _factura = factura;
            _cliente = cliente;
            _detalles = detalles;
            _pdfService = new PdfTicketService();
            _esDesdeGuardado = esDesdeGuardado;
        }

        #endregion

        #region Form Events

        private async void FrmVistaPreviaPdf_Load(object sender, EventArgs e)
        {
            try
            {
                this.Text = $"Vista Previa - Factura {_factura.NumeroFactura}";
                
                // Inicializar WebView2
                await InitializeWebView2();
                
                // Cargar vista previa
                await CargarVistaPrevia();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task InitializeWebView2()
        {
            try
            {
                // Inicializar WebView2
                await webViewPdf.EnsureCoreWebView2Async(null);
                
                // Deshabilitar barras de estado innecesarias para tickets más limpios
                webViewPdf.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webViewPdf.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                
                // Configurar eventos
                webViewPdf.CoreWebView2.NavigationCompleted += (sender, args) => {
                    lblInfo.Text = $"Factura: {_factura.NumeroFactura} | WebView2 listo para ticket 80mm";
                    
                    // Script opcional para forzar el zoom si el ticket se ve muy pequeño
                    // Descomentar la siguiente línea si el ticket se ve demasiado pequeño en pantalla
                    // webViewPdf.CoreWebView2.ExecuteScriptAsync("document.body.style.zoom='150%'");
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al inicializar WebView2: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Carga la vista previa del PDF usando WebView2 y archivos temporales
        /// </summary>
        private async Task CargarVistaPrevia()
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

                // Crear archivo temporal para PDF
                string tempPath = Path.GetTempFileName();
                string pdfTempPath = Path.ChangeExtension(tempPath, ".pdf");
                
                try
                {
                    // Guardar PDF temporalmente
                    File.WriteAllBytes(pdfTempPath, _pdfBytes);
                    
                    // Verificar que WebView2 esté inicializado
                    if (webViewPdf.CoreWebView2 == null)
                    {
                        throw new Exception("WebView2 no está inicializado correctamente");
                    }
                    
                    // Navegar al PDF temporal usando WebView2
                    webViewPdf.CoreWebView2.Navigate(pdfTempPath);
                    
                    // Programar eliminación del archivo temporal cuando se cierre el formulario
                    this.FormClosed += (s, e) => {
                        try
                        {
                            if (File.Exists(pdfTempPath))
                                File.Delete(pdfTempPath);
                        }
                        catch { /* Ignorar errores al eliminar archivo temporal */ }
                    };
                    
                    lblInfo.Text = $"Factura: {_factura.NumeroFactura} | Cliente: {_cliente.Nombre} | Total: ${_factura.TotalNeto:F2} | PDF cargado en WebView2";
                }
                catch (Exception ex)
                {
                    // Limpiar archivo temporal si hubo error
                    if (File.Exists(pdfTempPath))
                        File.Delete(pdfTempPath);
                    
                    throw new Exception($"Error al cargar PDF en WebView2: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                // Mostrar error con opción de fallback
                DialogResult result = MessageBox.Show(
                    $"Error al cargar vista previa con WebView2: {ex.Message}\n\n¿Desea intentar abrir el PDF con el visor predeterminado?",
                    "Error en WebView2",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    await AbrirPDFExterno();
                }
                else
                {
                    MessageBox.Show($"Error al generar vista previa: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task AbrirPDFExterno()
        {
            try
            {
                // Crear archivo temporal para abrir externamente
                string tempPath = Path.GetTempFileName();
                string pdfTempPath = Path.ChangeExtension(tempPath, ".pdf");
                
                File.WriteAllBytes(pdfTempPath, _pdfBytes);
                
                // Abrir con el visor predeterminado
                Process.Start(pdfTempPath);
                
                lblInfo.Text = $"Factura: {_factura.NumeroFactura} | PDF abierto externamente (fallback)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir PDF externamente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private async void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                // Verificar si estamos en modo "desde guardado" o "desde búsqueda"
                if (_esDesdeGuardado)
                {
                    // Flujo desde guardado: devolver Retry para que el formulario original se reactive
                    this.DialogResult = DialogResult.Retry;
                    this.Close();
                }
                else
                {
                    // Flujo desde búsqueda: abrir nuevo formulario de facturación con datos cargados
                    this.Hide();
                    
                    using (FrmFacturacion frmFacturacion = new FrmFacturacion(_factura, _cliente, _detalles))
                    {
                        var result = frmFacturacion.ShowDialog();
                        
                        // Si el usuario guardó cambios, actualizar vista previa
                        if (result == DialogResult.OK)
                        {
                            // Recargar datos actualizados
                            _factura = FacturacionManager.Instance.ObtenerFactura(_factura.ID_Factura);
                            _detalles = FacturacionManager.Instance.ObtenerDetallesFactura(_factura.ID_Factura);
                            
                            // Regenerar PDF y actualizar vista
                            await CargarVistaPrevia();
                        }
                    }
                    
                    // Cerrar este formulario
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir edición: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                ImprimirFactura();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al preparar impresión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ImprimirFactura()
        {
            try
            {
                // Detectar impresoras disponibles
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    // No hay impresoras, usar PrintToPDF
                    GuardarComoPrintToPDF();
                    return;
                }

                // Verificar que WebView2 esté inicializado
                if (webViewPdf.CoreWebView2 == null)
                {
                    throw new Exception("WebView2 no está inicializado para impresión");
                }

                // Mostrar diálogo de impresión
                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.AllowCurrentPage = true;
                    printDialog.AllowSomePages = true;
                    printDialog.UseEXDialog = true;

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Configurar opciones de impresión
                        var printSettings = webViewPdf.CoreWebView2.Environment.CreatePrintSettings();
                        printSettings.ShouldPrintBackgrounds = true;
                        
                        // Usar impresión nativa de WebView2
                        await webViewPdf.CoreWebView2.PrintAsync(printSettings);

                        MessageBox.Show($"Factura #{_factura.NumeroFactura} enviada a la impresora: {printDialog.PrinterSettings.PrinterName}", 
                                      "Impresión Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ofrecer fallback si falla la impresión nativa
                DialogResult result = MessageBox.Show(
                    $"Error al imprimir con WebView2: {ex.Message}\n\n¿Desea intentar imprimir con el método tradicional?",
                    "Error en Impresión WebView2",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    await ImprimirFacturaFallback();
                }
                else
                {
                    MessageBox.Show($"Error al imprimir: {ex.Message}\n\nPor favor, intente guardar el PDF e imprimirlo manualmente.", 
                                  "Error de Impresión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task ImprimirFacturaFallback()
        {
            try
            {
                // Detectar impresoras disponibles
                if (PrinterSettings.InstalledPrinters.Count == 0)
                {
                    GuardarComoPrintToPDF();
                    return;
                }

                // Mostrar diálogo de impresión
                using (PrintDialog printDialog = new PrintDialog())
                {
                    printDialog.AllowCurrentPage = true;
                    printDialog.AllowSomePages = true;
                    printDialog.UseEXDialog = true;

                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Guardar PDF temporalmente para impresión
                        string tempPath = Path.GetTempFileName();
                        string pdfTempPath = Path.ChangeExtension(tempPath, ".pdf");
                        
                        File.WriteAllBytes(pdfTempPath, _pdfBytes);

                        try
                        {
                            // Enviar a impresora (usando proceso externo para imprimir PDF)
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                Verb = "print",
                                FileName = pdfTempPath,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden
                            };

                            using (Process process = new Process { StartInfo = startInfo })
                            {
                                process.Start();
                                await Task.Run(() => process.WaitForExit(10000)); // Esperar máximo 10 segundos
                            }

                            MessageBox.Show($"Factura #{_factura.NumeroFactura} enviada a la impresora: {printDialog.PrinterSettings.PrinterName}", 
                                          "Impresión Fallback Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            // Eliminar archivo temporal
                            if (File.Exists(pdfTempPath))
                                File.Delete(pdfTempPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir con fallback: {ex.Message}\n\nPor favor, guarde el PDF e imprímalo manualmente.", 
                              "Error de Impresión Fallback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuardarComoPrintToPDF()
        {
            try
            {
                // Guardar PDF con nombre PrintToPDF
                string ticketsDir = Path.Combine(Application.StartupPath, "TicketsPDF");
                if (!Directory.Exists(ticketsDir))
                    Directory.CreateDirectory(ticketsDir);

                string fileName = $"PrintToPDF_Factura_{_factura.NumeroFactura}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string filePath = Path.Combine(ticketsDir, fileName);

                _pdfService.GuardarTicketPdf(filePath, _pdfBytes);

                DialogResult result = MessageBox.Show(
                    $"No se detectaron impresoras instaladas.\n\nEl PDF ha sido guardado como:\n{filePath}\n\n¿Desea abrir el archivo para imprimir manualmente?",
                    "PrintToPDF - Sin Impresoras",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (result == DialogResult.Yes)
                {
                    Process.Start(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar PrintToPDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Designer Generated Code

        private WebView2 webViewPdf;
        private Label lblInfo;
        private Button btnGuardar;
        private Button btnEmail;
        private Button btnCancelar;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVistaPreviaPdf));
            this.webViewPdf = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnEmail = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webViewPdf)).BeginInit();
            this.SuspendLayout();
            // 
            // webViewPdf
            // 
            this.webViewPdf.AllowExternalDrop = true;
            this.webViewPdf.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webViewPdf.CreationProperties = null;
            this.webViewPdf.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewPdf.Location = new System.Drawing.Point(16, 15);
            this.webViewPdf.Margin = new System.Windows.Forms.Padding(4);
            this.webViewPdf.MinimumSize = new System.Drawing.Size(400, 300);
            this.webViewPdf.Name = "webViewPdf";
            this.webViewPdf.Size = new System.Drawing.Size(1013, 553);
            this.webViewPdf.TabIndex = 0;
            this.webViewPdf.ZoomFactor = 1D;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(16, 576);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(97, 18);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Información";
            // 
            // btnGuardar
            // 
            this.btnGuardar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Location = new System.Drawing.Point(16, 609);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(133, 43);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "Guardar PDF";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnEmail
            // 
            this.btnEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEmail.Location = new System.Drawing.Point(157, 609);
            this.btnEmail.Margin = new System.Windows.Forms.Padding(4);
            this.btnEmail.Name = "btnEmail";
            this.btnEmail.Size = new System.Drawing.Size(133, 43);
            this.btnEmail.TabIndex = 3;
            this.btnEmail.Text = "Enviar Email";
            this.btnEmail.UseVisualStyleBackColor = true;
            this.btnEmail.Click += new System.EventHandler(this.btnEmail_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(896, 609);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(133, 43);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditar.Location = new System.Drawing.Point(298, 609);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(133, 43);
            this.btnEditar.TabIndex = 6;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // FrmVistaPreviaPdf
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 667);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnEmail);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.webViewPdf);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1061, 703);
            this.Name = "FrmVistaPreviaPdf";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Vista Previa PDF";
            this.Load += new System.EventHandler(this.FrmVistaPreviaPdf_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webViewPdf)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
