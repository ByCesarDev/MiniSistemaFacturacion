using MiniSistemaFacturacion.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            // Guardamos una copia de la imagen que pusiste en el diseñador
            // antes de que el código empiece a cambiar el fondo.
            if (this.BackgroundImage != null)
            {
                _imagenOriginal = (Image)this.BackgroundImage.Clone();
            }
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            // Solo si hay una imagen asignada
            if (this.BackgroundImage != null)
            {
                ActualizarFondoAdaptable();
            }
        }

        private Image _imagenOriginal;

        private void ActualizarFondoAdaptable()
        {
            if (_imagenOriginal == null) return;

            int canvasWidth = this.ClientSize.Width;
            int canvasHeight = this.ClientSize.Height;

            if (canvasWidth == 0 || canvasHeight == 0) return;

            float ratioX = (float)canvasWidth / (float)_imagenOriginal.Width;
            float ratioY = (float)canvasHeight / (float)_imagenOriginal.Height;
            float ratio = Math.Max(ratioX, ratioY);

            int newWidth = (int)(_imagenOriginal.Width * ratio);
            int newHeight = (int)(_imagenOriginal.Height * ratio);

            Bitmap bmp = new Bitmap(canvasWidth, canvasHeight);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                int posX = (canvasWidth - newWidth) / 2;
                int posY = (canvasHeight - newHeight) / 2;

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(_imagenOriginal, posX, posY, newWidth, newHeight);
            }

            // Liberar memoria del fondo anterior para no saturar el programa
            if (this.BackgroundImage != null) this.BackgroundImage.Dispose();

            this.BackgroundImage = bmp;
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
