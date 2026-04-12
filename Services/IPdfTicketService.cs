using System;
using System.Collections.Generic;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.Services
{
    /// <summary>
    /// Interfaz para el servicio de generación de tickets PDF
    /// Created by: Cesar Reyes
    /// Date: 2026-04-11
    /// </summary>
    public interface IPdfTicketService
    {
        /// <summary>
        /// Genera un ticket PDF en formato 80mm para una factura
        /// </summary>
        /// <param name="factura">Factura a generar</param>
        /// <param name="cliente">Cliente de la factura</param>
        /// <param name="detalles">Detalles de la factura</param>
        /// <returns>Array de bytes del PDF generado</returns>
        byte[] GenerarTicketPdf(Factura factura, Cliente cliente, List<DetalleFactura> detalles);

        /// <summary>
        /// Guarda un ticket PDF en la ruta especificada
        /// </summary>
        /// <param name="ruta">Ruta completa donde guardar el archivo</param>
        /// <param name="pdfBytes">Array de bytes del PDF</param>
        void GuardarTicketPdf(string ruta, byte[] pdfBytes);

        /// <summary>
        /// Genera el nombre del archivo para el ticket PDF
        /// </summary>
        /// <param name="factura">Factura para generar nombre</param>
        /// <returns>Nombre del archivo sugerido</returns>
        string GenerarNombreArchivo(Factura factura);

        /// <summary>
        /// Obtiene la ruta completa donde se guardará el ticket PDF
        /// </summary>
        /// <param name="factura">Factura para generar ruta</param>
        /// <returns>Ruta completa del archivo</returns>
        string ObtenerRutaCompleta(Factura factura);

        /// <summary>
        /// Valida que los datos sean suficientes para generar el ticket
        /// </summary>
        /// <param name="factura">Factura a validar</param>
        /// <param name="cliente">Cliente a validar</param>
        /// <param name="detalles">Detalles a validar</param>
        /// <param name="mensajeError">Mensaje de error si hay problemas</param>
        /// <returns>True si los datos son válidos</returns>
        bool ValidarDatosTicket(Factura factura, Cliente cliente, List<DetalleFactura> detalles, out string mensajeError);

        /// <summary>
        /// Genera una vista previa del ticket PDF
        /// </summary>
        /// <param name="factura">Factura para vista previa</param>
        /// <param name="cliente">Cliente para vista previa</param>
        /// <param name="detalles">Detalles para vista previa</param>
        /// <returns>Array de bytes del PDF de vista previa</returns>
        byte[] GenerarVistaPrevia(Factura factura, Cliente cliente, List<DetalleFactura> detalles);
    }
}
