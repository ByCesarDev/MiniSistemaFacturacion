using MiniSistemaFacturacion.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Configuration;
using MiniSistemaFacturacion.BusinessLogic;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmMain : Form
    {

        public FrmMain()
        {
            try
            {
                InitializeComponent();
                this.DoubleBuffered = true;
                ConfigurarDashboardCards();
                AplicarPermisosSesion();

                // Guardamos una copia de la imagen si existiese
                if (this.BackgroundImage != null)
                {
                    _imagenOriginal = (Image)this.BackgroundImage.Clone();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en constructor de FrmMain: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    "Error de Inicialización", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarDashboardCards()
        {
            VincularCard(cardClientes, (s, e) => clientesToolStripMenuItem_Click(s, e));
            VincularCard(cardProductos, (s, e) => productoToolStripMenuItem_Click(s, e));
            VincularCard(cardFacturacion, (s, e) => facturaciónToolStripMenuItem_Click(s, e));
            VincularCard(cardBuscarFactura, (s, e) => buscarFacturasToolStripMenuItem_Click(s, e));
            VincularCard(cardCobros, (s, e) => AbrirCuentasPorCobrar(s, e));
            VincularCard(cardReportes, (s, e) => reportesToolStripMenuItem_Click(s, e));
            VincularCard(cardUsuarios, (s, e) => usuariosToolStripMenuItem_Click(s, e));
            VincularCard(cardConfiguracion, (s, e) => configuracionEmpresaToolStripMenuItem_Click(s, e));

            if (btnHeaderCerrarSesion != null)
            {
                btnHeaderCerrarSesion.Click += (s, e) => cerrarSesionToolStripMenuItem_Click(s, e);
            }

            if (btnHeaderCambiarPassword != null)
            {
                btnHeaderCambiarPassword.Click += (s, e) => AbrirCambiarPassword();
            }
        }

        private void VincularCard(Panel card, EventHandler onClick)
        {
            if (card == null) return;

            Color defaultBg = Color.White;
            Color hoverBg = Color.FromArgb(245, 249, 255);

            Action<Control> attachEvents = null;
            attachEvents = (ctrl) =>
            {
                ctrl.Cursor = Cursors.Hand;
                ctrl.Click += onClick;
                ctrl.MouseEnter += (s, e) => { card.BackColor = hoverBg; };
                ctrl.MouseLeave += (s, e) =>
                {
                    Point clientPos = card.PointToClient(Cursor.Position);
                    if (!card.ClientRectangle.Contains(clientPos))
                    {
                        card.BackColor = defaultBg;
                    }
                };

                foreach (Control child in ctrl.Controls)
                {
                    if (child.Name.StartsWith("pnlAccent"))
                    {
                        child.Cursor = Cursors.Hand;
                        child.Click += onClick;
                    }
                    else
                    {
                        attachEvents(child);
                    }
                }
            };

            card.Cursor = Cursors.Hand;
            card.Click += onClick;
            card.MouseEnter += (s, e) => { card.BackColor = hoverBg; };
            card.MouseLeave += (s, e) =>
            {
                Point clientPos = card.PointToClient(Cursor.Position);
                if (!card.ClientRectangle.Contains(clientPos))
                {
                    card.BackColor = defaultBg;
                }
            };

            foreach (Control child in card.Controls)
            {
                if (!child.Name.StartsWith("pnlAccent"))
                {
                    attachEvents(child);
                }
                else
                {
                    child.Cursor = Cursors.Hand;
                    child.Click += onClick;
                }
            }
        }

        private void AbrirCuentasPorCobrar(object sender, EventArgs e)
        {
            frmCuentasPorCobrar frm = new frmCuentasPorCobrar();
            frm.ShowDialog();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            try
            {
                if (this.BackgroundImage != null)
                {
                    ActualizarFondoAdaptable();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en FrmMain_Resize: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    "Error de Redimensionamiento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image _imagenOriginal;

        private void ActualizarFondoAdaptable()
        {
            try
            {
                if (_imagenOriginal == null) return;

                // ClientSize ya nos da el área interna del Form (sin bordes ni barra de título)
                // Si el Form está maximizado, ClientSize.Height será: Alto de Pantalla - Alto de Barra de Tareas - Alto de Bordes
                int canvasWidth = this.ClientSize.Width;
                int canvasHeight = this.ClientSize.Height;

                if (canvasWidth <= 0 || canvasHeight <= 0) return;

                // Calculamos las proporciones
                float ratioX = (float)canvasWidth / (float)_imagenOriginal.Width;
                float ratioY = (float)canvasHeight / (float)_imagenOriginal.Height;

                // Si usas Math.Max, la imagen cubrirá todo el fondo (tipo 'Cover' en CSS)
                // Si usas Math.Min, la imagen se verá completa sin recortarse (tipo 'Contain')
                float ratio = Math.Max(ratioX, ratioY);

                int newWidth = (int)(_imagenOriginal.Width * ratio);
                int newHeight = (int)(_imagenOriginal.Height * ratio);

                Bitmap bmp = new Bitmap(canvasWidth, canvasHeight);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    // Centramos la imagen en el lienzo
                    int posX = (canvasWidth - newWidth) / 2;
                    int posY = canvasHeight - newHeight; // Alinea la base de la imagen con el borde inferior

                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.Clear(this.BackColor); // Limpia el fondo con el color del formulario
                    g.DrawImage(_imagenOriginal, posX, posY, newWidth, newHeight);
                }

                // Gestión de memoria: IMPORTANTE
                Image anterior = this.BackgroundImage;
                this.BackgroundImage = bmp;

                if (anterior != null)
                {
                    anterior.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en ActualizarFondoAdaptable: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    "Error de Fondo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmClientes frm = new FrmClientes();
            frm.ShowDialog();
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void productoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProductos frm = new FrmProductos();
            frm.ShowDialog();

        }

        private void facturaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFacturacion frm = new FrmFacturacion();

            frm.ShowDialog();
        }

        private void buscarFacturasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmBusquedaFacturas frm = new FrmBusquedaFacturas();
            frm.ShowDialog();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            frmCuentasPorCobrar frm = new frmCuentasPorCobrar();
            frm.ShowDialog();
        }

        private void configuracionEmpresaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SesionActual.EsAdministrador)
            {
                MessageBox.Show("Operación denegada: Solo los Administradores tienen autorización para modificar la configuración de la empresa.",
                                "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmConfiguracionEmpresa frm = new FrmConfiguracionEmpresa();
            frm.ShowDialog();
        }

        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmReportes frm = new FrmReportes();
            frm.ShowDialog();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SesionActual.EsAdministrador)
            {
                MessageBox.Show("Operación denegada: Solo los Administradores tienen autorización para gestionar usuarios.",
                                "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new FrmUsuarios())
            {
                frm.ShowDialog(this);
            }
        }

        private void AbrirCambiarPassword()
        {
            if (!SesionActual.EsAutenticado) return;

            using (var frm = new FrmCambiarPassword())
            {
                frm.ShowDialog(this);
            }
        }

        private void cambiarPasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirCambiarPassword();
        }

        private void cerrarSesionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "¿Está seguro de que desea cerrar la sesión actual y volver a la pantalla de acceso?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                SesionActual.CerrarSesion(volverALogin: true);
                this.Close();
            }
        }

        private void AplicarPermisosSesion()
        {
            if (SesionActual.EsAutenticado)
            {
                this.Text = $"Mini-Sistema de Facturación — Usuario: {SesionActual.NombreCompleto} ({SesionActual.Rol})";
                if (lblHeaderUsuario != null)
                {
                    lblHeaderUsuario.Text = $"👤 {SesionActual.NombreUsuario} ({SesionActual.Rol})";
                }
                if (btnHeaderCambiarPassword != null)
                {
                    btnHeaderCambiarPassword.Visible = true;
                }
                lblStatusUsuario.Text = $"👤 Sesión: {SesionActual.NombreUsuario} ({SesionActual.NombreCompleto}) | Rol: {SesionActual.Rol}";

                bool esAdmin = SesionActual.EsAdministrador;
                administracionToolStripMenuItem.Visible = esAdmin;
                configuracionEmpresaToolStripMenuItem.Visible = esAdmin;
                borrarDatosToolStripMenuItem.Visible = esAdmin;
                borrarDatosToolStripMenuItem.Enabled = esAdmin;

                if (cardUsuarios != null) cardUsuarios.Visible = esAdmin;
                if (cardConfiguracion != null) cardConfiguracion.Visible = esAdmin;
            }
            else
            {
                this.Text = "Mini-Sistema de Facturación";
                if (lblHeaderUsuario != null) lblHeaderUsuario.Text = "No autenticado";
                if (btnHeaderCambiarPassword != null) btnHeaderCambiarPassword.Visible = false;
                lblStatusUsuario.Text = "No autenticado";
                administracionToolStripMenuItem.Visible = false;
                configuracionEmpresaToolStripMenuItem.Visible = false;
                borrarDatosToolStripMenuItem.Visible = false;

                if (cardUsuarios != null) cardUsuarios.Visible = false;
                if (cardConfiguracion != null) cardConfiguracion.Visible = false;
            }
        }

        private void borrarDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!SesionActual.EsAdministrador)
            {
                MessageBox.Show("Operación denegada: Solo los Administradores tienen autorización para borrar datos.",
                                "Permisos Insuficientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = MessageBox.Show(
                    "¿Está seguro de que desea borrar TODOS los registros de:\n\n" +
                    "· Detalles de Facturas\n" +
                    "· Pagos\n" +
                    "· Facturas\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar Borrado Masivo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    var facturacionDAL = new FacturacionDAL();
                    facturacionDAL.BorrarTodosLosRegistros();
                    
                    MessageBox.Show(
                        "Todos los registros han sido borrados exitosamente.\n\n" +
                        "Revise la consola para ver los detalles de los registros borrados.",
                        "Borrado Completado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al borrar los registros: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Evento único para la entrada del mouse (MouseEnter)
        private void PictureBox_HoverEnter(object sender, EventArgs e)
        {
            // Convertimos el 'sender' en un PictureBox de forma segura
            if (sender is PictureBox pb)
            {
                pb.BackColor = Color.LightGray;
                pb.Cursor = Cursors.Hand; // Opcional: cambia el puntero a la "manito"
            }
        }

        // Evento único para la salida del mouse (MouseLeave)
        private void PictureBox_HoverLeave(object sender, EventArgs e)
        {
           if (sender is PictureBox pb)
            {
                pb.BackColor = Color.Transparent;
                pb.Cursor = Cursors.Default;
            }
        }
    }
}
