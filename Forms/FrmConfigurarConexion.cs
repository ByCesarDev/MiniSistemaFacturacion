using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiniSistemaFacturacion.DataAccess;
using MiniSistemaFacturacion.Utils;

namespace MiniSistemaFacturacion.Forms
{
    public partial class FrmConfigurarConexion : Form
    {
        private bool _passwordVisible = false;
        private string _cadenaProbadaValida = null;

        public FrmConfigurarConexion()
        {
            InitializeComponent();
            UITheme.AsignarIconoFormulario(this);
            btnGuardar.Enabled = false;
            btnInicializarBD.Enabled = false;
            CargarConfiguracionActual();
            SuscribirEventosCambioParametros();
        }

        private void SuscribirEventosCambioParametros()
        {
            txtServidor.TextChanged += OnParametrosModificados;
            txtBaseDatos.TextChanged += OnParametrosModificados;
            txtUsuario.TextChanged += OnParametrosModificados;
            txtPassword.TextChanged += OnParametrosModificados;
            rbWindowsAuth.CheckedChanged += OnParametrosModificados;
            rbSqlAuth.CheckedChanged += OnParametrosModificados;
        }

        private void OnParametrosModificados(object sender, EventArgs e)
        {
            _cadenaProbadaValida = null;
            btnGuardar.Enabled = false;
            btnInicializarBD.Enabled = false;
            lblEstadoIcono.Text = "⚠️";
            lblEstadoMensaje.Text = "Parámetros modificados. Debe probar la conexión antes de guardar o inicializar.";
            pnlEstado.BackColor = Color.FromArgb(254, 243, 199);
            lblEstadoMensaje.ForeColor = Color.FromArgb(146, 64, 14);
        }

        private void CargarConfiguracionActual()
        {
            try
            {
                string cadenaActual = ConnectionStringManager.GetConnectionString();
                if (!string.IsNullOrEmpty(cadenaActual) && ConnectionStringManager.IsValidConnectionString(cadenaActual))
                {
                    var info = ConnectionStringManager.ExtractConnectionInfo(cadenaActual);
                    txtServidor.Text = string.IsNullOrEmpty(info.Server) ? @"localhost\SQLEXPRESS" : info.Server;
                    txtBaseDatos.Text = string.IsNullOrEmpty(info.Database) ? "MiniSistemaFacturacion" : info.Database;

                    if (info.UseIntegratedSecurity)
                    {
                        rbWindowsAuth.Checked = true;
                    }
                    else
                    {
                        rbSqlAuth.Checked = true;
                        txtUsuario.Text = info.Username;
                    }
                }
                else
                {
                    txtServidor.Text = @"localhost\SQLEXPRESS";
                    txtBaseDatos.Text = "MiniSistemaFacturacion";
                    rbWindowsAuth.Checked = true;
                }
            }
            catch
            {
                txtServidor.Text = @"localhost\SQLEXPRESS";
                txtBaseDatos.Text = "MiniSistemaFacturacion";
                rbWindowsAuth.Checked = true;
            }

            ActualizarEstadoAutenticacion();
        }

        private void ActualizarEstadoAutenticacion()
        {
            bool esSql = rbSqlAuth.Checked;
            pnlCredencialesSql.Enabled = esSql;
            chkProtegerDPAPI.Enabled = esSql;
            if (!esSql)
            {
                chkProtegerDPAPI.Checked = false;
            }
            else
            {
                chkProtegerDPAPI.Checked = true;
            }
        }

        private void rbAuth_CheckedChanged(object sender, EventArgs e)
        {
            ActualizarEstadoAutenticacion();
        }

        private void btnVerPassword_Click(object sender, EventArgs e)
        {
            _passwordVisible = !_passwordVisible;
            txtPassword.PasswordChar = _passwordVisible ? '\0' : '●';
            btnVerPassword.Text = _passwordVisible ? "🔒" : "👁";
        }

        private string ConstruirCadenaActual()
        {
            string servidor = txtServidor.Text.Trim();
            string bd = txtBaseDatos.Text.Trim();

            if (string.IsNullOrEmpty(servidor))
            {
                throw new ArgumentException("Debe ingresar el servidor o instancia SQL Server.");
            }

            if (string.IsNullOrEmpty(bd))
            {
                throw new ArgumentException("Debe ingresar el nombre de la base de datos.");
            }

            bool integrated = rbWindowsAuth.Checked;
            string usuario = integrated ? "" : txtUsuario.Text.Trim();
            string password = integrated ? "" : txtPassword.Text;

            return ConnectionStringManager.BuildConnectionString(servidor, bd, integrated, usuario, password, 15);
        }

        private void SetControlesBloqueados(bool bloqueados)
        {
            txtServidor.Enabled = !bloqueados;
            txtBaseDatos.Enabled = !bloqueados;
            gbAutenticacion.Enabled = !bloqueados;
            btnProbarConexion.Enabled = !bloqueados;
            btnInicializarBD.Enabled = !bloqueados && btnInicializarBD.Enabled;
            btnGuardar.Enabled = !bloqueados;
            btnSalir.Enabled = !bloqueados;
            Cursor = bloqueados ? Cursors.WaitCursor : Cursors.Default;
        }

        private async void btnProbarConexion_Click(object sender, EventArgs e)
        {
            try
            {
                string cadena = ConstruirCadenaActual();
                SetControlesBloqueados(true);

                lblEstadoIcono.Text = "⏳";
                lblEstadoMensaje.Text = "Conectando al servidor y diagnosticando base de datos...";
                pnlEstado.BackColor = Color.FromArgb(241, 245, 249);
                lblEstadoMensaje.ForeColor = Color.FromArgb(71, 85, 105);

                var migrador = new DatabaseMigrationManager(cadena);
                var status = await Task.Run(() => migrador.EvaluarEstado());

                if (status.ServidorAccesible && status.BaseDatosExiste && status.EsquemaAlDia)
                {
                    _cadenaProbadaValida = cadena;
                    lblEstadoIcono.Text = "🟢";
                    lblEstadoMensaje.Text = $"Conexión exitosa a '{txtServidor.Text.Trim()}'. La base de datos '{txtBaseDatos.Text.Trim()}' está lista y con todas las migraciones al día ({status.VersionActual} aplicadas).";
                    pnlEstado.BackColor = Color.FromArgb(220, 252, 231);
                    lblEstadoMensaje.ForeColor = Color.FromArgb(22, 101, 52);
                    btnGuardar.Enabled = true;
                    btnInicializarBD.Enabled = false;
                }
                else if (status.ServidorAccesible && (!status.BaseDatosExiste || !status.EsquemaAlDia))
                {
                    _cadenaProbadaValida = cadena;
                    lblEstadoIcono.Text = "🟡";
                    string motivo = !status.BaseDatosExiste 
                        ? "La base de datos aún no existe en el servidor." 
                        : $"Existen {status.TotalMigracionesPendientes} migraciones de esquema pendientes.";

                    lblEstadoMensaje.Text = $"Servidor SQL conectado. {motivo} Haga clic en '⚡ Inicializar / Actualizar' para prepararla.";
                    pnlEstado.BackColor = Color.FromArgb(254, 243, 199);
                    lblEstadoMensaje.ForeColor = Color.FromArgb(146, 64, 14);
                    btnInicializarBD.Enabled = true;
                    btnGuardar.Enabled = false;
                }
                else
                {
                    _cadenaProbadaValida = null;
                    lblEstadoIcono.Text = "🔴";
                    string detalle = string.IsNullOrEmpty(status.MensajeError) ? "No se pudo establecer comunicación." : status.MensajeError;
                    lblEstadoMensaje.Text = $"Fallo al conectar con el servidor SQL '{txtServidor.Text.Trim()}'.\n{detalle}";
                    pnlEstado.BackColor = Color.FromArgb(254, 226, 226);
                    lblEstadoMensaje.ForeColor = Color.FromArgb(153, 27, 27);
                    btnInicializarBD.Enabled = false;
                    btnGuardar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblEstadoIcono.Text = "🔴";
                lblEstadoMensaje.Text = $"Error de validación: {ex.Message}";
                pnlEstado.BackColor = Color.FromArgb(254, 226, 226);
                lblEstadoMensaje.ForeColor = Color.FromArgb(153, 27, 27);
                btnInicializarBD.Enabled = false;
                btnGuardar.Enabled = false;
            }
            finally
            {
                SetControlesBloqueados(false);
            }
        }

        private async void btnInicializarBD_Click(object sender, EventArgs e)
        {
            string servidor = txtServidor.Text.Trim();
            string bd = txtBaseDatos.Text.Trim();

            var dialogConfirm = MessageBox.Show(
                $"¿Confirma la inicialización o actualización de la base de datos?\n\n" +
                $"• Servidor: {servidor}\n" +
                $"• Base de Datos: {bd}\n\n" +
                $"Acciones que se ejecutarán:\n" +
                $"1. Si la base no existe, se creará automáticamente.\n" +
                $"2. Si ya existen tablas con datos, se intentará un respaldo previo.\n" +
                $"3. Se aplicarán de forma transaccional las migraciones de tablas, índices y procedimientos.\n" +
                $"4. Cero datos de prueba ficticios serán insertados.\n\n" +
                $"¿Desea continuar?",
                "Confirmación de Inicialización de Esquema",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dialogConfirm != DialogResult.Yes) return;

            try
            {
                string cadena = ConstruirCadenaActual();
                SetControlesBloqueados(true);

                lblEstadoIcono.Text = "⚙️";
                lblEstadoMensaje.Text = "Aplicando migraciones versionadas y asegurando integridad...";
                pnlEstado.BackColor = Color.FromArgb(241, 245, 249);
                lblEstadoMensaje.ForeColor = Color.FromArgb(71, 85, 105);

                var migrador = new DatabaseMigrationManager(cadena);
                var resultado = await Task.Run(() => migrador.AplicarMigraciones(true));

                if (resultado.Exito)
                {
                    _cadenaProbadaValida = cadena;
                    lblEstadoIcono.Text = "🟢";
                    string backupInfo = !string.IsNullOrEmpty(resultado.RutaBackup) ? $" (Respaldo: {resultado.RutaBackup})" : "";
                    lblEstadoMensaje.Text = $"Inicialización completada con éxito. Se aplicaron {resultado.MigracionesAplicadas} migraciones.{backupInfo}";
                    pnlEstado.BackColor = Color.FromArgb(220, 252, 231);
                    lblEstadoMensaje.ForeColor = Color.FromArgb(22, 101, 52);
                    btnGuardar.Enabled = true;
                    btnInicializarBD.Enabled = false;

                    MessageBox.Show(
                        $"La base de datos '{bd}' ha sido inicializada y actualizada correctamente.\n\n" +
                        $"Migraciones aplicadas: {resultado.MigracionesAplicadas}\n" +
                        (string.IsNullOrEmpty(resultado.RutaBackup) ? "" : $"Respaldo previo: {resultado.RutaBackup}\n"),
                        "Migración Exitosa",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    lblEstadoIcono.Text = "🔴";
                    lblEstadoMensaje.Text = $"Error durante la migración: {resultado.Mensaje}";
                    pnlEstado.BackColor = Color.FromArgb(254, 226, 226);
                    lblEstadoMensaje.ForeColor = Color.FromArgb(153, 27, 27);

                    MessageBox.Show(
                        $"Ocurrió un error al aplicar las migraciones:\n\n{resultado.Mensaje}",
                        "Error en Migración",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                lblEstadoIcono.Text = "🔴";
                lblEstadoMensaje.Text = $"Error crítico: {ex.Message}";
                pnlEstado.BackColor = Color.FromArgb(254, 226, 226);
                lblEstadoMensaje.ForeColor = Color.FromArgb(153, 27, 27);
            }
            finally
            {
                SetControlesBloqueados(false);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_cadenaProbadaValida))
                {
                    MessageBox.Show("Debe probar la conexión exitosamente antes de guardar la configuración.",
                                    "Conexión No Verificada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string cadenaActual = ConstruirCadenaActual();
                if (_cadenaProbadaValida != cadenaActual)
                {
                    MessageBox.Show("Los parámetros de conexión han cambiado después de la prueba. Debe probar la conexión nuevamente antes de guardar.",
                                    "Conexión No Verificada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool protegerDpapi = chkProtegerDPAPI.Checked && rbSqlAuth.Checked;

                string errorGuardado;
                if (!ConnectionStringManager.SaveConnectionString(_cadenaProbadaValida, protegerDpapi, out errorGuardado))
                {
                    MessageBox.Show($"No se pudo guardar la configuración:\n{errorGuardado}",
                                    "Error al Guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la configuración:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
