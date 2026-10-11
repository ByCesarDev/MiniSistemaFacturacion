namespace MiniSistemaFacturacion.Forms
{
    partial class FrmCambiarPassword
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblPasswordActual = new System.Windows.Forms.Label();
            this.txtPasswordActual = new System.Windows.Forms.TextBox();
            this.lblPasswordNueva = new System.Windows.Forms.Label();
            this.txtPasswordNueva = new System.Windows.Forms.TextBox();
            this.lblPasswordConfirmar = new System.Windows.Forms.Label();
            this.txtPasswordConfirmar = new System.Windows.Forms.TextBox();
            this.chkMostrar = new System.Windows.Forms.CheckBox();
            this.lblError = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.panelHeader.SuspendLayout();
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
            this.panelHeader.Size = new System.Drawing.Size(464, 65);
            this.panelHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(215)))), ((int)(((byte)(235)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(18, 38);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(277, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Actualización segura de credenciales de acceso";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(16, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(188, 21);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cambio de Contraseña";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.lblUsuario.Location = new System.Drawing.Point(22, 78);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(89, 15);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario activo:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(240)))), ((int)(((byte)(245)))));
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(22, 98);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.ReadOnly = true;
            this.txtUsuario.Size = new System.Drawing.Size(420, 24);
            this.txtUsuario.TabIndex = 2;
            // 
            // lblPasswordActual
            // 
            this.lblPasswordActual.AutoSize = true;
            this.lblPasswordActual.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPasswordActual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.lblPasswordActual.Location = new System.Drawing.Point(22, 134);
            this.lblPasswordActual.Name = "lblPasswordActual";
            this.lblPasswordActual.Size = new System.Drawing.Size(109, 15);
            this.lblPasswordActual.TabIndex = 3;
            this.lblPasswordActual.Text = "Contraseña Actual:";
            // 
            // txtPasswordActual
            // 
            this.txtPasswordActual.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPasswordActual.Location = new System.Drawing.Point(22, 154);
            this.txtPasswordActual.MaxLength = 50;
            this.txtPasswordActual.Name = "txtPasswordActual";
            this.txtPasswordActual.Size = new System.Drawing.Size(420, 24);
            this.txtPasswordActual.TabIndex = 4;
            this.txtPasswordActual.UseSystemPasswordChar = true;
            // 
            // lblPasswordNueva
            // 
            this.lblPasswordNueva.AutoSize = true;
            this.lblPasswordNueva.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPasswordNueva.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.lblPasswordNueva.Location = new System.Drawing.Point(22, 190);
            this.lblPasswordNueva.Name = "lblPasswordNueva";
            this.lblPasswordNueva.Size = new System.Drawing.Size(227, 15);
            this.lblPasswordNueva.TabIndex = 5;
            this.lblPasswordNueva.Text = "Nueva Contraseña (mín. 6 caracteres):";
            // 
            // txtPasswordNueva
            // 
            this.txtPasswordNueva.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPasswordNueva.Location = new System.Drawing.Point(22, 210);
            this.txtPasswordNueva.MaxLength = 50;
            this.txtPasswordNueva.Name = "txtPasswordNueva";
            this.txtPasswordNueva.Size = new System.Drawing.Size(420, 24);
            this.txtPasswordNueva.TabIndex = 6;
            this.txtPasswordNueva.UseSystemPasswordChar = true;
            // 
            // lblPasswordConfirmar
            // 
            this.lblPasswordConfirmar.AutoSize = true;
            this.lblPasswordConfirmar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPasswordConfirmar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.lblPasswordConfirmar.Location = new System.Drawing.Point(22, 246);
            this.lblPasswordConfirmar.Name = "lblPasswordConfirmar";
            this.lblPasswordConfirmar.Size = new System.Drawing.Size(176, 15);
            this.lblPasswordConfirmar.TabIndex = 7;
            this.lblPasswordConfirmar.Text = "Confirmar Nueva Contraseña:";
            // 
            // txtPasswordConfirmar
            // 
            this.txtPasswordConfirmar.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPasswordConfirmar.Location = new System.Drawing.Point(22, 266);
            this.txtPasswordConfirmar.MaxLength = 50;
            this.txtPasswordConfirmar.Name = "txtPasswordConfirmar";
            this.txtPasswordConfirmar.Size = new System.Drawing.Size(420, 24);
            this.txtPasswordConfirmar.TabIndex = 8;
            this.txtPasswordConfirmar.UseSystemPasswordChar = true;
            // 
            // chkMostrar
            // 
            this.chkMostrar.AutoSize = true;
            this.chkMostrar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMostrar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.chkMostrar.Location = new System.Drawing.Point(22, 302);
            this.chkMostrar.Name = "chkMostrar";
            this.chkMostrar.Size = new System.Drawing.Size(133, 19);
            this.chkMostrar.TabIndex = 9;
            this.chkMostrar.Text = "Mostrar contraseñas";
            this.chkMostrar.UseVisualStyleBackColor = true;
            this.chkMostrar.CheckedChanged += new System.EventHandler(this.chkMostrar_CheckedChanged);
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lblError.Location = new System.Drawing.Point(22, 326);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(420, 22);
            this.lblError.TabIndex = 10;
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblError.Visible = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(180, 356);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(160, 34);
            this.btnGuardar.TabIndex = 11;
            this.btnGuardar.Text = "💾 Guardar Clave";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(242)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(60)))), ((int)(((byte)(75)))));
            this.btnCancelar.Location = new System.Drawing.Point(350, 356);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(92, 34);
            this.btnCancelar.TabIndex = 12;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FrmCambiarPassword
            // 
            this.AcceptButton = this.btnGuardar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.CancelButton = this.btnCancelar;
            this.ClientSize = new System.Drawing.Size(464, 406);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.chkMostrar);
            this.Controls.Add(this.txtPasswordConfirmar);
            this.Controls.Add(this.lblPasswordConfirmar);
            this.Controls.Add(this.txtPasswordNueva);
            this.Controls.Add(this.lblPasswordNueva);
            this.Controls.Add(this.txtPasswordActual);
            this.Controls.Add(this.lblPasswordActual);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.panelHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmCambiarPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cambiar Contraseña";
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPasswordActual;
        private System.Windows.Forms.TextBox txtPasswordActual;
        private System.Windows.Forms.Label lblPasswordNueva;
        private System.Windows.Forms.TextBox txtPasswordNueva;
        private System.Windows.Forms.Label lblPasswordConfirmar;
        private System.Windows.Forms.TextBox txtPasswordConfirmar;
        private System.Windows.Forms.CheckBox chkMostrar;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
