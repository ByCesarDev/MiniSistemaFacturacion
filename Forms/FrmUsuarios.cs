using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using MiniSistemaFacturacion.BusinessLogic;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.Models;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmUsuarios : Form
    {
        private readonly UsuarioManager _usuarioManager;
        private List<Usuario> _listaUsuarios = new List<Usuario>();
        private Usuario _usuarioSeleccionado = null;
        private bool _esModoNuevo = false;

        public FrmUsuarios()
        {
            InitializeComponent();
            _usuarioManager = new UsuarioManager();
        }

        public FrmUsuarios(UsuarioManager usuarioManager)
        {
            InitializeComponent();
            _usuarioManager = usuarioManager ?? new UsuarioManager();
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            // Verificación estricta de seguridad
            if (!SesionActual.EsAdministrador)
            {
                MessageBox.Show("Acceso denegado: Se requieren permisos de Administrador para abrir este módulo.",
                                "Seguridad del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            cmbFiltroRol.SelectedIndex = 0;
            cmbFiltroEstado.SelectedIndex = 0;

            cmbRol.Items.Clear();
            cmbRol.Items.AddRange(RolUsuario.RolesValidos);
            cmbRol.SelectedIndex = 1; // Empleado por defecto

            cmbEstado.Items.Clear();
            cmbEstado.Items.AddRange(EstadoUsuario.EstadosValidos);
            cmbEstado.SelectedIndex = 0; // Activo por defecto

            UITheme.AplicarTemaFormulario(this);
            UITheme.AplicarTemaDGV(dgvUsuarios);
            dgvUsuarios.AutoGenerateColumns = false;

            UITheme.EstilizarBotonPrimario(btnGuardar);
            UITheme.EstilizarBotonSuccess(btnNuevo);
            UITheme.EstilizarBotonSecundario(btnCancelar);
            UITheme.EstilizarBotonPeligro(btnActivarDesactivar);
            UITheme.EstilizarBotonWarning(btnRestablecerPassword);
            UITheme.EstilizarBotonHerramienta(btnRefrescar);

            UITheme.EstilizarTextBox(txtBuscar);
            UITheme.EstilizarTextBox(txtNombreCompleto);
            UITheme.EstilizarTextBox(txtNombreUsuario);
            UITheme.EstilizarTextBox(txtPassword);

            UITheme.EstilizarComboBox(cmbFiltroRol);
            UITheme.EstilizarComboBox(cmbFiltroEstado);
            UITheme.EstilizarComboBox(cmbRol);
            UITheme.EstilizarComboBox(cmbEstado);
            UITheme.EstilizarGroupBox(grpDetalle);

            CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            try
            {
                _listaUsuarios = _usuarioManager.ListarUsuarios();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar usuarios desde la base de datos:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarFiltros()
        {
            if (_listaUsuarios == null) return;

            string filtroTexto = txtBuscar.Text.Trim().ToLower();
            string filtroRol = cmbFiltroRol.SelectedItem?.ToString() ?? "Todos";
            string filtroEstado = cmbFiltroEstado.SelectedItem?.ToString() ?? "Todos";

            var filtrados = _listaUsuarios.Where(u =>
            {
                bool coincideTexto = string.IsNullOrEmpty(filtroTexto) ||
                    (u.NombreCompleto != null && u.NombreCompleto.ToLower().Contains(filtroTexto)) ||
                    (u.NombreUsuario != null && u.NombreUsuario.ToLower().Contains(filtroTexto));

                bool coincideRol = filtroRol == "Todos" ||
                    string.Equals(u.Rol, filtroRol, StringComparison.OrdinalIgnoreCase);

                bool coincideEstado = filtroEstado == "Todos" ||
                    string.Equals(u.Estado, filtroEstado, StringComparison.OrdinalIgnoreCase);

                return coincideTexto && coincideRol && coincideEstado;
            }).ToList();

            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = filtrados;

            // Formato visual de celdas
            foreach (DataGridViewRow row in dgvUsuarios.Rows)
            {
                var user = row.DataBoundItem as Usuario;
                if (user != null)
                {
                    if (user.Estado == EstadoUsuario.Inactivo)
                    {
                        row.DefaultCellStyle.ForeColor = Color.Gray;
                    }
                    else if (user.EstaBloqueado)
                    {
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                    }
                }
            }

            if (filtrados.Count > 0 && !_esModoNuevo)
            {
                dgvUsuarios.Rows[0].Selected = true;
                CargarDetalleUsuario(filtrados[0]);
            }
            else if (filtrados.Count == 0 && !_esModoNuevo)
            {
                LimpiarFormularioDetalle();
            }
        }

        private void CargarDetalleUsuario(Usuario usuario)
        {
            if (usuario == null) return;

            _usuarioSeleccionado = usuario;
            _esModoNuevo = false;
            lblMensajeDetalle.Text = string.Empty;

            txtNombreCompleto.Text = usuario.NombreCompleto;
            txtNombreUsuario.Text = usuario.NombreUsuario;
            txtNombreUsuario.ReadOnly = true;

            cmbRol.SelectedItem = usuario.Rol;
            cmbEstado.SelectedItem = usuario.Estado;
            cmbEstado.Enabled = true;

            txtPassword.Text = string.Empty;
            txtPassword.Enabled = false;
            chkMostrarPassword.Enabled = false;
            lblPasswordAyuda.Text = "Para modificar la contraseña use el botón 'Restablecer Clave'.";

            btnGuardar.Text = "Actualizar";
            btnActivarDesactivar.Enabled = true;
            btnActivarDesactivar.Text = usuario.EstaActivo ? "Desactivar Cuenta" : "Activar Cuenta";
            btnRestablecerPassword.Enabled = true;
        }

        private void LimpiarFormularioDetalle()
        {
            _usuarioSeleccionado = null;
            txtNombreCompleto.Text = string.Empty;
            txtNombreUsuario.Text = string.Empty;
            txtNombreUsuario.ReadOnly = false;
            txtPassword.Text = string.Empty;
            txtPassword.Enabled = true;
            chkMostrarPassword.Enabled = true;
            cmbRol.SelectedItem = RolUsuario.Empleado;
            cmbEstado.SelectedItem = EstadoUsuario.Activo;
            cmbEstado.Enabled = false;
            lblPasswordAyuda.Text = "Obligatoria al crear usuario (mín. 6 caracteres).";
            lblMensajeDetalle.Text = string.Empty;

            btnGuardar.Text = "Crear";
            btnActivarDesactivar.Enabled = false;
            btnRestablecerPassword.Enabled = false;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (_esModoNuevo) return;

            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                var usuario = dgvUsuarios.SelectedRows[0].DataBoundItem as Usuario;
                if (usuario != null)
                {
                    CargarDetalleUsuario(usuario);
                }
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            _esModoNuevo = true;
            LimpiarFormularioDetalle();
            txtNombreCompleto.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _esModoNuevo = false;
            if (_usuarioSeleccionado != null)
            {
                CargarDetalleUsuario(_usuarioSeleccionado);
            }
            else
            {
                LimpiarFormularioDetalle();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            lblMensajeDetalle.Text = string.Empty;

            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string nombreUsuario = txtNombreUsuario.Text.Trim();
            string rol = cmbRol.SelectedItem?.ToString();
            string estado = cmbEstado.SelectedItem?.ToString() ?? EstadoUsuario.Activo;

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                MostrarMensajeError("El nombre completo es obligatorio.");
                txtNombreCompleto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                MostrarMensajeError("El nombre de usuario es obligatorio.");
                txtNombreUsuario.Focus();
                return;
            }

            try
            {
                if (_esModoNuevo)
                {
                    string password = txtPassword.Text;

                    if (string.IsNullOrEmpty(password))
                    {
                        MostrarMensajeError("La contraseña es obligatoria para nuevos usuarios.");
                        txtPassword.Focus();
                        return;
                    }

                    var nuevoUsuario = new Usuario
                    {
                        NombreCompleto = nombreCompleto,
                        NombreUsuario = nombreUsuario,
                        Rol = rol,
                        Estado = EstadoUsuario.Activo
                    };

                    int nuevoId = _usuarioManager.CrearUsuario(nuevoUsuario, password);

                    MessageBox.Show($"Usuario '{nombreUsuario}' creado exitosamente con ID #{nuevoId}.",
                                    "Usuario Creado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _esModoNuevo = false;
                    CargarUsuarios();
                    SeleccionarUsuarioPorId(nuevoId);
                }
                else
                {
                    if (_usuarioSeleccionado == null) return;

                    _usuarioSeleccionado.NombreCompleto = nombreCompleto;
                    _usuarioSeleccionado.Rol = rol;
                    _usuarioSeleccionado.Estado = estado;

                    _usuarioManager.ActualizarUsuario(_usuarioSeleccionado);

                    MessageBox.Show($"Usuario '{_usuarioSeleccionado.NombreUsuario}' actualizado exitosamente.",
                                    "Usuario Actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    int idActual = _usuarioSeleccionado.ID_Usuario;
                    CargarUsuarios();
                    SeleccionarUsuarioPorId(idActual);
                }
            }
            catch (Exception ex)
            {
                MostrarMensajeError(ex.Message);
            }
        }

        private void btnActivarDesactivar_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null) return;

            try
            {
                if (_usuarioSeleccionado.EstaActivo)
                {
                    var confirm = MessageBox.Show(
                        $"¿Está seguro de que desea desactivar la cuenta de '{_usuarioSeleccionado.NombreUsuario}'?\nEl usuario no podrá iniciar sesión mientras permanezca inactivo.",
                        "Confirmar Desactivación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirm == DialogResult.Yes)
                    {
                        _usuarioManager.DesactivarUsuario(_usuarioSeleccionado.ID_Usuario);
                        MessageBox.Show("Cuenta desactivada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        int id = _usuarioSeleccionado.ID_Usuario;
                        CargarUsuarios();
                        SeleccionarUsuarioPorId(id);
                    }
                }
                else
                {
                    _usuarioManager.ActivarUsuario(_usuarioSeleccionado.ID_Usuario);
                    MessageBox.Show("Cuenta reactivada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    int id = _usuarioSeleccionado.ID_Usuario;
                    CargarUsuarios();
                    SeleccionarUsuarioPorId(id);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operación no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnRestablecerPassword_Click(object sender, EventArgs e)
        {
            if (_usuarioSeleccionado == null) return;

            string prompt = $"Ingrese la nueva contraseña para el usuario '{_usuarioSeleccionado.NombreUsuario}' (mínimo 6 caracteres):";
            string nuevaClave = Interaction.InputBox(prompt, "Restablecer Contraseña", "");

            if (string.IsNullOrEmpty(nuevaClave))
                return; // Usuario canceló

            try
            {
                _usuarioManager.CambiarPassword(_usuarioSeleccionado.ID_Usuario, nuevaClave, esAdminRestableciendo: true);
                MessageBox.Show($"La contraseña del usuario '{_usuarioSeleccionado.NombreUsuario}' ha sido restablecida exitosamente.",
                                "Contraseña Actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al Restablecer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SeleccionarUsuarioPorId(int idUsuario)
        {
            foreach (DataGridViewRow row in dgvUsuarios.Rows)
            {
                var u = row.DataBoundItem as Usuario;
                if (u != null && u.ID_Usuario == idUsuario)
                {
                    row.Selected = true;
                    CargarDetalleUsuario(u);
                    break;
                }
            }
        }

        private void MostrarMensajeError(string mensaje)
        {
            lblMensajeDetalle.Text = mensaje;
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkMostrarPassword.Checked;
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void Filtros_Changed(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            CargarUsuarios();
        }
    }
}
