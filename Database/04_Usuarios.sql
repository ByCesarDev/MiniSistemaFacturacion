-- =====================================================================
-- Mini-Sistema de Facturación y Cuentas por Cobrar
-- Script de Migración: 04_Usuarios.sql
-- Propósito: Estructura de Usuarios, Roles, Seguridad y Auditoría
-- =====================================================================

USE [MiniSistemaFacturacion];
GO

-- =====================================================================
-- 1. Tabla: Usuarios
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuarios](
        [ID_Usuario] [int] IDENTITY(1,1) NOT NULL,
        [NombreCompleto] [nvarchar](150) NOT NULL,
        [NombreUsuario] [nvarchar](50) NOT NULL,
        [PasswordHash] [nvarchar](256) NOT NULL,
        [PasswordSalt] [nvarchar](128) NOT NULL,
        [HashIterations] [int] NOT NULL CONSTRAINT [DF_Usuarios_HashIterations] DEFAULT (100000),
        [HashAlgorithm] [nvarchar](50) NOT NULL CONSTRAINT [DF_Usuarios_HashAlgorithm] DEFAULT ('PBKDF2-SHA256'),
        [Rol] [nvarchar](20) NOT NULL,
        [Estado] [nvarchar](20) NOT NULL CONSTRAINT [DF_Usuarios_Estado] DEFAULT ('Activo'),
        [IntentosFallidos] [int] NOT NULL CONSTRAINT [DF_Usuarios_IntentosFallidos] DEFAULT (0),
        [BloqueadoHasta] [datetime] NULL,
        [FechaCreacion] [datetime] NOT NULL CONSTRAINT [DF_Usuarios_FechaCreacion] DEFAULT (GETDATE()),
        [UltimoAcceso] [datetime] NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED ([ID_Usuario] ASC),
        CONSTRAINT [UQ_Usuarios_NombreUsuario] UNIQUE NONCLUSTERED ([NombreUsuario] ASC),
        CONSTRAINT [CK_Usuarios_Rol] CHECK ([Rol] IN ('Administrador', 'Empleado')),
        CONSTRAINT [CK_Usuarios_Estado] CHECK ([Estado] IN ('Activo', 'Inactivo', 'Bloqueado'))
    );

    PRINT 'Tabla [dbo].[Usuarios] creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla [dbo].[Usuarios] ya existe.';
END
GO

-- =====================================================================
-- 2. Índices para optimización de consultas de autenticación y roles
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Usuarios_NombreUsuario' AND object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Usuarios_NombreUsuario]
    ON [dbo].[Usuarios] ([NombreUsuario])
    INCLUDE ([ID_Usuario], [NombreCompleto], [PasswordHash], [PasswordSalt], [Rol], [Estado], [IntentosFallidos], [BloqueadoHasta]);

    PRINT 'Índice [IX_Usuarios_NombreUsuario] creado exitosamente.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Usuarios_Rol_Estado' AND object_id = OBJECT_ID(N'[dbo].[Usuarios]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Usuarios_Rol_Estado]
    ON [dbo].[Usuarios] ([Rol], [Estado]);

    PRINT 'Índice [IX_Usuarios_Rol_Estado] creado exitosamente.';
END
GO

-- =====================================================================
-- 3. Tabla: AuditoriaUsuarios (Registro de eventos de seguridad y sesiones)
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AuditoriaUsuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AuditoriaUsuarios](
        [ID_Auditoria] [int] IDENTITY(1,1) NOT NULL,
        [ID_Usuario] [int] NULL,
        [NombreUsuario] [nvarchar](50) NOT NULL,
        [Accion] [nvarchar](100) NOT NULL, -- 'InicioSesionExitoso', 'IntentoFallido', 'CierreSesion', 'UsuarioCreado', 'UsuarioModificado', 'ContrasenaCambiada', 'UsuarioDesactivado'
        [Detalles] [nvarchar](255) NULL,
        [Fecha] [datetime] NOT NULL CONSTRAINT [DF_AuditoriaUsuarios_Fecha] DEFAULT (GETDATE()),
        [Equipo] [nvarchar](100) NULL,
        CONSTRAINT [PK_AuditoriaUsuarios] PRIMARY KEY CLUSTERED ([ID_Auditoria] ASC),
        CONSTRAINT [FK_AuditoriaUsuarios_Usuarios] FOREIGN KEY ([ID_Usuario]) 
            REFERENCES [dbo].[Usuarios] ([ID_Usuario]) ON DELETE SET NULL
    );

    PRINT 'Tabla [dbo].[AuditoriaUsuarios] creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla [dbo].[AuditoriaUsuarios] ya existe.';
END
GO

-- =====================================================================
-- 4. Procedimiento auxiliar para verificar estado inicial de usuarios
-- =====================================================================
IF OBJECT_ID(N'[dbo].[sp_ExisteAdministradorActivo]', N'P') IS NOT NULL
    DROP PROCEDURE [dbo].[sp_ExisteAdministradorActivo];
GO

CREATE PROCEDURE [dbo].[sp_ExisteAdministradorActivo]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) AS TotalAdmins
    FROM [dbo].[Usuarios]
    WHERE [Rol] = 'Administrador' AND [Estado] = 'Activo';
END
GO

PRINT 'Procedimiento [dbo].[sp_ExisteAdministradorActivo] creado.';
GO
