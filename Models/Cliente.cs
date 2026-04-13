using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MiniSistemaFacturacion.Models
{
    /// <summary>
    /// Enumeración para tipos de cliente
    /// </summary>
    public enum TipoClienteEnum
    {
        ConsumidorFinal = 0,  // CF
        CreditoFiscal = 1     // CCF
    }

    /// <summary>
    /// Entidad que representa un cliente en el sistema
    /// Created by: Cesar Reyes
    /// Date: 2026-04-07
    /// </summary>
    public class Cliente
    {
        #region Properties

        /// <summary>
        /// Identificador único del cliente
        /// </summary>
        public int ID_Cliente { get; set; }

        /// <summary>
        /// Nombre completo del cliente
        /// </summary>
        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Número de cédula o documento de identidad
        /// </summary>
        [Required(ErrorMessage = "La cédula es requerida")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder 20 caracteres")]
        public string Cedula { get; set; }

        /// <summary>
        /// Dirección del cliente
        /// </summary>
        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres")]
        public string Direccion { get; set; }

        /// <summary>
        /// Número de teléfono del cliente
        /// </summary>
        [StringLength(25, ErrorMessage = "El teléfono no puede exceder 25 caracteres")]
        [Phone(ErrorMessage = "Formato de teléfono no válido")]
        public string Telefono { get; set; }

        /// <summary>
        /// Correo electrónico del cliente
        /// </summary>
        [StringLength(100, ErrorMessage = "El email no puede exceder 100 caracteres")]
        [EmailAddress(ErrorMessage = "Formato de email no válido")]
        public string Email { get; set; }

        /// <summary>
        /// Tipo de cliente (CF = Consumidor Final, CCF = Crédito Fiscal)
        /// </summary>
        [Required(ErrorMessage = "El tipo de cliente es requerido")]
        [StringLength(3, ErrorMessage = "El tipo de cliente no puede exceder 3 caracteres")]
        public string TipoCliente { get; set; }

        /// <summary>
        /// RNC del cliente (obligatorio para Crédito Fiscal)
        /// </summary>
        [StringLength(20, ErrorMessage = "El RNC no puede exceder 20 caracteres")]
        public string RNC { get; set; }

        /// <summary>
        /// Fecha de creación del registro
        /// </summary>
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Estado del cliente (Activo/Inactivo)
        /// </summary>
        public bool Estado { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public Cliente()
        {
            TipoCliente = "CF"; // Consumidor Final por defecto
            FechaCreacion = DateTime.Now;
            Estado = true;
        }

        /// <summary>
        /// Constructor con parámetros básicos
        /// </summary>
        /// <param name="nombre">Nombre del cliente</param>
        /// <param name="cedula">Cédula del cliente</param>
        public Cliente(string nombre, string cedula)
        {
            Nombre = nombre;
            Cedula = cedula;
            TipoCliente = "CF"; // Consumidor Final por defecto
            FechaCreacion = DateTime.Now;
            Estado = true;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="nombre">Nombre del cliente</param>
        /// <param name="cedula">Cédula del cliente</param>
        /// <param name="tipoCliente">Tipo de cliente</param>
        /// <param name="rnc">RNC del cliente (opcional)</param>
        public Cliente(string nombre, string cedula, string tipoCliente, string rnc = null)
        {
            Nombre = nombre;
            Cedula = cedula;
            TipoCliente = tipoCliente;
            RNC = rnc;
            FechaCreacion = DateTime.Now;
            Estado = true;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Valida que los datos del cliente sean correctos
        /// </summary>
        /// <returns>True si los datos son válidos</returns>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
                return false;

            if (string.IsNullOrWhiteSpace(Cedula))
                return false;

            if (string.IsNullOrWhiteSpace(TipoCliente))
                return false;

            if (Nombre.Length > 100)
                return false;

            if (Cedula.Length > 20)
                return false;

            if (TipoCliente.Length > 3)
                return false;

            // Validar que TipoCliente sea un valor válido
            if (TipoCliente != "CF" && TipoCliente != "CCF")
                return false;

            // Validar RNC para clientes de Crédito Fiscal
            if (TipoCliente == "CCF" && string.IsNullOrWhiteSpace(RNC))
                return false;

            if (!string.IsNullOrWhiteSpace(Direccion) && Direccion.Length > 200)
                return false;

            if (!string.IsNullOrWhiteSpace(Telefono) && Telefono.Length > 25)
                return false;

            if (!string.IsNullOrWhiteSpace(Email) && Email.Length > 100)
                return false;

            if (!string.IsNullOrWhiteSpace(RNC) && RNC.Length > 20)
                return false;

            return true;
        }

        /// <summary>
        /// Obtiene el mensaje de error de validación
        /// </summary>
        /// <returns>Mensaje de error o string vacío si no hay errores</returns>
        public string GetValidationError()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
                return "El nombre del cliente es requerido";

            if (string.IsNullOrWhiteSpace(Cedula))
                return "La cédula es requerida";

            if (string.IsNullOrWhiteSpace(TipoCliente))
                return "El tipo de cliente es requerido";

            if (Nombre.Length > 100)
                return "El nombre no puede exceder 100 caracteres";

            if (Cedula.Length > 20)
                return "La cédula no puede exceder 20 caracteres";

            if (TipoCliente.Length > 3)
                return "El tipo de cliente no puede exceder 3 caracteres";

            // Validar que TipoCliente sea un valor válido
            if (TipoCliente != "CF" && TipoCliente != "CCF")
                return "El tipo de cliente debe ser 'CF' (Consumidor Final) o 'CCF' (Crédito Fiscal)";

            // Validar RNC para clientes de Crédito Fiscal
            if (TipoCliente == "CCF" && string.IsNullOrWhiteSpace(RNC))
                return "El RNC es obligatorio para clientes de Crédito Fiscal";

            if (!string.IsNullOrWhiteSpace(Direccion) && Direccion.Length > 200)
                return "La dirección no puede exceder 200 caracteres";

            if (!string.IsNullOrWhiteSpace(Telefono) && Telefono.Length > 25)
                return "El teléfono no puede exceder 25 caracteres";

            if (!string.IsNullOrWhiteSpace(Email) && Email.Length > 100)
                return "El email no puede exceder 100 caracteres";

            if (!string.IsNullOrWhiteSpace(RNC) && RNC.Length > 20)
                return "El RNC no puede exceder 20 caracteres";

            return string.Empty;
        }

        #endregion

        #region Display Methods

        /// <summary>
        /// Obtiene una representación en texto del cliente
        /// </summary>
        /// <returns>String con información básica del cliente</returns>
        public override string ToString()
        {
            return $"{ID_Cliente} - {Nombre} ({Cedula})";
        }

        /// <summary>
        /// Obtiene el nombre para mostrar en listas
        /// </summary>
        /// <returns>Nombre completo del cliente</returns>
        public string GetDisplayName()
        {
            return Nombre;
        }

        /// <summary>
        /// Obtiene información completa del cliente
        /// </summary>
        /// <returns>String con toda la información del cliente</returns>
        public string GetFullInfo()
        {
            string info = $"Nombre: {Nombre}\n";
            info += $"Cédula: {Cedula}\n";
            info += $"Tipo: {GetTipoClienteDescripcion()}\n";
            
            if (!string.IsNullOrWhiteSpace(RNC))
                info += $"RNC: {RNC}\n";
            
            if (!string.IsNullOrWhiteSpace(Direccion))
                info += $"Dirección: {Direccion}\n";
            
            if (!string.IsNullOrWhiteSpace(Telefono))
                info += $"Teléfono: {Telefono}\n";
            
            if (!string.IsNullOrWhiteSpace(Email))
                info += $"Email: {Email}\n";
            
            info += $"Estado: {(Estado ? "Activo" : "Inactivo")}\n";
            info += $"Fecha de Creación: {FechaCreacion:dd/MM/yyyy HH:mm}";
            
            return info;
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Activa el cliente
        /// </summary>
        public void Activar()
        {
            Estado = true;
        }

        /// <summary>
        /// Desactiva el cliente
        /// </summary>
        public void Desactivar()
        {
            Estado = false;
        }

        /// <summary>
        /// Verifica si el cliente está activo
        /// </summary>
        /// <returns>True si el cliente está activo</returns>
        public bool IsActive()
        {
            return Estado;
        }

        /// <summary>
        /// Formatea la cédula para mostrar
        /// </summary>
        /// <returns>Cédula formateada</returns>
        public string GetFormattedCedula()
        {
            if (string.IsNullOrWhiteSpace(Cedula))
                return string.Empty;

            // Ejemplo de formato para El Salvador: 0000-0000-0000
            if (Cedula.Length == 9 && Cedula.Contains("-"))
                return Cedula;

            // Si no tiene guiones y tiene 8 dígitos, agregar formato
            if (Cedula.Length == 8 && Cedula.All(char.IsDigit))
            {
                return $"{Cedula.Substring(0, 4)}-{Cedula.Substring(4, 4)}";
            }

            return Cedula;
        }

        /// <summary>
        /// Verifica si el cliente es Consumidor Final
        /// </summary>
        /// <returns>True si es Consumidor Final</returns>
        public bool EsConsumidorFinal()
        {
            return TipoCliente == "CF";
        }

        /// <summary>
        /// Verifica si el cliente es de Crédito Fiscal
        /// </summary>
        /// <returns>True si es de Crédito Fiscal</returns>
        public bool EsCreditoFiscal()
        {
            return TipoCliente == "CCF";
        }

        /// <summary>
        /// Obtiene la descripción del tipo de cliente
        /// </summary>
        /// <returns>Descripción del tipo de cliente</returns>
        public string GetTipoClienteDescripcion()
        {
            switch (TipoCliente)
            {
                case "CF":
                    return "Consumidor Final";
                case "CCF":
                    return "Crédito Fiscal";
                default:
                    return "Desconocido";
            }
        }

        /// <summary>
        /// Formatea el RNC para mostrar
        /// </summary>
        /// <returns>RNC formateado o string vacío si no tiene</returns>
        public string GetFormattedRNC()
        {
            if (string.IsNullOrWhiteSpace(RNC))
                return string.Empty;

            return RNC;
        }

        /// <summary>
        /// Obtiene el identificador completo del cliente (con tipo)
        /// </summary>
        /// <returns>Identificador con tipo de cliente</returns>
        public string GetIdentificadorCompleto()
        {
            if (EsCreditoFiscal() && !string.IsNullOrWhiteSpace(RNC))
                return $"{Nombre} ({GetTipoClienteDescripcion()}) - RNC: {RNC}";
            else
                return $"{Nombre} ({GetTipoClienteDescripcion()}) - Cédula: {GetFormattedCedula()}";
        }

        #endregion
    }
}
