-- ============================================================================
-- Script para Hashear Contraseñas - PostgreSQL + BCrypt
-- ============================================================================
-- Este script actualiza las contraseñas de texto plano a hashes BCrypt
-- EJECUTAR DESPUÉS DE GENERAR LOS HASHES CON LA HERRAMIENTA C#
-- ============================================================================

-- PASO 1: Generar hashes con herramienta C# (ver GUIA_ANAHY.md)
-- Crea el proyecto HashGenerator y ejecuta para obtener los hashes

-- PASO 2: Reemplaza HASH_ADMIN_AQUI y HASH_USUARIO_AQUI con los hashes generados

-- ============================================================================
-- ACTUALIZAR CONTRASEÑAS HASHEADAS
-- ============================================================================

-- Actualizar contraseña del administrador
-- Password original: Admin123!
UPDATE usuarios 
SET passwordhash = 'HASH_ADMIN_AQUI'
WHERE nombreusuario = 'admin';

-- Actualizar contraseña del usuario normal
-- Password original: Usuario123!
UPDATE usuarios 
SET passwordhash = 'HASH_USUARIO_AQUI'
WHERE nombreusuario = 'usuario';

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

-- Verificar que los hashes se actualizaron correctamente
SELECT 
    nombreusuario,
    LEFT(passwordhash, 10) || '...' AS password_hash_preview,
    LENGTH(passwordhash) AS hash_length,
    rol,
    activo
FROM usuarios;

-- El hash_length debe ser 60 para hashes BCrypt válidos

-- ============================================================================
-- EJEMPLO DE HASHES BCRYPT VÁLIDOS (SOLO PARA REFERENCIA)
-- ============================================================================
/*
Estos son ejemplos de cómo se ve un hash BCrypt.
NO uses estos en producción - genera tus propios hashes únicos.

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
-- PROBAR AUTENTICACIÓN
-- ============================================================================

-- Una vez actualizados los hashes, prueba la autenticación desde el cliente:
-- 1. Ejecuta el servidor SOAP
-- 2. Ejecuta el cliente
-- 3. Opción 1: Autenticar
-- 4. Usuario: admin / Contraseña: Admin123!
-- 5. Deberías recibir un token JWT

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================

SELECT '?? IMPORTANTE: Reemplaza los hashes de ejemplo con los generados por tu herramienta BCrypt' AS advertencia;
