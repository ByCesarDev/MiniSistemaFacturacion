# Changelog

## [1.2.0](https://github.com/ByCesarDev/MiniSistemaFacturacion/compare/v1.1.0...v1.2.0) (2026-10-11)


### Features

* **db:** asistente de configuracion de conexion, migraciones versionadas y proteccion dpapi ([d9de03a](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/d9de03adfb92fc78a567acfd10f38a2a39db9048))
* **fase-1:** migracion sql de usuarios y correccion de tasa de impuesto a 18% ([145e554](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/145e554db026d7ead6f37d49f63612469f17fd7f))
* **fase-2:** backend de autenticacion, pbkdf2, usuariomanager, dal y sesion ([2f2824b](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/2f2824b0b6faa807d3d43438336ad9c0ea1037df))
* **fase-3:** pantalla de login frmlogin, asistente inicial y control en program.cs ([746c39f](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/746c39f8ab4b3ec30cd26c2788da5e7f28b446fb))
* **fase-4:** formulario frmusuarios con datagridview, crud y reseteo de claves ([413a6ae](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/413a6ae5f3f4a5d3e2eba951d5fe66f31a823ba6))
* **fase-5:** integracion de permisos, menu de administracion, logout y statusstrip en frmmain ([ba3a438](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/ba3a43826c29b323cc81147c782ce57a06b83880))
* **installer:** completar Fase 2 con verificacion oficial SQL, conectividad LAN y blindaje UAC ([fe8b93b](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/fe8b93b198cc3c073186ab25852d27fcbd487f8c))
* **installer:** implementar instalador personalizado con Inno Setup 6 e integracion CI ([b5724c2](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/b5724c2e47d6b22387f62de2cffc132368f3301c))
* **ui:** ampliacion de clientes y usuarios, reorganizacion de busqueda de facturas y dashboard corporativo en frmmain ([6ff5deb](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/6ff5debdf47b407e726f52fc6c1a1c65ba98cd6d))
* **ui:** boton cambiar clave en header, eliminacion de submenu duplicado y creacion de frmcambiarpassword ([e1511a9](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/e1511a9c155bf69bc7aa12696bc4118c44b21071))
* **ui:** doble clic y soporte de enter para registrar abonos en frmcuentasporcobrar ([54d805a](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/54d805a02d1542a1fe6e89aa1d6fd62bfd4c8684))
* **ui:** modernizacion integral de interfaces con UITheme Slate Navy ([415e85e](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/415e85e23b6a7512526557940264089581d0b4c9))


### Bug Fixes

* **db:** blindar motor de migraciones, persistencia DPAPI y asistente de conexion ([080df75](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/080df75256106d32e5c3368b90674f7ffa453359))
* **security:** configurar ACLs explicitas en ProgramData para soporte de usuarios estandar ([1f2fed3](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/1f2fed325a0d92dac860fa58081a91f8857844c3))
* **security:** restringir permisos de ProgramData a solo lectura para usuarios estandar y cifrado DPAPI mandatorio ([df517b1](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/df517b109e7070afc38299b2314eda7e4721030f))
* **ui:** asignacion de icono a frmusuarios y alineacion completa de grilla en frmclientes ([6594afd](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/6594afd26dd88cf982e256cf036f513d0fb8b60f))

## [1.1.0](https://github.com/ByCesarDev/MiniSistemaFacturacion/compare/v1.0.0...v1.1.0) (2026-04-14)


### Features

* first automated release ([569b72a](https://github.com/ByCesarDev/MiniSistemaFacturacion/commit/569b72aeaa4fbb554a13a5ee06c0086c723056a3))

## Changelog


Todos los cambios importantes se documentarán aquí.
