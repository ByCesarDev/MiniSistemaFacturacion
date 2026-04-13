-- =====================================================
-- Script de Actualización para Soporte NCF
-- Sistema de Facturación
-- Created by: Cesar Reyes
-- Date: 2026-04-11
-- =====================================================

USE [MiniSistemaFacturacion];
GO

-- =====================================================
-- 1. Agregar campos NCF a tabla Facturas
-- =====================================================

-- Verificar si los campos ya existen antes de agregar
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Facturas' AND COLUMN_NAME = 'NCF')
BEGIN
    PRINT 'Agregando campo NCF a la tabla Facturas...';
    ALTER TABLE Facturas 
    ADD NCF NVARCHAR(19) NULL;
    
    PRINT 'Campo NCF agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo NCF ya existe en la tabla Facturas.';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Facturas' AND COLUMN_NAME = 'TipoComprobante')
BEGIN
    PRINT 'Agregando campo TipoComprobante a la tabla Facturas...';
    ALTER TABLE Facturas 
    ADD TipoComprobante NVARCHAR(2) NULL;
    
    PRINT 'Campo TipoComprobante agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo TipoComprobante ya existe en la tabla Facturas.';
END
GO

-- =====================================================
-- 2. Crear índices para los nuevos campos
-- =====================================================

-- Índice para NCF (para búsquedas rápidas)
IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_Facturas_NCF' AND object_id = OBJECT_ID('Facturas'))
BEGIN
    PRINT 'Creando índice IX_Facturas_NCF...';
    CREATE NONCLUSTERED INDEX [IX_Facturas_NCF] ON [dbo].[Facturas]
    (
        [NCF] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
           DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
    ON [PRIMARY];
    
    PRINT 'Índice IX_Facturas_NCF creado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El índice IX_Facturas_NCF ya existe.';
END
GO

-- Índice para TipoComprobante
IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_Facturas_TipoComprobante' AND object_id = OBJECT_ID('Facturas'))
BEGIN
    PRINT 'Creando índice IX_Facturas_TipoComprobante...';
    CREATE NONCLUSTERED INDEX [IX_Facturas_TipoComprobante] ON [dbo].[Facturas]
    (
        [TipoComprobante] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
           DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
    ON [PRIMARY];
    
    PRINT 'Índice IX_Facturas_TipoComprobante creado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El índice IX_Facturas_TipoComprobante ya existe.';
END
GO

-- =====================================================
-- 3. Actualizar facturas existentes con valores por defecto
-- =====================================================

PRINT 'Actualizando facturas existentes con valores por defecto...';

-- Actualizar facturas existentes que no tienen TipoComprobante
UPDATE Facturas 
SET TipoComprobante = '02' -- Consumidor Final por defecto
WHERE TipoComprobante IS NULL;

-- Actualizar facturas existentes que no tienen NCF (generar secuenciales)
DECLARE @MaxNCF INT = 0;

-- Obtener el máximo NCF existente (si hay alguno)
SELECT @MaxNCF = MAX(CAST(SUBSTRING(NCF, 9, 8) AS INT)) 
FROM Facturas 
WHERE NCF IS NOT NULL 
  AND NCF LIKE 'B010%' -- NCF de consumidor final
  AND ISNUMERIC(SUBSTRING(NCF, 9, 8)) = 1;

-- Si no hay NCF existentes, empezar desde 1
IF @MaxNCF IS NULL OR @MaxNCF = 0
    SET @MaxNCF = 0;

-- Actualizar facturas sin NCF
DECLARE @ID_Factura INT;
DECLARE @NCF NVARCHAR(19);

DECLARE cursor_facturas CURSOR FOR
SELECT ID_Factura 
FROM Facturas 
WHERE NCF IS NULL 
ORDER BY ID_Factura;

OPEN cursor_facturas;
FETCH NEXT FROM cursor_facturas INTO @ID_Factura;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @MaxNCF = @MaxNCF + 1;
    SET @NCF = 'B010' + RIGHT('00000000' + CAST(@MaxNCF AS NVARCHAR), 8);
    
    UPDATE Facturas 
    SET NCF = @NCF
    WHERE ID_Factura = @ID_Factura;
    
    FETCH NEXT FROM cursor_facturas INTO @ID_Factura;
END

CLOSE cursor_facturas;
DEALLOCATE cursor_facturas;

PRINT 'Facturas existentes actualizadas exitosamente.';
GO

-- =====================================================
-- 4. Crear restricción CHECK para TipoComprobante
-- =====================================================

IF NOT EXISTS (SELECT * FROM sys.check_constraints 
               WHERE name = 'CK_Facturas_TipoComprobante' AND parent_object_id = OBJECT_ID('Facturas'))
BEGIN
    PRINT 'Creando restricción CK_Facturas_TipoComprobante...';
    ALTER TABLE [dbo].[Facturas]  WITH CHECK ADD  CONSTRAINT [CK_Facturas_TipoComprobante] CHECK  (([TipoComprobante]='01' OR [TipoComprobante]='02' OR [TipoComprobante]='03' OR [TipoComprobante]='14' OR [TipoComprobante]='15' OR [TipoComprobante]='16'));
    ALTER TABLE [dbo].[Facturas] CHECK CONSTRAINT [CK_Facturas_TipoComprobante];
    
    PRINT 'Restricción CK_Facturas_TipoComprobante creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La restricción CK_Facturas_TipoComprobante ya existe.';
END
GO

-- =====================================================
-- 5. (Opcional) Agregar campos fiscales a Clientes
-- =====================================================

-- Descomentar si se desea agregar información fiscal a clientes
/*
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'ClienteTipo')
BEGIN
    PRINT 'Agregando campo ClienteTipo a la tabla Clientes...';
    ALTER TABLE Clientes 
    ADD ClienteTipo NVARCHAR(20) NULL;
    
    PRINT 'Campo ClienteTipo agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo ClienteTipo ya existe en la tabla Clientes.';
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'RNC_Cliente')
BEGIN
    PRINT 'Agregando campo RNC_Cliente a la tabla Clientes...';
    ALTER TABLE Clientes 
    ADD RNC_Cliente NVARCHAR(20) NULL;
    
    PRINT 'Campo RNC_Cliente agregado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El campo RNC_Cliente ya existe en la tabla Clientes.';
END
GO

-- Actualizar clientes existentes con valores por defecto
UPDATE Clientes 
SET ClienteTipo = 'Final'
WHERE ClienteTipo IS NULL;
GO
*/
-- =====================================================
-- 6. Verificación de la actualización
-- =====================================================

PRINT '====================================================';
PRINT 'VERIFICACIÓN DE LA ACTUALIZACIÓN';
PRINT '====================================================';

-- Verificar estructura de la tabla Facturas
PRINT '';
PRINT 'Estructura actual de la tabla Facturas:';
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Facturas'
ORDER BY ORDINAL_POSITION;

-- Verificar índices creados
PRINT '';
PRINT 'Índices en la tabla Facturas:';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(c.name, ', ') AS Columns
FROM sys.indexes i
JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('Facturas')
  AND i.name IS NOT NULL
GROUP BY i.name, i.type_desc
ORDER BY i.name;

-- Verificar restricciones
PRINT '';
PRINT 'Restricciones en la tabla Facturas:';
SELECT 
    name AS ConstraintName,
    type_desc AS ConstraintType,
    definition AS Definition
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID('Facturas');

-- Verificar datos de muestra
PRINT '';
PRINT 'Muestra de datos actualizados:';
SELECT TOP 5
    ID_Factura,
    NumeroFactura,
    NCF,
    TipoComprobante,
    Fecha,
    TotalNeto,
    Estado
FROM Facturas
ORDER BY ID_Factura;

PRINT '';
PRINT '====================================================';
PRINT 'ACTUALIZACIÓN COMPLETADA EXITOSAMENTE';
PRINT '====================================================';
PRINT '';
PRINT 'Resumen de los cambios realizados:';
PRINT '1. Campos NCF y TipoComprobante agregados a Facturas';
PRINT '2. Índices creados para optimizar búsquedas';
PRINT '3. Facturas existentes actualizadas con NCF secuenciales';
PRINT '4. Restricción CHECK agregada para TipoComprobante';
PRINT '5. (Opcional) Campos fiscales listos para Clientes';
PRINT '';
PRINT 'El sistema ahora está listo para generar facturas con NCF.';
PRINT '====================================================';
GO
