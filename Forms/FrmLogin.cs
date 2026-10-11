using System;
using System.Drawing;
using System.Windows.Forms;
using MiniSistemaFacturacion.BusinessLogic;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.Models;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmLogin : Form
    {
        private readonly UsuarioManager _usuarioManager;
        private bool _esModoPrimerUso = false;

        public FrmLogin()
        {
            InitializeComponent();
            _usuarioManager = new UsuarioManager();
        }

        public FrmLogin(UsuarioManager usuarioManager)
        {
            InitializeComponent();
            _usuarioManager = usuarioManager ?? new UsuarioManager();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                VerificarEstadoInicial();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error al conectar con la base de datos.";
                MessageBox.Show($"Error al verificar el estado del sistema:\n{ex.Message}\n\nVerifique que SQL Server esté ejecutándose.",
                                "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VerificarEstadoInicial()
        {
            bool hayAdmin = _usuarioManager.ExisteAdministradorActivo();

            if (!hayAdmin)
            {
                // Modo asistente: Primer arranque sin administradores
                _esModoPrimerUso = true;
                ConfigurarModoAsistente();
            }
            else
            {
                // Modo normal de inicio de sesión
                _esModoPrimerUso = false;
                ConfigurarModoNormal();
            }
        }

        private void ConfigurarModoAsistente()
        {
            this.Text = "Configuración Inicial — Mini-Sistema de Facturación";
            this.ClientSize = new Size(460, 490);

            lblAvisoAsistente.Visible = true;
            lblAvisoAsistente.Location = new Point(25, 85);

            lblNombreCompleto.Visible = true;
            lblNombreCompleto.Location = new Point(25, 142);
            txtNombreCompleto.Visible = true;
            txtNombreCompleto.Location = new Point(25, 162);

            lblUsuario.Location = new Point(25, 198);
            txtUsuario.Location = new Point(25, 218);

            lblPassword.Location = new Point(25, 252);
            txtPassword.Location = new Point(25, 272);

            lblConfirmarPassword.Visible = true;
            lblConfirmarPassword.Location = new Point(25, 307);
            txtConfirmarPassword.Visible = true;
            txtConfirmarPassword.Location = new Point(25, 327);

            chkMostrarPassword.Location = new Point(25, 360);
            lblError.Location = new Point(25, 385);

            btnIngresar.Location = new Point(25, 432);
            btnIngresar.Size = new Size(280, 36);
            btnIngresar.Text = "Crear Administrador y Acceder";

            btnSalir.Location = new Point(320, 432);

            txtNombreCompleto.Focus();
        }

        private void ConfigurarModoNormal()
        {
            this.Text = "Inicio de Sesión — Mini-Sistema de Facturación";
            this.ClientSize = new Size(460, 365);

            lblAvisoAsistente.Visible = false;
            lblNombreCompleto.Visible = false;
            txtNombreCompleto.Visible = false;
            lblConfirmarPassword.Visible = false;
            txtConfirmarPassword.Visible = false;

            // Reorganizar verticalmente para modo compacto
            lblUsuario.Location = new Point(25, 95);
            txtUsuario.Location = new Point(25, 117);

            lblPassword.Location = new Point(25, 153);
            txtPassword.Location = new Point(25, 175);

            chkMostrarPassword.Location = new Point(25, 210);
            lblError.Location = new Point(25, 235);
            lblError.Size = new Size(410, 50);

            btnIngresar.Location = new Point(25, 305);
            btnIngresar.Size = new Size(280, 36);
            btnIngresar.Text = "Iniciar Sesión";

            btnSalir.Location = new Point(320, 305);

            txtUsuario.Focus();
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool mostrar = chkMostrarPassword.Checked;
            txtPassword.UseSystemPasswordChar = !mostrar;
            txtConfirmarPassword.UseSystemPasswordChar = !mostrar;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            lblError.Text = string.Empty;

            if (_esModoPrimerUso)
            {
                ProcesarPrimerAdministrador();
            }
            else
            {
                ProcesarInicioSesion();
            }
        }

        private void ProcesarPrimerAdministrador()
        {
            string nombreCompleto = txtNombreCompleto.Text.Trim();
            string nombreUsuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;
            string confirmarPassword = txtConfirmarPassword.Text;

            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                lblError.Text = "Por favor ingrese su nombre completo.";
                txtNombreCompleto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(nombreUsuario))
            {
                lblError.Text = "Por favor elija un nombre de usuario.";
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                lblError.Text = "Por favor defina una contraseña.";
                txtPassword.Focus();
                return;
            }

            if (password != confirmarPassword)
            {
                lblError.Text = "Las contraseñas no coinciden.";
                txtConfirmarPassword.SelectAll();
                txtConfirmarPassword.Focus();
                return;
            }

            try
            {
                int nuevoId = _usuarioManager.CrearPrimerAdministrador(nombreCompleto, nombreUsuario, password);

                // Autenticar automáticamente al nuevo administrador
                Usuario adminAutenticado = _usuarioManager.Autenticar(nombreUsuario, password, out string err);

                if (adminAutenticado != null)
                {
                    MessageBox.Show(
                        $"¡Bienvenido, {adminAutenticado.NombreCompleto}!\n\nLa cuenta de Administrador principal ha sido creada exitosamente. Puede gestionar usuarios y módulos desde el menú principal.",
                        "Configuración Inicial Completada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = err ?? "No se pudo iniciar sesión automáticamente tras crear el usuario.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void ProcesarInicioSesion()
        {
            string usuario = txtUsuario.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(usuario))
            {
                lblError.Text = "Por favor ingrese su nombre de usuario.";
                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                lblError.Text = "Por favor ingrese su contraseña.";
                txtPassword.Focus();
                return;
            }

            try
            {
                Usuario user = _usuarioManager.Autenticar(usuario, password, out string mensajeError);

                if (user != null)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = mensajeError;
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Error: {ex.Message}";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
