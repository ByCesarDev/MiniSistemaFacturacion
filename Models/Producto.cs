using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Entidad que representa un producto en el sistema
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class Producto
    {
        #region Properties

        /// <summary>
        /// Identificador único del producto
        /// </summary>
        public int ID_Producto { get; set; }

        /// <summary>
        /// Código único del producto
        /// </summary>
        [Required(ErrorMessage = "El código del producto es requerido")]
        [StringLength(50, ErrorMessage = "El código no puede exceder 50 caracteres")]
        public string Codigo { get; set; }

        /// <summary>
        /// Descripción del producto
        /// </summary>
        [Required(ErrorMessage = "La descripción del producto es requerida")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Precio unitario del producto
        /// </summary>
        [Required(ErrorMessage = "El precio unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Cantidad disponible en stock
        /// </summary>
        [Required(ErrorMessage = "El stock es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        /// <summary>
        /// Stock mínimo para alertas
        /// </summary>
        [Required(ErrorMessage = "El stock mínimo es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo")]
        public int StockMinimo { get; set; }

        /// <summary>
        /// Categoría del producto
        /// </summary>
        [StringLength(50, ErrorMessage = "La categoría no puede exceder 50 caracteres")]
        public string Categoria { get; set; }

        /// <summary>
        /// Estado del producto (Activo/Inactivo)
        /// </summary>
        public bool Estado { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Producto()
        {
            FechaCreacion = DateTime.Now;
            Estado = true;
            StockMinimo = 5;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="codigo">Código del producto</param>
        /// <param name="descripcion">Descripción del producto</param>
        /// <param name="precioUnitario">Precio unitario</param>
        public Producto(string codigo, string descripcion, decimal precioUnitario)
        {
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioUnitario = precioUnitario;
            FechaCreacion = DateTime.Now;
            Estado = true;
            Stock = 0;
            StockMinimo = 5;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="codigo">Código del producto</param>
        /// <param name="descripcion">Descripción del producto</param>
        /// <param name="precioUnitario">Precio unitario</param>
        /// <param name="stock">Stock inicial</param>
        /// <param name="categoria">Categoría</param>
        public Producto(string codigo, string descripcion, decimal precioUnitario, int stock, string categoria = "")
        {
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioUnitario = precioUnitario;
            Stock = stock;
            Categoria = categoria;
            FechaCreacion = DateTime.Now;
            Estado = true;
            StockMinimo = 5;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que los datos del producto sean correctos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
                return false;

            if (string.IsNullOrWhiteSpace(Descripcion))
                return false;

            if (PrecioUnitario <= 0)
                return false;

            if (Stock < 0)
                return false;

            if (StockMinimo < 0)
                return false;

            if (Codigo.Length > 50)
                return false;

            if (Descripcion.Length > 200)
                return false;

            if (!string.IsNullOrWhiteSpace(Categoria) && Categoria.Length > 50)
                return false;

            return true;
        }

        /// <summary>
        /// Obtiene el mensaje de error de validación
        /// </summary>
        /// <returns>Mensaje de error o string vacío si no hay errores</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(Codigo))
                return "El código del producto es requerido";

            if (string.IsNullOrWhiteSpace(Descripcion))
                return "La descripción del producto es requerida";

            if (PrecioUnitario <= 0)
                return "El precio unitario debe ser mayor que cero";

            if (Stock < 0)
                return "El stock no puede ser negativo";

            if (StockMinimo < 0)
                return "El stock mínimo no puede ser negativo";

            if (Codigo.Length > 50)
                return "El código no puede exceder 50 caracteres";

            if (Descripcion.Length > 200)
                return "La descripción no puede exceder 200 caracteres";

            if (!string.IsNullOrWhiteSpace(Categoria) && Categoria.Length > 50)
                return "La categoría no puede exceder 50 caracteres";

            return string.Empty;
        }

        #endregion

        #region Display Methods

        /// <summary>
        /// Obtiene una representación en texto del producto
        /// </summary>
        /// <returns>String con información básica del producto</returns>
        public override string ToString()
        {
            return $"{ID_Producto} - {Codigo}: {Descripcion} (${PrecioUnitario:F2})";
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en listas
        /// </summary>
        /// <returns>Descripción del producto</returns>
        public string GetDisplayName()
        {
            return Descripcion;
        }

        /// <summary>
        /// Obtiene información completa del producto
        /// </summary>
        /// <returns>String con toda la información del producto</returns>
        public string GetFullInfo()
        {
            string info = $"Código: {Codigo}\n";
            info += $"Descripción: {Descripcion}\n";
            info += $"Precio Unitario: ${PrecioUnitario:F2}\n";
            info += $"Stock Actual: {Stock}\n";
            info += $"Stock Mínimo: {StockMinimo}\n";
            
            if (!string.IsNullOrWhiteSpace(Categoria))
                info += $"Categoría: {Categoria}\n";
            
            info += $"Estado: {(Estado ? "Activo" : "Inactivo")}\n";
            info += $"Fecha de Creación: {FechaCreacion:dd/MM/yyyy HH:mm}";
            
            return info;
        }

        #endregion

        #region Stock Management

        /// <summary>
        /// Verifica si el producto tiene bajo stock
        /// </summary>
        /// <returns>True si el stock es bajo</returns>
        public bool HasLowStock()
        {
            return Stock <= StockMinimo;
        }

        /// <summary>
        /// Verifica si el producto está sin stock
        /// </summary>
        /// <returns>True si no hay stock</returns>
        public bool IsOutOfStock()
        {
            return Stock == 0;
        }

        /// <summary>
        /// Obtiene el estado del stock
        /// </summary>
        /// <returns>Estado del stock</returns>
        public StockStatus GetStockStatus()
        {
            if (Stock == 0)
                return StockStatus.OutOfStock;
            else if (Stock <= StockMinimo)
                return StockStatus.LowStock;
            else
                return StockStatus.Normal;
        }

        /// <summary>
        /// Agrega stock al producto
        /// </summary>
        /// <param name="cantidad">Cantidad a agregar</param>
        /// <returns>True si se agregó correctamente</returns>
        public bool AddStock(int cantidad)
        {
            if (cantidad <= 0)
                return false;

            Stock += cantidad;
            return true;
        }

        /// <summary>
        /// Reduce stock del producto
        /// </summary>
        /// <param name="cantidad">Cantidad a reducir</param>
        /// <returns>True si se redujo correctamente</returns>
        public bool ReduceStock(int cantidad)
        {
            if (cantidad <= 0)
                return false;

            if (Stock < cantidad)
                return false;

            Stock -= cantidad;
            return true;
        }

        /// <summary>
        /// Verifica si hay suficiente stock para una venta
        /// </summary>
        /// <param name="cantidadRequerida">Cantidad requerida</param>
        /// <returns>True si hay stock suficiente</returns>
        public bool HasEnoughStock(int cantidadRequerida)
        {
            return Stock >= cantidadRequerida;
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Activa el producto
        /// </summary>
        public void Activar()
        {
            Estado = true;
        }

        /// <summary>
        /// Desactiva el producto
        /// </summary>
        public void Desactivar()
        {
            Estado = false;
        }

        /// <summary>
        /// Verifica si el producto está activo
        /// </summary>
        /// <returns>True si el producto está activo</returns>
        public bool IsActive()
        {
            return Estado;
        }

        /// <summary>
        /// Calcula el subtotal para una cantidad específica
        /// </summary>
        /// <param name="cantidad">Cantidad de productos</param>
        /// <returns>Subtotal calculado</returns>
        public decimal CalcularSubtotal(int cantidad)
        {
            if (cantidad <= 0)
                return 0;

            return PrecioUnitario * cantidad;
        }

        /// <summary>
        /// Formatea el precio para mostrar
        /// </summary>
        /// <returns>Precio formateado</returns>
        public string GetFormattedPrice()
        {
            return $"${PrecioUnitario:F2}";
        }

        /// <summary>
        /// Obtiene el texto del estado del stock
        /// </summary>
        /// <returns>Texto del estado del stock</returns>
        public string GetStockStatusText()
        {
            switch (GetStockStatus())
            {
                case StockStatus.OutOfStock:
                    return "Sin Stock";
                case StockStatus.LowStock:
                    return "Stock Crítico";
                default:
                    return "Stock Normal";
            }
        }

        #endregion
    }

    /// <summary>
    /// Enumeración para los estados del stock
    /// </summary>
    public enum StockStatus
    {
        Normal,
        LowStock,
        OutOfStock
    }
}
