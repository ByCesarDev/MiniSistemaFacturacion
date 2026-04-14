-- =============================================
-- Mini-Sistema de Facturación y Cuentas por Cobrar
-- Script de Creación de Base de Datos y Tablas
-- Autor: César Reyes
-- =============================================

-- Crear base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'MiniSistemaFacturacion')
BEGIN
    CREATE DATABASE MiniSistemaFacturacion;
END
GO

USE MiniSistemaFacturacion;
GO

-- =============================================
-- Tabla de Clientes
-- =============================================
CREATE TABLE [dbo].[Clientes](
    [ID_Cliente] [int] IDENTITY(1,1) NOT NULL,
    [Nombre] [nvarchar](100) NOT NULL,
    [Cedula] [nvarchar](20) NOT NULL,
    [Direccion] [nvarchar](200) NULL,
    [Telefono] [nvarchar](25) NULL,
    [Email] [nvarchar](100) NULL,
    [TipoCliente] [varchar](3) NOT NULL DEFAULT 'CF',
    [RNC] [nvarchar](20) NULL,
    [FechaCreacion] [datetime] NOT NULL DEFAULT GETDATE(),
    [Estado] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED ([ID_Cliente] ASC),
 CONSTRAINT [UQ_Clientes_Cedula] UNIQUE NONCLUSTERED ([Cedula] ASC),
 CONSTRAINT [UQ_Clientes_RNC] UNIQUE NONCLUSTERED ([RNC] ASC)
)
GO

-- =============================================
-- Tabla de Productos
-- =============================================
CREATE TABLE [dbo].[Productos](
    [ID_Producto] [int] IDENTITY(1,1) NOT NULL,
    [Codigo] [nvarchar](50) NOT NULL,
    [Descripcion] [nvarchar](200) NOT NULL,
    [PrecioUnitario] [decimal](18,2) NOT NULL,
    [Stock] [int] NOT NULL DEFAULT 0,
    [StockMinimo] [int] NOT NULL DEFAULT 5,
    [Categoria] [nvarchar](50) NULL,
    [FechaCreacion] [datetime] NOT NULL DEFAULT GETDATE(),
    [Estado] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED ([ID_Producto] ASC),
 CONSTRAINT [UQ_Productos_Codigo] UNIQUE NONCLUSTERED ([Codigo] ASC)
)
GO

-- =============================================
-- Tabla de Configuración de Empresa
-- =============================================
CREATE TABLE [dbo].[ConfiguracionEmpresa](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [NombreEmpresa] [nvarchar](200) NOT NULL,
    [RNC] [nvarchar](20) NOT NULL,
    [Direccion] [nvarchar](500) NULL,
    [Telefono] [nvarchar](50) NULL,
    [Email] [nvarchar](100) NULL,
    [WebSite] [nvarchar](200) NULL,
    [Logo] [varbinary](max) NULL,
    [PorcentajeImpuesto] [decimal](5,2) NOT NULL DEFAULT 18.00,
    [Moneda] [nvarchar](10) NOT NULL DEFAULT 'RD$',
    [Activo] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_ConfiguracionEmpresa] PRIMARY KEY CLUSTERED ([ID] ASC)
)
GO

-- =============================================
-- Tabla de Secuencias NCF (Números de Comprobante Fiscal)
-- =============================================
CREATE TABLE [dbo].[SecuenciasNCF](
    [TipoComprobante] [varchar](2) NOT NULL,
    [SecuenciaActual] [bigint] NOT NULL DEFAULT 1,
    [Prefijo] [varchar](20) NOT NULL,
    [Activo] [bit] NOT NULL DEFAULT 1,
 CONSTRAINT [PK_SecuenciasNCF] PRIMARY KEY CLUSTERED ([TipoComprobante] ASC)
)
GO

-- =============================================
-- Tabla de Facturas
-- =============================================
CREATE TABLE [dbo].[Facturas](
    [ID_Factura] [int] IDENTITY(1,1) NOT NULL,
    [NumeroFactura] [nvarchar](20) NOT NULL,
    [NCF] [nvarchar](19) NOT NULL,
    [ID_Cliente] [int] NOT NULL,
    [FechaFactura] [datetime] NOT NULL DEFAULT GETDATE(),
    [TotalBruto] [decimal](18,2) NOT NULL DEFAULT 0,
    [PorcentajeImpuesto] [decimal](5,2) NOT NULL DEFAULT 18.00,
    [ValorImpuesto] [decimal](18,2) NOT NULL DEFAULT 0,
    [TotalNeto] [decimal](18,2) NOT NULL DEFAULT 0,
    [SaldoPendiente] [decimal](18,2) NOT NULL DEFAULT 0,
    [Estado] [nvarchar](20) NOT NULL DEFAULT 'Pendiente',
    [TipoComprobante] [varchar](2) NOT NULL DEFAULT '01',
    [Observaciones] [nvarchar](500) NULL,
    [FechaCreacion] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_Facturas] PRIMARY KEY CLUSTERED ([ID_Factura] ASC),
 CONSTRAINT [UQ_Facturas_NumeroFactura] UNIQUE NONCLUSTERED ([NumeroFactura] ASC),
 CONSTRAINT [UQ_Facturas_NCF] UNIQUE NONCLUSTERED ([NCF] ASC),
 CONSTRAINT [FK_Facturas_Clientes] FOREIGN KEY([ID_Cliente]) REFERENCES [dbo].[Clientes] ([ID_Cliente]),
 CONSTRAINT [FK_Facturas_SecuenciasNCF] FOREIGN KEY([TipoComprobante]) REFERENCES [dbo].[SecuenciasNCF] ([TipoComprobante])
)
GO

-- =============================================
-- Tabla de Detalles de Factura
-- =============================================
CREATE TABLE [dbo].[DetallesFactura](
    [ID_Detalle] [int] IDENTITY(1,1) NOT NULL,
    [ID_Factura] [int] NOT NULL,
    [ID_Producto] [int] NOT NULL,
    [Cantidad] [int] NOT NULL,
    [PrecioUnitario] [decimal](18,2) NOT NULL,
    [Subtotal] [decimal](18,2) NOT NULL,
    [PorcentajeImpuesto] [decimal](5,2) NOT NULL DEFAULT 18.00,
    [ValorImpuesto] [decimal](18,2) NOT NULL DEFAULT 0,
    [TotalLinea] [decimal](18,2) NOT NULL,
 CONSTRAINT [PK_DetallesFactura] PRIMARY KEY CLUSTERED ([ID_Detalle] ASC),
 CONSTRAINT [FK_DetallesFactura_Facturas] FOREIGN KEY([ID_Factura]) REFERENCES [dbo].[Facturas] ([ID_Factura]) ON DELETE CASCADE,
 CONSTRAINT [FK_DetallesFactura_Productos] FOREIGN KEY([ID_Producto]) REFERENCES [dbo].[Productos] ([ID_Producto])
)
GO

-- =============================================
-- Tabla de Pagos
-- =============================================
CREATE TABLE [dbo].[Pagos](
    [ID_Pago] [int] IDENTITY(1,1) NOT NULL,
    [ID_Factura] [int] NOT NULL,
    [FechaPago] [datetime] NOT NULL DEFAULT GETDATE(),
    [MontoPagado] [decimal](18,2) NOT NULL,
    [FormaPago] [nvarchar](50) NOT NULL,
    [Referencia] [nvarchar](100) NULL,
    [Observaciones] [nvarchar](500) NULL,
    [FechaCreacion] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [PK_Pagos] PRIMARY KEY CLUSTERED ([ID_Pago] ASC),
 CONSTRAINT [FK_Pagos_Facturas] FOREIGN KEY([ID_Factura]) REFERENCES [dbo].[Facturas] ([ID_Factura])
)
GO

-- =============================================
-- Crear índices para optimización de consultas
-- =============================================

-- Índices para Clientes
CREATE NONCLUSTERED INDEX [IX_Clientes_Cedula] ON [dbo].[Clientes] ([Cedula] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Clientes_Nombre] ON [dbo].[Clientes] ([Nombre] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Clientes_Estado] ON [dbo].[Clientes] ([Estado] ASC)
GO

-- Índices para Productos
CREATE NONCLUSTERED INDEX [IX_Productos_Codigo] ON [dbo].[Productos] ([Codigo] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Productos_Descripcion] ON [dbo].[Productos] ([Descripcion] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Productos_Stock] ON [dbo].[Productos] ([Stock] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Productos_Estado] ON [dbo].[Productos] ([Estado] ASC)
GO

-- Índices para Facturas
CREATE NONCLUSTERED INDEX [IX_Facturas_ID_Cliente] ON [dbo].[Facturas] ([ID_Cliente] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Facturas_FechaFactura] ON [dbo].[Facturas] ([FechaFactura] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Facturas_Estado] ON [dbo].[Facturas] ([Estado] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Facturas_NCF] ON [dbo].[Facturas] ([NCF] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Facturas_NumeroFactura] ON [dbo].[Facturas] ([NumeroFactura] ASC)
GO

-- Índices para DetallesFactura
CREATE NONCLUSTERED INDEX [IX_DetallesFactura_ID_Factura] ON [dbo].[DetallesFactura] ([ID_Factura] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_DetallesFactura_ID_Producto] ON [dbo].[DetallesFactura] ([ID_Producto] ASC)
GO

-- Índices para Pagos
CREATE NONCLUSTERED INDEX [IX_Pagos_ID_Factura] ON [dbo].[Pagos] ([ID_Factura] ASC)
GO
CREATE NONCLUSTERED INDEX [IX_Pagos_FechaPago] ON [dbo].[Pagos] ([FechaPago] ASC)
GO

-- =============================================
-- Crear restricciones CHECK para validaciones
-- =============================================

-- Validaciones para Facturas
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([TotalBruto] >= (0)))
GO
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([ValorImpuesto] >= (0)))
GO
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([TotalNeto] >= (0)))
GO
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([SaldoPendiente] >= (0)))
GO
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([PorcentajeImpuesto] >= (0)))
GO
ALTER TABLE [dbo].[Facturas] WITH CHECK ADD CHECK (([TipoComprobante] IN ('01', '02', '03', '04', '11', '12', '13', '14', '15')))
GO

-- Validaciones para DetallesFactura
ALTER TABLE [dbo].[DetallesFactura] WITH CHECK ADD CHECK (([Cantidad] > (0)))
GO
ALTER TABLE [dbo].[DetallesFactura] WITH CHECK ADD CHECK (([PrecioUnitario] > (0)))
GO
ALTER TABLE [dbo].[DetallesFactura] WITH CHECK ADD CHECK (([Subtotal] >= (0)))
GO
ALTER TABLE [dbo].[DetallesFactura] WITH CHECK ADD CHECK (([ValorImpuesto] >= (0)))
GO
ALTER TABLE [dbo].[DetallesFactura] WITH CHECK ADD CHECK (([TotalLinea] >= (0)))
GO

-- Validaciones para Pagos
ALTER TABLE [dbo].[Pagos] WITH CHECK ADD CHECK (([FormaPago] = 'Cheque' OR [FormaPago] = 'Transferencia' OR [FormaPago] = 'Tarjeta Debito' OR [FormaPago] = 'Tarjeta Credito' OR [FormaPago] = 'Efectivo'))
GO
ALTER TABLE [dbo].[Pagos] WITH CHECK ADD CHECK (([MontoPagado] > (0)))
GO

-- Validaciones para Productos
ALTER TABLE [dbo].[Productos] WITH CHECK ADD CHECK (([PrecioUnitario] > (0)))
GO
ALTER TABLE [dbo].[Productos] WITH CHECK ADD CHECK (([Stock] >= (0)))
GO
ALTER TABLE [dbo].[Productos] WITH CHECK ADD CHECK (([StockMinimo] >= (0)))
GO

-- =============================================
-- Crear vistas para consultas optimizadas
-- =============================================

-- Vista para resumen de facturas con cliente
CREATE VIEW [dbo].[vw_FacturasResumen]
AS
SELECT 
    f.ID_Factura,
    f.NumeroFactura,
    f.NCF,
    f.FechaFactura,
    f.TotalNeto,
    f.SaldoPendiente,
    f.Estado,
    c.Nombre AS NombreCliente,
    c.Cedula AS CedulaCliente,
    c.TipoCliente
FROM Facturas f
INNER JOIN Clientes c ON f.ID_Cliente = c.ID_Cliente
GO

-- Vista para cuentas por cobrar
CREATE VIEW [dbo].[vw_CuentasPorCobrar]
AS
SELECT 
    c.ID_Cliente,
    c.Nombre,
    c.Cedula,
    c.TipoCliente,
    COUNT(f.ID_Factura) AS TotalFacturas,
    SUM(f.TotalNeto) AS TotalFacturado,
    SUM(f.SaldoPendiente) AS SaldoTotalPendiente
FROM Clientes c
LEFT JOIN Facturas f ON c.ID_Cliente = f.ID_Cliente AND f.SaldoPendiente > 0
WHERE c.Estado = 1
GROUP BY c.ID_Cliente, c.Nombre, c.Cedula, c.TipoCliente
HAVING SUM(f.SaldoPendiente) > 0
GO

-- Vista para productos con stock bajo
CREATE VIEW [dbo].[vw_ProductosStockBajo]
AS
SELECT 
    p.ID_Producto,
    p.Codigo,
    p.Descripcion,
    p.PrecioUnitario,
    p.Stock,
    p.StockMinimo,
    CASE 
        WHEN p.Stock = 0 THEN 'Sin Stock'
        WHEN p.Stock <= p.StockMinimo THEN 'Stock Crítico'
        WHEN p.Stock <= p.StockMinimo * 2 THEN 'Stock Bajo'
        ELSE 'Stock Normal'
    END AS EstadoStock
FROM Productos p
WHERE p.Estado = 1 AND p.Stock <= p.StockMinimo * 2
GO

PRINT 'Base de datos y tablas creadas exitosamente'
GO
