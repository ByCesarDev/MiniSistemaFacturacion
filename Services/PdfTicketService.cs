using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Configuration;
using QuestPDF.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace MiniSistemaFacturacion.Services
{
    /// <summary>
    /// Servicio para generación de tickets PDF en formato 80mm
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public class PdfTicketService : IPdfTicketService
    {
        #region Constructor

        public PdfTicketService()
        {
            // Inicializar QuestPDF si es necesario
        }

        #endregion

        #region IPdfTicketService Implementation

        /// <summary>
        /// Genera un ticket PDF en formato 80mm para una factura
        /// </summary>
        public byte[] GenerarTicketPdf(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            try
            {
                // Inicializar QuestPDF (solo una vez en la aplicación)
                QuestPDF.Settings.License = LicenseType.Community;
                
                using (var memoryStream = new MemoryStream())
                {
                    // Crear documento PDF real con QuestPDF
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(new PageSize(226.77f, 400)); // 80mm x altura variable
                            page.Margin(10);
                            page.DefaultTextStyle(x => x.FontSize(8).FontFamily(Fonts.Courier));

                            // Encabezado de la empresa
                            page.Header().Element(header =>
                            {
                                header.Column(column =>
                                {
                                    column.Item().AlignCenter().Text(EmpresaConfig.Instance.Nombre.ToUpper()).Bold().FontSize(10);
                                    column.Item().AlignCenter().Text(EmpresaConfig.Instance.Direccion).FontSize(7);
                                    column.Item().AlignCenter().Text($"Tel: {EmpresaConfig.Instance.Telefono}").FontSize(7);
                                    column.Item().AlignCenter().Text($"RNC: {FormatearRNC(EmpresaConfig.Instance.RNC)}").FontSize(7);
                                    column.Item().LineHorizontal(0.5f);
                                });
                            });

                            // Contenido principal
                            page.Content().Element(content =>
                            {
                                content.Column(column =>
                                {
                                    // Información fiscal
                                    column.Item().Text($"NCF: {factura.NCF ?? "Pendiente"}").Bold();
                                    column.Item().Text($"Factura: #{factura.NumeroFactura}");
                                    column.Item().Text($"Fecha: {factura.Fecha:dd/MM/yyyy HH:mm}");
                                    column.Item().LineHorizontal(0.5f);

                                    // Datos del cliente
                                    column.Item().Text("DATOS DEL CLIENTE:").Bold();
                                    column.Item().Text($"Nombre: {cliente.Nombre ?? "N/A"}");
                                    column.Item().Text($"Cédula: {cliente.Cedula ?? "N/A"}");
                                    column.Item().Text("Tipo: Consumidor Final");
                                    column.Item().LineHorizontal(0.5f);

                                    // Detalles de productos
                                    column.Item().Text("DETALLE DE PRODUCTOS:").Bold();
                                    
                                    foreach (var detalle in detalles)
                                    {
                                        column.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text($"{detalle.Cantidad}");
                                            row.RelativeItem(3).Text(detalle.Descripcion ?? "");
                                            row.RelativeItem().AlignRight().Text($"{detalle.PrecioUnitarioVenta:C2}");
                                            row.RelativeItem().AlignRight().Text($"{detalle.Subtotal:C2}");
                                        });
                                    }
                                    
                                    column.Item().LineHorizontal(0.5f);

                                    // Resumen financiero
                                    column.Item().AlignRight().Column(resumen =>
                                    {
                                        resumen.Item().Text($"Subtotal: {factura.TotalBruto:C2}");
                                        resumen.Item().Text($"ITBIS (18%): {factura.ValorImpuesto:C2}");
                                        resumen.Item().Text($"TOTAL: {factura.TotalNeto:C2}").Bold();
                                        resumen.Item().Text("Forma de Pago: Contado");
                                    });
                                    
                                    column.Item().LineHorizontal(0.5f);
                                });
                            });

                            // Pie de página
                            page.Footer().Element(footer =>
                            {
                                footer.Column(column =>
                                {
                                    column.Item().AlignCenter().Text("¡Gracias por su compra!").FontSize(7);
                                    column.Item().AlignCenter().Text("Este documento no es válido como factura fiscal").FontSize(6);
                                    column.Item().AlignCenter().Text("sin el sello y firma correspondientes.").FontSize(6);
                                });
                            });
                        });
                    });

                    // Generar PDF
                    document.GeneratePdf(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar ticket PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Guarda un ticket PDF en la ruta especificada
        /// </summary>
        public void GuardarTicketPdf(string ruta, byte[] pdfBytes)
        {
            try
            {
                // Asegurar que el directorio exista
                string directorio = Path.GetDirectoryName(ruta);
                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                // Guardar archivo
                File.WriteAllBytes(ruta, pdfBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar ticket PDF: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera el nombre del archivo para el ticket PDF
        /// </summary>
        public string GenerarNombreArchivo(Factura factura)
        {
            return $"Ticket_{factura.NumeroFactura}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        }

        /// <summary>
        /// Obtiene la ruta completa donde se guardará el ticket PDF
        /// </summary>
        public string ObtenerRutaCompleta(Factura factura)
        {
            string rutaBase = EmpresaConfig.Instance.RutaPdfTickets;
            string nombreArchivo = GenerarNombreArchivo(factura);
            
            // Asegurar que la ruta base termine con \
            if (!rutaBase.EndsWith("\\"))
                rutaBase += "\\";
                
            return Path.Combine(rutaBase, nombreArchivo);
        }

        /// <summary>
        /// Valida que los datos sean suficientes para generar el ticket
        /// </summary>
        public bool ValidarDatosTicket(Factura factura, Cliente cliente, List<DetalleFactura> detalles, out string mensajeError)
        {
            mensajeError = string.Empty;

            // Validar factura
            if (factura == null)
            {
                mensajeError = "La factura no puede ser nula";
                return false;
            }

            if (string.IsNullOrWhiteSpace(factura.NumeroFactura))
            {
                mensajeError = "El número de factura es requerido";
                return false;
            }

            // Validar cliente
            if (cliente == null)
            {
                mensajeError = "El cliente no puede ser nulo";
                return false;
            }

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
            {
                mensajeError = "El nombre del cliente es requerido";
                return false;
            }

            // Validar detalles
            if (detalles == null || detalles.Count == 0)
            {
                mensajeError = "La factura debe tener al menos un detalle";
                return false;
            }

            foreach (var detalle in detalles)
            {
                if (detalle == null)
                {
                    mensajeError = "Hay detalles nulos en la factura";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(detalle.Descripcion))
                {
                    mensajeError = "Todos los detalles deben tener descripción";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Genera vista previa del ticket
        /// </summary>
        public byte[] GenerarVistaPrevia(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            // Para vista previa, generamos el mismo PDF
            return GenerarTicketPdf(factura, cliente, detalles);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Formatea el RNC con guiones
        /// </summary>
        private string FormatearRNC(string rnc)
        {
            if (string.IsNullOrWhiteSpace(rnc))
                return "N/A";

            if (rnc.Length == 9)
                return $"{rnc.Substring(0, 3)}-{rnc.Substring(3, 3)}-{rnc.Substring(6, 3)}";
            else if (rnc.Length == 11)
                return $"{rnc.Substring(0, 3)}-{rnc.Substring(3, 7)}-{rnc.Substring(10, 1)}";
            else
                return rnc;
        }

        #endregion
    }
}
