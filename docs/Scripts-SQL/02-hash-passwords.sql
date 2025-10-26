-- ============================================================================
-- Script para Hashear Contrase�as - PostgreSQL + BCrypt
-- ============================================================================
-- Este script actualiza las contrase�as de texto plano a hashes BCrypt
-- EJECUTAR DESPU�S DE GENERAR LOS HASHES CON LA HERRAMIENTA C#
-- ============================================================================

-- PASO 1: Generar hashes con herramienta C# (ver GUIA_ANAHY.md)
-- Crea el proyecto HashGenerator y ejecuta para obtener los hashes

-- PASO 2: Reemplaza HASH_ADMIN_AQUI y HASH_USUARIO_AQUI con los hashes generados

-- ============================================================================
-- ACTUALIZAR CONTRASE�AS HASHEADAS
-- ============================================================================

-- Actualizar contrase�a del administrador
-- Password original: Admin123!
UPDATE usuarios 
SET passwordhash = 'HASH_ADMIN_AQUI'
WHERE nombreusuario = 'admin';

-- Actualizar contrase�a del usuario normal
-- Password original: Usuario123!
UPDATE usuarios 
SET passwordhash = 'HASH_USUARIO_AQUI'
WHERE nombreusuario = 'usuario';

-- ============================================================================
-- VERIFICACI�N
-- ============================================================================

-- Verificar que los hashes se actualizaron correctamente
SELECT 
    nombreusuario,
    LEFT(passwordhash, 10) || '...' AS password_hash_preview,
    LENGTH(passwordhash) AS hash_length,
    rol,
    activo
FROM usuarios;

-- El hash_length debe ser 60 para hashes BCrypt v�lidos

-- ============================================================================
-- EJEMPLO DE HASHES BCRYPT V�LIDOS (SOLO PARA REFERENCIA)
-- ============================================================================
/*
Estos son ejemplos de c�mo se ve un hash BCrypt.
NO uses estos en producci�n - genera tus propios hashes �nicos.

Password: Admin123!
Hash ejemplo: $2a$12$KIXDQkBZvZ8OQG8Zy5j6F.7kY9vX8YqB6jH3FxH1qP3wZ8Fy6j6Fe

Password: Usuario123!
Hash ejemplo: $2a$12$LJYEQlCAwZ9PRI9Az6k7G.8lZ0wY9ZrC7kI4GxI2rQ4xA9Gz7k7Gf

Estructura del hash BCrypt:
$2a$    -> Identificador de algoritmo BCrypt
12$     -> Factor de costo (2^12 iteraciones)
XXXXXX  -> Salt (22 caracteres)
YYYYYY  -> Hash (31 caracteres)
Total: 60 caracteres
*/

-- ============================================================================
-- PROBAR AUTENTICACI�N
-- ============================================================================

-- Una vez actualizados los hashes, prueba la autenticaci�n desde el cliente:
-- 1. Ejecuta el servidor SOAP
-- 2. Ejecuta el cliente
-- 3. Opci�n 1: Autenticar
-- 4. Usuario: admin / Contrase�a: Admin123!
-- 5. Deber�as recibir un token JWT

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================

SELECT '?? IMPORTANTE: Reemplaza los hashes de ejemplo con los generados por tu herramienta BCrypt' AS advertencia;
