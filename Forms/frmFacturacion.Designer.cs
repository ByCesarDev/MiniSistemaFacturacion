namespace MiniSistemaFacturacion.Forms
{
    partial class FrmFacturacion
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblNumeroFactura = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbProductos = new System.Windows.Forms.ComboBox();
            this.btnAnadir = new System.Windows.Forms.Button();
            this.btnEliminarItem = new System.Windows.Forms.Button();
            this.numCantidad = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSubtotal = new System.Windows.Forms.TextBox();
            this.txtIVA = new System.Windows.Forms.TextBox();
            this.txtTotalNeto = new System.Windows.Forms.TextBox();
            this.txtSaldoPendiente = new System.Windows.Forms.TextBox();
            this.btnGuardarFactura = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.chkEnviarEmail = new System.Windows.Forms.CheckBox();
            this.chkImprimirDirecto = new System.Windows.Forms.CheckBox();
            this.cmbClientes = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Producto:";
            // 
            // lblNumeroFactura
            // 
            this.lblNumeroFactura.AutoSize = true;
            this.lblNumeroFactura.Location = new System.Drawing.Point(97, 30);
            this.lblNumeroFactura.Name = "lblNumeroFactura";
            this.lblNumeroFactura.Size = new System.Drawing.Size(0, 18);
            this.lblNumeroFactura.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(353, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fecha:";
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Location = new System.Drawing.Point(11, 141);
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.RowHeadersWidth = 62;
            this.dgvDetalle.RowTemplate.Height = 28;
            this.dgvDetalle.Size = new System.Drawing.Size(638, 260);
            this.dgvDetalle.TabIndex = 4;
            this.dgvDetalle.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetalle_CellValueChanged);
            this.dgvDetalle.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvDetalle_CurrentCellDirtyStateChanged);
            // 
            // dtpFecha
            // 
            this.dtpFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFecha.Location = new System.Drawing.Point(409, 14);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(240, 24);
            this.dtpFecha.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "Nombre del Cliente:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "Cantidad:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "No. Factura:";
            // 
            // cmbProductos
            // 
            this.cmbProductos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductos.FormattingEnabled = true;
            this.cmbProductos.Location = new System.Drawing.Point(86, 80);
            this.cmbProductos.Name = "cmbProductos";
            this.cmbProductos.Size = new System.Drawing.Size(373, 26);
            this.cmbProductos.TabIndex = 11;
            // 
            // btnAnadir
            // 
            this.btnAnadir.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnadir.Location = new System.Drawing.Point(461, 109);
            this.btnAnadir.Name = "btnAnadir";
            this.btnAnadir.Size = new System.Drawing.Size(91, 26);
            this.btnAnadir.TabIndex = 12;
            this.btnAnadir.Text = "Añadir";
            this.btnAnadir.UseVisualStyleBackColor = true;
            this.btnAnadir.Click += new System.EventHandler(this.btnAnadir_Click_1);
            // 
            // btnEliminarItem
            // 
            this.btnEliminarItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminarItem.Location = new System.Drawing.Point(558, 109);
            this.btnEliminarItem.Name = "btnEliminarItem";
            this.btnEliminarItem.Size = new System.Drawing.Size(91, 26);
            this.btnEliminarItem.TabIndex = 13;
            this.btnEliminarItem.Text = "Eliminar";
            this.btnEliminarItem.UseVisualStyleBackColor = true;
            this.btnEliminarItem.Click += new System.EventHandler(this.btnEliminarItem_Click_1);
            // 
            // numCantidad
            // 
            this.numCantidad.Location = new System.Drawing.Point(86, 109);
            this.numCantidad.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCantidad.Name = "numCantidad";
            this.numCantidad.Size = new System.Drawing.Size(120, 24);
            this.numCantidad.TabIndex = 14;
            this.numCantidad.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(22, 513);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(75, 18);
            this.label6.TabIndex = 15;
            this.label6.Text = "Subtotal:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(114, 513);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 18);
            this.label7.TabIndex = 16;
            this.label7.Text = "ITBIS (18%):";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(222, 513);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 18);
            this.label8.TabIndex = 17;
            this.label8.Text = "Total Neto:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(325, 513);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(135, 18);
            this.label9.TabIndex = 18;
            this.label9.Text = "Saldo Pendiente:";
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Location = new System.Drawing.Point(8, 489);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.ReadOnly = true;
            this.txtSubtotal.Size = new System.Drawing.Size(86, 24);
            this.txtSubtotal.TabIndex = 19;
            // 
            // txtIVA
            // 
            this.txtIVA.Location = new System.Drawing.Point(102, 489);
            this.txtIVA.Name = "txtIVA";
            this.txtIVA.ReadOnly = true;
            this.txtIVA.Size = new System.Drawing.Size(100, 24);
            this.txtIVA.TabIndex = 20;
            // 
            // txtTotalNeto
            // 
            this.txtTotalNeto.Location = new System.Drawing.Point(208, 489);
            this.txtTotalNeto.Name = "txtTotalNeto";
            this.txtTotalNeto.ReadOnly = true;
            this.txtTotalNeto.Size = new System.Drawing.Size(100, 24);
            this.txtTotalNeto.TabIndex = 21;
            // 
            // txtSaldoPendiente
            // 
            this.txtSaldoPendiente.Location = new System.Drawing.Point(314, 489);
            this.txtSaldoPendiente.Name = "txtSaldoPendiente";
            this.txtSaldoPendiente.ReadOnly = true;
            this.txtSaldoPendiente.Size = new System.Drawing.Size(128, 24);
            this.txtSaldoPendiente.TabIndex = 22;
            // 
            // btnGuardarFactura
            // 
            this.btnGuardarFactura.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarFactura.Location = new System.Drawing.Point(463, 506);
            this.btnGuardarFactura.Name = "btnGuardarFactura";
            this.btnGuardarFactura.Size = new System.Drawing.Size(91, 25);
            this.btnGuardarFactura.TabIndex = 23;
            this.btnGuardarFactura.Text = "Guardar";
            this.btnGuardarFactura.UseVisualStyleBackColor = true;
            this.btnGuardarFactura.Click += new System.EventHandler(this.btnGuardarFactura_Click_1);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.Location = new System.Drawing.Point(560, 506);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(91, 25);
            this.btnCancelar.TabIndex = 24;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // chkEnviarEmail
            // 
            this.chkEnviarEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEnviarEmail.Location = new System.Drawing.Point(8, 422);
            this.chkEnviarEmail.Name = "chkEnviarEmail";
            this.chkEnviarEmail.Size = new System.Drawing.Size(104, 24);
            this.chkEnviarEmail.TabIndex = 1;
            this.chkEnviarEmail.Text = "Enviar";
            // 
            // chkImprimirDirecto
            // 
            this.chkImprimirDirecto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkImprimirDirecto.Location = new System.Drawing.Point(100, 422);
            this.chkImprimirDirecto.Name = "chkImprimirDirecto";
            this.chkImprimirDirecto.Size = new System.Drawing.Size(104, 24);
            this.chkImprimirDirecto.TabIndex = 0;
            this.chkImprimirDirecto.Text = "Imprimir";
            // 
            // cmbClientes
            // 
            this.cmbClientes.Location = new System.Drawing.Point(174, 51);
            this.cmbClientes.Name = "cmbClientes";
            this.cmbClientes.Size = new System.Drawing.Size(285, 26);
            this.cmbClientes.TabIndex = 4;
            // 
            // FrmFacturacion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 543);
            this.Controls.Add(this.chkImprimirDirecto);
            this.Controls.Add(this.chkEnviarEmail);
            this.Controls.Add(this.cmbClientes);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardarFactura);
            this.Controls.Add(this.txtSaldoPendiente);
            this.Controls.Add(this.txtTotalNeto);
            this.Controls.Add(this.txtIVA);
            this.Controls.Add(this.txtSubtotal);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.numCantidad);
            this.Controls.Add(this.btnEliminarItem);
            this.Controls.Add(this.btnAnadir);
            this.Controls.Add(this.cmbProductos);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblNumeroFactura);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmFacturacion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Facturación";
            this.Load += new System.EventHandler(this.FrmFacturacion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCantidad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNumeroFactura;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbProductos;
        private System.Windows.Forms.Button btnAnadir;
        private System.Windows.Forms.Button btnEliminarItem;
        private System.Windows.Forms.NumericUpDown numCantidad;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSubtotal;
        private System.Windows.Forms.TextBox txtIVA;
        private System.Windows.Forms.TextBox txtTotalNeto;
        private System.Windows.Forms.TextBox txtSaldoPendiente;
        private System.Windows.Forms.Button btnGuardarFactura;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.CheckBox chkEnviarEmail;
        private System.Windows.Forms.CheckBox chkImprimirDirecto;
        private System.Windows.Forms.ComboBox cmbClientes;
    }
}