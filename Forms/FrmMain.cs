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

                // Guardamos una copia de la imagen que pusiste en el diseñador
                // antes de que el código empiece a cambiar el fondo.
                if (this.BackgroundImage != null)
                {
                    _imagenOriginal = (Image)this.BackgroundImage.Clone();
                }

                // Dashboard movido a FrmReportes
                // InicializarDashboard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en constructor de FrmMain: {ex.Message}\n\nStackTrace: {ex.StackTrace}", 
                    "Error de Inicialización", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            try
            {
                // Solo si hay una imagen asignada
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
            FrmConfiguracionEmpresa frm = new FrmConfiguracionEmpresa();
            frm.ShowDialog();
        }

        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmReportes frm = new FrmReportes();
            frm.ShowDialog();
        }

        private void borrarDatosToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
