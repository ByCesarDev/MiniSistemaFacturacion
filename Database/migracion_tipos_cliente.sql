-- Migración para agregar soporte de Tipos de Cliente
-- Created by: Cesar Reyes
-- Date: 2026-04-12

/* =========================================
   1. AGREGAR COLUMNAS (SI NO EXISTEN)
========================================= */

IF COL_LENGTH('Clientes', 'TipoCliente') IS NULL
BEGIN
    ALTER TABLE Clientes 
    ADD TipoCliente VARCHAR(3) NOT NULL 
    CONSTRAINT DF_Clientes_TipoCliente DEFAULT 'CF';
END
GO

IF COL_LENGTH('Clientes', 'RNC') IS NULL
BEGIN
    ALTER TABLE Clientes 
    ADD RNC VARCHAR(20) NULL;
END
GO

/* =========================================
   2. CONSTRAINT CHECK (SI NO EXISTE)
========================================= */

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints 
    WHERE name = 'CK_TipoCliente'
)
BEGIN
    ALTER TABLE Clientes 
    ADD CONSTRAINT CK_TipoCliente 
    CHECK (TipoCliente IN ('CF', 'CCF'));
END
GO

/* =========================================
   3. NORMALIZAR DATOS EXISTENTES
========================================= */

UPDATE Clientes 
SET TipoCliente = 'CF'
WHERE TipoCliente IS NULL OR TipoCliente = '';
GO

/* =========================================
   4. PROCEDIMIENTO: INSERTAR CLIENTE
========================================= */

IF OBJECT_ID('sp_InsertarCliente', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertarCliente;
GO

CREATE PROCEDURE sp_InsertarCliente
    @Nombre NVARCHAR(100),
    @Cedula NVARCHAR(20),
    @Direccion NVARCHAR(200) = NULL,
    @Telefono NVARCHAR(25) = NULL,
    @Email NVARCHAR(100) = NULL,
    @TipoCliente VARCHAR(3) = 'CF',
    @RNC VARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @TipoCliente = 'CCF' AND (@RNC IS NULL OR @RNC = '')
    BEGIN
        RAISERROR('Los clientes de Crédito Fiscal deben tener RNC', 16, 1);
        RETURN;
    END

    IF @TipoCliente = 'CF' AND @RNC IS NOT NULL AND @RNC != ''
    BEGIN
        SET @RNC = NULL;
    END

    INSERT INTO Clientes (Nombre, Cedula, Direccion, Telefono, Email, TipoCliente, RNC)
    VALUES (@Nombre, @Cedula, @Direccion, @Telefono, @Email, @TipoCliente, @RNC);

    SELECT SCOPE_IDENTITY() AS ID_Cliente;
END
GO

/* =========================================
   5. PROCEDIMIENTO: ACTUALIZAR CLIENTE
========================================= */

IF OBJECT_ID('sp_ActualizarCliente', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActualizarCliente;
GO

CREATE PROCEDURE sp_ActualizarCliente
    @ID_Cliente INT,
    @Nombre NVARCHAR(100),
    @Cedula NVARCHAR(20),
    @Direccion NVARCHAR(200) = NULL,
    @Telefono NVARCHAR(25) = NULL,
    @Email NVARCHAR(100) = NULL,
    @TipoCliente VARCHAR(3) = 'CF',
    @RNC VARCHAR(20) = NULL,
    @Estado BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @TipoCliente = 'CCF' AND (@RNC IS NULL OR @RNC = '')
    BEGIN
        RAISERROR('Los clientes de Crédito Fiscal deben tener RNC', 16, 1);
        RETURN;
    END

    IF @TipoCliente = 'CF' AND @RNC IS NOT NULL AND @RNC != ''
    BEGIN
        SET @RNC = NULL;
    END

    UPDATE Clientes
    SET Nombre = @Nombre,
        Cedula = @Cedula,
        Direccion = @Direccion,
        Telefono = @Telefono,
        Email = @Email,
        TipoCliente = @TipoCliente,
        RNC = @RNC,
        Estado = @Estado
    WHERE ID_Cliente = @ID_Cliente;
END
GO

/* =========================================
   6. PROCEDIMIENTO: OBTENER CLIENTE POR ID
========================================= */

IF OBJECT_ID('sp_ObtenerClientePorId', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerClientePorId;
GO

CREATE PROCEDURE sp_ObtenerClientePorId
    @ID_Cliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email,
           TipoCliente, RNC, FechaCreacion, Estado
    FROM Clientes
    WHERE ID_Cliente = @ID_Cliente AND Estado = 1;
END
GO

/* =========================================
   7. PROCEDIMIENTO: OBTENER CLIENTES POR TIPO
========================================= */

IF OBJECT_ID('sp_ObtenerClientesPorTipo', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerClientesPorTipo;
GO

CREATE PROCEDURE sp_ObtenerClientesPorTipo
    @TipoCliente VARCHAR(3)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email,
           TipoCliente, RNC, FechaCreacion, Estado
    FROM Clientes
    WHERE TipoCliente = @TipoCliente AND Estado = 1
    ORDER BY Nombre;
END
GO

/* =========================================
   8. PROCEDIMIENTO: OBTENER TODOS LOS CLIENTES
========================================= */

IF OBJECT_ID('sp_ObtenerTodosLosClientes', 'P') IS NOT NULL
    DROP PROCEDURE sp_ObtenerTodosLosClientes;
GO

CREATE PROCEDURE sp_ObtenerTodosLosClientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ID_Cliente, Nombre, Cedula, Direccion, Telefono, Email,
           TipoCliente, RNC, FechaCreacion, Estado
    FROM Clientes
    WHERE Estado = 1
    ORDER BY Nombre;
END
GO

PRINT 'Migración de Tipos de Cliente completada exitosamente.';