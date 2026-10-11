# Mini-Sistema de Facturación y Cuentas por Cobrar

> **Sistema Integral de Gestión Comercial, Control de Inventario, Facturación Fiscal (18% ITBIS), Cuentas por Cobrar y Seguridad Corporativa**  
> Desarrollado en **C# (.NET Framework 4.7.2)**, **Windows Forms** y **SQL Server Express**.

---

<details>
<summary>📑 <b>Índice de Contenido (Clic para expandir)</b></summary>

- [1. Descripción General](#1-descripción-general)
- [2. Arquitectura del Sistema](#2-arquitectura-del-sistema)
  - [Estructura en Capas del Proyecto](#estructura-en-capas-del-proyecto)
  - [Seguridad y Autenticación Criptográfica](#seguridad-y-autenticación-criptográfica)
  - [Sistema de Diseño Corporativo (UITheme)](#sistema-de-diseño-corporativo-uitheme)
- [3. Módulos y Funcionalidades Principales](#3-módulos-y-funcionalidades-principales)
  - [Dashboard Ejecutivo (FrmMain)](#dashboard-ejecutivo-frmmain)
  - [Módulo de Seguridad y Usuarios (FrmUsuarios / FrmLogin / FrmCambiarPassword)](#módulo-de-seguridad-y-usuarios)
  - [Módulo de Facturación y Tickets PDF (FrmFacturacion / FrmVistaPreviaPdf)](#módulo-de-facturación-y-tickets-pdf)
  - [Módulo de Consultas y Búsqueda de Facturas (FrmBusquedaFacturas)](#módulo-de-consultas-y-búsqueda-de-facturas)
  - [Módulo de Cuentas por Cobrar y Abonos (frmCuentasPorCobrar / frmRegistrarPago)](#módulo-de-cuentas-por-cobrar-y-abonos)
  - [Módulo de Clientes (FrmClientes / FrmAgregarCliente / FrmEditarCliente)](#módulo-de-clientes)
  - [Módulo de Productos e Inventario (FrmProductos)](#módulo-de-productos-e-inventario)
  - [Módulo de Reportes Gerenciales y Dashboard Analítico (FrmReportes)](#módulo-de-reportes-gerenciales-y-dashboard-analítico)
  - [Configuración de Empresa (FrmConfiguracionEmpresa)](#configuración-de-empresa)
- [4. Base de Datos y Persistencia](#4-base-de-datos-y-persistencia)
  - [Tablas Principales](#tablas-principales)
  - [Auditoría y Transacciones ACID](#auditoría-y-transacciones-acid)
- [5. Tecnologías y Librerías Utilizadas](#5-tecnologías-y-librerías-utilizadas)
- [6. Instalación, Configuración y Puesta en Marcha](#6-instalación-configuración-y-puesta-en-marcha)
  - [Requisitos Previos](#requisitos-previos)
  - [Paso 1: Configurar la Base de Datos en SQL Server](#paso-1-configurar-la-base-de-datos-en-sql-server)
  - [Paso 2: Cadena de Conexión (App.config)](#paso-2-cadena-de-conexión-appconfig)
  - [Paso 3: Compilación y Primer Acceso](#paso-3-compilación-y-primer-acceso)
- [7. Atajos de Teclado y Experiencia de Usuario](#7-atajos-de-teclado-y-experiencia-de-usuario)
- [8. Equipo y Créditos](#8-equipo-y-créditos)

</details>

---

## 1. Descripción General

**Mini-Sistema de Facturación** es una solución de escritorio empresarial diseñada bajo estándares de ingeniería de software robustos. Combina una arquitectura multicapa desacoplada con un diseño corporativo moderno basado en **Slate Navy (`#253959`)** y una capa de seguridad criptográfica con control de acceso basado en roles (RBAC).

El sistema cubre el ciclo operativo completo de un negocio:
* Control de inventario en tiempo real con alertas de stock mínimo.
* Directorio completo de clientes con validación fiscal de Cédula y RNC.
* Facturación fiscal con cálculo automático del **18% ITBIS**, numeración NCF dominicana y generación instantánea de tickets térmicos en PDF (80mm) con vista previa integrada mediante WebView2.
* Control de cuentas por cobrar, abonos parciales e historial de pagos.
* Autenticación segura mediante PBKDF2-HMAC-SHA256, políticas de bloqueo contra ataques de fuerza bruta y trazabilidad en bitácora de auditoría.

---

## 2. Arquitectura del Sistema

### Estructura en Capas del Proyecto

```text
MiniSistemaFacturacion/
├── 📁 Models/                    (Entidades de Dominio)
│   ├── Cliente.cs                  (Entidad Cliente con validaciones)
│   ├── Producto.cs                 (Entidad Producto y stock)
│   ├── Factura.cs                 (Cabecera de Factura con 18% ITBIS)
│   ├── DetalleFactura.cs          (Renglones de Factura)
│   ├── Pago.cs                    (Registro de Abonos y Pagos)
│   ├── Usuario.cs                 (Modelo de Usuario y perfil sanitizado)
│   ├── RolUsuario.cs              (Constantes de Roles y Estados)
│   └── EmpresaConfig.cs           (Datos de cabecera fiscal de la empresa)
├── 📁 DataAccess/               (Capa de Persistencia y ADO.NET)
│   ├── DbHelper.cs                (Singleton con consultas parametrizadas)
│   ├── ClienteDAL.cs             (CRUD y búsquedas de Clientes)
│   ├── ProductoDAL.cs            (CRUD y control de existencias)
│   ├── FacturaDAL.cs             (Persistencia transaccional de facturas)
│   ├── DetalleFacturaDAL.cs      (Renglones asociados)
│   ├── PagoDAL.cs                (Abonos a facturas y estados)
│   ├── UsuarioDAL.cs             (Consultas y auditoría de usuarios)
│   └── ReportesDAL.cs            (Métricas para dashboard y reportes)
├── 📁 BusinessLogic/            (Reglas de Negocio y Seguridad)
│   ├── FacturacionManager.cs       (Transacciones ACID de emisión)
│   ├── PagoManager.cs            (Aplicación de abonos y saldos)
│   ├── StockManager.cs           (Validación y reserva de inventario)
│   ├── NCFManager.cs             (Generación y correlativos de NCF)
│   ├── PasswordHasher.cs         (PBKDF2-HMAC-SHA256 con salt de 256 bits)
│   ├── UsuarioManager.cs         (Autenticación, bloqueos y auditoría)
│   └── TransaccionHelper.cs      (Manejo de SqlTransaction)
├── 📁 Configuration/            (Estado de Sesión y Ajustes)
│   └── SesionActual.cs           (Usuario autenticado en memoria)
├── 📁 Utils/                    (Sistema de Diseño y Utilidades)
│   ├── UITheme.cs                (Tokens Slate Navy, estilos y asignación de íconos)
│   ├── DesignModeHelper.cs       (Detección segura del diseñador WinForms)
│   └── ValidadorCliente.cs       (Reglas de formato de Cédula/RNC)
├── 📁 Services/                (Servicios de Generación de Documentos)
│   ├── PdfTicketService.cs       (Generación de PDF en formato 80mm con QuestPDF)
│   └── EmpresaConfigService.cs   (Lectura y guardado de datos corporativos)
├── 📁 Forms/                    (Capa de Presentación - Windows Forms)
│   ├── FrmLogin.cs               (Acceso seguro y asistente de primer Admin)
│   ├── FrmMain.cs                (Dashboard ejecutivo y navegación interactiva)
│   ├── FrmUsuarios.cs            (Administración de usuarios y roles)
│   ├── FrmCambiarPassword.cs     (Cambio y restablecimiento de contraseña)
│   ├── FrmClientes.cs            (Directorio expandido de clientes)
│   ├── FrmAgregarCliente.cs      (Formulario modal de alta de cliente)
│   ├── FrmEditarCliente.cs       (Formulario modal de edición)
│   ├── FrmSelectorClientes.cs    (Selector rápido con búsqueda F3)
│   ├── FrmProductos.cs           (Catálogo de productos y stock)
│   ├── frmFacturacion.cs         (Punto de venta y emisión fiscal)
│   ├── FrmBusquedaFacturas.cs    (Consulta histórica de facturación)
│   ├── frmCuentasPorCobrar.cs    (Saldos pendientes con soporte de doble clic)
│   ├── frmRegistrarPago.cs       (Modal de registro de abonos)
│   ├── FrmReportes.cs            (Dashboard gerencial con gráficos LiveCharts)
│   ├── FrmConfiguracionEmpresa.cs(Datos fiscales de la empresa)
│   └── FrmVistaPreviaPdf.cs      (Visor nativo con Microsoft WebView2)
├── 📁 Database/                 (Scripts SQL Relacionales)
│   ├── 01_CreateTables.sql       (Estructura base de tablas)
│   ├── 02_InsertData.sql         (Semillas y datos de prueba)
│   ├── 03_StoredProcedures.sql   (Procedimientos almacenados)
│   └── 04_Usuarios.sql           (Módulo de seguridad, auditoría e índices)
├── App.config                   (Cadena de conexión y tasa fiscal unificada 18%)
├── MiniSistemaFacturacion.slnx  (Solución Visual Studio)
└── MiniSistemaFacturacion.csproj(Proyecto C#)
```

---

### Seguridad y Autenticación Criptográfica

El módulo de seguridad implementa controles de grado bancario:
1. **Derivación de Claves (PBKDF2):**
   * Algoritmo: **PBKDF2-HMAC-SHA256**.
   * Salt criptográfico único de **256 bits** generado mediante `RNGCryptoServiceProvider`.
   * **100,000 iteraciones** de derivación.
   * Verificación en **tiempo constante** (`CompararTiempoConstante`) para mitigar ataques de temporización (*Timing Attacks*).
2. **Defensa contra Fuerza Bruta:**
   * Bloqueo automático por **15 minutos** tras **5 intentos fallidos consecutivos**.
3. **Control de Acceso Basado en Roles (RBAC):**
   * **Administrador:** Acceso irrestricto, gestión de usuarios, reseteo de claves, configuración de empresa y borrado de datos.
   * **Empleado:** Restricción estricta a facturación, clientes, productos, cuentas por cobrar y reportes generales.
4. **Protección del Último Administrador:**
   * El sistema impide desactivar o degradar al último administrador activo, garantizando la gobernanza del sistema.
5. **Auditoría de Seguridad:**
   * Registro en la tabla `AuditoriaUsuarios` de inicios de sesión, intentos fallidos, bloqueos, cambios de contraseña y modificaciones de usuarios.

---

### Sistema de Diseño Corporativo (UITheme)

Toda la aplicación utiliza un sistema de diseño centralizado en [UITheme.cs](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Utils/UITheme.cs):
* **Color Primario / Encabezados:** Slate Navy `#253959`.
* **Acentos:** Azul primario `#2563EB`, Verde éxito `#16A34A`, Rojo peligro `#DC2626`, Naranja alerta `#EA580C`.
* **Fondos:** Slate Workspace `#F1F5F9`, Form Background `#F8F9FA`.
* **Tipografía:** Escala armónica Segoe UI (desde 8pt para metadatos hasta 14pt negrita para títulos).
* **Tablas DataGridView:** Encabezados planos, filas alternadas con contraste suave, selección nítida, anchos automáticos con `FillWeight` y eliminación de bordes gruesos obsoletos.
* **Ícono Oficial:** Asignación automática y propagación del ícono corporativo del sistema (`Recurso_2logosisfac.ico`) en todos los formularios.

---

## 3. Módulos y Funcionalidades Principales

### Dashboard Ejecutivo (FrmMain)
* **Encabezado Corporativo:** Muestra el título del sistema, la insignia de tasa fiscal (18% ITBIS), el usuario activo en sesión, el botón directo de **`[ 🔑 Cambiar Clave ]`** y el botón **`[ Cerrar Sesión ]`**.
* **Menú Horizontal Limpio:** Navegación en Segoe UI libre de submenús redundantes.
* **Cuadrícula de 8 Tarjetas Interactivas:**
  1. 👥 **Clientes:** Directorio y saldos de clientes.
  2. 📦 **Productos:** Catálogo y stock en tiempo real.
  3. 🧾 **Facturación:** Punto de venta y emisión fiscal.
  4. 🔍 **Buscar Facturas:** Consulta histórica y reimpresión.
  5. 💳 **Cuentas por Cobrar:** Control de créditos y saldos.
  6. 📊 **Reportes:** Métricas analíticas y gráficos de ventas.
  7. 🛡️ **Usuarios:** Panel de seguridad (exclusivo Administrador).
  8. ⚙️ **Configuración:** Datos de la empresa (exclusivo Administrador).
* **Interactividad:** Micro-animación de iluminación al pasar el cursor (hover), cursor de mano y navegación con un solo clic.

---

### Módulo de Seguridad y Usuarios
* **[FrmLogin](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmLogin.cs):**
  * Pantalla de autenticación con alternancia para mostrar/ocultar contraseña.
  * Asistente automático de primer uso: si no existen administradores en la base de datos, guía al usuario para registrar el primer Administrador del sistema antes de habilitar el acceso.
* **[FrmUsuarios](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmUsuarios.cs):**
  * Dimensiones ampliadas (`1220 x 660 px`) con grilla espaciosa de 800 px.
  * Ocultamiento riguroso de columnas técnicas (hashes, salts e intentos fallidos quedan fuera de la vista del operador).
  * Panel lateral para altas, edición de rol/estado, activación/desactivación y botón directo de restablecimiento de clave.
* **[FrmCambiarPassword](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmCambiarPassword.cs):**
  * Modal diseñado con Slate Navy que sustituye los antiguos diálogos `InputBox`.
  * **Modo Personal (desde FrmMain):** Pide contraseña actual, nueva contraseña (mínimo 6 caracteres), confirmación y validación de discrepancias.
  * **Modo Administrativo (desde FrmUsuarios):** Permite al Administrador asignar una nueva clave a un usuario seleccionado sin requerir la anterior.

---

### Módulo de Facturación y Tickets PDF
* **Emisión Fiscal Rápida:**
  * Búsqueda ágil de clientes con atajo **F3** y selector modal.
  * Agregado dinámico de renglones con validación de stock disponible.
  * Cálculos inmediatos: Subtotal, **18% ITBIS** y Total Neto.
  * Selección de tipo de comprobante (Consumidor Final, Crédito Fiscal, etc.) con asignación correlativa de NCF.
* **Generación de Ticket PDF (80mm):**
  * Diseñado con **QuestPDF** optimizado para impresoras térmicas de 80mm o estándares de oficina.
  * Incluye datos fiscales de la empresa (RNC, dirección, teléfono), cliente, NCF, desglose de ITBIS y pie de página.
  * Visualización instantánea con **Microsoft WebView2** en [FrmVistaPreviaPdf](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmVistaPreviaPdf.cs) con opciones para imprimir directamente o exportar.

---

### Módulo de Consultas y Búsqueda de Facturas
* **[FrmBusquedaFacturas](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmBusquedaFacturas.cs):**
  * Barra de filtros reorganizada en una sola fila armoniosa: N° Factura, selector de cliente con botón 🔍, selectores de fecha con formato compacto `dd/MM/yyyy` (resolviendo truncamientos del calendario en español) y botones `[ 🔍 Buscar ]` y `[ 🔄 Limpiar ]` alineados.
  * DataGridView en modo `Fill`, aprovechando el 100% del ancho de la pantalla sin espacios muertos.
  * Doble clic en cualquier fila para abrir la vista previa e impresión de la factura.

---

### Módulo de Cuentas por Cobrar y Abonos
* **[frmCuentasPorCobrar](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/frmCuentasPorCobrar.cs):**
  * Filtro en tiempo real por N° de factura o nombre de cliente.
  * Visualización clara del total facturado y saldo pendiente restante en color de alerta.
  * **Doble Clic y Enter:** Hacer doble clic sobre cualquier factura con saldo pendiente abre directamente el formulario modal de registro de abono.
  * Mecanismo anti-rebote (*debounce*) para evitar aperturas duplicadas.
* **[frmRegistrarPago](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/frmRegistrarPago.cs):**
  * Registro de abonos parciales o saldos totales con múltiples formas de pago (Efectivo, Tarjeta, Transferencia, Cheque).
  * Actualización atómica de saldo y cambio automático de estado de factura (`Pendiente` $\rightarrow$ `Parcial` $\rightarrow$ `Pagada`).

---

### Módulo de Clientes
* **[FrmClientes](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmClientes.cs):**
  * Formulario ampliado a **`1260 x 680 px`**.
  * Grilla expandida a **`1227 px`**, perfectamente alineada hasta el borde derecho del botón **Eliminar** (ambos a `17 px` del margen derecho).
  * 10 columnas visibles sin cortes: *ID, Nombre/Razón Social, Cédula, Dirección, Teléfono, Email, Tipo, RNC, Estado, Fecha Creación* (formateada en `dd/MM/yyyy`).
  * Validación estricta de formato de Cédula y RNC al crear o editar clientes.

---

### Módulo de Productos e Inventario
* **[FrmProductos](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmProductos.cs):**
  * Catálogo completo de artículos con control de stock actual y stock mínimo.
  * Alertas visuales de inventario bajo o crítico.
  * Descuento automático de existencias durante la facturación bajo transacciones SQL.

---

### Módulo de Reportes Gerenciales y Dashboard Analítico
* **[FrmReportes](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmReportes.cs):**
  * Métricas clave en tiempo real: Ventas del día, cuentas por cobrar pendientes, total facturado y volumen de facturas emitidas.
  * Gráficos interactivos mediante **LiveCharts**: ventas por período, productos más vendidos y balance de cartera.
  * Reportes tabulares exportables de clientes con deuda, facturación por rango de fechas e inventario valorizado.

---

### Configuración de Empresa
* **[FrmConfiguracionEmpresa](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/Forms/FrmConfiguracionEmpresa.cs):**
  * Mantenimiento de Nombre Comercial, RNC, Dirección, Teléfono, Correo y mensaje de pie de factura.
  * Estos datos alimentan automáticamente los tickets PDF emitidos.

---

## 4. Base de Datos y Persistencia

### Tablas Principales

| Tabla | Propósito | Características Técnicas |
| :--- | :--- | :--- |
| `Usuarios` | Cuentas del sistema | `ID_Usuario`, `NombreUsuario` UNIQUE, `PasswordHash`, `PasswordSalt`, `Rol`, `Estado`, `IntentosFallidos`, `FechaBloqueo`. |
| `AuditoriaUsuarios` | Trazabilidad de seguridad | Registra cada intento de acceso, cambios de clave y modificaciones con timestamp. |
| `Clientes` | Cartera de clientes | `ID_Cliente`, `Nombre`, `Cedula` UNIQUE, `RNC`, `Direccion`, `Telefono`, `Email`, `Estado`. |
| `Productos` | Catálogo de artículos | `ID_Producto`, `Codigo` UNIQUE, `Descripcion`, `Precio`, `Stock`, `StockMinimo`. |
| `Facturas` | Encabezados de venta | `ID_Factura`, `NumeroFactura`, `ID_Cliente`, `Fecha`, `SubTotal`, `Impuesto (18%)`, `TotalNeto`, `SaldoPendiente`, `Estado`, `NCF`. |
| `DetalleFacturas` | Renglones facturados | `ID_Detalle`, `ID_Factura`, `ID_Producto`, `Cantidad`, `PrecioUnitario`, `SubTotal`. |
| `Pagos` | Abonos e ingresos | `ID_Pago`, `ID_Factura`, `Fecha`, `Monto`, `MetodoPago`, `Referencia`. |
| `ConfiguracionEmpresa` | Datos de la empresa | Almacena la cabecera fiscal y datos comerciales para los comprobantes. |

### Auditoría y Transacciones ACID

La emisión de facturas y los abonos se ejecutan mediante bloques `SqlTransaction`:
1. Registro del encabezado de factura / pago.
2. Inserción de renglones de detalle.
3. Descuento de stock en `Productos` o actualización de saldo en `Facturas`.
4. Si cualquier paso falla, se ejecuta `Rollback()` automático, garantizando integridad referencial y cero inconsistencias de inventario o saldos.

---

## 5. Tecnologías y Librerías Utilizadas

* **Framework:** Microsoft .NET Framework 4.7.2
* **Lenguaje:** C# 7.3+
* **Motor de Base de Datos:** Microsoft SQL Server Express 2019 / 2022
* **Acceso a Datos:** ADO.NET clásico (`System.Data.SqlClient`) mediante patrón Singleton parametrizado
* **Librerías de Presentación y Documentos:**
  * **QuestPDF (v2026.2.4):** Motor fluido de generación de PDFs y tickets térmicos de 80mm.
  * **Microsoft.Web.WebView2 (v1.0.3856.49):** Control de renderizado web basado en Microsoft Edge Chromium para vista previa nativa e impresión de documentos PDF.
  * **LiveCharts (v0.9.7):** Gráficos analíticos vectoriales y responsivos para el módulo de reportes.

---

## 6. Instalación, Configuración y Puesta en Marcha

### Requisitos Previos
* Windows 10 / Windows 11 (x64).
* SQL Server Express 2019 o superior con instancia local (`localhost\SQLEXPRESS`).
* .NET Framework 4.7.2 instalado en el sistema.
* Visual Studio 2022 (para compilar y depurar).

### Paso 1: Configurar la Base de Datos en SQL Server
Abre SQL Server Management Studio (SSMS) y ejecuta los scripts ubicados en la carpeta `Database/` en el siguiente orden estricto:

1. `Database/01_CreateTables.sql`: Crea la base de datos `MiniSistemaFacturacion` y las tablas de negocio.
2. `Database/02_InsertData.sql`: Inserta datos iniciales de prueba (clientes, productos y configuración).
3. `Database/03_StoredProcedures.sql`: Registra procedimientos almacenados de consulta y reportes.
4. `Database/04_Usuarios.sql`: Crea las tablas de seguridad `Usuarios`, `AuditoriaUsuarios`, índices de alta velocidad y el procedimiento `sp_ExisteAdministradorActivo`.

### Paso 2: Cadena de Conexión (App.config)
Verifica que la cadena de conexión en [App.config](file:///c:/Users/Usuario/Desktop/Proyectos/MiniSistemaFacturacion/App.config) coincida con tu instancia local de SQL Server:

```xml
<connectionStrings>
  <add name="MiniFacturacionDB"
       connectionString="Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

> **Nota sobre el Impuesto:** El parámetro de impuesto está configurado en `App.config` con valor `18` para coincidir con la tasa del **18% ITBIS**.

### Paso 3: Compilación y Primer Acceso
1. Abre la solución `MiniSistemaFacturacion.slnx` en Visual Studio.
2. Restaura los paquetes NuGet y compila el proyecto (`Ctrl + Shift + B`).
3. Ejecuta la aplicación (`F5` o ejecutando `MiniSistemaFacturacion.exe`).
4. **Primer Inicio:**
   * Si la base de datos no tiene administradores, el sistema desplegará automáticamente el **Asistente de Configuración Inicial** solicitando el Nombre Completo, Nombre de Usuario y Contraseña para el primer Administrador.
   * Al crearlo, serás redirigido a la pantalla de Login para acceder con las nuevas credenciales.

---

## 7. Atajos de Teclado y Experiencia de Usuario

* **`F3`:** Búsqueda rápida de clientes desde el módulo de facturación.
* **`Enter`:** En la grilla de Cuentas por Cobrar, abre instantáneamente la ventana modal de abono sobre la factura seleccionada.
* **Doble Clic:**
  * En **Cuentas por Cobrar:** Abre el registro de abono para la factura seleccionada.
  * En **Búsqueda de Facturas:** Abre la vista previa del comprobante PDF en WebView2.
  * En **Clientes:** Abre la edición del cliente seleccionado.

---

## 8. Equipo y Créditos

* **César Adrián Reyes Dinzey** — *Arquitectura de Seguridad, Backend Criptográfico, Base de Datos, Sistema de Diseño Slate Navy y Refactorización Integral.*
* **Equipo de Desarrollo del Proyecto:** Julio García, Luis García, Marileidi.
* **Institución:** Universidad Abierta para Adultos (UAPA) — *Ingeniería de Software / Programación*.
* **Año:** 2026.

---

© 2026 Mini-Sistema de Facturación — Todos los derechos reservados.
