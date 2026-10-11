using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSistemaFacturacion.Utils
{
    /// <summary>
    /// Sistema de diseño visual moderno y consistente para la aplicación.
    /// Paleta Slate Navy, Segoe UI, acabados limpios y profesionales.
    /// </summary>
    public static class UITheme
    {
        #region Colores de la Marca y Sistema

        // Encabezados y barras principales
        public static readonly Color HeaderNavy = Color.FromArgb(37, 57, 89);
        public static readonly Color HeaderSubtitle = Color.FromArgb(200, 215, 235);

        // Fondo del lienzo de formularios
        public static readonly Color FormBackground = Color.FromArgb(248, 249, 250);
        public static readonly Color SurfaceWhite = Color.White;
        public static readonly Color PanelFiltros = Color.FromArgb(255, 255, 255);

        // Botones y estados
        public static readonly Color PrimaryBlue = Color.FromArgb(37, 99, 170);
        public static readonly Color PrimaryBlueHover = Color.FromArgb(28, 80, 140);
        public static readonly Color SuccessGreen = Color.FromArgb(46, 139, 87);
        public static readonly Color DangerRed = Color.FromArgb(210, 50, 50);
        public static readonly Color WarningYellow = Color.FromArgb(255, 248, 225);
        public static readonly Color WarningBorder = Color.FromArgb(220, 170, 50);
        public static readonly Color WarningText = Color.FromArgb(140, 70, 0);

        // Controles secundarios y neutros
        public static readonly Color SecondaryBtnBg = Color.FromArgb(240, 242, 245);
        public static readonly Color SecondaryBtnText = Color.FromArgb(50, 50, 50);
        public static readonly Color SecondaryBtnBorder = Color.FromArgb(205, 212, 220);

        // Textos
        public static readonly Color TextMain = Color.FromArgb(33, 37, 41);
        public static readonly Color TextMuted = Color.FromArgb(108, 117, 125);
        public static readonly Color TextDarkGray = Color.FromArgb(40, 40, 40);

        // Bordes y separadores
        public static readonly Color BorderLight = Color.FromArgb(220, 226, 234);
        public static readonly Color BorderCard = Color.FromArgb(205, 215, 225);

        // Tablas DataGridView
        public static readonly Color GridHeaderBg = Color.FromArgb(240, 244, 248);
        public static readonly Color GridHeaderFore = Color.FromArgb(40, 40, 40);
        public static readonly Color GridRowAlternate = Color.FromArgb(250, 252, 255);
        public static readonly Color GridSelectionBg = Color.FromArgb(215, 230, 250);
        public static readonly Color GridSelectionFore = Color.FromArgb(20, 20, 20);

        #endregion

        #region Fuentes

        public static readonly Font FontHeaderTitle = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        public static readonly Font FontHeaderSubtitle = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FontSectionTitle = new Font("Segoe UI", 11F, FontStyle.Bold);
        public static readonly Font FontBodyBold = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font FontBodyRegular = new Font("Segoe UI", 9F, FontStyle.Regular);
        public static readonly Font FontSmallRegular = new Font("Segoe UI", 8F, FontStyle.Regular);
        public static readonly Font FontButton = new Font("Segoe UI", 9F, FontStyle.Bold);

        #endregion

        #region Métodos de Creación y Estilización

        /// <summary>
        /// Aplica la configuración base a un formulario (fondo, fuente, doble buffer).
        /// </summary>
        public static void AplicarTemaFormulario(Form form)
        {
            if (form == null) return;
            form.BackColor = FormBackground;
            form.Font = FontBodyRegular;
        }

        /// <summary>
        /// Crea un encabezado profesional con banner Slate Navy y subtítulo explicativo.
        /// </summary>
        public static Panel CrearHeader(string titulo, string subtitulo, int altura = 68)
        {
            Panel panelHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = altura,
                BackColor = HeaderNavy
            };

            Label lblTitulo = new Label
            {
                Text = titulo,
                Font = FontHeaderTitle,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(16, 12)
            };

            Label lblSubtitulo = new Label
            {
                Text = subtitulo,
                Font = FontHeaderSubtitle,
                ForeColor = HeaderSubtitle,
                AutoSize = true,
                Location = new Point(18, 40)
            };

            panelHeader.Controls.Add(lblTitulo);
            panelHeader.Controls.Add(lblSubtitulo);
            return panelHeader;
        }

        /// <summary>
        /// Estiliza un botón como Primario (Azul corporativo).
        /// </summary>
        public static void EstilizarBotonPrimario(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = PrimaryBlue;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Estiliza un botón como Éxito (Verde moderno).
        /// </summary>
        public static void EstilizarBotonExito(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = SuccessGreen;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void EstilizarBotonSuccess(Button btn) => EstilizarBotonExito(btn);
        public static void AplicarTemaDGV(DataGridView dgv) => EstilizarDataGridView(dgv);

        /// <summary>
        /// Estiliza un botón como Peligro / Eliminar (Rojo moderno).
        /// </summary>
        public static void EstilizarBotonPeligro(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = DangerRed;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Estiliza un botón como Secundario / Cancelar / Salir (Gris suave y limpio).
        /// </summary>
        public static void EstilizarBotonSecundario(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = SecondaryBtnBg;
            btn.ForeColor = SecondaryBtnText;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = SecondaryBtnBorder;
            btn.FlatAppearance.BorderSize = 1;
            btn.Font = FontBodyRegular;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Estiliza un botón para herramientas / filtros (Buscar, Refrescar).
        /// </summary>
        public static void EstilizarBotonHerramienta(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = Color.FromArgb(240, 244, 248);
            btn.ForeColor = SecondaryBtnText;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btn.FlatAppearance.BorderSize = 1;
            btn.Font = FontBodyRegular;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Estiliza un DataGridView con encabezados modernos, colores suaves y selección nítida.
        /// </summary>
        public static void EstilizarDataGridView(DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.BackgroundColor = SurfaceWhite;
            dgv.BorderStyle = BorderStyle.Fixed3D;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(235, 238, 242);
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;

            // Encabezados
            dgv.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderFore;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontBodyBold;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = GridHeaderBg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = GridHeaderFore;
            dgv.ColumnHeadersHeight = 34;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Celdas normales
            dgv.DefaultCellStyle.BackColor = SurfaceWhite;
            dgv.DefaultCellStyle.ForeColor = TextMain;
            dgv.DefaultCellStyle.Font = FontBodyRegular;
            dgv.DefaultCellStyle.SelectionBackColor = GridSelectionBg;
            dgv.DefaultCellStyle.SelectionForeColor = GridSelectionFore;
            dgv.RowTemplate.Height = 28;

            // Filas alternas
            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridRowAlternate;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextMain;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = GridSelectionBg;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = GridSelectionFore;
        }

        /// <summary>
        /// Estiliza un botón como Advertencia / Alerta (Amarillo / Dorado).
        /// </summary>
        public static void EstilizarBotonWarning(Button btn)
        {
            if (btn == null) return;
            btn.BackColor = WarningYellow;
            btn.ForeColor = WarningText;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = WarningBorder;
            btn.FlatAppearance.BorderSize = 1;
            btn.Font = FontButton;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        /// <summary>
        /// Estiliza un TextBox con tipografía Segoe UI.
        /// </summary>
        public static void EstilizarTextBox(TextBox txt)
        {
            if (txt == null) return;
            txt.Font = FontBodyRegular;
            txt.ForeColor = TextMain;
        }

        /// <summary>
        /// Estiliza un ComboBox con tipografía Segoe UI.
        /// </summary>
        public static void EstilizarComboBox(ComboBox cmb)
        {
            if (cmb == null) return;
            cmb.Font = FontBodyRegular;
            cmb.ForeColor = TextMain;
        }

        /// <summary>
        /// Estiliza un GroupBox con tipografía nítida y color de borde suave.
        /// </summary>
        public static void EstilizarGroupBox(GroupBox grp)
        {
            if (grp == null) return;
            grp.Font = FontBodyBold;
            grp.ForeColor = TextDarkGray;
        }

        #endregion
    }
}
