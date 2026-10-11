
# Master Plan — Sistema de Usuarios para MiniSistemaFacturacion

Repositorio accesible

El objetivo es convertir tu proyecto de Programación III en una versión ampliada que cumpla los requisitos funcionales de Ingeniería de Software II, sin reescribir los módulos existentes ni cambiar de tecnología.

He revisado la estructura del repositorio, el punto de entrada, la configuración, el formulario principal, la conexión a SQL Server, la lógica de facturación y los scripts de base de datos. Hay varios detalles concretos que debemos considerar.

## 1. Diagnóstico técnico

Proyecto actual

Rama master

Lenguaje

C#

Framework

.NET Framework 4.7.2

Interfaz

Windows Forms

Base de datos

SQL Server Express

Acceso a datos

ADO.NET

Arquitectura

Capas

### Hallazgos importantes

1. `Program.cs` inicia directamente `Application.Run(new Forms.FrmMain())`. Actualmente no existe una barrera de autenticación al arrancar.
2. `FrmMain.cs` abre los módulos mediante eventos como `clientesToolStripMenuItem_Click`, `facturaciónToolStripMenuItem_Click` y `reportesToolStripMenuItem_Click`.
3. `DbHelper.cs` centraliza las conexiones y operaciones SQL. Podemos reutilizarlo para `UsuarioDAL`.
4. `App.config` contiene la cadena de conexión `MiniSistemaFacturacionDB`, apuntando a `localhost\SQLEXPRESS` mediante autenticación integrada.
5. Las carpetas `Models`, `BusinessLogic` y `Forms` no contienen todavía las clases de autenticación y administración de usuarios propuestas.
6. `FrmMain.cs` tiene una opción `borrarDatosToolStripMenuItem_Click` que permite eliminar registros de facturas, detalles y pagos. Esta operación debe restringirse exclusivamente al administrador.
7. El proyecto usa un `.csproj` clásico de .NET Framework. Por tanto, agregar archivos `.cs` requiere verificar que estén incluidos en el proyecto, no simplemente crearlos en las carpetas.

Estos hallazgos justifican implementar la autenticación como una extensión de la arquitectura existente.

## 2. Arquitectura objetivo

Program.cs

Inicio de la aplicación

FrmLogin

Validación de credenciales

UsuarioManager + UsuarioDAL

Autenticación, roles y permisos

SQL Server

Usuarios, credenciales y auditoría básica

FrmMain

Menú según permisos del usuario autenticado

La sesión debe existir únicamente después de autenticar correctamente al usuario. Ningún formulario administrativo debería depender de una variable modificable libremente para decidir si alguien es administrador.

## 3. Archivos que agregaremos

```
MiniSistemaFacturacion/
├── Models/
│   ├── Usuario.cs                    [NUEVO]
│   └── RolUsuario.cs                 [NUEVO]
├── DataAccess/
│   └── UsuarioDAL.cs                 [NUEVO]
├── BusinessLogic/
│   ├── UsuarioManager.cs             [NUEVO]
│   └── PasswordHasher.cs             [NUEVO]
├── Configuration/
│   └── SesionActual.cs               [NUEVO]
├── Forms/
│   ├── FrmLogin.cs                   [NUEVO]
│   ├── FrmLogin.Designer.cs          [NUEVO]
│   ├── FrmUsuarios.cs                [NUEVO]
│   ├── FrmUsuarios.Designer.cs       [NUEVO]
│   ├── FrmEditarUsuario.cs           [OPCIONAL]
│   ├── FrmMain.cs                    [MODIFICAR]
│   └── FrmMain.Designer.cs           [MODIFICAR]
├── Database/
│   └── 04_Usuarios.sql               [NUEVO]
├── Program.cs                        [MODIFICAR]
└── MiniSistemaFacturacion.csproj     [MODIFICAR]
```

Mantendremos intactos los módulos de clientes, productos, facturación, pagos y reportes, excepto donde sea necesario proteger operaciones sensibles.

## 4. Plan de implementación por fases

Fase 1 — Base de datos

Prioridad alta

Crear `04_Usuarios.sql` como migración independiente, sin modificar ni borrar las tablas existentes.

La tabla `Usuarios` contendrá `ID_Usuario`, `NombreCompleto`, `NombreUsuario`, `PasswordHash`, `PasswordSalt`, `Rol`, `Estado`, `FechaCreacion` y `UltimoAcceso`.

`NombreUsuario` tendrá una restricción `UNIQUE`, `Rol` admitirá únicamente `Administrador` o `Empleado` y `Estado` permitirá desactivar cuentas sin eliminarlas.

Utilizar PBKDF2-HMAC-SHA256 con salt aleatorio por usuario, parámetros versionados y comparación de hashes en tiempo constante. No almacenar contraseñas en texto plano.

Para crear el primer administrador, utilizar un asistente de configuración inicial disponible únicamente cuando no exista ningún usuario, con una contraseña elegida por quien instala el sistema. Evitar credenciales predeterminadas.

Fase 2 — Lógica de autenticación

Implementar `Usuario`, `RolUsuario`, `UsuarioDAL`, `UsuarioManager` y `PasswordHasher`.

Métodos principales:

```
Autenticar(usuario, contraseña)
CrearUsuario(datos, contraseña)
ActualizarUsuario(datos)
CambiarContraseña(id, contraseña)
DesactivarUsuario(id)
ListarUsuarios()
```

Validaciones: nombres únicos, contraseñas suficientemente largas, cuentas activas, intentos fallidos limitados y prohibición de desactivar al último administrador activo.

Las consultas deben ser parametrizadas. No devolver hashes ni salts a los formularios administrativos.

Fase 3 — Pantalla de inicio de sesión

Crear `FrmLogin` siguiendo el estilo de Windows Forms del sistema actual.

Elementos: campo de usuario, contraseña enmascarada, botón Iniciar sesión, opción Mostrar contraseña y mensajes de error claros.

Modificar `Program.cs` para abrir `FrmMain` únicamente después de una autenticación válida. Al cerrar sesión, regresar al login sin mantener abiertos formularios privilegiados.

No permitir que cerrar el login sin autenticarse abra el menú principal.

Fase 4 — Administración de usuarios

Crear `FrmUsuarios` con un `DataGridView` y las siguientes operaciones:

- Buscar y listar usuarios.
- Agregar usuarios.
- Editar nombre y rol.
- Activar o desactivar cuentas.
- Restablecer contraseñas.

Solo los administradores podrán abrir este formulario.

No incluir eliminación física de usuarios ni mostrar contraseñas existentes.

Fase 5 — Integración de permisos

Modificar `FrmMain.cs` y `FrmMain.Designer.cs` para agregar:

- Menú Administración → Usuarios.
- Nombre y rol del usuario autenticado.
- Opción Cerrar sesión.
- Visibilidad de opciones según permisos.

Proteger especialmente `borrarDatosToolStripMenuItem_Click` y `configuracionEmpresaToolStripMenuItem_Click`.

Además, comprobar permisos antes de ejecutar operaciones privilegiadas, incluso si alguien intenta abrir directamente un formulario.

Como el sistema se conecta a SQL Server mediante autenticación integrada, los permisos de aplicación no sustituyen los permisos de la base de datos. Para una instalación real habría que limitar también los privilegios de la cuenta de conexión.

Fase 6 — Pruebas y regresión

Compilar el proyecto y probar tanto el módulo nuevo como las funcionalidades anteriores.

Probar autenticación, permisos, gestión de usuarios, facturación, inventario, pagos, reportes y conexión SQL Server.

Verificar también que un error de autenticación no cierre inesperadamente la aplicación y que los datos existentes permanezcan intactos.

## 5. Matriz de permisos

Esta es la configuración que recomiendo inicialmente.

| Módulo / Operación       | Administrador | Empleado |
| -------------------------- | ------------- | -------- |
| Consultar clientes         | Sí           | Sí      |
| Crear y editar clientes    | Sí           | Sí      |
| Gestionar productos        | Sí           | Sí      |
| Crear facturas             | Sí           | Sí      |
| Registrar pagos            | Sí           | Sí      |
| Consultar reportes         | Sí           | Sí      |
| Configuración de empresa  | Sí           | No       |
| Borrar datos masivamente   | Sí           | No       |
| Administrar usuarios       | Sí           | No       |
| Cambiar contraseña propia | Sí           | Sí      |

Para evitar duplicar comprobaciones, centralizaremos las autorizaciones en un servicio de permisos y no en condiciones dispersas por cada botón.

## 6. Puntos técnicos que debemos corregir o vigilar

Durante la inspección encontré algunos detalles que afectan directamente a la implementación.

Primero: el proyecto tiene dos mecanismos relacionados con la cadena de conexión. `DbHelper` y `ConnectionStringManager` leen la configuración. No crearía una tercera forma de conectarse a SQL Server; `UsuarioDAL` debe utilizar `DbHelper.Instance`.

Segundo: `App.config` incluye un proveedor de membership y roles de `System.Web.ClientServices`. Esto no significa que el proyecto ya tenga un sistema funcional de usuarios. La implementación propuesta será independiente, para evitar depender de un servicio de autenticación externo que no está configurado.

Tercero: el archivo de proyecto contiene referencias explícitas a las clases y formularios. Debemos registrar los nuevos archivos y sus correspondientes recursos de diseñador. De lo contrario, pueden existir físicamente y aun así no compilarse.

Cuarto: hay una discrepancia de configuración tributaria. `App.config` declara `PorcentajeImpuestoDefecto=15`, mientras que `Database/01_CreateTables.sql` utiliza 18.00 como valor predeterminado. No es parte del sistema de usuarios, pero conviene comprobar qué valor prevalece realmente durante la facturación antes de grabar el video.

Quinto: la opción de borrado masivo del menú principal es especialmente sensible. Actualmente tiene confirmación visual, pero no comprobación de rol en el controlador revisado. Esto debe corregirse como parte de la integración.

## 7. Pruebas de aceptación

Estas son las pruebas que exigiría antes de considerar terminado el módulo.

Checklist de validación

0/12 aprobadas

AUTH-01

&#x20;Administrador inicia sesión con credenciales válidas

AUTH-02

&#x20;Contraseña incorrecta impide el acceso

AUTH-03

&#x20;Usuario desactivado no puede iniciar sesión

AUTH-04

&#x20;Empleado no puede administrar usuarios

AUTH-05

&#x20;Empleado no puede borrar datos masivamente

USR-01

&#x20;Administrador crea una cuenta nueva

USR-02

&#x20;No se admiten nombres de usuario duplicados

USR-03

&#x20;No se puede desactivar al último administrador

USR-04

&#x20;Cambio de contraseña invalida la anterior

SES-01

&#x20;Cerrar sesión impide reutilizar formularios privilegiados

REG-01

&#x20;Facturación e inventario siguen funcionando

REG-02

&#x20;Pagos y reportes mantienen su funcionamiento

## 8. Orden exacto de ejecución para Antigravity

Si vas a encargarle la implementación a Antigravity, este sería el prompt maestro. Está diseñado para que inspeccione todo el repositorio antes de modificarlo y evite introducir regresiones.

Prompt maestro de implementación


Actúa como ingeniero senior especializado en C#, Windows Forms, .NET Framework 4.7.2 y SQL Server.

Repositorio: https://github.com/ByCesarDev/MiniSistemaFacturacion
Objetivo: implementar un sistema completo de autenticación y administración de usuarios, preservando todas las funcionalidades existentes.

FASE 0 - AUDITORÍA

1. Inspecciona TODOS los archivos reales del repositorio, incluidos .csproj, formularios, designers, DAL, lógica, modelos, scripts SQL y configuración.
2. Identifica dependencias, convenciones, puntos de entrada y riesgos de regresión.
3. No asumas que los archivos descritos en README existen; verifica el árbol real.
4. Establece una línea base de compilación y reporta errores preexistentes.

FASE 1 - BASE DE DATOS

1. Crear Database/04_Usuarios.sql, migración repetible y no destructiva.
2. Tabla Usuarios con identificador, nombre completo, usuario único, hash, salt, parámetros de hash versionados, rol, estado, fechas de creación y último acceso.
3. Roles Administrador y Empleado, con constraints e índices.
4. No borrar ni alterar registros comerciales existentes.
5. Implementar inicialización segura del primer administrador cuando no haya usuarios, sin contraseña fija.

FASE 2 - BACKEND
Crear Models/Usuario.cs, Models/RolUsuario.cs, DataAccess/UsuarioDAL.cs, BusinessLogic/UsuarioManager.cs, BusinessLogic/PasswordHasher.cs y Configuration/SesionActual.cs.
Utilizar consultas SQL parametrizadas y DbHelper.Instance.
Usar PBKDF2-HMAC-SHA256 con salt criptográficamente aleatorio, iteraciones configuradas y versionadas, y comparación en tiempo constante.
No registrar contraseñas ni hashes en logs.
Implementar autenticación, CRUD lógico de usuarios, cambio de contraseña, validaciones, bloqueo temporal por intentos fallidos y protección del último administrador.
Mantener separación de responsabilidades.

FASE 3 - INTERFAZ
Crear FrmLogin y FrmUsuarios con sus archivos Designer y recursos necesarios.
Mantener estilo visual compatible con Windows Forms del proyecto.
FrmLogin: usuario, contraseña enmascarada, mostrar contraseña, ingresar y errores claros.
FrmUsuarios: DataGridView, buscar, crear, editar, activar/desactivar, restablecer contraseña.
No permitir acceso a FrmUsuarios sin permisos.

FASE 4 - INTEGRACIÓN
Modificar Program.cs para exigir autenticación antes de abrir FrmMain.
Implementar sesión de usuario con cierre de sesión seguro.
Modificar FrmMain y su Designer para añadir Administración > Usuarios, identidad del usuario y Cerrar sesión.
Aplicar roles a navegación y operaciones sensibles.
Proteger obligatoriamente el borrado masivo y la configuración empresarial.
No confiar solo en ocultar botones: validar autorización antes de operaciones privilegiadas.

FASE 5 - COMPATIBILIDAD
Actualizar MiniSistemaFacturacion.csproj incluyendo todos los nuevos archivos y metadatos de WinForms.
No migrar de .NET Framework 4.7.2.
No cambiar SQL Server por otra base de datos.
No refactorizar módulos existentes sin necesidad.
No modificar la lógica fiscal o de facturación salvo correcciones indispensables y justificadas.
Mantener compatibilidad con la estructura actual.

FASE 6 - VALIDACIÓN
Compilar la solución en el entorno Windows compatible.
Ejecutar pruebas de login, contraseñas, usuarios desactivados, roles, último administrador y cierre de sesión.
Ejecutar pruebas de regresión para clientes, productos, facturas, pagos y reportes.
Verificar migración SQL en base de datos existente y base limpia.
Documentar resultados reales y errores pendientes; no declarar pruebas aprobadas sin ejecutarlas.

ENTREGABLES

1. Código completo integrado.
2. Script SQL de migración.
3. Instrucciones de instalación y primer acceso.
4. Lista exacta de archivos creados y modificados.
5. Informe de pruebas con resultados comprobados.
6. Resumen de decisiones técnicas.
7. Commits separados por fase.

Trabaja por fases y verifica la compilación antes de considerar terminada la implementación. No sobrescribas ni borres datos existentes.

## 9. Criterios de finalización

Consideraremos terminada la adaptación cuando se cumplan estas condiciones:

- El sistema no abre `FrmMain` sin autenticación.
- El administrador puede gestionar usuarios y permisos.
- El empleado puede trabajar con los módulos autorizados sin acceder a funciones administrativas.
- Las contraseñas están protegidas y no existe una cuenta administrativa con contraseña fija.
- La migración SQL no destruye información existente.
- Los formularios nuevos están correctamente incluidos en el proyecto.
- La aplicación compila y las pruebas funcionales y de regresión se ejecutan satisfactoriamente.

Alcance de la revisión: pude inspeccionar directamente los principales puntos de integración y los archivos estructurales del repositorio, pero no he ejecutado la solución en Windows ni auditado línea por línea todos sus archivos. Por eso el plan exige una auditoría inicial completa antes de editar y una compilación real al finalizar.

Con este plan no necesitamos reconstruir MiniSistemaFacturacion. La implementación se concentra en una nueva funcionalidad, su integración y la seguridad de acceso, mientras que SCRUM, los diagramas y el informe quedan como entregables académicos separados.
