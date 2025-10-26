-- ============================================================================
-- Script de Creaci�n de Base de Datos - Sistema Inventario Ferreter�a
-- ============================================================================
-- Base de Datos: InventarioFerreteriaDB
-- SGBD: PostgreSQL 15+
-- Autor: Anahy Herrera
-- Fecha: 26 de Octubre, 2025
-- Descripci�n: Creaci�n de esquema completo con tablas, constraints, 
--              �ndices, triggers y datos de prueba
-- ============================================================================

-- ============================================================================
-- 1. CONFIGURACI�N INICIAL
-- ============================================================================

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;

-- ============================================================================
-- 2. CREACI�N DE TABLAS
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 2.1 Tabla: usuarios
-- Descripci�n: Almacena usuarios del sistema con autenticaci�n
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS usuarios (
    id SERIAL PRIMARY KEY,
    nombreusuario VARCHAR(50) NOT NULL UNIQUE,
    passwordhash VARCHAR(255) NOT NULL,
    rol VARCHAR(20) NOT NULL DEFAULT 'Usuario',
    fechacreacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    
    CONSTRAINT chk_rol CHECK (rol IN ('Administrador', 'Usuario'))
);

COMMENT ON TABLE usuarios IS 'Usuarios del sistema con credenciales de acceso';
COMMENT ON COLUMN usuarios.passwordhash IS 'Contrase�a hasheada con BCrypt';
COMMENT ON COLUMN usuarios.rol IS 'Rol del usuario: Administrador o Usuario';

-- ----------------------------------------------------------------------------
-- 2.2 Tabla: categorias
-- Descripci�n: Categor�as de productos de la ferreter�a
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS categorias (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    
    CONSTRAINT uk_categoria_nombre UNIQUE (nombre)
);

COMMENT ON TABLE categorias IS 'Categor�as de art�culos';

-- ----------------------------------------------------------------------------
-- 2.3 Tabla: proveedores
-- Descripci�n: Proveedores de productos
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS proveedores (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL,
    telefono VARCHAR(20),
    email VARCHAR(100),
    direccion TEXT,
    
    CONSTRAINT uk_proveedor_nombre UNIQUE (nombre)
);

COMMENT ON TABLE proveedores IS 'Proveedores de art�culos';

-- ----------------------------------------------------------------------------
-- 2.4 Tabla: articulos
-- Descripci�n: Cat�logo de art�culos del inventario
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS articulos (
    id SERIAL PRIMARY KEY,
    codigo VARCHAR(50) NOT NULL UNIQUE,
    nombre VARCHAR(200) NOT NULL,
    descripcion TEXT,
    categoriaid INTEGER,
    preciocompra NUMERIC(10,2) NOT NULL,
    precioventa NUMERIC(10,2) NOT NULL,
    stock INTEGER NOT NULL DEFAULT 0,
    stockminimo INTEGER NOT NULL DEFAULT 5,
    proveedorid INTEGER,
    fechacreacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fechaactualizacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    activo BOOLEAN NOT NULL DEFAULT TRUE,
    
    -- Constraints de validaci�n (RF3)
    CONSTRAINT chk_preciocompra CHECK (preciocompra > 0),
    CONSTRAINT chk_precioventa CHECK (precioventa > 0),
    CONSTRAINT chk_precios CHECK (precioventa >= preciocompra),
    CONSTRAINT chk_stock CHECK (stock >= 0),
    CONSTRAINT chk_stockminimo CHECK (stockminimo >= 0),
    
    -- Foreign Keys
    CONSTRAINT fk_articulo_categoria FOREIGN KEY (categoriaid) 
        REFERENCES categorias(id) ON DELETE SET NULL,
    CONSTRAINT fk_articulo_proveedor FOREIGN KEY (proveedorid) 
        REFERENCES proveedores(id) ON DELETE SET NULL
);

COMMENT ON TABLE articulos IS 'Cat�logo de art�culos del inventario';
COMMENT ON COLUMN articulos.codigo IS 'C�digo �nico del art�culo (RN1)';
COMMENT ON COLUMN articulos.preciocompra IS 'Precio de compra (debe ser > 0)';
COMMENT ON COLUMN articulos.precioventa IS 'Precio de venta (debe ser > preciocompra)';
COMMENT ON COLUMN articulos.stock IS 'Cantidad actual en inventario';
COMMENT ON COLUMN articulos.stockminimo IS 'Stock m�nimo para alerta de reposici�n (RF7)';

-- ----------------------------------------------------------------------------
-- 2.5 Tabla: logoperaciones
-- Descripci�n: Auditor�a de operaciones en el sistema
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS logoperaciones (
    id SERIAL PRIMARY KEY,
    operacion VARCHAR(50) NOT NULL,
    entidad VARCHAR(50) NOT NULL,
    entidadid INTEGER,
    usuarioid INTEGER,
    detalles TEXT,
    fechahora TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT fk_log_usuario FOREIGN KEY (usuarioid) 
        REFERENCES usuarios(id) ON DELETE SET NULL
);

COMMENT ON TABLE logoperaciones IS 'Log de auditor�a de operaciones (RNF7)';

-- ============================================================================
-- 3. �NDICES PARA RENDIMIENTO (RNF2)
-- ============================================================================

-- �ndices en tabla articulos
CREATE INDEX IF NOT EXISTS idx_articulos_codigo ON articulos(codigo);
CREATE INDEX IF NOT EXISTS idx_articulos_nombre ON articulos(nombre);
CREATE INDEX IF NOT EXISTS idx_articulos_categoria ON articulos(categoriaid);
CREATE INDEX IF NOT EXISTS idx_articulos_activo ON articulos(activo);
CREATE INDEX IF NOT EXISTS idx_articulos_stock ON articulos(stock);

-- �ndices en tabla usuarios
CREATE INDEX IF NOT EXISTS idx_usuarios_nombreusuario ON usuarios(nombreusuario);
CREATE INDEX IF NOT EXISTS idx_usuarios_activo ON usuarios(activo);

COMMENT ON INDEX idx_articulos_codigo IS 'Optimizaci�n para b�squeda por c�digo (UC05)';
COMMENT ON INDEX idx_articulos_nombre IS 'Optimizaci�n para b�squeda por nombre (UC02)';

-- ============================================================================
-- 4. FUNCIONES Y TRIGGERS
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 4.1 Funci�n: Actualizar fechaactualizacion autom�ticamente
-- ----------------------------------------------------------------------------
CREATE OR REPLACE FUNCTION actualizar_fecha_modificacion()
RETURNS TRIGGER AS $$
BEGIN
    NEW.fechaactualizacion = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

COMMENT ON FUNCTION actualizar_fecha_modificacion() IS 'Actualiza autom�ticamente fechaactualizacion en UPDATE';

-- ----------------------------------------------------------------------------
-- 4.2 Trigger: Actualizar fecha en articulos
-- ----------------------------------------------------------------------------
DROP TRIGGER IF EXISTS trg_articulos_fecha_actualizacion ON articulos;
CREATE TRIGGER trg_articulos_fecha_actualizacion
    BEFORE UPDATE ON articulos
    FOR EACH ROW
    EXECUTE FUNCTION actualizar_fecha_modificacion();

-- ============================================================================
-- 5. DATOS INICIALES (PRUEBAS)
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 5.1 Insertar Usuarios
-- NOTA: Las contrase�as deben ser hasheadas con BCrypt antes de producci�n
-- ----------------------------------------------------------------------------
INSERT INTO usuarios (nombreusuario, passwordhash, rol, activo) VALUES
('admin', 'Admin123!', 'Administrador', TRUE),
('usuario', 'Usuario123!', 'Usuario', TRUE)
ON CONFLICT (nombreusuario) DO NOTHING;

COMMENT ON TABLE usuarios IS 'IMPORTANTE: Hashear contrase�as con BCrypt antes de producci�n';

-- ----------------------------------------------------------------------------
-- 5.2 Insertar Categor�as
-- ----------------------------------------------------------------------------
INSERT INTO categorias (nombre, descripcion) VALUES
('Herramientas Manuales', 'Martillos, destornilladores, llaves, etc.'),
('Herramientas El�ctricas', 'Taladros, lijadoras, sierras el�ctricas'),
('Materiales de Construcci�n', 'Cemento, arena, ladrillos'),
('Pintura', 'Pinturas, brochas, rodillos'),
('Plomer�a', 'Tubos, llaves, accesorios de ba�o'),
('Electricidad', 'Cables, interruptores, tomacorrientes')
ON CONFLICT (nombre) DO NOTHING;

-- ----------------------------------------------------------------------------
-- 5.3 Insertar Proveedores
-- ----------------------------------------------------------------------------
INSERT INTO proveedores (nombre, telefono, email, direccion) VALUES
('Ferreter�a Total S.A.', '555-1234', 'ventas@ferreteriatotal.com', 'Av. Principal 123'),
('Distribuidora El Tornillo', '555-5678', 'info@eltornillo.com', 'Calle Industrial 456'),
('Importadora HerramientasPlus', '555-9012', 'contacto@herramientasplus.com', 'Zona Industrial Lote 10')
ON CONFLICT (nombre) DO NOTHING;

-- ----------------------------------------------------------------------------
-- 5.4 Insertar Art�culos de Prueba
-- ----------------------------------------------------------------------------
INSERT INTO articulos (codigo, nombre, descripcion, categoriaid, preciocompra, precioventa, stock, stockminimo, proveedorid) VALUES
('MART-001', 'Martillo de U�a 16oz', 'Martillo con mango de fibra de vidrio', 1, 8.50, 15.00, 25, 10, 1),
('TALAD-001', 'Taladro El�ctrico 500W', 'Taladro percutor con malet�n', 2, 45.00, 89.99, 15, 5, 3),
('PINT-001', 'Pintura L�tex Blanco 1GL', 'Pintura l�tex interior/exterior', 4, 12.00, 22.50, 50, 15, 2),
('DEST-001', 'Destornillador Phillips #2', 'Destornillador magn�tico', 1, 2.50, 5.00, 100, 20, 1),
('CABLE-001', 'Cable THW #12 AWG', 'Cable el�ctrico calibre 12 (metro)', 6, 0.80, 1.50, 500, 100, 2),
('TUBO-001', 'Tubo PVC 1/2" x 3m', 'Tubo para agua fr�a', 5, 3.20, 6.00, 80, 25, 1),
('LIJA-001', 'Lijadora Orbital 200W', 'Lijadora el�ctrica con bolsa recolectora', 2, 35.00, 69.99, 8, 5, 3),
('CERROJO-001', 'Cerrojo de Seguridad 6"', 'Cerrojo con llave dorado', 1, 8.00, 15.99, 30, 10, 2)
ON CONFLICT (codigo) DO NOTHING;

-- ============================================================================
-- 6. VISTAS �TILES
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 6.1 Vista: Art�culos que requieren reposici�n (RF7)
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW v_articulos_reposicion AS
SELECT 
    a.id,
    a.codigo,
    a.nombre,
    c.nombre AS categoria,
    a.stock,
    a.stockminimo,
    (a.stockminimo - a.stock) AS cantidad_requerida,
    p.nombre AS proveedor,
    p.telefono AS telefono_proveedor
FROM articulos a
LEFT JOIN categorias c ON a.categoriaid = c.id
LEFT JOIN proveedores p ON a.proveedorid = p.id
WHERE a.stock < a.stockminimo 
  AND a.activo = TRUE
ORDER BY (a.stockminimo - a.stock) DESC;

COMMENT ON VIEW v_articulos_reposicion IS 'Art�culos con stock bajo que requieren reposici�n (RF7)';

-- ----------------------------------------------------------------------------
-- 6.2 Vista: Inventario valorizado
-- ----------------------------------------------------------------------------
CREATE OR REPLACE VIEW v_inventario_valorizado AS
SELECT 
    a.codigo,
    a.nombre,
    c.nombre AS categoria,
    a.stock,
    a.preciocompra,
    a.precioventa,
    (a.stock * a.preciocompra) AS valor_compra,
    (a.stock * a.precioventa) AS valor_venta,
    ((a.precioventa - a.preciocompra) * a.stock) AS margen_potencial
FROM articulos a
LEFT JOIN categorias c ON a.categoriaid = c.id
WHERE a.activo = TRUE
ORDER BY margen_potencial DESC;

COMMENT ON VIEW v_inventario_valorizado IS 'Valorizaci�n del inventario con m�rgenes';

-- ============================================================================
-- 7. CONSULTAS DE VERIFICACI�N
-- ============================================================================

-- Verificar creaci�n de tablas
SELECT 
    table_name, 
    (SELECT COUNT(*) FROM information_schema.columns WHERE table_name = t.table_name) AS num_columnas
FROM information_schema.tables t
WHERE table_schema = 'public'
  AND table_type = 'BASE TABLE'
ORDER BY table_name;

-- Verificar datos iniciales
SELECT 'Usuarios' AS tabla, COUNT(*) AS registros FROM usuarios
UNION ALL
SELECT 'Categor�as', COUNT(*) FROM categorias
UNION ALL
SELECT 'Proveedores', COUNT(*) FROM proveedores
UNION ALL
SELECT 'Art�culos', COUNT(*) FROM articulos;

-- Verificar art�culos con stock bajo
SELECT * FROM v_articulos_reposicion;

-- ============================================================================
-- 8. SCRIPT DE LIMPIEZA (OPCIONAL - SOLO DESARROLLO)
-- ============================================================================

-- DESCOMENTA SOLO SI NECESITAS ELIMINAR TODAS LAS TABLAS
-- WARNING: Esto eliminar� TODOS los datos

/*
DROP VIEW IF EXISTS v_inventario_valorizado CASCADE;
DROP VIEW IF EXISTS v_articulos_reposicion CASCADE;
DROP TABLE IF EXISTS logoperaciones CASCADE;
DROP TABLE IF EXISTS articulos CASCADE;
DROP TABLE IF EXISTS proveedores CASCADE;
DROP TABLE IF EXISTS categorias CASCADE;
DROP TABLE IF EXISTS usuarios CASCADE;
DROP FUNCTION IF EXISTS actualizar_fecha_modificacion() CASCADE;
*/

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================

SELECT '? Script ejecutado correctamente. Base de datos configurada.' AS status;
