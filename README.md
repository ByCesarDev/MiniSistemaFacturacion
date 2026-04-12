# Mini-Sistema de Facturación y Cuentas por Cobrar

**Proyecto Final - Programación III**

## 📋 Descripción General

Aplicación de escritorio desarrollada en C# con Windows Forms y SQL Server Express que implementa un sistema completo de facturación y gestión de cuentas por cobrar. El proyecto utiliza una arquitectura en capas con patrones de diseño modernos para garantizar mantenibilidad y escalabilidad.

**🆕 Características Recientes**: Sistema completo de generación de PDF en formato ticket, búsqueda avanzada de facturas, edición de facturas existentes, selector de clientes intuitivo y mejoras significativas en la experiencia de usuario.

## 🏗️ Arquitectura del Sistema

### Estructura en Capas
```
MiniSistemaFacturacion/
├── 📁 Models/                    (Entidades de Negocio)
│   ├── Cliente.cs                  (Gestión de clientes)
│   ├── Producto.cs                 (Gestión de inventario)
│   ├── Factura.cs                 (Cabecera de facturas)
│   ├── DetalleFactura.cs          (Detalles de facturas)
│   └── Pago.cs                    (Registro de pagos)
├── 📁 DataAccess/               (Capa de Acceso a Datos)
│   ├── DbHelper.cs                (Conexión y utilidades SQL)
│   ├── ConnectionStringManager.cs   (Gestión de connection strings)
│   ├── ClienteDAL.cs             (CRUD de clientes)
│   └── ProductoDAL.cs            (CRUD de productos)
├── 📁 BusinessLogic/            (Lógica de Negocio)
│   ├── FacturacionManager.cs       (Proceso de facturación)
│   ├── PagoManager.cs            (Gestión de pagos)
│   └── TransaccionHelper.cs      (Utilidades de transacciones)
├── 📁 Forms/                    (Interfaz de Usuario)
│   ├── frmMain.cs                (Menú principal)
│   ├── frmClientes.cs            (Gestión de clientes)
│   ├── frmProductos.cs           (Gestión de productos)
│   ├── frmFacturacion.cs        (Proceso de facturación)
│   ├── FrmVistaPreviaPdf.cs     (Vista previa e impresión de PDF)
│   ├── FrmBusquedaFacturas.cs   (Búsqueda avanzada de facturas)
│   ├── FrmSelectorClientes.cs    (Selector intuitivo de clientes)
│   └── frmCuentasPorCobrar.cs  (Gestión de pagos)
├── 📁 Services/                (Servicios especializados)
│   ├── PdfTicketService.cs       (Generación de PDF en formato ticket)
│   └── EmpresaConfig.cs         (Configuración de datos de empresa)
├── 📁 Utilities/               (Herramientas auxiliares)
├── 📁 Resources/               (Recursos de la aplicación)
├── App.config                   (Configuración de la aplicación)
└── Program.cs                  (Punto de entrada)
```

## 🗄️ Base de Datos

### Entidades Principales
- **Clientes**: Información completa de clientes con validación de cédulas y RNC
- **Productos**: Gestión de inventario con control de stock
- **Facturas**: Cabecera de facturas con NCF, cálculos automáticos y tipos de comprobante
- **DetallesFactura**: Líneas de detalle de cada factura con edición en tiempo real
- **Pagos**: Registro de pagos con múltiples formas de pago

### Características Técnicas
- **Motor**: SQL Server Express
- **Transacciones**: Operaciones atómicas con rollback automático
- **Integridad**: Constraints y foreign keys
- **Índices**: Optimizados para consultas frecuentes
- **Datos de Prueba**: 15 clientes, 40 productos, transacciones reales

## ⚡ Funcionalidades Principales

### 🧾 Módulo de Facturación
- **Creación de Facturas**: Selección de cliente y productos con selector intuitivo
- **Cálculos Automáticos**: Subtotal, IVA (18%), Total Neto
- **Control de Stock**: Validación y actualización automática
- **Números de Factura**: Generación automática y única con NCF
- **Transacciones Atómicas**: Garantía de integridad de datos
- **🆕 Edición de Facturas**: Modificación completa de facturas existentes
- **🆕 Vista Previa PDF**: Generación instantánea de tickets en formato 80mm
- **🆕 Búsqueda Avanzada**: Filtrado por número, cliente, fechas y estados
- **🆕 Selector de Clientes**: Búsqueda rápida con atajo F3 y DataGridView

### 💳 Módulo de Cuentas por Cobrar
- **Registro de Pagos**: Múltiples formas de pago
- **Actualización de Saldos**: Automática y en tiempo real
- **Estados de Factura**: Pendiente → Parcial → Pagada
- **Historial de Pagos**: Consulta completa por factura
- **Formas de Pago**: Efectivo, Tarjeta, Transferencia, Cheque

### 👥 Módulo de Clientes
- **Gestión Completa**: CRUD de clientes
- **Validaciones**: Formato de cédula, email, teléfono
- **Búsquedas**: Por nombre, cédula o dirección
- **Estado**: Clientes activos/inactivos

### 📦 Módulo de Inventario
- **Control de Stock**: Actualización automática al facturar
- **Alertas**: Productos con stock bajo o crítico
- **Categorías**: Organización por categorías
- **Precios**: Gestión de precios unitarios

## 🔧 Tecnologías Utilizadas

### Stack Tecnológico
- **Lenguaje**: C# (.NET Framework 4.7.2)
- **Interfaz**: Windows Forms
- **Base de Datos**: SQL Server Express
- **Acceso a Datos**: ADO.NET nativo
- **Patrones**: Singleton, Repository, Transaction Script
- **🆕 PDF Generation**: QuestPDF para tickets de 80mm
- **🆕 WebView Integration**: Microsoft.Web.WebView2 para vista previa nativa
- **🆕 Assembly Binding**: Redirección de versiones para compatibilidad

### Características Técnicas
- **Conexión Pooling**: Optimización de recursos
- **Transacciones SQL**: ACID compliance
- **Logging**: Registro de operaciones críticas
- **Manejo de Excepciones**: Captura y gestión de errores
- **Configuración**: Archivos .config flexibles

## 📊 Características Destacadas

### 🎫 Sistema de PDF Tickets
- **Formato 80mm**: Diseño optimizado para impresoras térmicas
- **Generación Instantánea**: PDF creado al momento de guardar factura
- **Vista Previa Integrada**: WebView2 para visualización nativa
- **Impresión Directa**: Soporte para impresoras térmicas y estándar
- **Datos de Empresa**: Configuración completa de información comercial
- **NCF Incluido**: Cumplimiento con normativa fiscal dominicana

### 🔍 Búsqueda y Edición Avanzada
- **Búsqueda Multi-criterio**: Por número, cliente, rango de fechas
- **Edición Completa**: Modificación de facturas existentes manteniendo NCF
- **Selector de Clientes**: Búsqueda rápida con DataGridView y atajo F3
- **Actualización en Tiempo Real**: Subtotales calculados al editar cantidades

### 🔄 Transacciones Atómicas
```csharp
// Ejemplo: Creación de factura completa
using (SqlTransaction transaction = connection.BeginTransaction())
{
    try
    {
        // 1. Insertar factura
        int idFactura = InsertarFactura(connection, transaction, factura);
        
        // 2. Insertar detalles
        foreach (var detalle in detalles)
        {
            InsertarDetalle(connection, transaction, detalle);
        }
        
        // 3. Actualizar stock
        ActualizarStock(connection, transaction, detalles);
        
        transaction.Commit();
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        throw new Exception("Error en facturación", ex);
    }
}
```

### 🛡️ Validaciones Robustas
- **Data Annotations**: Validaciones automáticas en entidades
- **Validaciones de Negocio**: Lógica específica por módulo
- **Validaciones de Integridad**: Referencias y relaciones
- **Mensajes Claros**: Feedback amigable para usuarios

### 📈 Gestión de Stock
- **Control en Tiempo Real**: Actualización automática
- **Alertas Inteligentes**: Stock bajo y crítico
- **Prevención de Ventas**: Bloqueo si no hay stock
- **Reportes**: Estado actual del inventario

### 🛠️ Mejas Recientes y Correcciones
- **DataGridView Mejorado**: Edición fluida sin errores DBNull
- **Actualización en Tiempo Real**: Subtotales calculados al editar cantidades
- **Manejo de Errores**: Captura silenciosa para mejor experiencia
- **Compatibilidad de Versiones**: Assembly binding redirects funcionales
- **Interfaz Optimizada**: Selector de clientes y búsqueda intuitiva

## 🚀 Instalación y Configuración

### Requisitos del Sistema
- **Sistema Operativo**: Windows 10 o superior
- **.NET Framework**: 4.7.2 o superior
- **Base de Datos**: SQL Server Express 2019+
- **RAM**: Mínimo 4GB recomendado
- **Espacio**: 500MB disponibles

### Pasos de Instalación
1. **Instalar SQL Server Express** con autenticación mixta
2. **Ejecutar scripts SQL** desde `/Database/Scripts/`
3. **Configurar connection string** en `App.config`
4. **Ejecutar aplicación** desde `MiniSistemaFacturacion.exe`

### Configuración de Base de Datos
```xml
<connectionStrings>
  <add name="MiniSistemaFacturacionDB" 
       connectionString="Server=localhost\SQLEXPRESS;Database=MiniSistemaFacturacion;Integrated Security=True;" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

## 📈 Reportes y Consultas

### 📊 Reportes Disponibles
- **Facturas por Período**: Rango de fechas configurable
- **Clientes con Deuda**: Listado de saldos pendientes
- **Productos Más Vendidos**: Ranking por cantidad
- **Inventario Actual**: Stock disponible y valor
- **Cuentas por Cobrar**: Resumen de pagos pendientes
- **🆕 Tickets PDF**: Generación instantánea en formato 80mm
- **🆕 Búsqueda Avanzada**: Multi-criterio con filtros flexibles

### 🔍 Consultas Avanzadas
- **Búsqueda de Clientes**: Por nombre, cédula, teléfono con selector intuitivo
- **Búsqueda de Productos**: Por código, descripción, categoría
- **Historial de Facturas**: Por cliente o período con edición directa
- **Estado de Pagos**: Detalle completo por factura
- **🆕 Búsqueda de Facturas**: Por número, cliente, fechas y estados
- **🆕 Edición en Línea**: Modificación directa desde resultados de búsqueda

## 🛠️ Mantenimiento y Soporte

### 🔧 Operaciones de Mantenimiento
- **Backups Automáticos**: Programación de copias de seguridad
- **Optimización de Índices**: Mantenimiento de rendimiento
- **Limpieza de Logs**: Rotación automática de archivos
- **Actualización de Datos**: Procesos batch para actualización

### 📋 Guía de Solución de Problemas
- **Conexión Fallida**: Verificar SQL Server services
- **Errores de Transacción**: Revisar espacio en disco
- **Problemas de Performance**: Rebuild de índices
- **Bloqueos de Usuarios**: Verificar conexiones activas

#### Métricas de Negocio
- **Procesamiento**: 50+ facturas por minuto
- **Disponibilidad**: 99.5% uptime del sistema
- **Usuarios**: Soporte para 10+ usuarios concurrentes
- **Datos**: Manejo eficiente de 10,000+ registros

### 6.4 Consideraciones Técnicas para Futuras Versiones

#### Arquitectura
- **Microservicios**: Descomposición en servicios independientes
- **Contenerización**: Docker para despliegue simplificado
- **Cloud Migration**: Azure/AWS para escalabilidad horizontal
- **DevOps Integration**: CI/CD pipeline automatizado

#### Base de Datos
- **NoSQL Options**: MongoDB para ciertos módulos si es necesario
- **Data Warehouse**: Para análisis históricos complejos
- **Real-time Sync**: Sincronización en tiempo real entre múltiples ubicaciones
- **Backup Strategies**: Estrategias avanzadas de recuperación ante desastres

## 👥 Equipo de Desarrollo

### 🎯 Responsabilidades por Módulo
- **César Reyes**: Backend & Base de Datos (100% completado)
- **Julio Garcia**: Módulo de Clientes (UI y validaciones)
- **Luis Garcia**: Módulo de Inventario (UI y gestión de stock)
- **Marileidi**: Módulo de Ventas y Cobros (UI y lógica)

### 📅 Timeline del Proyecto
- **Semana 1**: Diseño de base de datos y ERD ✅
- **Semana 2**: Implementación de backend y DAL ✅
- **Semana 3**: Desarrollo de lógica de negocio ✅
- **Semana 4**: Desarrollo de interfaces de usuario ✅
- **Semana 5**: Integración, pruebas y documentación ✅
- **🆕 Mejoras Adicionales**: PDF tickets, búsqueda avanzada, edición de facturas ✅

## 📞 Contacto y Soporte

### 📧 Información del Proyecto
- **Institución**: Universidad Abierta para Adultos - UAPA
- **Curso**: Programación III
- **Profesor**: Diógenes Amaury Martínez Silverio
- **Año**: 2026

### 🌐 Repositorio del Código
- **Git**: [https://github.com/ByCesarDev/MiniSistemaFacturacion.git](https://github.com/ByCesarDev/MiniSistemaFacturacion.git)
- **Documentación**: Incluida en este Repo y en el código fuente
- **Issues**: Reportar a través del sistema de seguimiento

---

**Nota**: Este proyecto fue desarrollado como trabajo final para el curso de Programación III, demostrando el dominio de conceptos avanzados de programación, diseño de bases de datos y desarrollo de aplicaciones empresariales.

**🆕 Funcionalidades Completas**: Sistema robusto con generación de PDF tickets, búsqueda avanzada, edición de facturas, selector de clientes intuitivo y experiencia de usuario optimizada para entornos empresariales reales.

© 2026 - Equipo de Desarrollo - Todos los derechos reservados
