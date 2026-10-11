# 🛒 MiniSistemaFacturacion

> **Software libre y gratuito de facturación, control de inventario y cuentas por cobrar para pequeños y medianos comercios.**  
> Diseñado para ser rápido, confiable, fácil de usar y listo para trabajar en una sola computadora o en red local multi-caja.

[![Release](https://img.shields.io/badge/Versión-v1.2.0-blue.svg?style=flat-square)](https://github.com/ByCesarDev/MiniSistemaFacturacion/releases)
[![Plataforma](https://img.shields.io/badge/Plataforma-Windows_10_|_11-0078D6.svg?style=flat-square&logo=windows)](https://www.microsoft.com/windows)
[![Framework](https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4.svg?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Base de Datos](https://img.shields.io/badge/Base_de_Datos-SQL_Server_Express-CC292B.svg?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![Licencia](https://img.shields.io/badge/Licencia-Software_Libre-brightgreen.svg?style=flat-square)](LICENSE)

---

## 🌟 ¿Qué es MiniSistemaFacturacion?

**MiniSistemaFacturacion** es un sistema de escritorio empresarial de código abierto creado para resolver el día a día de tiendas, colmados, farmacias, ferreterías, talleres y comercios independientes.

A diferencia de otros sistemas que requieren suscripciones mensuales costosas, configuraciones complejas o conexión permanente a internet, este software se instala directamente en tu equipo, guarda tus datos bajo tu control y funciona con total agilidad desde el primer minuto.

```
┌────────────────────────────────────────────────────────────────────────┐
│                   Mini Sistema de Facturación                          │
├───────────────────┬───────────────────┬────────────────────────────────┤
│ 🧾 Facturación    │ 📦 Inventario     │ 👥 Clientes y Deudas           │
│ Tickets PDF 80mm  │ Stock en vivo     │ Pagos parciales y abonos       │
├───────────────────┼───────────────────┼────────────────────────────────┤
│ 🖥️ Servidor       │ 💻 Terminales     │ 📊 Reportes en Vivo            │
│ Base central local│ Cajas en red LAN  │ Ventas del día y estadísticas  │
└───────────────────┴───────────────────┴────────────────────────────────┘
```

---

## ✨ Características Principales

### 🧾 1. Punto de Venta y Facturación Inmediata
* **Emisión en segundos:** Agrega productos con un clic o por código, calcula subtotales automáticamente y emite facturas al instante.
* **Cálculo de impuestos:** Soporte automático para **18% ITBIS / IVA** y desglose fiscal transparente.
* **Comprobantes fiscales (NCF):** Asignación correlativa de comprobantes (Consumidor Final, Crédito Fiscal, etc.).
* **Tickets térmicos en PDF (80mm):** Generación automática de comprobantes listos para impresoras térmicas de punto de venta o estándar, con vista previa integrada para imprimir o exportar.

### 📦 2. Control de Inventario y Productos
* Catálogo completo con código, descripción, precio y existencias.
* **Alertas de stock mínimo:** Avisos visuales automáticos cuando un producto se está agotando.
* Descuento automático de existencias con cada venta para mantener tu inventario siempre cuadrado.

### 💳 3. Cuentas por Cobrar y Gestión de Abonos
* Venta a crédito y seguimiento detallado de saldos pendientes por cliente.
* Registro de **abonos parciales** o pagos completos en efectivo, tarjeta o transferencia.
* Historial de pagos por factura con cambio de estado automático (*Pendiente*, *Parcial*, *Pagada*).
* Atajos rápidos: pulsa **Enter** o haz **doble clic** sobre cualquier deuda para abonar de inmediato.

### 👥 4. Directorio de Clientes
* Registro de clientes con Cédula, RNC, dirección y teléfono.
* Historial de compras y deudas por cliente.
* Búsqueda instantánea con la tecla **F3** dentro del punto de venta.

### 📊 5. Dashboard y Reportes Comerciales
* **Métricas del día:** Total facturado hoy, dinero por cobrar, volumen de ventas y balance de caja.
* **Gráficos interactivos:** Productos más vendidos, evolución de ventas y estado de cartera de clientes.
* Exportación y consulta histórica de facturas con reimpresión en cualquier momento.

### 🛡️ 6. Seguridad y Perfiles de Usuario
* **Roles definidos:** 
  * **Administrador:** Control total, reportes, usuarios, configuración del negocio y base de datos.
  * **Cajero / Vendedor:** Acceso exclusivo a facturar, registrar pagos y consultar productos, sin acceso a configuraciones críticas.
* Contraseñas protegidas mediante algoritmos de alta seguridad (PBKDF2) y cierre de sesión seguro.
* Asistente automático de primer uso: crea tu primer usuario Administrador en menos de 30 segundos.

---

## 🌐 Funciona en Una Sola Computadora o en Red Local (LAN)

El sistema está preparado para adaptarse al tamaño de tu negocio:

* **Modo Caja Única (Servidor):** Instálalo en una sola computadora. El sistema administra su base de datos localmente y funciona sin necesidad de internet.
* **Modo Multi-Caja (Red Local):** Si tienes varias cajas o computadoras en tu local:
  1. Instala el sistema como **Servidor Principal** en la computadora central.
  2. Instala el sistema como **Terminal Cliente** en las demás computadoras indicando la IP del servidor.
  3. ¡Todas las cajas facturan compartiendo el mismo inventario y clientes en tiempo real!

> [!NOTE]
> En Modo Servidor, el instalador configura automáticamente las reglas seguras del Firewall de Windows (restringidas a tu red privada), para que no tengas que pelear con configuraciones de red complejas.

---

## 📥 ¿Cómo Instalarlo? (En 3 Pasos Sencillos)

¡Olvídate de abrir herramientas complicadas o ejecutar código! El instalador se encarga de todo:

1. **Descarga el instalador:**  
   Ve a la sección de [Releases de GitHub](https://github.com/ByCesarDev/MiniSistemaFacturacion/releases) y descarga el archivo:  
   👉 **`MiniSistemaFacturacion_Setup_v1.2.0.exe`**

2. **Ejecuta el asistente de instalación:**  
   Haz doble clic sobre el archivo descargado y sigue las instrucciones en pantalla.

3. **Elige el modo de tu equipo:**
   * Elige **Modo Servidor** si es la computadora principal de tu negocio.
   * Elige **Modo Terminal Cliente** si es una computadora secundaria que se conectará a la caja principal en red.

¡Y listo! Al terminar la instalación, el sistema creará un acceso directo en tu escritorio para abrir la aplicación.

---

## 🏁 Guía Rápida de Primer Uso

Una vez instalado, poner tu negocio en marcha toma menos de 5 minutos:

1. **Crea tu primer Administrador:**  
   Al abrir la aplicación por primera vez, el sistema detectará que es una instalación nueva y te mostrará el *Asistente Inicial*. Ingresa tu nombre, usuario y contraseña.
2. **Configura los datos de tu negocio:**  
   Inicia sesión como Administrador y haz clic en **Configuración**. Ingresa el Nombre Comercial, RNC/Cédula, Dirección, Teléfono y el mensaje de agradecimiento que saldrá en tus tickets impresos.
3. **Agrega tus primeros productos y clientes:**  
   Entra a **Productos** para registrar lo que vendes con su precio y stock. Entra a **Clientes** para agregar a tus compradores habituales.
4. **¡Listo para facturar!**  
   Entra a **Facturación**, selecciona los productos, elige si es al contado o a crédito, ¡y genera tu primer comprobante de venta!

---

## ⌨️ Atajos de Teclado Útiles

Diseñado para que los cajeros cobren a máxima velocidad:

| Tecla / Acción | Dónde se usa | Qué hace |
| :--- | :--- | :--- |
| **`F3`** | Facturación | Abre la búsqueda rápida de clientes. |
| **`Enter`** | Cuentas por Cobrar | Abre directamente la ventana para registrar un abono sobre la factura seleccionada. |
| **Doble Clic** | Cuentas por Cobrar | Registra un abono inmediato a la factura elegida. |
| **Doble Clic** | Buscar Facturas | Abre la vista previa e impresión del ticket PDF. |
| **Doble Clic** | Clientes / Productos | Abre la edición del registro seleccionado. |

---

## 🖥️ Requisitos del Sistema

* **Sistema Operativo:** Windows 10, Windows 11 o Windows Server 2016+ (64 bits).
* **Entorno de ejecución:** Microsoft .NET Framework 4.7.2 o superior *(ya viene incluido en Windows 10 y 11 modernos)*.
* **Motor de Base de Datos:** Microsoft SQL Server Express 2019 / 2022 *(gratuito de Microsoft, requerido únicamente en la computadora Servidor)*.
* **Memoria RAM:** 2 GB mínimo (4 GB o más recomendado).
* **Espacio en Disco:** 300 MB libres para el sistema y sus datos.

---

## 🛠️ Para Desarrolladores y Contribuyentes

¿Eres programador y quieres estudiar el código, mejorarlo o colaborar con el proyecto? ¡Eres más que bienvenido!

### Tecnologías Utilizadas
* **Lenguaje:** C# (.NET Framework 4.7.2 / Windows Forms)
* **Base de Datos:** SQL Server (ADO.NET relacional optimizado)
* **Generación de PDFs:** [QuestPDF](https://www.questpdf.com/) (diseño fluido de tickets térmicos)
* **Visor de Documentos:** [Microsoft WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (motor Edge Chromium integrado)
* **Gráficos:** [LiveCharts](https://lvcharts.net/) (gráficos interactivos en tiempo real)
* **Instalador:** [Inno Setup 6](https://jrsoftware.org/isinfo.php) con compilación continua en GitHub Actions

### Pasos para compilar el proyecto localmente
1. Clona el repositorio:
   ```bash
   git clone https://github.com/ByCesarDev/MiniSistemaFacturacion.git
   ```
2. Abre la solución `MiniSistemaFacturacion.slnx` con **Visual Studio 2022**.
3. Restaura los paquetes NuGet (`Clic derecho en la solución -> Restaurar paquetes NuGet`).
4. Compila y ejecuta con `F5` o `Ctrl + F5`. El sistema levantará automáticamente el asistente de base de datos en su primer arranque.

---

## 🤝 ¿Cómo Contribuir?

Las contribuciones de la comunidad son las que hacen grande al software libre:

1. Haz un **Fork** de este repositorio.
2. Crea una rama para tu mejora (`git checkout -b feature/nueva-funcionalidad`).
3. Realiza tus cambios y haz commit (`git commit -m 'feat: agregar soporte para lectores de código de barra'`).
4. Sube tu rama (`git push origin feature/nueva-funcionalidad`).
5. Abre un **Pull Request** explicando tu propuesta.

---

## 📜 Licencia y Créditos

Este proyecto es software libre y de código abierto. Eres libre de usarlo en tu negocio, modificarlo, adaptarlo a tus necesidades y compartirlo.

* **Desarrollado y mantenido por:** **César Adrián Reyes Dinzey (ByCesarDev)**  
* **Colaboradores del proyecto:** Julio García, Luis García, Marileidi.  
* **Institución:** Universidad Abierta para Adultos (UAPA) — *Ingeniería de Software*.

---

<p align="center">
  <b>¿Te ha sido útil este sistema para tu negocio o estudios?</b><br>
  ¡No olvides dejarle una ⭐ estrella al repositorio en GitHub para apoyar su desarrollo!
</p>
