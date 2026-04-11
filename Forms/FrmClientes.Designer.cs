namespace MiniSistemaFacturacion.Forms
{
    partial class FrmClientes
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
            this.dgvClientes = new System.Windows.Forms.DataGridView();
            this.btnNuevoCli = new System.Windows.Forms.Button();
            this.btnEditarCli = new System.Windows.Forms.Button();
            this.btnCambiarEstado = new System.Windows.Forms.Button();
            this.txtBuscarCli = new System.Windows.Forms.TextBox();
            this.txtIdCli = new System.Windows.Forms.TextBox();
            this.txtNombreCli = new System.Windows.Forms.TextBox();
            this.txtCedulaCli = new System.Windows.Forms.TextBox();
            this.txtDireccionCli = new System.Windows.Forms.TextBox();
            this.txtTelefonoCli = new System.Windows.Forms.TextBox();
            this.txtCorreoCli = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnLimpiar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvClientes
            // 
            this.dgvClientes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClientes.Location = new System.Drawing.Point(11, 78);
            this.dgvClientes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dgvClientes.Name = "dgvClientes";
            this.dgvClientes.ReadOnly = true;
            this.dgvClientes.RowHeadersWidth = 51;
            this.dgvClientes.Size = new System.Drawing.Size(1164, 402);
            this.dgvClientes.TabIndex = 0;
            this.dgvClientes.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClientes_DoubleClick);
            // 
            // btnNuevoCli
            // 
            this.btnNuevoCli.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoCli.Location = new System.Drawing.Point(616, 487);
            this.btnNuevoCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNuevoCli.Name = "btnNuevoCli";
            this.btnNuevoCli.Size = new System.Drawing.Size(180, 28);
            this.btnNuevoCli.TabIndex = 1;
            this.btnNuevoCli.Text = "Nuevo Cliente";
            this.btnNuevoCli.UseVisualStyleBackColor = true;
            this.btnNuevoCli.Click += new System.EventHandler(this.btnNuevoCli_Click);
            // 
            // btnEditarCli
            // 
            this.btnEditarCli.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditarCli.Location = new System.Drawing.Point(804, 487);
            this.btnEditarCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnEditarCli.Name = "btnEditarCli";
            this.btnEditarCli.Size = new System.Drawing.Size(180, 28);
            this.btnEditarCli.TabIndex = 2;
            this.btnEditarCli.Text = "Editar Cliente";
            this.btnEditarCli.UseVisualStyleBackColor = true;
            this.btnEditarCli.Click += new System.EventHandler(this.btnEditarCli_Click);
            // 
            // btnCambiarEstado
            // 
            this.btnCambiarEstado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarEstado.Location = new System.Drawing.Point(995, 487);
            this.btnCambiarEstado.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCambiarEstado.Name = "btnCambiarEstado";
            this.btnCambiarEstado.Size = new System.Drawing.Size(180, 28);
            this.btnCambiarEstado.TabIndex = 3;
            this.btnCambiarEstado.Text = "Cambiar estado";
            this.btnCambiarEstado.UseVisualStyleBackColor = true;
            this.btnCambiarEstado.Click += new System.EventHandler(this.btnCambiarEstado_Click);
            // 
            // txtBuscarCli
            // 
            this.txtBuscarCli.Location = new System.Drawing.Point(11, 490);
            this.txtBuscarCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtBuscarCli.Name = "txtBuscarCli";
            this.txtBuscarCli.Size = new System.Drawing.Size(596, 22);
            this.txtBuscarCli.TabIndex = 4;
            this.txtBuscarCli.TextChanged += new System.EventHandler(this.txtBuscarCli_TextChanged);
            // 
            // txtIdCli
            // 
            this.txtIdCli.Location = new System.Drawing.Point(11, 46);
            this.txtIdCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIdCli.Name = "txtIdCli";
            this.txtIdCli.ReadOnly = true;
            this.txtIdCli.Size = new System.Drawing.Size(47, 22);
            this.txtIdCli.TabIndex = 5;
            // 
            // txtNombreCli
            // 
            this.txtNombreCli.Location = new System.Drawing.Point(67, 46);
            this.txtNombreCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNombreCli.Name = "txtNombreCli";
            this.txtNombreCli.Size = new System.Drawing.Size(235, 22);
            this.txtNombreCli.TabIndex = 6;
            // 
            // txtCedulaCli
            // 
            this.txtCedulaCli.Location = new System.Drawing.Point(311, 46);
            this.txtCedulaCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCedulaCli.Name = "txtCedulaCli";
            this.txtCedulaCli.Size = new System.Drawing.Size(99, 22);
            this.txtCedulaCli.TabIndex = 7;
            // 
            // txtDireccionCli
            // 
            this.txtDireccionCli.Location = new System.Drawing.Point(419, 46);
            this.txtDireccionCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtDireccionCli.Name = "txtDireccionCli";
            this.txtDireccionCli.Size = new System.Drawing.Size(325, 22);
            this.txtDireccionCli.TabIndex = 8;
            // 
            // txtTelefonoCli
            // 
            this.txtTelefonoCli.Location = new System.Drawing.Point(753, 46);
            this.txtTelefonoCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtTelefonoCli.Name = "txtTelefonoCli";
            this.txtTelefonoCli.Size = new System.Drawing.Size(135, 22);
            this.txtTelefonoCli.TabIndex = 9;
            // 
            // txtCorreoCli
            // 
            this.txtCorreoCli.Location = new System.Drawing.Point(897, 46);
            this.txtCorreoCli.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCorreoCli.Name = "txtCorreoCli";
            this.txtCorreoCli.Size = new System.Drawing.Size(181, 22);
            this.txtCorreoCli.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(161, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "Nombre";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(337, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Cedula";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(539, 26);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 17);
            this.label4.TabIndex = 14;
            this.label4.Text = "Direccion";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(791, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 17);
            this.label5.TabIndex = 15;
            this.label5.Text = "Telefono";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(957, 26);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 17);
            this.label6.TabIndex = 16;
            this.label6.Text = "Email";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(269, 518);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(113, 17);
            this.label7.TabIndex = 17;
            this.label7.Text = "Buscar Cliente";
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Location = new System.Drawing.Point(1088, 26);
            this.btnLimpiar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(87, 44);
            this.btnLimpiar.TabIndex = 18;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // FrmClientes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1191, 539);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCorreoCli);
            this.Controls.Add(this.txtTelefonoCli);
            this.Controls.Add(this.txtDireccionCli);
            this.Controls.Add(this.txtCedulaCli);
            this.Controls.Add(this.txtNombreCli);
            this.Controls.Add(this.txtIdCli);
            this.Controls.Add(this.txtBuscarCli);
            this.Controls.Add(this.btnCambiarEstado);
            this.Controls.Add(this.btnEditarCli);
            this.Controls.Add(this.btnNuevoCli);
            this.Controls.Add(this.dgvClientes);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FrmClientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Clientes";
            this.Load += new System.EventHandler(this.FrmClientes_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClientes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvClientes;
        private System.Windows.Forms.Button btnNuevoCli;
        private System.Windows.Forms.Button btnEditarCli;
        private System.Windows.Forms.Button btnCambiarEstado;
        private System.Windows.Forms.TextBox txtBuscarCli;
        private System.Windows.Forms.TextBox txtIdCli;
        private System.Windows.Forms.TextBox txtNombreCli;
        private System.Windows.Forms.TextBox txtCedulaCli;
        private System.Windows.Forms.TextBox txtDireccionCli;
        private System.Windows.Forms.TextBox txtTelefonoCli;
        private System.Windows.Forms.TextBox txtCorreoCli;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnLimpiar;
    }
}