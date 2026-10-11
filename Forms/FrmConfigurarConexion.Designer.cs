namespace MiniSistemaFacturacion.Forms
{
    partial class FrmConfigurarConexion
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlContenido = new System.Windows.Forms.Panel();
            this.pnlEstado = new System.Windows.Forms.Panel();
            this.lblEstadoMensaje = new System.Windows.Forms.Label();
            this.lblEstadoIcono = new System.Windows.Forms.Label();
            this.gbAutenticacion = new System.Windows.Forms.GroupBox();
            this.chkProtegerDPAPI = new System.Windows.Forms.CheckBox();
            this.pnlCredencialesSql = new System.Windows.Forms.Panel();
            this.btnVerPassword = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.rbSqlAuth = new System.Windows.Forms.RadioButton();
            this.rbWindowsAuth = new System.Windows.Forms.RadioButton();
            this.txtBaseDatos = new System.Windows.Forms.TextBox();
            this.lblBaseDatos = new System.Windows.Forms.Label();
            this.txtServidor = new System.Windows.Forms.TextBox();
            this.lblServidor = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnInicializarBD = new System.Windows.Forms.Button();
            this.btnProbarConexion = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlContenido.SuspendLayout();
            this.pnlEstado.SuspendLayout();
            this.gbAutenticacion.SuspendLayout();
            this.pnlCredencialesSql.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.pnlHeader.Controls.Add(this.lblSubtitulo);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(584, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(22, 45);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(462, 15);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Configuración segura de SQL Server Express, credenciales DPAPI y control de versiones";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 16);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(434, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "⚙️ Asistente de Conexión a Base de Datos";
            // 
            // pnlContenido
            // 
            this.pnlContenido.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlContenido.Controls.Add(this.pnlEstado);
            this.pnlContenido.Controls.Add(this.gbAutenticacion);
            this.pnlContenido.Controls.Add(this.txtBaseDatos);
            this.pnlContenido.Controls.Add(this.lblBaseDatos);
            this.pnlContenido.Controls.Add(this.txtServidor);
            this.pnlContenido.Controls.Add(this.lblServidor);
            this.pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenido.Location = new System.Drawing.Point(0, 80);
            this.pnlContenido.Name = "pnlContenido";
            this.pnlContenido.Padding = new System.Windows.Forms.Padding(25, 15, 25, 15);
            this.pnlContenido.Size = new System.Drawing.Size(584, 465);
            this.pnlContenido.TabIndex = 1;
            // 
            // pnlEstado
            // 
            this.pnlEstado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlEstado.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEstado.Controls.Add(this.lblEstadoMensaje);
            this.pnlEstado.Controls.Add(this.lblEstadoIcono);
            this.pnlEstado.Location = new System.Drawing.Point(25, 375);
            this.pnlEstado.Name = "pnlEstado";
            this.pnlEstado.Padding = new System.Windows.Forms.Padding(10);
            this.pnlEstado.Size = new System.Drawing.Size(534, 75);
            this.pnlEstado.TabIndex = 5;
            // 
            // lblEstadoMensaje
            // 
            this.lblEstadoMensaje.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoMensaje.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.lblEstadoMensaje.Location = new System.Drawing.Point(40, 8);
            this.lblEstadoMensaje.Name = "lblEstadoMensaje";
            this.lblEstadoMensaje.Size = new System.Drawing.Size(480, 56);
            this.lblEstadoMensaje.TabIndex = 1;
            this.lblEstadoMensaje.Text = "Haga clic en \"Probar Conexión\" para comprobar el estado del servidor y las migraciones.";
            this.lblEstadoMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblEstadoIcono
            // 
            this.lblEstadoIcono.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstadoIcono.Location = new System.Drawing.Point(6, 18);
            this.lblEstadoIcono.Name = "lblEstadoIcono";
            this.lblEstadoIcono.Size = new System.Drawing.Size(30, 32);
            this.lblEstadoIcono.TabIndex = 0;
            this.lblEstadoIcono.Text = "ℹ️";
            this.lblEstadoIcono.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbAutenticacion
            // 
            this.gbAutenticacion.Controls.Add(this.chkProtegerDPAPI);
            this.gbAutenticacion.Controls.Add(this.pnlCredencialesSql);
            this.gbAutenticacion.Controls.Add(this.rbSqlAuth);
            this.gbAutenticacion.Controls.Add(this.rbWindowsAuth);
            this.gbAutenticacion.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbAutenticacion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.gbAutenticacion.Location = new System.Drawing.Point(25, 140);
            this.gbAutenticacion.Name = "gbAutenticacion";
            this.gbAutenticacion.Size = new System.Drawing.Size(534, 225);
            this.gbAutenticacion.TabIndex = 4;
            this.gbAutenticacion.TabStop = false;
            this.gbAutenticacion.Text = "Autenticación de SQL Server";
            // 
            // chkProtegerDPAPI
            // 
            this.chkProtegerDPAPI.AutoSize = true;
            this.chkProtegerDPAPI.Checked = true;
            this.chkProtegerDPAPI.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkProtegerDPAPI.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkProtegerDPAPI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.chkProtegerDPAPI.Location = new System.Drawing.Point(22, 195);
            this.chkProtegerDPAPI.Name = "chkProtegerDPAPI";
            this.chkProtegerDPAPI.Size = new System.Drawing.Size(432, 19);
            this.chkProtegerDPAPI.TabIndex = 3;
            this.chkProtegerDPAPI.Text = "Cifrar la cadena de conexión con DPAPI de Windows en App.config (Recomendado)";
            this.chkProtegerDPAPI.UseVisualStyleBackColor = true;
            // 
            // pnlCredencialesSql
            // 
            this.pnlCredencialesSql.BackColor = System.Drawing.Color.White;
            this.pnlCredencialesSql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCredencialesSql.Controls.Add(this.btnVerPassword);
            this.pnlCredencialesSql.Controls.Add(this.txtPassword);
            this.pnlCredencialesSql.Controls.Add(this.lblPassword);
            this.pnlCredencialesSql.Controls.Add(this.txtUsuario);
            this.pnlCredencialesSql.Controls.Add(this.lblUsuario);
            this.pnlCredencialesSql.Enabled = false;
            this.pnlCredencialesSql.Location = new System.Drawing.Point(22, 85);
            this.pnlCredencialesSql.Name = "pnlCredencialesSql";
            this.pnlCredencialesSql.Size = new System.Drawing.Size(490, 100);
            this.pnlCredencialesSql.TabIndex = 2;
            // 
            // btnVerPassword
            // 
            this.btnVerPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.btnVerPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerPassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.btnVerPassword.Location = new System.Drawing.Point(445, 57);
            this.btnVerPassword.Name = "btnVerPassword";
            this.btnVerPassword.Size = new System.Drawing.Size(32, 26);
            this.btnVerPassword.TabIndex = 4;
            this.btnVerPassword.Text = "👁";
            this.btnVerPassword.UseVisualStyleBackColor = false;
            this.btnVerPassword.Click += new System.EventHandler(this.btnVerPassword_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(120, 58);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(320, 24);
            this.txtPassword.TabIndex = 3;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblPassword.Location = new System.Drawing.Point(15, 62);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(70, 15);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Contraseña:";
            // 
            // txtUsuario
            // 
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUsuario.Location = new System.Drawing.Point(120, 18);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(357, 24);
            this.txtUsuario.TabIndex = 1;
            this.txtUsuario.Text = "sa";
            // 
            // lblUsuario
            // 
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUsuario.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.lblUsuario.Location = new System.Drawing.Point(15, 22);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(72, 15);
            this.lblUsuario.TabIndex = 0;
            this.lblUsuario.Text = "Usuario SQL:";
            // 
            // rbSqlAuth
            // 
            this.rbSqlAuth.AutoSize = true;
            this.rbSqlAuth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSqlAuth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.rbSqlAuth.Location = new System.Drawing.Point(22, 55);
            this.rbSqlAuth.Name = "rbSqlAuth";
            this.rbSqlAuth.Size = new System.Drawing.Size(271, 19);
            this.rbSqlAuth.TabIndex = 1;
            this.rbSqlAuth.Text = "Autenticación de SQL Server (Usuario y Clave)";
            this.rbSqlAuth.UseVisualStyleBackColor = true;
            this.rbSqlAuth.CheckedChanged += new System.EventHandler(this.rbAuth_CheckedChanged);
            // 
            // rbWindowsAuth
            // 
            this.rbWindowsAuth.AutoSize = true;
            this.rbWindowsAuth.Checked = true;
            this.rbWindowsAuth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbWindowsAuth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.rbWindowsAuth.Location = new System.Drawing.Point(22, 28);
            this.rbWindowsAuth.Name = "rbWindowsAuth";
            this.rbWindowsAuth.Size = new System.Drawing.Size(306, 19);
            this.rbWindowsAuth.TabIndex = 0;
            this.rbWindowsAuth.TabStop = true;
            this.rbWindowsAuth.Text = "Autenticación Integrada de Windows (Recomendada)";
            this.rbWindowsAuth.UseVisualStyleBackColor = true;
            this.rbWindowsAuth.CheckedChanged += new System.EventHandler(this.rbAuth_CheckedChanged);
            // 
            // txtBaseDatos
            // 
            this.txtBaseDatos.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBaseDatos.Location = new System.Drawing.Point(25, 100);
            this.txtBaseDatos.Name = "txtBaseDatos";
            this.txtBaseDatos.Size = new System.Drawing.Size(534, 24);
            this.txtBaseDatos.TabIndex = 3;
            this.txtBaseDatos.Text = "MiniSistemaFacturacion";
            // 
            // lblBaseDatos
            // 
            this.lblBaseDatos.AutoSize = true;
            this.lblBaseDatos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBaseDatos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.lblBaseDatos.Location = new System.Drawing.Point(22, 80);
            this.lblBaseDatos.Name = "lblBaseDatos";
            this.lblBaseDatos.Size = new System.Drawing.Size(130, 15);
            this.lblBaseDatos.TabIndex = 2;
            this.lblBaseDatos.Text = "Nombre Base de Datos:";
            // 
            // txtServidor
            // 
            this.txtServidor.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServidor.Location = new System.Drawing.Point(25, 42);
            this.txtServidor.Name = "txtServidor";
            this.txtServidor.Size = new System.Drawing.Size(534, 24);
            this.txtServidor.TabIndex = 1;
            this.txtServidor.Text = "localhost\\SQLEXPRESS";
            // 
            // lblServidor
            // 
            this.lblServidor.AutoSize = true;
            this.lblServidor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServidor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(57)))), ((int)(((byte)(89)))));
            this.lblServidor.Location = new System.Drawing.Point(22, 22);
            this.lblServidor.Name = "lblServidor";
            this.lblServidor.Size = new System.Drawing.Size(149, 15);
            this.lblServidor.TabIndex = 0;
            this.lblServidor.Text = "Servidor SQL / Instancia:";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlFooter.Controls.Add(this.btnInicializarBD);
            this.pnlFooter.Controls.Add(this.btnProbarConexion);
            this.pnlFooter.Controls.Add(this.btnGuardar);
            this.pnlFooter.Controls.Add(this.btnSalir);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 545);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
            this.pnlFooter.Size = new System.Drawing.Size(584, 65);
            this.pnlFooter.TabIndex = 2;
            // 
            // btnInicializarBD
            // 
            this.btnInicializarBD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.btnInicializarBD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInicializarBD.Enabled = false;
            this.btnInicializarBD.FlatAppearance.BorderSize = 0;
            this.btnInicializarBD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInicializarBD.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInicializarBD.ForeColor = System.Drawing.Color.White;
            this.btnInicializarBD.Location = new System.Drawing.Point(155, 15);
            this.btnInicializarBD.Name = "btnInicializarBD";
            this.btnInicializarBD.Size = new System.Drawing.Size(155, 36);
            this.btnInicializarBD.TabIndex = 1;
            this.btnInicializarBD.Text = "⚡ Inicializar / Actualizar";
            this.btnInicializarBD.UseVisualStyleBackColor = false;
            this.btnInicializarBD.Click += new System.EventHandler(this.btnInicializarBD_Click);
            // 
            // btnProbarConexion
            // 
            this.btnProbarConexion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this.btnProbarConexion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProbarConexion.FlatAppearance.BorderSize = 0;
            this.btnProbarConexion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProbarConexion.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProbarConexion.ForeColor = System.Drawing.Color.White;
            this.btnProbarConexion.Location = new System.Drawing.Point(20, 15);
            this.btnProbarConexion.Name = "btnProbarConexion";
            this.btnProbarConexion.Size = new System.Drawing.Size(125, 36);
            this.btnProbarConexion.TabIndex = 0;
            this.btnProbarConexion.Text = "🔌 Probar Conexión";
            this.btnProbarConexion.UseVisualStyleBackColor = false;
            this.btnProbarConexion.Click += new System.EventHandler(this.btnProbarConexion_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(163)))), ((int)(((byte)(74)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(340, 15);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 36);
            this.btnGuardar.TabIndex = 2;
            this.btnGuardar.Text = "💾 Guardar y Conectar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.White;
            this.btnSalir.Location = new System.Drawing.Point(480, 15);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(85, 36);
            this.btnSalir.TabIndex = 3;
            this.btnSalir.Text = "❌ Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // FrmConfigurarConexion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 610);
            this.Controls.Add(this.pnlContenido);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfigurarConexion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración de Conexión a Base de Datos — MiniSistemaFacturacion";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlContenido.ResumeLayout(false);
            this.pnlContenido.PerformLayout();
            this.pnlEstado.ResumeLayout(false);
            this.gbAutenticacion.ResumeLayout(false);
            this.gbAutenticacion.PerformLayout();
            this.pnlCredencialesSql.ResumeLayout(false);
            this.pnlCredencialesSql.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Panel pnlContenido;
        private System.Windows.Forms.Label lblServidor;
        private System.Windows.Forms.TextBox txtServidor;
        private System.Windows.Forms.Label lblBaseDatos;
        private System.Windows.Forms.TextBox txtBaseDatos;
        private System.Windows.Forms.GroupBox gbAutenticacion;
        private System.Windows.Forms.RadioButton rbWindowsAuth;
        private System.Windows.Forms.RadioButton rbSqlAuth;
        private System.Windows.Forms.Panel pnlCredencialesSql;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnVerPassword;
        private System.Windows.Forms.CheckBox chkProtegerDPAPI;
        private System.Windows.Forms.Panel pnlEstado;
        private System.Windows.Forms.Label lblEstadoIcono;
        private System.Windows.Forms.Label lblEstadoMensaje;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnProbarConexion;
        private System.Windows.Forms.Button btnInicializarBD;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnSalir;
    }
}
