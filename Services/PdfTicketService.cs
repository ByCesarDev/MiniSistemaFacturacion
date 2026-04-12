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
                QuestPDF.Settings.EnableDebugging = true;
                
                using (var memoryStream = new MemoryStream())
                {
                    // Crear documento PDF real con QuestPDF
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            // 80mm son aproximadamente 227 puntos. 
                            // Calcular altura dinámicamente basada en el contenido
                            float alturaEstimada = CalcularAlturaTicket(factura, cliente, detalles);
                            page.Size(227, alturaEstimada); // Ancho fijo 80mm, altura dinámica
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
                                            row.ConstantItem(15).Text($"{detalle.Cantidad}");           // Ancho fijo para cantidad
                                            row.RelativeItem().Text(detalle.Descripcion ?? "");        // Espacio restante para descripción
                                            row.ConstantItem(35).AlignRight().Text($"{detalle.PrecioUnitarioVenta:C2}"); // Ancho fijo para precio
                                            row.ConstantItem(35).AlignRight().Text($"{detalle.Subtotal:C2}");            // Ancho fijo para subtotal
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
                                    
                                    // Espacio para el mensaje de agradecimiento al final
                                    column.Item().Text("").FontSize(20);
                                });
                            });

                            // Mensaje de agradecimiento al fondo del ticket
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
        /// Calcula la altura dinámica del ticket basada en su contenido
        /// </summary>
        private float CalcularAlturaTicket(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            try
            {
                // Altura base para elementos fijos (encabezado, cliente, resumen, footer)
                float alturaBase = 200; // pts para elementos estáticos
                
                // Altura para cada línea de producto
                float alturaPorProducto = 15; // pts por línea de detalle
                
                // Altura para líneas de información fiscal y cliente
                float alturaInfoFiscal = 60; // pts para NCF, factura, fecha, cliente
                
                // Altura para resumen financiero
                float alturaResumen = 40; // pts para subtotal, ITBIS, total
                
                // Altura para mensaje de agradecimiento
                float alturaAgradecimiento = 30; // pts para mensaje final
                
                // Calcular altura total
                float alturaTotal = alturaBase + alturaInfoFiscal + alturaResumen + alturaAgradecimiento;
                
                // Agregar altura por cada producto
                if (detalles != null && detalles.Count > 0)
                {
                    alturaTotal += (detalles.Count * alturaPorProducto);
                    
                    // Agregar espacio extra si hay muchos productos
                    if (detalles.Count > 10)
                    {
                        alturaTotal += (detalles.Count - 10) * 2; // 2 pts extra por cada producto adicional
                    }
                }
                
                // Altura mínima para tickets simples
                float alturaMinima = 300;
                float alturaMaxima = 2000; // Límite razonable para evitar problemas
                
                alturaTotal = Math.Max(alturaTotal, alturaMinima);
                alturaTotal = Math.Min(alturaTotal, alturaMaxima);
                
                return alturaTotal;
            }
            catch (Exception)
            {
                // En caso de error, retornar una altura predeterminada segura
                return 500;
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
