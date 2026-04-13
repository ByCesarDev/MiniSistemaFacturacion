using System;
using System.ComponentModel.DataAnnotations;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Entidad que representa una factura en el sistema
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class Factura
    {
        #region Properties

        /// <summary>
        /// Identificador único de la factura
        /// </summary>
        public int ID_Factura { get; set; }

        /// <summary>
        /// Número único de factura
        /// </summary>
        [Required(ErrorMessage = "El número de factura es requerido")]
        [StringLength(20, ErrorMessage = "El número de factura no puede exceder 20 caracteres")]
        public string NumeroFactura { get; set; }

        /// <summary>
        /// Fecha de emisión de la factura
        /// </summary>
        [Required(ErrorMessage = "La fecha de la factura es requerida")]
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Identificador del cliente
        /// </summary>
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ID_Cliente { get; set; }

        /// <summary>
        /// Total bruto de la factura (sin impuestos)
        /// </summary>
        [Required(ErrorMessage = "El total bruto es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El total bruto no puede ser negativo")]
        public decimal TotalBruto { get; set; }

        /// <summary>
        /// Porcentaje de impuesto aplicado
        /// </summary>
        [Required(ErrorMessage = "El porcentaje de impuesto es requerido")]
        [Range(0, 100, ErrorMessage = "El porcentaje de impuesto debe estar entre 0 y 100")]
        public decimal PorcentajeImpuesto { get; set; }

        /// <summary>
        /// Valor del impuesto calculado
        /// </summary>
        [Required(ErrorMessage = "El valor del impuesto es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El valor del impuesto no puede ser negativo")]
        public decimal ValorImpuesto { get; set; }

        /// <summary>
        /// Total neto de la factura (con impuestos)
        /// </summary>
        [Required(ErrorMessage = "El total neto es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El total neto no puede ser negativo")]
        public decimal TotalNeto { get; set; }

        /// <summary>
        /// Saldo pendiente de pago
        /// </summary>
        [Required(ErrorMessage = "El saldo pendiente es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El saldo pendiente no puede ser negativo")]
        public decimal SaldoPendiente { get; set; }

        /// <summary>
        /// Número de Comprobante Fiscal (NCF)
        /// </summary>
        [StringLength(19, ErrorMessage = "El NCF no puede exceder 19 caracteres")]
        public string NCF { get; set; }

        /// <summary>
        /// Estado de la factura
        /// </summary>
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]

        public string Estado { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Nombre del cliente (propiedad calculada para DataGridView)
        /// </summary>
        public string ClienteNombre { get; set; }

        /// <summary>
        /// Cédula del cliente (propiedad calculada para DataGridView)
        /// </summary>
        public string Cedula { get; set; }

        /// <summary>
        /// Dirección del cliente (propiedad calculada para DataGridView)
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// Teléfono del cliente (propiedad calculada para DataGridView)
        /// </summary>
        public string Telefono { get; set; }

        /// <summary>
        /// Email del cliente (propiedad calculada para DataGridView)
        /// </summary>
        public string Email { get; set; }

        public string TipoVenta { get; set; }

        /// <summary>
        /// Forma de pago (EFECTIVO, TARJETA, TRANSFERENCIA, CHEQUE)
        /// </summary>
        [StringLength(20, ErrorMessage = "La forma de pago no puede exceder 20 caracteres")]
        public string FormaPago { get; set; }

        /// <summary>
        /// Indica si la factura es a crédito
        /// </summary>
        public bool Credito { get; set; }

        /// <summary>
        /// Tipo de comprobante fiscal (01, 02, 03, 14, 15, 16)
        /// </summary>
        [StringLength(2, ErrorMessage = "El tipo de comprobante no puede exceder 2 caracteres")]
        public string TipoComprobante { get; set; }

        public string TipoComprobanteDescripcion
        {
            get
            {
                switch (TipoComprobante)
                {
                    case "01":
                        return "Crédito Fiscal";
                    case "02":
                        return "Consumidor Final";

                    default:
                        return TipoComprobante;
                }
            }
        }

        /// <summary>
        /// Propiedad de navegación para el cliente
        /// </summary>
        public Cliente Cliente { get; set; }

        /// <summary>
        /// Propiedad de navegación para los detalles de la factura
        /// </summary>
        public System.Collections.Generic.List<DetalleFactura> Detalles { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Factura()
        {
            Fecha = DateTime.Now;
            FechaCreacion = DateTime.Now;
            PorcentajeImpuesto = 15.00m;
            Estado = "Pendiente";
            TipoComprobante = "02"; // Por defecto Consumidor Final
            NCF = null; // Se generará al guardar
            Detalles = new System.Collections.Generic.List<DetalleFactura>();
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="numeroFactura">Número de factura</param>
        /// <param name="idCliente">ID del cliente</param>
        public Factura(string numeroFactura, int idCliente)
        {
            NumeroFactura = numeroFactura;
            ID_Cliente = idCliente;
            Fecha = DateTime.Now;
            FechaCreacion = DateTime.Now;
            PorcentajeImpuesto = 15.00m;
            Estado = "Pendiente";
            TipoComprobante = "02"; // Por defecto Consumidor Final
            NCF = null; // Se generará al guardar
            Detalles = new System.Collections.Generic.List<DetalleFactura>();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que los datos de la factura sean correctos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(NumeroFactura))
                return false;

            if (ID_Cliente <= 0)
                return false;

            if (TotalBruto < 0)
                return false;

            if (PorcentajeImpuesto < 0 || PorcentajeImpuesto > 100)
                return false;

            if (ValorImpuesto < 0)
                return false;

            if (TotalNeto < 0)
                return false;

            if (SaldoPendiente < 0)
                return false;

            if (string.IsNullOrWhiteSpace(Estado))
                return false;

            if (!IsValidEstado(Estado))
                return false;

            return true;
        }

        /// <summary>
        /// Valida si un estado es válido
        /// </summary>
        /// <param name="estado">Estado a validar</param>
        /// <returns>True si el estado es válido</returns>
        private bool IsValidEstado(string estado)
        {
            return estado == "Pendiente" || estado == "Parcial" || estado == "Pagada" || estado == "Anulada";
        }

        /// <summary>
        /// Obtiene el mensaje de error de validación
        /// </summary>
        /// <returns>Mensaje de error o string vacío si no hay errores</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(NumeroFactura))
                return "El número de factura es requerido";

            if (ID_Cliente <= 0)
                return "El cliente es requerido";

            if (TotalBruto < 0)
                return "El total bruto no puede ser negativo";

            if (PorcentajeImpuesto < 0 || PorcentajeImpuesto > 100)
                return "El porcentaje de impuesto debe estar entre 0 y 100";

            if (ValorImpuesto < 0)
                return "El valor del impuesto no puede ser negativo";

            if (TotalNeto < 0)
                return "El total neto no puede ser negativo";

            if (SaldoPendiente < 0)
                return "El saldo pendiente no puede ser negativo";

            if (string.IsNullOrWhiteSpace(Estado))
                return "El estado de la factura es requerido";

            if (!IsValidEstado(Estado))
                return "El estado de la factura no es válido";

            return string.Empty;
        }

        #endregion

        #region Display Methods

        /// <summary>
        /// Obtiene una representación en texto de la factura
        /// </summary>
        /// <returns>String con información básica de la factura</returns>
        public override string ToString()
        {
            return $"{ID_Factura} - {NumeroFactura} ({Cliente?.Nombre ?? "Sin Cliente"}) - ${TotalNeto:F2}";
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en listas
        /// </summary>
        /// <returns>Número de factura con información básica</returns>
        public string GetDisplayName()
        {
            return $"{NumeroFactura} - ${TotalNeto:F2}";
        }

        /// <summary>
        /// Obtiene información completa de la factura
        /// </summary>
        /// <returns>String con toda la información de la factura</returns>
        public string GetFullInfo()
        {
            string info = $"Factura #: {NumeroFactura}\n";
            info += $"NCF: {NCF ?? "No asignado"}\n";
            info += $"Tipo: {MiniSistemaFacturacion.Configuration.EmpresaConfig.Instance.GetDescripcionTipoComprobante(TipoComprobante)}\n";
            info += $"Fecha: {Fecha:dd/MM/yyyy HH:mm}\n";
            info += $"Cliente: {Cliente?.Nombre ?? "N/A"}\n";
            info += $"Total Bruto: ${TotalBruto:F2}\n";
            info += $"Impuesto ({PorcentajeImpuesto}%): ${ValorImpuesto:F2}\n";
            info += $"Total Neto: ${TotalNeto:F2}\n";
            info += $"Saldo Pendiente: ${SaldoPendiente:F2}\n";
            info += $"Estado: {Estado}\n";
            info += $"Fecha de Creación: {FechaCreacion:dd/MM/yyyy HH:mm}";
            
            return info;
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Calcula los totales de la factura basado en los detalles
        /// </summary>
        public void CalcularTotales()
        {
            if (Detalles == null || Detalles.Count == 0)
            {
                TotalBruto = 0;
                ValorImpuesto = 0;
                TotalNeto = 0;
                SaldoPendiente = 0;
                return;
            }

            // Calcular total bruto sumando los subtotales de los detalles
            TotalBruto = 0;
            foreach (var detalle in Detalles)
            {
                TotalBruto += detalle.Subtotal;
            }

            // Calcular valor del impuesto
            ValorImpuesto = TotalBruto * (PorcentajeImpuesto / 100);

            // Calcular total neto
            TotalNeto = TotalBruto + ValorImpuesto;

            // Inicializar saldo pendiente igual al total neto
            SaldoPendiente = TotalNeto;
        }

        /// <summary>
        /// Agrega un detalle a la factura
        /// </summary>
        /// <param name="detalle">Detalle a agregar</param>
        public void AgregarDetalle(DetalleFactura detalle)
        {
            if (detalle == null)
                throw new ArgumentException("El detalle no puede ser nulo");

            if (Detalles == null)
                Detalles = new System.Collections.Generic.List<DetalleFactura>();

            Detalles.Add(detalle);
            CalcularTotales();
        }

        /// <summary>
        /// Elimina un detalle de la factura
        /// </summary>
        /// <param name="idDetalle">ID del detalle a eliminar</param>
        /// <returns>True si se eliminó correctamente</returns>
        public bool EliminarDetalle(int idDetalle)
        {
            if (Detalles == null)
                return false;

            var detalle = Detalles.Find(d => d.ID_Detalle == idDetalle);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                CalcularTotales();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Aplica un pago a la factura
        /// </summary>
        /// <param name="montoPago">Monto del pago</param>
        /// <returns>True si se aplicó correctamente</returns>
        public bool AplicarPago(decimal montoPago)
        {
            if (montoPago <= 0)
                return false;

            if (montoPago > SaldoPendiente)
                return false;

            SaldoPendiente -= montoPago;

            // Actualizar estado según el saldo pendiente
            if (SaldoPendiente == 0)
                Estado = "Pagada";
            else if (SaldoPendiente < TotalNeto)
                Estado = "Parcial";

            return true;
        }

        /// <summary>
        /// Anula la factura
        /// </summary>
        public void Anular()
        {
            Estado = "Anulada";
            SaldoPendiente = 0;
        }

        /// <summary>
        /// Verifica si la factura está pagada
        /// </summary>
        /// <returns>True si está pagada</returns>
        public bool IsPagada()
        {
            return Estado == "Pagada";
        }

        /// <summary>
        /// Verifica si la factura tiene saldo pendiente
        /// </summary>
        /// <returns>True si tiene saldo pendiente</returns>
        public bool HasSaldoPendiente()
        {
            return SaldoPendiente > 0;
        }

        /// <summary>
        /// Verifica si la factura está anulada
        /// </summary>
        /// <returns>True si está anulada</returns>
        public bool IsAnulada()
        {
            return Estado == "Anulada";
        }

        /// <summary>
        /// Obtiene el número de días pendientes
        /// </summary>
        /// <returns>Número de días desde la emisión</returns>
        public int GetDiasPendientes()
        {
            return (DateTime.Now - Fecha).Days;
        }

        /// <summary>
        /// Formatea el total para mostrar
        /// </summary>
        /// <returns>Total formateado</returns>
        public string GetFormattedTotal()
        {
            return $"${TotalNeto:F2}";
        }

        /// <summary>
        /// Formatea el saldo pendiente para mostrar
        /// </summary>
        /// <returns>Saldo pendiente formateado</returns>
        public string GetFormattedSaldoPendiente()
        {
            return $"${SaldoPendiente:F2}";
        }

        /// <summary>
        /// Genera el siguiente número de factura
        /// </summary>
        /// <param name="ultimoNumero">Último número usado</param>
        /// <returns>Siguiente número de factura</returns>
        public static string GenerarSiguienteNumero(string ultimoNumero)
        {
            if (string.IsNullOrWhiteSpace(ultimoNumero))
                return "FAC-2026-001";

            try
            {
                // Extraer el número secuencial
                string[] partes = ultimoNumero.Split('-');
                if (partes.Length >= 3)
                {
                    int numero = int.Parse(partes[2]);
                    numero++;
                    return $"FAC-2026-{numero:D3}";
                }
            }
            catch
            {
                // Si hay error, retornar número por defecto
            }

            return "FAC-2026-001";
        }

        #endregion
    }
}
