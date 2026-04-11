using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Configuration;
using System.Drawing;
using System.Drawing.Printing;

namespace MiniSistemaFacturacion.Services
{
    /// <summary>
    /// Servicio para generación de tickets PDF en formato 80mm
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public class PdfTicketService : IPdfTicketService
    {
        #region Constants

        // Constantes para formato de ticket 80mm
        private const float ANCHO_TICKET_MM = 80f; // 80mm = 3.15 pulgadas
        private const float ANCHO_TICKET_POINTS = 226.77f; // 80mm en puntos (1mm = 2.8346 puntos)
        private const float MARGEN = 10f;
        private const float FUENTE_TITULO = 12f;
        private const float FUENTE_NORMAL = 8f;
        private const float FUENTE_PEQUENA = 7f;

        #endregion

        #region Constructor

        public PdfTicketService()
        {
            // Inicializar configuración si es necesario
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
                using (var memoryStream = new MemoryStream())
                {
                    // Crear documento PDF usando System.Drawing (alternativa a QuestPDF)
                    var bitmap = CrearBitmapTicket(factura, cliente, detalles);
                    
                    // Convertir bitmap a bytes
                    using (var imageStream = new MemoryStream())
                    {
                        bitmap.Save(imageStream, System.Drawing.Imaging.ImageFormat.Png);
                        bitmap.Dispose();
                        
                        // Aquí deberíamos usar una librería PDF real, pero por ahora retornamos la imagen
                        // En producción, se usaría QuestPDF o PDFSharp
                        return ConvertirImagenAPdf(imageStream.ToArray());
                    }
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
        /// Genera una vista previa del ticket PDF
        /// </summary>
        public byte[] GenerarVistaPrevia(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            // Para vista previa, agregamos marca de agua
            byte[] pdfBytes = GenerarTicketPdf(factura, cliente, detalles);
            
            // Aquí podríamos agregar marca de agua "VISTA PREVIA"
            // Por ahora retornamos el mismo PDF
            
            return pdfBytes;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Crea un bitmap con el diseño del ticket
        /// </summary>
        private Bitmap CrearBitmapTicket(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            // Calcular altura necesaria
            float altura = CalcularAlturaTicket(factura, cliente, detalles);
            
            // Crear bitmap con dimensiones adecuadas
            var bitmap = new Bitmap((int)ANCHO_TICKET_POINTS, (int)altura);
            
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                
                // Usar fuente monoespaciada para alineación perfecta
                using (var fontTitulo = new Font("Courier New", FUENTE_TITULO, FontStyle.Bold))
                using (var fontNormal = new Font("Courier New", FUENTE_NORMAL))
                using (var fontPequena = new Font("Courier New", FUENTE_PEQUENA))
                using (var brush = new SolidBrush(Color.Black))
                {
                    float y = MARGEN;
                    var empresa = EmpresaConfig.Instance;
                    
                    // Encabezado
                    y = DibujarEncabezado(graphics, fontTitulo, fontNormal, brush, ref y);
                    
                    // Línea separadora
                    y = DibujarLinea(graphics, brush, ref y);
                    
                    // Información fiscal y factura
                    y = DibujarInfoFactura(graphics, factura, fontNormal, brush, ref y);
                    
                    // Línea separadora
                    y = DibujarLinea(graphics, brush, ref y);
                    
                    // Datos del cliente
                    y = DibujarDatosCliente(graphics, cliente, fontNormal, brush, ref y);
                    
                    // Línea separadora
                    y = DibujarLinea(graphics, brush, ref y);
                    
                    // Detalles de productos
                    y = DibujarDetalles(graphics, detalles, fontNormal, fontPequena, brush, ref y);
                    
                    // Línea separadora
                    y = DibujarLinea(graphics, brush, ref y);
                    
                    // Resumen financiero
                    y = DibujarResumen(graphics, factura, fontNormal, brush, ref y);
                    
                    // Línea separadora
                    y = DibujarLinea(graphics, brush, ref y);
                    
                    // Pie de página
                    y = DibujarPie(graphics, fontPequena, brush, ref y);
                }
            }
            
            return bitmap;
        }

        /// <summary>
        /// Calcula la altura necesaria para el ticket
        /// </summary>
        private float CalcularAlturaTicket(Factura factura, Cliente cliente, List<DetalleFactura> detalles)
        {
            float altura = MARGEN * 2; // Márgenes superior e inferior
            
            // Encabezado empresa (3 líneas)
            altura += FUENTE_TITULO + FUENTE_NORMAL * 2;
            
            // Líneas separadoras (5 líneas)
            altura += 5 * 5;
            
            // Info fiscal (3 líneas)
            altura += FUENTE_NORMAL * 3;
            
            // Datos cliente (3 líneas)
            altura += FUENTE_NORMAL * 3;
            
            // Encabezado detalles (1 línea)
            altura += FUENTE_NORMAL;
            
            // Detalles (1 línea por producto + 1 por cada 2 productos si hay descripciones largas)
            altura += FUENTE_NORMAL * detalles.Count;
            
            // Resumen (4 líneas)
            altura += FUENTE_NORMAL * 4;
            
            // Pie de página (4 líneas)
            altura += FUENTE_PEQUENA * 4;
            
            return altura + 50; // Extra para espacio
        }

        /// <summary>
        /// Dibuja el encabezado con datos de la empresa
        /// </summary>
        private float DibujarEncabezado(Graphics graphics, Font fontTitulo, Font fontNormal, SolidBrush brush, ref float y)
        {
            var empresa = EmpresaConfig.Instance;
            float centerX = ANCHO_TICKET_POINTS / 2;
            
            // Nombre de empresa (centrado)
            string nombre = empresa.Nombre;
            var size = graphics.MeasureString(nombre, fontTitulo);
            graphics.DrawString(nombre, fontTitulo, brush, centerX - size.Width / 2, y);
            y += FUENTE_TITULO;
            
            // Dirección (centrado)
            string direccion = empresa.Direccion;
            size = graphics.MeasureString(direccion, fontNormal);
            graphics.DrawString(direccion, fontNormal, brush, centerX - size.Width / 2, y);
            y += FUENTE_NORMAL;
            
            // Teléfono (centrado)
            string telefono = $"Tel: {empresa.Telefono}";
            size = graphics.MeasureString(telefono, fontNormal);
            graphics.DrawString(telefono, fontNormal, brush, centerX - size.Width / 2, y);
            y += FUENTE_NORMAL;
            
            // RNC (centrado)
            string rnc = $"RNC: {empresa.GetFormattedRNC()}";
            size = graphics.MeasureString(rnc, fontNormal);
            graphics.DrawString(rnc, fontNormal, brush, centerX - size.Width / 2, y);
            y += FUENTE_NORMAL;
            
            return y;
        }

        /// <summary>
        /// Dibuja una línea separadora
        /// </summary>
        private float DibujarLinea(Graphics graphics, SolidBrush brush, ref float y)
        {
            graphics.DrawLine(new Pen(brush, 1), MARGEN, y, ANCHO_TICKET_POINTS - MARGEN, y);
            y += 5;
            return y;
        }

        /// <summary>
        /// Dibuja información de la factura y datos fiscales
        /// </summary>
        private float DibujarInfoFactura(Graphics graphics, Factura factura, Font fontNormal, SolidBrush brush, ref float y)
        {
            // NCF
            string ncf = $"NCF: {factura.NCF ?? "N/A"}";
            graphics.DrawString(ncf, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Número de factura
            string numFactura = $"FACTURA: {factura.NumeroFactura}";
            graphics.DrawString(numFactura, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Fecha
            string fecha = $"FECHA: {factura.Fecha:dd/MM/yyyy HH:mm}";
            graphics.DrawString(fecha, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            return y;
        }

        /// <summary>
        /// Dibuja los datos del cliente
        /// </summary>
        private float DibujarDatosCliente(Graphics graphics, Cliente cliente, Font fontNormal, SolidBrush brush, ref float y)
        {
            // Título
            graphics.DrawString("DATOS DEL CLIENTE:", fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Nombre
            string nombre = $"Nombre: {cliente.Nombre}";
            graphics.DrawString(nombre, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Cédula
            string cedula = $"Cédula: {cliente.Cedula}";
            graphics.DrawString(cedula, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Tipo (si aplica)
            string tipo = $"Tipo: Consumidor Final";
            graphics.DrawString(tipo, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            return y;
        }

        /// <summary>
        /// Dibuja los detalles de los productos
        /// </summary>
        private float DibujarDetalles(Graphics graphics, List<DetalleFactura> detalles, Font fontNormal, Font fontPequena, SolidBrush brush, ref float y)
        {
            // Título
            graphics.DrawString("DETALLE DE COMPRA:", fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Encabezado tabla
            string encabezado = "CANT  DESCRIPCIÓN        PRECIO   TOTAL";
            graphics.DrawString(encabezado, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Línea separadora
            graphics.DrawLine(new Pen(brush, 1), MARGEN, y, ANCHO_TICKET_POINTS - MARGEN, y);
            y += 3;
            
            // Detalles
            foreach (var detalle in detalles)
            {
                // Formatear línea
                string linea = FormatearLineaDetalle(detalle);
                graphics.DrawString(linea, fontPequena, brush, MARGEN, y);
                y += FUENTE_PEQUENA;
            }
            
            return y;
        }

        /// <summary>
        /// Formatea una línea de detalle para el ticket
        /// </summary>
        private string FormatearLineaDetalle(DetalleFactura detalle)
        {
            string cantidad = detalle.Cantidad.ToString().PadLeft(3);
            string descripcion = (detalle.Descripcion ?? "").PadRight(18).Substring(0, 18);
            string precio = $"${detalle.PrecioUnitarioVenta:F2}".PadLeft(7);
            string total = $"${detalle.Subtotal:F2}".PadLeft(7);
            
            return $"{cantidad} {descripcion} {precio} {total}";
        }

        /// <summary>
        /// Dibuja el resumen financiero
        /// </summary>
        private float DibujarResumen(Graphics graphics, Factura factura, Font fontNormal, SolidBrush brush, ref float y)
        {
            // Subtotal
            string subtotal = $"Subtotal:           {factura.TotalBruto:F2}";
            graphics.DrawString(subtotal, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // ITBIS
            string itbis = $"ITBIS ({factura.PorcentajeImpuesto}%):        {factura.ValorImpuesto:F2}";
            graphics.DrawString(itbis, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Total
            string total = $"TOTAL:             {factura.TotalNeto:F2}";
            graphics.DrawString(total, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            // Forma de pago (por defecto efectivo)
            string formaPago = "FORMA DE PAGO: Efectivo";
            graphics.DrawString(formaPago, fontNormal, brush, MARGEN, y);
            y += FUENTE_NORMAL;
            
            return y;
        }

        /// <summary>
        /// Dibuja el pie de página con términos y condiciones
        /// </summary>
        private float DibujarPie(Graphics graphics, Font fontPequena, SolidBrush brush, ref float y)
        {
            // Línea decorativa
            string linea1 = "================================";
            graphics.DrawString(linea1, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            // Mensaje de agradecimiento (centrado)
            float centerX = ANCHO_TICKET_POINTS / 2;
            string gracias = "     GRACIAS POR SU COMPRA";
            var size = graphics.MeasureString(gracias, fontPequena);
            graphics.DrawString(gracias, fontPequena, brush, centerX - size.Width / 2, y);
            y += FUENTE_PEQUENA;
            
            // Vuelva pronto (centrado)
            string vuelva = "       VUELVA PRONTO";
            size = graphics.MeasureString(vuelva, fontPequena);
            graphics.DrawString(vuelva, fontPequena, brush, centerX - size.Width / 2, y);
            y += FUENTE_PEQUENA;
            
            // Línea decorativa
            string linea2 = "================================";
            graphics.DrawString(linea2, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            // Términos
            string terminos = "TÉRMINOS Y CONDICIONES:";
            graphics.DrawString(terminos, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            string termino1 = "* Los productos tienen 30 días de garantía";
            graphics.DrawString(termino1, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            string termino2 = "* No se aceptan devoluciones sin factura";
            graphics.DrawString(termino2, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            string termino3 = "* Para reclamaciones conserve esta factura";
            graphics.DrawString(termino3, fontPequena, brush, MARGEN, y);
            y += FUENTE_PEQUENA;
            
            return y;
        }

        /// <summary>
        /// Convierte una imagen a PDF (método temporal)
        /// </summary>
        private byte[] ConvertirImagenAPdf(byte[] imageBytes)
        {
            // Este es un método temporal que retorna la imagen como si fuera PDF
            // En producción, se usaría una librería PDF real como QuestPDF o PDFSharp
            
            // Por ahora, retornamos los bytes de la imagen
            // El usuario puede verla como vista previa y luego implementaremos la conversión real
            
            return imageBytes;
        }

        #endregion
    }
}
