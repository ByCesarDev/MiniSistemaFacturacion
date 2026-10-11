namespace MiniSistemaFacturacion.Forms
{
    partial class FrmReportes
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReportes));
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.dashboardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.chartFacturasPanel = new System.Windows.Forms.Panel();
            this.chartProductosPanel = new System.Windows.Forms.Panel();
            this.panelInventario = new System.Windows.Forms.Panel();
            this.panelCuentasPorCobrar = new System.Windows.Forms.Panel();
            this.lblTituloDeuda = new System.Windows.Forms.Label();
            this.dgvClientesDeuda = new System.Windows.Forms.DataGridView();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
            this.dashboardLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesDeuda)).BeginInit();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.panelHeader.Controls.Add(this.lblSubtitulo);
            this.panelHeader.Controls.Add(this.lblTitulo);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1184, 68);
            this.panelHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(215)))), ((int)(((byte)(235)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(18, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(438, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Indicadores comerciales, tendencias de venta, catálogo top y cuentas por cobrar";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(16, 11);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(221, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Dashboard de Reportes";
            // 
            // dashboardLayout
            // 
            this.dashboardLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dashboardLayout.ColumnCount = 2;
            this.dashboardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.dashboardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.dashboardLayout.Controls.Add(this.chartFacturasPanel, 0, 0);
            this.dashboardLayout.Controls.Add(this.chartProductosPanel, 0, 1);
            this.dashboardLayout.Controls.Add(this.panelInventario, 1, 0);
            this.dashboardLayout.Controls.Add(this.panelCuentasPorCobrar, 1, 1);
            this.dashboardLayout.Location = new System.Drawing.Point(12, 78);
            this.dashboardLayout.Name = "dashboardLayout";
            this.dashboardLayout.Padding = new System.Windows.Forms.Padding(6);
            this.dashboardLayout.RowCount = 2;
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardLayout.Size = new System.Drawing.Size(1160, 480);
            this.dashboardLayout.TabIndex = 1;
            // 
            // chartFacturasPanel
            // 
            this.chartFacturasPanel.BackColor = System.Drawing.Color.White;
            this.chartFacturasPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chartFacturasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartFacturasPanel.Location = new System.Drawing.Point(9, 9);
            this.chartFacturasPanel.Name = "chartFacturasPanel";
            this.chartFacturasPanel.Size = new System.Drawing.Size(682, 228);
            this.chartFacturasPanel.TabIndex = 0;
            // 
            // chartProductosPanel
            // 
            this.chartProductosPanel.BackColor = System.Drawing.Color.White;
            this.chartProductosPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chartProductosPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartProductosPanel.Location = new System.Drawing.Point(9, 243);
            this.chartProductosPanel.Name = "chartProductosPanel";
            this.chartProductosPanel.Size = new System.Drawing.Size(682, 228);
            this.chartProductosPanel.TabIndex = 1;
            // 
            // panelInventario
            // 
            this.panelInventario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(170)))));
            this.panelInventario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInventario.Location = new System.Drawing.Point(697, 9);
            this.panelInventario.Name = "panelInventario";
            this.panelInventario.Padding = new System.Windows.Forms.Padding(12);
            this.panelInventario.Size = new System.Drawing.Size(454, 228);
            this.panelInventario.TabIndex = 2;
            // 
            // panelCuentasPorCobrar
            // 
            this.panelCuentasPorCobrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.panelCuentasPorCobrar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCuentasPorCobrar.Location = new System.Drawing.Point(697, 243);
            this.panelCuentasPorCobrar.Name = "panelCuentasPorCobrar";
            this.panelCuentasPorCobrar.Padding = new System.Windows.Forms.Padding(12);
            this.panelCuentasPorCobrar.Size = new System.Drawing.Size(454, 228);
            this.panelCuentasPorCobrar.TabIndex = 3;
            // 
            // lblTituloDeuda
            // 
            this.lblTituloDeuda.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTituloDeuda.AutoSize = true;
            this.lblTituloDeuda.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloDeuda.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblTituloDeuda.Location = new System.Drawing.Point(12, 566);
            this.lblTituloDeuda.Name = "lblTituloDeuda";
            this.lblTituloDeuda.Size = new System.Drawing.Size(189, 17);
            this.lblTituloDeuda.TabIndex = 2;
            this.lblTituloDeuda.Text = "Top Clientes con Saldo Deudor";
            // 
            // dgvClientesDeuda
            // 
            this.dgvClientesDeuda.AllowUserToAddRows = false;
            this.dgvClientesDeuda.AllowUserToDeleteRows = false;
            this.dgvClientesDeuda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClientesDeuda.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClientesDeuda.BackgroundColor = System.Drawing.Color.White;
            this.dgvClientesDeuda.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientesDeuda.Location = new System.Drawing.Point(12, 588);
            this.dgvClientesDeuda.Name = "dgvClientesDeuda";
            this.dgvClientesDeuda.ReadOnly = true;
            this.dgvClientesDeuda.RowHeadersVisible = false;
            this.dgvClientesDeuda.RowHeadersWidth = 51;
            this.dgvClientesDeuda.RowTemplate.Height = 26;
            this.dgvClientesDeuda.Size = new System.Drawing.Size(1160, 130);
            this.dgvClientesDeuda.TabIndex = 3;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.btnCerrar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCerrar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(212)))), ((int)(((byte)(220)))));
            this.btnCerrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCerrar.Location = new System.Drawing.Point(1072, 726);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(100, 32);
            this.btnCerrar.TabIndex = 4;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmReportes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1184, 766);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.dgvClientesDeuda);
            this.Controls.Add(this.lblTituloDeuda);
            this.Controls.Add(this.dashboardLayout);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmReportes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reportes y Estadísticas";
            this.Load += new System.EventHandler(this.FrmReportes_Load);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.dashboardLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesDeuda)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.TableLayoutPanel dashboardLayout;
        private System.Windows.Forms.Panel chartFacturasPanel;
        private System.Windows.Forms.Panel chartProductosPanel;
        private System.Windows.Forms.Panel panelInventario;
        private System.Windows.Forms.Panel panelCuentasPorCobrar;
        private System.Windows.Forms.Label lblTituloDeuda;
        private System.Windows.Forms.DataGridView dgvClientesDeuda;
        private System.Windows.Forms.Button btnCerrar;
    }
}
