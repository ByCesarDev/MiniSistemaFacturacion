using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Entidad que representa un pago en el sistema
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class Pago
    {
        #region Properties

        /// <summary>
        /// Identificador único del pago
        /// </summary>
        public int ID_Pago { get; set; }

        /// <summary>
        /// Identificador de la factura
        /// </summary>
        [Required(ErrorMessage = "La factura es requerida")]
        public int ID_Factura { get; set; }

        /// <summary>
        /// Fecha del pago
        /// </summary>
        [Required(ErrorMessage = "La fecha del pago es requerida")]
        public DateTime FechaPago { get; set; }

        /// <summary>
        /// Monto pagado
        /// </summary>
        [Required(ErrorMessage = "El monto pagado es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto pagado debe ser mayor que cero")]
        public decimal MontoPagado { get; set; }

        /// <summary>
        /// Forma de pago utilizada
        /// </summary>
        [Required(ErrorMessage = "La forma de pago es requerida")]
        public string FormaPago { get; set; }

        /// <summary>
        /// Número de referencia del pago
        /// </summary>
        [StringLength(100, ErrorMessage = "La referencia no puede exceder 100 caracteres")]
        public string Referencia { get; set; }

        /// <summary>
        /// Observaciones del pago
        /// </summary>
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string Observaciones { get; set; }

        /// <summary>
        /// Estado del pago (Activo/Inactivo)
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Propiedad de navegación para la factura
        /// </summary>
        public Factura Factura { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Pago()
        {
            FechaPago = DateTime.Now;
            Estado = true;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="montoPagado">Monto pagado</param>
        /// <param name="formaPago">Forma de pago</param>
        public Pago(int idFactura, decimal montoPagado, string formaPago)
        {
            ID_Factura = idFactura;
            MontoPagado = montoPagado;
            FormaPago = formaPago;
            FechaPago = DateTime.Now;
            Estado = true;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="montoPagado">Monto pagado</param>
        /// <param name="formaPago">Forma de pago</param>
        /// <param name="referencia">Referencia</param>
        /// <param name="observaciones">Observaciones</param>
        public Pago(int idFactura, decimal montoPagado, string formaPago, string referencia, string observaciones = "")
        {
            ID_Factura = idFactura;
            MontoPagado = montoPagado;
            FormaPago = formaPago;
            Referencia = referencia;
            Observaciones = observaciones;
            FechaPago = DateTime.Now;
            Estado = true;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que los datos del pago sean correctos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        public bool IsValid()
        {
            if (ID_Factura <= 0)
                return false;

            if (MontoPagado <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(FormaPago))
                return false;

            if (!IsValidFormaPago(FormaPago))
                return false;

            if (!string.IsNullOrWhiteSpace(Referencia) && Referencia.Length > 100)
                return false;

            if (!string.IsNullOrWhiteSpace(Observaciones) && Observaciones.Length > 500)
                return false;

            return true;
        }

        /// <summary>
        /// Valida si una forma de pago es válida
        /// </summary>
        /// <param name="formaPago">Forma de pago a validar</param>
        /// <returns>True si la forma de pago es válida</returns>
        private bool IsValidFormaPago(string formaPago)
        {
            return formaPago == "Efectivo" || 
                   formaPago == "Tarjeta Credito" || 
                   formaPago == "Tarjeta Debito" || 
                   formaPago == "Transferencia" || 
                   formaPago == "Cheque";
        }

        /// <summary>
        /// Obtiene el mensaje de error de validación
        /// </summary>
        /// <returns>Mensaje de error o string vacío si no hay errores</returns>
        public string GetValidationError()
        {
            if (ID_Factura <= 0)
                return "La factura es requerida";

            if (MontoPagado <= 0)
                return "El monto pagado debe ser mayor que cero";

            if (string.IsNullOrWhiteSpace(FormaPago))
                return "La forma de pago es requerida";

            if (!IsValidFormaPago(FormaPago))
                return "La forma de pago no es válida";

            if (!string.IsNullOrWhiteSpace(Referencia) && Referencia.Length > 100)
                return "La referencia no puede exceder 100 caracteres";

            if (!string.IsNullOrWhiteSpace(Observaciones) && Observaciones.Length > 500)
                return "Las observaciones no pueden exceder 500 caracteres";

            return string.Empty;
        }

        #endregion

        #region Display Methods

        /// <summary>
        /// Obtiene una representación en texto del pago
        /// </summary>
        /// <returns>String con información básica del pago</returns>
        public override string ToString()
        {
            return $"{ID_Pago} - ${MontoPagado:F2} ({FormaPago}) - {FechaPago:dd/MM/yyyy}";
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en listas
        /// </summary>
        /// <returns>Información básica del pago</returns>
        public string GetDisplayName()
        {
            return $"{FormaPago} - ${MontoPagado:F2}";
        }

        /// <summary>
        /// Obtiene información completa del pago
        /// </summary>
        /// <returns>String con toda la información del pago</returns>
        public string GetFullInfo()
        {
            string info = $"Pago #: {ID_Pago}\n";
            info += $"Factura #: {Factura?.NumeroFactura ?? "N/A"}\n";
            info += $"Fecha de Pago: {FechaPago:dd/MM/yyyy HH:mm}\n";
            info += $"Monto Pagado: ${MontoPagado:F2}\n";
            info += $"Forma de Pago: {FormaPago}\n";
            
            if (!string.IsNullOrWhiteSpace(Referencia))
                info += $"Referencia: {Referencia}\n";
            
            if (!string.IsNullOrWhiteSpace(Observaciones))
                info += $"Observaciones: {Observaciones}\n";
            
            info += $"Estado: {(Estado ? "Activo" : "Inactivo")}";
            
            return info;
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Activa el pago
        /// </summary>
        public void Activar()
        {
            Estado = true;
        }

        /// <summary>
        /// Desactiva el pago
        /// </summary>
        public void Desactivar()
        {
            Estado = false;
        }

        /// <summary>
        /// Verifica si el pago está activo
        /// </summary>
        /// <returns>True si el pago está activo</returns>
        public bool IsActive()
        {
            return Estado;
        }

        /// <summary>
        /// Verifica si el pago requiere referencia
        /// </summary>
        /// <returns>True si requiere referencia</returns>
        public bool RequiereReferencia()
        {
            return FormaPago == "Transferencia" || 
                   FormaPago == "Tarjeta Credito" || 
                   FormaPago == "Tarjeta Debito" || 
                   FormaPago == "Cheque";
        }

        /// <summary>
        /// Verifica si la referencia es válida para la forma de pago
        /// </summary>
        /// <returns>True si la referencia es válida</returns>
        public bool IsValidReferencia()
        {
            if (!RequiereReferencia())
                return true; // No requiere referencia

            if (string.IsNullOrWhiteSpace(Referencia))
                return false; // Requiere referencia pero no tiene

            // Validaciones específicas por forma de pago
            switch (FormaPago)
            {
                case "Tarjeta Credito":
                case "Tarjeta Debito":
                    // Validar formato de número de tarjeta (16 dígitos)
                    return Referencia.Replace(" ", "").Replace("-", "").Length == 16 && 
                           Referencia.Replace(" ", "").Replace("-", "").All(char.IsDigit);

                case "Transferencia":
                    // Validar número de cuenta o referencia bancaria
                    return Referencia.Length >= 4;

                case "Cheque":
                    // Validar número de cheque
                    return Referencia.Length >= 3;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Obtiene el mensaje de error de referencia
        /// </summary>
        /// <returns>Mensaje de error específico</returns>
        public string GetReferenciaError()
        {
            if (!RequiereReferencia())
                return string.Empty;

            if (string.IsNullOrWhiteSpace(Referencia))
                return $"La referencia es requerida para {FormaPago}";

            switch (FormaPago)
            {
                case "Tarjeta Credito":
                case "Tarjeta Debito":
                    return "El número de tarjeta debe tener 16 dígitos";

                case "Transferencia":
                    return "La referencia bancaria debe tener al menos 4 caracteres";

                case "Cheque":
                    return "El número de cheque debe tener al menos 3 caracteres";

                default:
                    return "Referencia inválida";
            }
        }

        /// <summary>
        /// Formatea el monto para mostrar
        /// </summary>
        /// <returns>Monto formateado</returns>
        public string GetFormattedMonto()
        {
            return $"${MontoPagado:F2}";
        }

        /// <summary>
        /// Formatea la fecha para mostrar
        /// </summary>
        /// <returns>Fecha formateada</returns>
        public string GetFormattedFecha()
        {
            return FechaPago.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Obtiene una descripción corta del pago
        /// </summary>
        /// <returns>Descripción corta</returns>
        public string GetShortDescription()
        {
            return $"{FormaPago} - ${MontoPagado:F2} - {FechaPago:dd/MM/yyyy}";
        }

        /// <summary>
        /// Verifica si el pago es reciente (últimos 7 días)
        /// </summary>
        /// <returns>True si es reciente</returns>
        public bool IsReciente()
        {
            return (DateTime.Now - FechaPago).TotalDays <= 7;
        }

        /// <summary>
        /// Obtiene el número de días desde el pago
        /// </summary>
        /// <returns>Número de días</returns>
        public int GetDiasDesdePago()
        {
            return (DateTime.Now - FechaPago).Days;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Obtiene las formas de pago disponibles
        /// </summary>
        /// <returns>Array con las formas de pago</returns>
        public static string[] GetFormasPagoDisponibles()
        {
            return new string[]
            {
                "Efectivo",
                "Tarjeta Credito",
                "Tarjeta Debito",
                "Transferencia",
                "Cheque"
            };
        }

        /// <summary>
        /// Valida que un pago pueda ser creado con los parámetros dados
        /// </summary>
        /// <param name="factura">Factura</param>
        /// <param name="montoPagado">Monto a pagar</param>
        /// <param name="formaPago">Forma de pago</param>
        /// <param name="errorMessage">Mensaje de error de salida</param>
        /// <returns>True si es válido</returns>
        public static bool ValidarCreacion(Factura factura, decimal montoPagado, string formaPago, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (factura == null)
            {
                errorMessage = "La factura no puede ser nula";
                return false;
            }

            if (factura.IsAnulada())
            {
                errorMessage = "No se puede pagar una factura anulada";
                return false;
            }

            if (factura.IsPagada())
            {
                errorMessage = "La factura ya está pagada";
                return false;
            }

            if (montoPagado <= 0)
            {
                errorMessage = "El monto pagado debe ser mayor que cero";
                return false;
            }

            if (montoPagado > factura.SaldoPendiente)
            {
                errorMessage = $"El monto pagado (${montoPagado:F2}) excede el saldo pendiente (${factura.SaldoPendiente:F2})";
                return false;
            }

            if (string.IsNullOrWhiteSpace(formaPago))
            {
                errorMessage = "La forma de pago es requerida";
                return false;
            }

            string[] formasValidas = GetFormasPagoDisponibles();
            if (!formasValidas.Contains(formaPago))
            {
                errorMessage = "La forma de pago no es válida";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Genera una referencia automática según la forma de pago
        /// </summary>
        /// <param name="formaPago">Forma de pago</param>
        /// <param name="idPago">ID del pago</param>
        /// <returns>Referencia generada</returns>
        public static string GenerarReferencia(string formaPago, int idPago)
        {
            switch (formaPago)
            {
                case "Efectivo":
                    return $"EFECT-{idPago:D6}";
                
                case "Tarjeta Credito":
                case "Tarjeta Debito":
                    return $"TARJ-{DateTime.Now:yyMMdd}-{idPago:D3}";
                
                case "Transferencia":
                    return $"TRANSF-{DateTime.Now:yyMMddHHmm}-{idPago:D3}";
                
                case "Cheque":
                    return $"CHEQ-{idPago:D6}";
                
                default:
                    return $"REF-{idPago:D6}";
            }
        }

        #endregion
    }
}
