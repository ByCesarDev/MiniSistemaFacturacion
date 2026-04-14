-- =============================================
-- Mini-Sistema de Facturación y Cuentas por Cobrar
-- Script de Inserción de Datos Iniciales
-- Autor: César Reyes
-- =============================================

USE MiniSistemaFacturacion;
GO

-- =============================================
-- Insertar configuración de empresa por defecto
-- =============================================
IF NOT EXISTS (SELECT 1 FROM ConfiguracionEmpresa WHERE Activo = 1)
BEGIN
    INSERT INTO ConfiguracionEmpresa (
        NombreEmpresa, 
        RNC, 
        Direccion, 
        Telefono, 
        Email, 
        WebSite, 
        PorcentajeImpuesto, 
        Moneda, 
        Activo
    )
    VALUES (
        'MiniSistema Facturación SRL', 
        '131234567', 
        'Calle Principal #123, Santo Domingo, República Dominicana', 
        '809-555-1234', 
        'info@minisistema.com', 
        'www.minisistema.com', 
        18.00, 
        'RD$', 
        1
    );
END
GO

-- =============================================
-- Insertar secuencias NCF iniciales
-- =============================================
IF NOT EXISTS (SELECT 1 FROM SecuenciasNCF)
BEGIN
    INSERT INTO SecuenciasNCF (TipoComprobante, SecuenciaActual, Prefijo, Activo)
    VALUES 
        ('01', 1, 'A0100100100', 1), -- Facturas con valor fiscal
        ('02', 1, 'A0100100100', 1), -- Facturas para consumidor final
        ('03', 1, 'A0100100100', 1), -- Notas de débito
        ('04', 1, 'A0100100100', 1), -- Notas de crédito
        ('11', 1, 'B0100100100', 1), -- Comprobantes de compras
        ('12', 1, 'B0100100100', 1), -- Comprobantes de gastos menores
        ('13', 1, 'B0100100100', 1), -- Comprobantes de regímenes especiales
        ('14', 1, 'B0100100100', 1), -- Comprobantes únicos
        ('15', 1, 'B0100100100', 1); -- Comprobantes gubernamentales
END
GO

-- =============================================
-- Insertar clientes de prueba
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '00123456789')
BEGIN
    INSERT INTO Clientes (Nombre, Cedula, Direccion, Telefono, Email, TipoCliente, RNC)
    VALUES 
        ('Juan Pérez Rodríguez', '00123456789', 'Calle Duarte #45, Santo Domingo', '809-555-0001', 'juan.perez@email.com', 'CF', NULL),
        ('María García López', '00234567890', 'Avenida Lincoln #123, Santiago', '809-555-0002', 'maria.garcia@email.com', 'CF', NULL),
        ('Carlos Martínez Santana', '00345678901', 'Calle 16 de Agosto #789, La Romana', '809-555-0003', 'carlos.martinez@email.com', 'CF', NULL),
        ('Ana Rodríguez Hernández', '00456789012', 'Avenida Mirador Sur #456, San Pedro de Macorís', '809-555-0004', 'ana.rodriguez@email.com', 'CF', NULL),
        ('Luis Díaz Jiménez', '00567890123', 'Calle Principal #234, Puerto Plata', '809-555-0005', 'luis.diaz@email.com', 'CF', NULL),
        ('Rosa Luna Vargas', '00678901234', 'Avenida Central #567, La Vega', '809-555-0006', 'rosa.luna@email.com', 'CF', NULL),
        ('Pedro Santos Reyes', '00789012345', 'Calle Sol #890, San Francisco de Macorís', '809-555-0007', 'pedro.santos@email.com', 'CF', NULL),
        ('Carmen Flores Díaz', '00890123456', 'Avenida Libertad #123, Higüey', '809-555-0008', 'carmen.flores@email.com', 'CF', NULL),
        ('Miguel Ángel Torres', '00901234567', 'Calle Esperanza #345, San Cristóbal', '809-555-0009', 'miguel.torres@email.com', 'CF', NULL),
        ('Sofía Castillo Morales', '01012345678', 'Avenida del Sol #678, Baní', '809-555-0010', 'sofia.castillo@email.com', 'CF', NULL),
        ('Empresa ABC SRL', NULL, 'Zona Industrial #100, Santo Domingo', '809-555-0111', 'contacto@empresabc.com', 'CCF', '131456789'),
        ('Distribuidora XYZ CxA', NULL, 'Calle Comercio #200, Santiago', '809-555-0222', 'info@distribuidoraxyz.com', 'CCF', '131789012'),
        ('Servicios Profesionales SA', NULL, 'Avenida Empresarial #300, Santo Domingo', '809-555-0333', 'admin@serviciosprofesionales.com', 'CCF', '131012345'),
        ('Compañía Comercial del Caribe', NULL, 'Calle Mercantil #400, La Romana', '809-555-0444', 'ventas@companiacaribe.com', 'CCF', '131345678'),
        ('Industrias Dominicanas SRL', NULL, 'Zona Franca #500, San Pedro de Macorís', '809-555-0555', 'contacto@industriasrd.com', 'CCF', '131678901');
END
GO

-- =============================================
-- Insertar productos de prueba
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo = 'PROD001')
BEGIN
    INSERT INTO Productos (Codigo, Descripcion, PrecioUnitario, Stock, StockMinimo, Categoria)
    VALUES 
        ('PROD001', 'Laptop Dell Inspiron 15', 25999.99, 15, 5, 'Computación'),
        ('PROD002', 'Mouse USB Inalámbrico', 599.99, 50, 10, 'Accesorios'),
        ('PROD003', 'Teclado Mecánico Gaming', 1299.99, 25, 8, 'Accesorios'),
        ('PROD004', 'Monitor LED 24 pulgadas', 3999.99, 20, 6, 'Monitores'),
        ('PROD005', 'Impresora HP LaserJet', 8999.99, 8, 3, 'Impresoras'),
        ('PROD006', 'Disco Duro Externo 1TB', 2499.99, 30, 10, 'Almacenamiento'),
        ('PROD007', 'Memoria RAM 8GB DDR4', 1899.99, 40, 12, 'Componentes'),
        ('PROD008', 'Tarjeta Gráfica GTX 1650', 12999.99, 12, 4, 'Componentes'),
        ('PROD009', 'Webcam HD 1080p', 799.99, 35, 15, 'Accesorios'),
        ('PROD010', 'Auriculares Bluetooth', 699.99, 45, 20, 'Audio'),
        ('PROD011', 'Silla de Oficina Ergonómica', 4999.99, 10, 3, 'Muebles'),
        ('PROD012', 'Escritorio de Madera', 3999.99, 8, 2, 'Muebles'),
        ('PROD013', 'Router WiFi 6', 1599.99, 22, 7, 'Redes'),
        ('PROD014', 'Switch Ethernet 8 puertos', 899.99, 18, 6, 'Redes'),
        ('PROD015', 'Cable HDMI 2m', 199.99, 100, 25, 'Cables'),
        ('PROD016', 'USB Flash Drive 64GB', 299.99, 80, 30, 'Almacenamiento'),
        ('PROD017', 'Batería Externa 10000mAh', 899.99, 28, 10, 'Accesorios'),
        ('PROD018', 'Cargador USB-C 65W', 599.99, 35, 12, 'Accesorios'),
        ('PROD019', 'Tablet Android 10 pulgadas', 8999.99, 15, 5, 'Tablets'),
        ('PROD020', 'Smartphone Android 6.5 pulgadas', 15999.99, 20, 8, 'Teléfonos'),
        ('PROD021', 'Cámara DSLR 24MP', 25999.99, 6, 2, 'Fotografía'),
        ('PROD022', 'Tripode de Aluminio', 1299.99, 15, 5, 'Fotografía'),
        ('PROD023', 'Micrófono USB Condenser', 2499.99, 12, 4, 'Audio'),
        ('PROD024', 'Altavoces Bluetooth 2.1', 1999.99, 25, 8, 'Audio'),
        ('PROD025', 'Case para Laptop 15.6', 499.99, 40, 15, 'Accesorios'),
        ('PROD026', 'Soporte para Monitor', 399.99, 30, 10, 'Accesorios'),
        ('PROD027', 'Lámpara LED Escritorio', 299.99, 35, 12, 'Iluminación'),
        ('PROD028', 'Extensión Eléctrica 6 tomas', 399.99, 50, 20, 'Electricidad'),
        ('PROD029', 'Multímetro Digital', 699.99, 18, 6, 'Herramientas'),
        ('PROD030', 'Set de Destornilladores', 399.99, 25, 8, 'Herramientas'),
        ('PROD031', 'Disco SSD 480GB', 3499.99, 22, 7, 'Almacenamiento'),
        ('PROD032', 'Fuente de Poder 600W', 1899.99, 16, 5, 'Componentes'),
        ('PROD033', 'Cooler CPU para AMD', 599.99, 30, 10, 'Componentes'),
        ('PROD034', 'Ventilador Case 120mm', 299.99, 45, 15, 'Componentes'),
        ('PROD035', 'Pasta Térmica 5g', 199.99, 60, 20, 'Componentes'),
        ('PROD036', 'Hub USB 3.0 4 puertos', 399.99, 28, 9, 'Accesorios'),
        ('PROD037', 'Adaptador VGA a HDMI', 299.99, 35, 12, 'Adaptadores'),
        ('PROD038', 'Cable Ethernet 10m', 199.99, 50, 18, 'Cables'),
        ('PROD039', 'Conector RJ45 10 unidades', 149.99, 80, 25, 'Redes'),
        ('PROD040', 'Surge Protector 6 salidas', 599.99, 20, 6, 'Electricidad');
END
GO

-- =============================================
-- Insertar algunas facturas de prueba (con datos básicos)
-- =============================================
-- Nota: Las facturas completas con detalles se insertarían a través de la aplicación
-- Aquí solo insertamos algunas facturas básicas para demostración

IF NOT EXISTS (SELECT 1 FROM Facturas)
BEGIN
    -- Obtener NCF para facturas
    DECLARE @NCF1 NVARCHAR(19), @NCF2 NVARCHAR(19), @NCF3 NVARCHAR(19);
    
    SELECT @NCF1 = Prefijo + RIGHT('00000000' + CAST(SecuenciaActual AS NVARCHAR), 8)
    FROM SecuenciasNCF WHERE TipoComprobante = '01';
    
    SELECT @NCF2 = Prefijo + RIGHT('00000000' + CAST(SecuenciaActual + 1 AS NVARCHAR), 8)
    FROM SecuenciasNCF WHERE TipoComprobante = '01';
    
    SELECT @NCF3 = Prefijo + RIGHT('00000000' + CAST(SecuenciaActual + 2 AS NVARCHAR), 8)
    FROM SecuenciasNCF WHERE TipoComprobante = '01';
    
    INSERT INTO Facturas (
        NumeroFactura, 
        NCF, 
        ID_Cliente, 
        FechaFactura, 
        TotalBruto, 
        PorcentajeImpuesto, 
        ValorImpuesto, 
        TotalNeto, 
        SaldoPendiente, 
        Estado, 
        TipoComprobante
    )
    VALUES 
        ('FAC-2026-001', @NCF1, 1, GETDATE() - 7, 10000.00, 18.00, 1800.00, 11800.00, 11800.00, 'Pendiente', '01'),
        ('FAC-2026-002', @NCF2, 2, GETDATE() - 5, 15000.00, 18.00, 2700.00, 17700.00, 17700.00, 'Pendiente', '01'),
        ('FAC-2026-003', @NCF3, 3, GETDATE() - 3, 8000.00, 18.00, 1440.00, 9440.00, 9440.00, 'Pendiente', '01');
    
    -- Actualizar secuencia de NCF
    UPDATE SecuenciasNCF 
    SET SecuenciaActual = SecuenciaActual + 3 
    WHERE TipoComprobante = '01';
END
GO

-- =============================================
-- Insertar detalles de facturas de prueba
-- =============================================
IF NOT EXISTS (SELECT 1 FROM DetallesFactura)
BEGIN
    INSERT INTO DetallesFactura (
        ID_Factura, 
        ID_Producto, 
        Cantidad, 
        PrecioUnitario, 
        Subtotal, 
        PorcentajeImpuesto, 
        ValorImpuesto, 
        TotalLinea
    )
    VALUES 
        -- Detalles para factura FAC-2026-001 (ID_Factura = 1)
        (1, 1, 1, 25999.99, 25999.99, 18.00, 4679.99, 30679.98),
        (1, 2, 2, 599.99, 1199.98, 18.00, 215.99, 1415.97),
        
        -- Detalles para factura FAC-2026-002 (ID_Factura = 2)
        (2, 3, 1, 1299.99, 1299.99, 18.00, 233.99, 1533.98),
        (2, 4, 1, 3999.99, 3999.99, 18.00, 719.99, 4719.98),
        (2, 5, 1, 8999.99, 8999.99, 18.00, 1619.99, 10619.98),
        
        -- Detalles para factura FAC-2026-003 (ID_Factura = 3)
        (3, 6, 2, 2499.99, 4999.98, 18.00, 899.99, 5899.97);
    
    -- Actualizar totales de facturas
    UPDATE Facturas 
    SET 
        TotalBruto = (
            SELECT SUM(Subtotal) 
            FROM DetallesFactura 
            WHERE ID_Factura = Facturas.ID_Factura
        ),
        ValorImpuesto = (
            SELECT SUM(ValorImpuesto) 
            FROM DetallesFactura 
            WHERE ID_Factura = Facturas.ID_Factura
        ),
        TotalNeto = (
            SELECT SUM(TotalLinea) 
            FROM DetallesFactura 
            WHERE ID_Factura = Facturas.ID_Factura
        ),
        SaldoPendiente = (
            SELECT SUM(TotalLinea) 
            FROM DetallesFactura 
            WHERE ID_Factura = Facturas.ID_Factura
        )
    WHERE ID_Factura IN (1, 2, 3);
END
GO

-- =============================================
-- Insertar algunos pagos de prueba
-- =============================================
IF NOT EXISTS (SELECT 1 FROM Pagos)
BEGIN
    INSERT INTO Pagos (
        ID_Factura, 
        FechaPago, 
        MontoPagado, 
        FormaPago, 
        Referencia, 
        Observaciones
    )
    VALUES 
        (1, GETDATE() - 6, 5000.00, 'Efectivo', NULL, 'Pago parcial'),
        (2, GETDATE() - 4, 10000.00, 'Tarjeta Credito', '4532-1234-5678-9012', 'Pago con tarjeta'),
        (3, GETDATE() - 2, 3000.00, 'Transferencia', 'BAN-123456789', 'Transferencia bancaria');
    
    -- Actualizar saldos pendientes de facturas
    UPDATE Facturas 
    SET 
        SaldoPendiente = SaldoPendiente - (
            SELECT SUM(MontoPagado) 
            FROM Pagos 
            WHERE ID_Factura = Facturas.ID_Factura
        ),
        Estado = CASE 
            WHEN SaldoPendiente - (SELECT SUM(MontoPagado) FROM Pagos WHERE ID_Factura = Facturas.ID_Factura) = 0 THEN 'Pagada'
            WHEN SaldoPendiente - (SELECT SUM(MontoPagado) FROM Pagos WHERE ID_Factura = Facturas.ID_Factura) > 0 THEN 'Parcial'
            ELSE Estado
        END
    WHERE ID_Factura IN (1, 2, 3);
END
GO

-- =============================================
-- Actualizar stock de productos basado en ventas
-- =============================================
UPDATE Productos 
SET Stock = Stock - (
    SELECT SUM(df.Cantidad) 
    FROM DetallesFactura df 
    WHERE df.ID_Producto = Productos.ID_Producto
)
WHERE ID_Producto IN (
    SELECT DISTINCT ID_Producto 
    FROM DetallesFactura
);
GO

PRINT 'Datos iniciales insertados exitosamente'
PRINT 'Resumen de datos insertados:'
PRINT '- 1 Configuración de Empresa'
PRINT '- 9 Secuencias NCF'
PRINT '- 15 Clientes (10 CF + 5 CCF)'
PRINT '- 40 Productos'
PRINT '- 3 Facturas con detalles'
PRINT '- 3 Pagos registrados'
GO
