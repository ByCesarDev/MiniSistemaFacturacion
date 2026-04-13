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
            this.dashboardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.chartFacturasPanel = new System.Windows.Forms.Panel();
            this.chartProductosPanel = new System.Windows.Forms.Panel();
            this.panelInventario = new System.Windows.Forms.Panel();
            this.panelCuentasPorCobrar = new System.Windows.Forms.Panel();
            this.dgvClientesDeuda = new System.Windows.Forms.DataGridView();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.dashboardLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesDeuda)).BeginInit();
            this.SuspendLayout();
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
            this.dashboardLayout.Location = new System.Drawing.Point(12, 12);
            this.dashboardLayout.Name = "dashboardLayout";
            this.dashboardLayout.Padding = new System.Windows.Forms.Padding(10);
            this.dashboardLayout.RowCount = 2;
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dashboardLayout.Size = new System.Drawing.Size(1160, 550);
            this.dashboardLayout.TabIndex = 0;
            // 
            // chartFacturasPanel
            // 
            this.chartFacturasPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chartFacturasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartFacturasPanel.Location = new System.Drawing.Point(13, 13);
            this.chartFacturasPanel.Name = "chartFacturasPanel";
            this.chartFacturasPanel.Size = new System.Drawing.Size(678, 259);
            this.chartFacturasPanel.TabIndex = 0;
            // 
            // chartProductosPanel
            // 
            this.chartProductosPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.chartProductosPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartProductosPanel.Location = new System.Drawing.Point(13, 278);
            this.chartProductosPanel.Name = "chartProductosPanel";
            this.chartProductosPanel.Size = new System.Drawing.Size(678, 259);
            this.chartProductosPanel.TabIndex = 1;
            // 
            // panelInventario
            // 
            this.panelInventario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.panelInventario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelInventario.Location = new System.Drawing.Point(697, 13);
            this.panelInventario.Name = "panelInventario";
            this.panelInventario.Padding = new System.Windows.Forms.Padding(10);
            this.panelInventario.Size = new System.Drawing.Size(450, 259);
            this.panelInventario.TabIndex = 2;
            // 
            // panelCuentasPorCobrar
            // 
            this.panelCuentasPorCobrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.panelCuentasPorCobrar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCuentasPorCobrar.Location = new System.Drawing.Point(697, 278);
            this.panelCuentasPorCobrar.Name = "panelCuentasPorCobrar";
            this.panelCuentasPorCobrar.Padding = new System.Windows.Forms.Padding(10);
            this.panelCuentasPorCobrar.Size = new System.Drawing.Size(450, 259);
            this.panelCuentasPorCobrar.TabIndex = 3;
            // 
            // dgvClientesDeuda
            // 
            this.dgvClientesDeuda.AllowUserToAddRows = false;
            this.dgvClientesDeuda.AllowUserToDeleteRows = false;
            this.dgvClientesDeuda.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvClientesDeuda.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClientesDeuda.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientesDeuda.Location = new System.Drawing.Point(12, 568);
            this.dgvClientesDeuda.Name = "dgvClientesDeuda";
            this.dgvClientesDeuda.ReadOnly = true;
            this.dgvClientesDeuda.RowHeadersVisible = false;
            this.dgvClientesDeuda.RowHeadersWidth = 51;
            this.dgvClientesDeuda.RowTemplate.Height = 24;
            this.dgvClientesDeuda.Size = new System.Drawing.Size(1160, 150);
            this.dgvClientesDeuda.TabIndex = 4;
            // 
            // btnCerrar
            // 
            this.btnCerrar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCerrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrar.Location = new System.Drawing.Point(1097, 724);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 30);
            this.btnCerrar.TabIndex = 5;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // FrmReportes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 766);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.dgvClientesDeuda);
            this.Controls.Add(this.dashboardLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmReportes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reportes y Estadísticas";
            this.Load += new System.EventHandler(this.FrmReportes_Load);
            this.dashboardLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientesDeuda)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel dashboardLayout;
        private System.Windows.Forms.Panel chartFacturasPanel;
        private System.Windows.Forms.Panel chartProductosPanel;
        private System.Windows.Forms.Panel panelInventario;
        private System.Windows.Forms.Panel panelCuentasPorCobrar;
        private System.Windows.Forms.DataGridView dgvClientesDeuda;
        private System.Windows.Forms.Button btnCerrar;
    }
}
