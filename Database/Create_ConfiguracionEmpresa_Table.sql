-- =============================================
-- Author: Cesar Reyes
-- Create date: 2026-04-12
-- Description: Script para crear la tabla de Configuración de Empresa
-- =============================================

-- Crear tabla para almacenar la configuración de la empresa
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ConfiguracionEmpresa' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[ConfiguracionEmpresa] (
        [ID] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Nombre] [nvarchar](200) NOT NULL,
        [Direccion] [nvarchar](500) NULL,
        [Telefono] [nvarchar](50) NULL,
        [Email] [nvarchar](100) NULL,
        [RNC] [nvarchar](20) NOT NULL,
        [NCFActual] [nvarchar](20) NOT NULL,
        [NCFConsumidorFinal] [nvarchar](20) NOT NULL,
        [RutaPdfTickets] [nvarchar](500) NOT NULL,
        [FechaCreacion] [datetime] NOT NULL DEFAULT GETDATE(),
        [FechaActualizacion] [datetime] NOT NULL DEFAULT GETDATE(),
        [Activo] [bit] NOT NULL DEFAULT 1
    )
    
    PRINT 'Tabla ConfiguracionEmpresa creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla ConfiguracionEmpresa ya existe.'
END

-- Insertar configuración por defecto si no existe
IF NOT EXISTS (SELECT * FROM [dbo].[ConfiguracionEmpresa] WHERE Activo = 1)
BEGIN
    INSERT INTO [dbo].[ConfiguracionEmpresa] (
        [Nombre], 
        [Direccion], 
        [Telefono], 
        [Email], 
        [RNC], 
        [NCFActual], 
        [NCFConsumidorFinal], 
        [RutaPdfTickets]
    )
    VALUES (
        'Mi Empresa S.A.',
        'Dirección no configurada',
        'Teléfono no configurado',
        'email@empresa.com',
        '123456789',
        'B0100000001',
        'B0100000001',
        './TicketsPDF/'
    )
    
    PRINT 'Configuración por defecto insertada exitosamente.'
END
ELSE
BEGIN
    PRINT 'Ya existe una configuración activa en la base de datos.'
END

-- Crear índice para búsquedas rápidas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ConfiguracionEmpresa_Activo' AND object_id = OBJECT_ID('[dbo].[ConfiguracionEmpresa]'))
BEGIN
    CREATE INDEX [IX_ConfiguracionEmpresa_Activo] ON [dbo].[ConfiguracionEmpresa] ([Activo])
    PRINT 'Índice IX_ConfiguracionEmpresa_Activo creado.'
END

PRINT 'Proceso de creación de tabla ConfiguracionEmpresa completado.'
