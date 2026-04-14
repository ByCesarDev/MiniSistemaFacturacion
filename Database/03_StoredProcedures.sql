-- =============================================
-- Mini-Sistema de Facturación y Cuentas por Cobrar
-- Script de Procedimientos Almacenados
-- Autor: César Reyes
-- =============================================

USE MiniSistemaFacturacion;
GO

-- =============================================
-- Procedimiento para insertar cliente
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_InsertarCliente]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_InsertarCliente]
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

-- =============================================
-- Procedimiento para actualizar cliente
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_ActualizarCliente]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ActualizarCliente]
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

-- =============================================
-- Procedimiento para obtener cliente por ID
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_ObtenerClientePorId]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ObtenerClientePorId]
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

-- =============================================
-- Procedimiento para obtener todos los clientes
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_ObtenerTodosLosClientes]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ObtenerTodosLosClientes]
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

-- =============================================
-- Procedimiento para obtener clientes por tipo
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_ObtenerClientesPorTipo]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_ObtenerClientesPorTipo]
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

-- =============================================
-- Procedimiento para insertar producto
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_InsertarProducto]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_InsertarProducto]
    @Codigo NVARCHAR(50),
    @Descripcion NVARCHAR(200),
    @PrecioUnitario DECIMAL(18,2),
    @Stock INT = 0,
    @StockMinimo INT = 5,
    @Categoria NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Productos (Codigo, Descripcion, PrecioUnitario, Stock, StockMinimo, Categoria)
    VALUES (@Codigo, @Descripcion, @PrecioUnitario, @Stock, @StockMinimo, @Categoria);

    SELECT SCOPE_IDENTITY() AS ID_Producto;
END
GO

-- =============================================
-- Procedimiento para crear factura con detalles (transaccional)
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_CrearFactura]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_CrearFactura]
    @NumeroFactura NVARCHAR(20),
    @ID_Cliente INT,
    @PorcentajeImpuesto DECIMAL(5,2) = 18.00,
    @Detalles NVARCHAR(MAX) -- JSON con los detalles de la factura
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @ID_Factura INT;
        DECLARE @TotalBruto DECIMAL(18,2) = 0;
        DECLARE @ValorImpuesto DECIMAL(18,2) = 0;
        DECLARE @TotalNeto DECIMAL(18,2) = 0;

        -- Insertar cabecera de factura
        INSERT INTO Facturas (NumeroFactura, ID_Cliente, PorcentajeImpuesto)
        VALUES (@NumeroFactura, @ID_Cliente, @PorcentajeImpuesto);

        SET @ID_Factura = SCOPE_IDENTITY();

        -- Procesar detalles (usando JSON parse si SQL Server 2016+, sino usar tabla temporal)
        -- Por ahora, implementación básica con parámetros separados

        -- Actualizar totales (se calcularán después de insertar detalles)

        COMMIT TRANSACTION;

        SELECT @ID_Factura AS ID_Factura;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedimiento para registrar pago
-- =============================================
/****** Object:  StoredProcedure [dbo].[sp_RegistrarPago]    Script Date: 13/4/2026 11:16:30 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_RegistrarPago]
    @ID_Factura INT,
    @MontoPagado DECIMAL(18,2),
    @FormaPago NVARCHAR(50),
    @Referencia NVARCHAR(100) = NULL,
    @Observaciones NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @SaldoActual DECIMAL(18,2);
        DECLARE @NuevoSaldo DECIMAL(18,2);
        DECLARE @NuevoEstado NVARCHAR(20);

        -- Obtener saldo actual
        SELECT @SaldoActual = SaldoPendiente
        FROM Facturas
        WHERE ID_Factura = @ID_Factura;

        -- Validar que el monto no exceda el saldo
        IF @MontoPagado > @SaldoActual
        BEGIN
            RAISERROR('El monto del pago excede el saldo pendiente', 16, 1);
            RETURN;
        END

        -- Insertar pago
        INSERT INTO Pagos (ID_Factura, MontoPagado, FormaPago, Referencia, Observaciones)
        VALUES (@ID_Factura, @MontoPagado, @FormaPago, @Referencia, @Observaciones);

        -- Actualizar saldo pendiente
        SET @NuevoSaldo = @SaldoActual - @MontoPagado;

        -- Determinar nuevo estado
        IF @NuevoSaldo = 0
            SET @NuevoEstado = 'Pagada';
        ELSE IF @NuevoSaldo < @SaldoActual
            SET @NuevoEstado = 'Parcial';
        ELSE
            SET @NuevoEstado = 'Pendiente';

        UPDATE Facturas
        SET SaldoPendiente = @NuevoSaldo,
            Estado = @NuevoEstado
        WHERE ID_Factura = @ID_Factura;

        COMMIT TRANSACTION;

        SELECT SCOPE_IDENTITY() AS ID_Pago, @NuevoSaldo AS NuevoSaldo, @NuevoEstado AS NuevoEstado;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedimiento para obtener facturas por cliente
-- =============================================
CREATE PROCEDURE [dbo].[sp_ObtenerFacturasPorCliente]
    @ID_Cliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        f.ID_Factura,
        f.NumeroFactura,
        f.NCF,
        f.FechaFactura,
        f.TotalNeto,
        f.SaldoPendiente,
        f.Estado,
        f.TipoComprobante,
        f.Observaciones
    FROM Facturas f
    WHERE f.ID_Cliente = @ID_Cliente
    ORDER BY f.FechaFactura DESC;
END
GO

-- =============================================
-- Procedimiento para obtener productos con stock bajo
-- =============================================
CREATE PROCEDURE [dbo].[sp_ObtenerProductosStockBajo]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.ID_Producto,
        p.Codigo,
        p.Descripcion,
        p.PrecioUnitario,
        p.Stock,
        p.StockMinimo,
        p.Categoria,
        CASE 
            WHEN p.Stock = 0 THEN 'Sin Stock'
            WHEN p.Stock <= p.StockMinimo THEN 'Stock Crítico'
            WHEN p.Stock <= p.StockMinimo * 2 THEN 'Stock Bajo'
            ELSE 'Stock Normal'
        END AS EstadoStock
    FROM Productos p
    WHERE p.Estado = 1 AND p.Stock <= p.StockMinimo * 2
    ORDER BY p.Stock ASC, p.Descripcion;
END
GO

-- =============================================
-- Procedimiento para generar NCF
-- =============================================
CREATE PROCEDURE [dbo].[sp_GenerarNCF]
    @TipoComprobante VARCHAR(2),
    @NCF NVARCHAR(19) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    -- Obtener y actualizar secuencia
    SELECT @NCF = Prefijo + RIGHT('00000000' + CAST(SecuenciaActual AS NVARCHAR), 8)
    FROM SecuenciasNCF 
    WHERE TipoComprobante = @TipoComprobante AND Activo = 1;
    
    IF @NCF IS NULL
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Tipo de comprobante no válido o inactivo', 16, 1);
        RETURN;
    END
    
    -- Actualizar secuencia
    UPDATE SecuenciasNCF
    SET SecuenciaActual = SecuenciaActual + 1
    WHERE TipoComprobante = @TipoComprobante;
    
    COMMIT TRANSACTION;
END
GO

-- =============================================
-- Procedimiento para actualizar stock de producto
-- =============================================
CREATE PROCEDURE [dbo].[sp_ActualizarStockProducto]
    @ID_Producto INT,
    @Cantidad INT,
    @Operacion VARCHAR(10) -- 'SUMAR' o 'RESTAR'
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @StockActual INT;
        DECLARE @NuevoStock INT;
        
        -- Obtener stock actual
        SELECT @StockActual = Stock
        FROM Productos
        WHERE ID_Producto = @ID_Producto AND Estado = 1;
        
        IF @StockActual IS NULL
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Producto no encontrado o inactivo', 16, 1);
            RETURN;
        END
        
        -- Calcular nuevo stock
        IF @Operacion = 'SUMAR'
            SET @NuevoStock = @StockActual + @Cantidad;
        ELSE IF @Operacion = 'RESTAR'
            SET @NuevoStock = @StockActual - @Cantidad;
        ELSE
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Operación no válida. Use SUMAR o RESTAR', 16, 1);
            RETURN;
        END
        
        -- Validar stock no negativo
        IF @NuevoStock < 0
        BEGIN
            ROLLBACK TRANSACTION;
            RAISERROR('Stock insuficiente para la operación', 16, 1);
            RETURN;
        END
        
        -- Actualizar stock
        UPDATE Productos
        SET Stock = @NuevoStock
        WHERE ID_Producto = @ID_Producto;
        
        COMMIT TRANSACTION;
        
        SELECT @NuevoStock AS NuevoStock;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        THROW;
    END CATCH
END
GO

-- =============================================
-- Procedimiento para obtener cuentas por cobrar
-- =============================================
CREATE PROCEDURE [dbo].[sp_ObtenerCuentasPorCobrar]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        c.ID_Cliente,
        c.Nombre,
        c.Cedula,
        c.TipoCliente,
        c.RNC,
        COUNT(f.ID_Factura) AS TotalFacturas,
        SUM(f.TotalNeto) AS TotalFacturado,
        SUM(f.SaldoPendiente) AS SaldoTotalPendiente,
        CASE 
            WHEN SUM(f.SaldoPendiente) = 0 THEN 'Sin Deuda'
            WHEN SUM(f.SaldoPendiente) < 5000 THEN 'Deuda Baja'
            WHEN SUM(f.SaldoPendiente) < 20000 THEN 'Deuda Media'
            ELSE 'Deuda Alta'
        END AS CategoriaDeuda
    FROM Clientes c
    LEFT JOIN Facturas f ON c.ID_Cliente = f.ID_Cliente AND f.SaldoPendiente > 0
    WHERE c.Estado = 1
    GROUP BY c.ID_Cliente, c.Nombre, c.Cedula, c.TipoCliente, c.RNC
    HAVING SUM(f.SaldoPendiente) > 0
    ORDER BY SaldoTotalPendiente DESC;
END
GO

-- =============================================
-- Procedimiento para obtener reporte de ventas
-- =============================================
CREATE PROCEDURE [dbo].[sp_ReporteVentas]
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        f.ID_Factura,
        f.NumeroFactura,
        f.NCF,
        f.FechaFactura,
        c.Nombre AS NombreCliente,
        c.TipoCliente,
        f.TotalNeto,
        f.SaldoPendiente,
        f.Estado,
        CASE 
            WHEN f.SaldoPendiente = 0 THEN 'Pagada'
            WHEN f.SaldoPendiente < f.TotalNeto THEN 'Parcial'
            ELSE 'Pendiente'
        END AS EstadoPago,
        p.MontoPagado,
        p.FechaPago,
        p.FormaPago
    FROM Facturas f
    INNER JOIN Clientes c ON f.ID_Cliente = c.ID_Cliente
    LEFT JOIN Pagos p ON f.ID_Factura = p.ID_Factura
    WHERE f.FechaFactura BETWEEN @FechaInicio AND @FechaFin
    ORDER BY f.FechaFactura DESC, f.NumeroFactura;
END
GO

PRINT 'Procedimientos almacenados creados exitosamente'
GO
