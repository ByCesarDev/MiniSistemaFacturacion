using System;
using System.Windows.Forms;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.DataAccess;

namespace MiniSistemaFacturacion.Utils
{
    /// <summary>
    /// Clase base para formularios de cliente (Agregar/Editar)
    /// Created by: Cesar Reyes
    /// Date: 2026-04-12
    /// </summary>
    public class FormularioClienteBase : Form
    {
        protected ClienteDAL clienteDAL = new ClienteDAL();
        protected Cliente clienteActual = null;

        public FormularioClienteBase()
        {
            // Solo inicializar si no estamos en modo diseño
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                this.KeyPreview = true;
                this.KeyDown += FormularioClienteBase_KeyDown;
            }
        }

        private void FormularioClienteBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        #region Métodos Virtuales

        /// <summary>
        /// Valida que todos los campos obligatorios estén completos
        /// </summary>
        protected virtual bool ValidarFormulario()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
            return true; // En modo diseño siempre retornar true
        }

        /// <summary>
        /// Crea un objeto Cliente a partir de los datos del formulario
        /// </summary>
        protected virtual Cliente CrearClienteDesdeControles()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
            return null; // En modo diseño retornar null
        }

        /// <summary>
        /// Guarda el cliente (nuevo o actualizado)
        /// </summary>
        protected virtual void GuardarCliente()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
        }

        /// <summary>
        /// Carga los datos del cliente en los controles (solo para edición)
        /// </summary>
        protected virtual void CargarDatosCliente()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
        }

        /// <summary>
        /// Carga los tipos de cliente en el ComboBox
        /// </summary>
        protected virtual void CargarTiposCliente()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
        }

        /// <summary>
        /// Configura las validaciones de los controles
        /// </summary>
        protected virtual void ConfigurarValidaciones()
        {
            // Implementación por defecto - las clases hijas deben sobrescribir esto
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                throw new NotImplementedException("Este método debe ser sobrescrito en la clase hija");
            }
        }

        #endregion

        #region Manejo de Eventos

        /// <summary>
        /// Maneja el evento Click del botón Guardar
        /// </summary>
        protected virtual void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarFormulario())
                    return;

                GuardarCliente();

                MessageBox.Show("Cliente guardado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Maneja el evento Click del botón Cancelar
        /// </summary>
        protected virtual void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Cliente actual que se está editando (null para nuevo cliente)
        /// </summary>
        public Cliente ClienteActual
        {
            get { return clienteActual; }
            set { clienteActual = value; }
        }

        #endregion
    }
}
