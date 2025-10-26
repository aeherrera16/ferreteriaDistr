# ?? Solución de Problemas de Autenticación

## ?? PROBLEMA IMPORTANTE: Contraseñas en la Base de Datos

Tu base de datos tiene contraseñas en **texto plano**, pero tu código usa **BCrypt** para hashear contraseñas.

**Contraseñas actuales en la BD:**
```sql
admin -> "Admin123!"  (texto plano)
usuario -> "Usuario123!" (texto plano)
```

**El código espera:**
```
Contraseñas hasheadas con BCrypt
```

## ??? Solución: Actualizar las Contraseñas

### Opción 1: Script SQL para Hashear las Contraseñas

Ejecuta estos comandos en PostgreSQL para hashear las contraseñas:

```sql
-- Hash de "Admin123!" usando BCrypt
UPDATE usuarios 
SET passwordhash = '$2a$11$rQJ8vXqK7ZXN8X8X8X8X8eO9sT5qJ5qJ5qJ5qJ5qJ5qJ5qJ5qJ5qJ'
WHERE nombreusuario = 'admin';

-- Hash de "Usuario123!" usando BCrypt
UPDATE usuarios 
SET passwordhash = '$2a$11$sQJ8vXqK7ZXN8X8X8X8X8eO9sT5qJ5qJ5qJ5qJ5qJ5qJ5qJ5qJ5qJ'
WHERE nombreusuario = 'usuario';
```

**NOTA:** Los hashes de arriba son ejemplos. Necesitas generar los hashes reales.

### Opción 2: Crear un Script de Migración de Datos

Voy a crear un script C# que genere los hashes correctos:

1. Ejecuta este script para obtener los hashes:
```csharp
using BCrypt.Net;

string adminPassword = "Admin123!";
string usuarioPassword = "Usuario123!";

string adminHash = BCrypt.HashPassword(adminPassword);
string usuarioHash = BCrypt.HashPassword(usuarioPassword);

Console.WriteLine($"Admin hash: {adminHash}");
Console.WriteLine($"Usuario hash: {usuarioHash}");
```

2. Luego ejecuta el UPDATE en PostgreSQL con los hashes generados.

### Opción 3: Modificar Temporalmente el Código (NO RECOMENDADO)

Puedes temporalmente modificar `AutenticacionService.cs` para comparar texto plano:

```csharp
// TEMPORAL - Solo para pruebas
public async Task<RespuestaDTO<string>> Autenticar(string nombreUsuario, string password)
{
    var usuario = await _usuarioRepository.ObtenerPorNombreUsuario(nombreUsuario);
    
    if (usuario == null || !usuario.Activo)
    {
        return new RespuestaDTO<string>
        {
            Exito = false,
            Mensaje = "Usuario o contraseña incorrectos"
        };
    }

    // COMPARACIÓN TEMPORAL EN TEXTO PLANO
    bool esPasswordValido = usuario.PasswordHash == password;
    
    if (!esPasswordValido)
    {
        return new RespuestaDTO<string>
        {
            Exito = false,
            Mensaje = "Usuario o contraseña incorrectos"
        };
    }
    
    // ... resto del código
}
```

## ? Solución Correcta Paso a Paso

### 1. Generar los Hashes BCrypt

Ejecuta este código en una consola .NET o usa un generador online:

**C# Console:**
```bash
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

Edita Program.cs:
```csharp
using BCrypt.Net;

Console.WriteLine("=== Generador de Hashes BCrypt ===\n");

string[] passwords = { "Admin123!", "Usuario123!" };

foreach (var pwd in passwords)
{
    string hash = BCrypt.HashPassword(pwd);
    Console.WriteLine($"Password: {pwd}");
    Console.WriteLine($"Hash: {hash}\n");
}
```

Ejecuta:
```bash
dotnet run
```

### 2. Actualizar la Base de Datos

Copia los hashes generados y ejecuta:

```sql
-- Reemplaza HASH_AQUI con el hash generado
UPDATE usuarios 
SET passwordhash = 'HASH_DE_ADMIN_AQUI'
WHERE nombreusuario = 'admin';

UPDATE usuarios 
SET passwordhash = 'HASH_DE_USUARIO_AQUI'
WHERE nombreusuario = 'usuario';

-- Verificar cambios
SELECT nombreusuario, passwordhash FROM usuarios;
```

### 3. Probar la Autenticación

Ahora deberías poder autenticarte con:
- Usuario: `admin` / Contraseña: `Admin123!`
- Usuario: `usuario` / Contraseña: `Usuario123!`

## ?? Verificar que Funciona

```bash
# En el cliente, selecciona opción 1
1. Autenticar

Usuario: admin
Contraseña: Admin123!

# Deberías ver:
? Autenticación exitosa
Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## ?? Referencias

- BCrypt.Net-Next: https://github.com/BcryptNet/bcrypt.net
- Online BCrypt Generator: https://bcrypt-generator.com/

---

**¡Importante!** Nunca almacenes contraseñas en texto plano en producción.
