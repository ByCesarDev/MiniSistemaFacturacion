using System;
using System.ComponentModel.DataAnnotations;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Entidad que representa un detalle de factura en el sistema
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class DetalleFactura
    {
        #region Properties

        /// <summary>
        /// Identificador único del detalle
        /// </summary>
        public int ID_Detalle { get; set; }

        /// <summary>
        /// Identificador de la factura
        /// </summary>
        [Required(ErrorMessage = "La factura es requerida")]
        public int ID_Factura { get; set; }

        /// <summary>
        /// Identificador del producto
        /// </summary>
        [Required(ErrorMessage = "El producto es requerido")]
        public int ID_Producto { get; set; }

        //Descripción del producto para mostrar en la factura 
        public string Descripcion { get; set; }

        /// <summary>
        /// Cantidad de productos
        /// </summary>
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Cantidad { get; set; }

        /// <summary>
        /// Precio unitario al momento de la venta
        /// </summary>
        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero")]
        public decimal PrecioUnitarioVenta { get; set; }

        /// <summary>
        /// Subtotal del detalle (Cantidad * PrecioUnitarioVenta)
        /// </summary>
        [Required(ErrorMessage = "El subtotal es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor que cero")]
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Propiedad de navegación para el producto
        /// </summary>
        public Producto Producto { get; set; }

        /// <summary>
        /// Propiedad de navegación para la factura
        /// </summary>
        public Factura Factura { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public DetalleFactura()
        {
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidad">Cantidad</param>
        /// <param name="precioUnitarioVenta">Precio unitario de venta</param>
        public DetalleFactura(int idFactura, int idProducto, int cantidad, decimal precioUnitarioVenta)
        {
            ID_Factura = idFactura;
            ID_Producto = idProducto;
            Cantidad = cantidad;
            PrecioUnitarioVenta = precioUnitarioVenta;
            CalcularSubtotal();
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="producto">Producto</param>
        /// <param name="cantidad">Cantidad</param>
        public DetalleFactura(int idFactura, Producto producto, int cantidad)
        {
            ID_Factura = idFactura;
            ID_Producto = producto.ID_Producto;
            Producto = producto;
            Cantidad = cantidad;
            PrecioUnitarioVenta = producto.PrecioUnitario;
            CalcularSubtotal();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que los datos del detalle sean correctos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        public bool IsValid()
        {
            if (ID_Factura <= 0)
                return false;

            if (ID_Producto <= 0)
                return false;

            if (Cantidad <= 0)
                return false;

            if (PrecioUnitarioVenta <= 0)
                return false;

            if (Subtotal <= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Obtiene el mensaje de error de validación
        /// </summary>
        /// <returns>Mensaje de error o string vacío si no hay errores</returns>
        public string GetValidationError()
        {
            if (ID_Factura <= 0)
                return "La factura es requerida";

            if (ID_Producto <= 0)
                return "El producto es requerido";

            if (Cantidad <= 0)
                return "La cantidad debe ser mayor que cero";

            if (PrecioUnitarioVenta <= 0)
                return "El precio unitario debe ser mayor que cero";

            if (Subtotal <= 0)
                return "El subtotal debe ser mayor que cero";

            return string.Empty;
        }

        #endregion

        #region Display Methods

        /// <summary>
        /// Obtiene una representación en texto del detalle
        /// </summary>
        /// <returns>String con información básica del detalle</returns>
        public override string ToString()
        {
            return $"{Producto?.Descripcion ?? "Producto N/A"} x{Cantidad} @ ${PrecioUnitarioVenta:F2} = ${Subtotal:F2}";
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en listas
        /// </summary>
        /// <returns>Descripción del producto con cantidad</returns>
        public string GetDisplayName()
        {
            return $"{Producto?.Descripcion ?? "Producto N/A"} x{Cantidad}";
        }

        /// <summary>
        /// Obtiene información completa del detalle
        /// </summary>
        /// <returns>String con toda la información del detalle</returns>
        public string GetFullInfo()
        {
            string info = $"Producto: {Producto?.Descripcion ?? "N/A"}\n";
            info += $"Código: {Producto?.Codigo ?? "N/A"}\n";
            info += $"Cantidad: {Cantidad}\n";
            info += $"Precio Unitario: ${PrecioUnitarioVenta:F2}\n";
            info += $"Subtotal: ${Subtotal:F2}";
            
            return info;
        }

        /// <summary>
        /// Obtiene una descripción corta para mostrar en tablas
        /// </summary>
        /// <returns>Descripción corta</returns>
        public string GetShortDescription()
        {
            string descripcion = Producto?.Descripcion ?? "Producto N/A";
            if (descripcion.Length > 30)
                descripcion = descripcion.Substring(0, 27) + "...";
            
            return $"{descripcion} x{Cantidad}";
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Calcula el subtotal del detalle
        /// </summary>
        public void CalcularSubtotal()
        {
            Subtotal = Cantidad * PrecioUnitarioVenta;
        }

        /// <summary>
        /// Actualiza la cantidad y recalcula el subtotal
        /// </summary>
        /// <param name="nuevaCantidad">Nueva cantidad</param>
        /// <returns>True si se actualizó correctamente</returns>
        public bool ActualizarCantidad(int nuevaCantidad)
        {
            if (nuevaCantidad <= 0)
                return false;

            Cantidad = nuevaCantidad;
            CalcularSubtotal();
            return true;
        }

        /// <summary>
        /// Actualiza el precio unitario y recalcula el subtotal
        /// </summary>
        /// <param name="nuevoPrecio">Nuevo precio unitario</param>
        /// <returns>True si se actualizó correctamente</returns>
        public bool ActualizarPrecioUnitario(decimal nuevoPrecio)
        {
            if (nuevoPrecio <= 0)
                return false;

            PrecioUnitarioVenta = nuevoPrecio;
            CalcularSubtotal();
            return true;
        }

        /// <summary>
        /// Verifica si el detalle tiene stock suficiente
        /// </summary>
        /// <returns>True si hay stock suficiente</returns>
        public bool HasSufficientStock()
        {
            if (Producto == null)
                return false;

            return Producto.HasEnoughStock(Cantidad);
        }

        /// <summary>
        /// Obtiene el precio original del producto
        /// </summary>
        /// <returns>Precio original del producto</returns>
        public decimal GetPrecioOriginal()
        {
            return Producto?.PrecioUnitario ?? 0;
        }

        /// <summary>
        /// Verifica si el precio de venta es diferente del precio original
        /// </summary>
        /// <returns>True si el precio cambió</returns>
        public bool HasPriceChanged()
        {
            if (Producto == null)
                return false;

            return PrecioUnitarioVenta != Producto.PrecioUnitario;
        }

        /// <summary>
        /// Obtiene el porcentaje de cambio en el precio
        /// </summary>
        /// <returns>Porcentaje de cambio</returns>
        public decimal GetPriceChangePercentage()
        {
            if (Producto == null || Producto.PrecioUnitario == 0)
                return 0;

            decimal diferencia = PrecioUnitarioVenta - Producto.PrecioUnitario;
            return (diferencia / Producto.PrecioUnitario) * 100;
        }

        /// <summary>
        /// Formatea el subtotal para mostrar
        /// </summary>
        /// <returns>Subtotal formateado</returns>
        public string GetFormattedSubtotal()
        {
            return $"${Subtotal:F2}";
        }

        /// <summary>
        /// Formatea el precio unitario para mostrar
        /// </summary>
        /// <returns>Precio unitario formateado</returns>
        public string GetFormattedPrecioUnitario()
        {
            return $"${PrecioUnitarioVenta:F2}";
        }

        /// <summary>
        /// Obtiene información del cambio de precio
        /// </summary>
        /// <returns>Texto con información del cambio de precio</returns>
        public string GetPriceChangeInfo()
        {
            if (!HasPriceChanged())
                return "Precio normal";

            decimal porcentaje = GetPriceChangePercentage();
            if (porcentaje > 0)
                return $"+{porcentaje:F1}% (aumento)";
            else
                return $"{porcentaje:F1}% (descuento)";
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Crea un detalle de factura a partir de un producto
        /// </summary>
        /// <param name="idFactura">ID de la factura</param>
        /// <param name="producto">Producto</param>
        /// <param name="cantidad">Cantidad</param>
        /// <param name="aplicarDescuento">Aplicar descuento</param>
        /// <param name="porcentajeDescuento">Porcentaje de descuento</param>
        /// <returns>Nuevo detalle de factura</returns>
        public static DetalleFactura CrearDesdeProducto(int idFactura, Producto producto, int cantidad, bool aplicarDescuento = false, decimal porcentajeDescuento = 0)
        {
            if (producto == null)
                throw new ArgumentException("El producto no puede ser nulo");

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero");

            decimal precioVenta = producto.PrecioUnitario;

            if (aplicarDescuento && porcentajeDescuento > 0)
            {
                precioVenta = precioVenta * (1 - (porcentajeDescuento / 100));
            }

            return new DetalleFactura(idFactura, producto, cantidad)
            {
                PrecioUnitarioVenta = precioVenta
            };
        }

        /// <summary>
        /// Valida que un detalle pueda ser creado con los parámetros dados
        /// </summary>
        /// <param name="producto">Producto</param>
        /// <param name="cantidad">Cantidad</param>
        /// <param name="errorMessage">Mensaje de error de salida</param>
        /// <returns>True si es válido</returns>
        public static bool ValidarCreacion(Producto producto, int cantidad, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (producto == null)
            {
                errorMessage = "El producto no puede ser nulo";
                return false;
            }

            if (!producto.IsActive())
            {
                errorMessage = "El producto no está activo";
                return false;
            }

            if (cantidad <= 0)
            {
                errorMessage = "La cantidad debe ser mayor que cero";
                return false;
            }

            if (!producto.HasEnoughStock(cantidad))
            {
                errorMessage = $"Stock insuficiente. Disponible: {producto.Stock}, Solicitado: {cantidad}";
                return false;
            }

            return true;
        }

        #endregion
    }
}
