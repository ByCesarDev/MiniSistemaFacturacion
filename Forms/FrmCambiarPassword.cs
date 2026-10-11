using System;
using System.Drawing;
using System.Windows.Forms;
using MiniSistemaFacturacion.BusinessLogic;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmCambiarPassword : Form
    {
        private readonly UsuarioManager _usuarioManager;
        private readonly int _idUsuario;
        private readonly string _nombreUsuario;
        private readonly bool _esAdminRestableciendo;

        /// <summary>
        /// Constructor para el cambio de contraseña propio del usuario autenticado en sesión.
        /// </summary>
        public FrmCambiarPassword()
        {
            InitializeComponent();
            UITheme.AsignarIconoFormulario(this);
            _usuarioManager = new UsuarioManager();

            if (SesionActual.EsAutenticado)
            {
                _idUsuario = SesionActual.UsuarioLogueado.ID_Usuario;
                _nombreUsuario = SesionActual.UsuarioLogueado.NombreUsuario;
                txtUsuario.Text = $"{_nombreUsuario} ({SesionActual.UsuarioLogueado.NombreCompleto})";
            }
            else
            {
                _idUsuario = 0;
                _nombreUsuario = "Desconocido";
                txtUsuario.Text = "No autenticado";
            }

            _esAdminRestableciendo = false;
            ConfigurarEstilos();
        }

        /// <summary>
        /// Constructor para que un Administrador restablezca la contraseña de otro usuario.
        /// </summary>
        public FrmCambiarPassword(Usuario usuarioAfectado)
        {
            InitializeComponent();
            UITheme.AsignarIconoFormulario(this);
            _usuarioManager = new UsuarioManager();

            if (usuarioAfectado != null)
            {
                _idUsuario = usuarioAfectado.ID_Usuario;
                _nombreUsuario = usuarioAfectado.NombreUsuario;
                txtUsuario.Text = $"{_nombreUsuario} ({usuarioAfectado.NombreCompleto})";
            }
            else
            {
                _idUsuario = 0;
                _nombreUsuario = "Desconocido";
            }

            _esAdminRestableciendo = true;
            ConfigurarModoRestablecer();
            ConfigurarEstilos();
        }

        private void ConfigurarEstilos()
        {
            UITheme.AplicarTemaFormulario(this);
            UITheme.EstilizarTextBox(txtPasswordActual);
            UITheme.EstilizarTextBox(txtPasswordNueva);
            UITheme.EstilizarTextBox(txtPasswordConfirmar);
            UITheme.EstilizarBotonPrimario(btnGuardar);
            UITheme.EstilizarBotonCancelar(btnCancelar);
        }

        private void ConfigurarModoRestablecer()
        {
            this.Text = "Restablecer Contraseña";
            lblTitulo.Text = "Restablecer Contraseña";
            lblSubtitulo.Text = $"Restablecimiento administrativo de credenciales para '{_nombreUsuario}'";

            // Ocultar campo de contraseña actual y reacomodar hacia arriba
            lblPasswordActual.Visible = false;
            txtPasswordActual.Visible = false;

            int offset = 56;
            lblPasswordNueva.Top -= offset;
            txtPasswordNueva.Top -= offset;
            lblPasswordConfirmar.Top -= offset;
            txtPasswordConfirmar.Top -= offset;
            chkMostrar.Top -= offset;
            lblError.Top -= offset;
            btnGuardar.Top -= offset;
            btnCancelar.Top -= offset;

            this.ClientSize = new Size(this.ClientSize.Width, this.ClientSize.Height - offset);
        }

        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            bool mostrar = chkMostrar.Checked;
            txtPasswordActual.UseSystemPasswordChar = !mostrar;
            txtPasswordNueva.UseSystemPasswordChar = !mostrar;
            txtPasswordConfirmar.UseSystemPasswordChar = !mostrar;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            if (_idUsuario <= 0)
            {
                MostrarError("Usuario no válido.");
                return;
            }

            string passActual = txtPasswordActual.Text;
            string passNueva = txtPasswordNueva.Text;
            string passConfirmar = txtPasswordConfirmar.Text;

            if (!_esAdminRestableciendo && string.IsNullOrEmpty(passActual))
            {
                MostrarError("Debe ingresar su contraseña actual.");
                txtPasswordActual.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passNueva))
            {
                MostrarError("Debe ingresar la nueva contraseña.");
                txtPasswordNueva.Focus();
                return;
            }

            if (passNueva.Length < 6)
            {
                MostrarError("La nueva contraseña debe tener al menos 6 caracteres.");
                txtPasswordNueva.Focus();
                return;
            }

            if (passNueva != passConfirmar)
            {
                MostrarError("Las contraseñas no coinciden. Por favor verifíquelas.");
                txtPasswordConfirmar.Focus();
                return;
            }

            if (!_esAdminRestableciendo && passActual == passNueva)
            {
                MostrarError("La nueva contraseña no puede ser igual a la anterior.");
                txtPasswordNueva.Focus();
                return;
            }

            try
            {
                _usuarioManager.CambiarPassword(
                    _idUsuario,
                    passNueva,
                    _esAdminRestableciendo ? null : passActual,
                    esAdminRestableciendo: _esAdminRestableciendo
                );

                string mensajeExito = _esAdminRestableciendo
                    ? $"La contraseña del usuario '{_nombreUsuario}' ha sido restablecida exitosamente."
                    : "Su contraseña ha sido actualizada exitosamente.";

                MessageBox.Show(mensajeExito, "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = $"⚠ {mensaje}";
            lblError.Visible = true;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
