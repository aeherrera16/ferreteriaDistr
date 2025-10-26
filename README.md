# ?? Sistema de Inventario Ferretería - SOAP Service

Sistema de gestión de inventario para ferretería usando arquitectura SOAP con .NET 9.

## ?? Arquitectura del Proyecto

```
InventarioFerreteria/
??? ??? InventarioFerreteria.SoapService   (Servidor SOAP - ASP.NET Core Web)
??? ?? InventarioFerreteria.Client        (Cliente de Consola)
??? ?? InventarioFerreteria.Business      (Capa de Negocio)
??? ??? InventarioFerreteria.DataAccess    (Capa de Datos - EF Core)
??? ?? InventarioFerreteria.Entities      (Modelos y DTOs)
??? ?? docs/                              (Documentación técnica)
```

## ?? EQUIPO DEL PROYECTO

| Miembro | Responsabilidad | Archivos Importantes |
|---------|----------------|---------------------|
| **Anahy** | Documentación, Base de Datos, ERS/DDA | `docs/LEEME_ANAHY.md` ? |
| **Oscar** | Backend, Servicios SOAP, Lógica de Negocio | Business/, SoapService/ |
| **Camila** | Frontend, Cliente, DevOps | Client/, README.md |

### ?? **ANAHY: Lee esto primero ?** [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)

---

## ?? Cómo Ejecutar el Sistema

### **Opción 1: Visual Studio (Recomendado)**

1. Abre `InventarioFerreteria.sln` en Visual Studio
2. Click derecho en la **Solución** ? **Configurar proyectos de inicio**
3. Selecciona **"Varios proyectos de inicio"**
4. Marca como **"Iniciar"**:
   - ? InventarioFerreteria.SoapService
   - ? InventarioFerreteria.Client
5. Click **Aceptar**
6. Presiona **F5**

### **Opción 2: Script Automatizado (Windows)**

**PowerShell:**
```powershell
.\run-both.ps1
```

**Batch:**
```cmd
run-both.bat
```

### **Opción 3: Manualmente (2 Terminales)**

**Terminal 1 - Servidor:**
```bash
dotnet run --project InventarioFerreteria.SoapService
```

**Terminal 2 - Cliente (espera 5 segundos):**
```bash
dotnet run --project InventarioFerreteria.Client
```

---

## ??? CONFIGURACIÓN DE BASE DE DATOS (Para Anahy)

### **Paso 1: Crear la Base de Datos**

1. Abre **pgAdmin**
2. Click derecho en "Databases" ? **Create** ? **Database**
3. Nombre: `ferreteria`
4. Click "Save"

### **Paso 2: Ejecutar Script SQL**

1. Click derecho en la base de datos `ferreteria` ? **Query Tool**
2. Abre y ejecuta el archivo:
   ```
   docs/Scripts-SQL/01-create-schema.sql
   ```

### **Paso 3: Hashear Contraseñas (IMPORTANTE)**

Ver guía completa en: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)

**Opción rápida:**
```bash
cd C:\Users\Anahy\Desktop
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

Edita `Program.cs` y ejecuta `dotnet run` para obtener los hashes BCrypt.

---

## ?? Credenciales de Acceso

### Usuario Administrador
- **Usuario:** `admin`
- **Contraseña:** `Admin123!`

### Usuario Normal
- **Usuario:** `usuario`
- **Contraseña:** `Usuario123!`

---

## ?? Endpoints

- **Servicio SOAP:** http://localhost:5233/InventarioService.asmx
- **WSDL:** http://localhost:5233/InventarioService.asmx?wsdl

---

## ??? Estructura de la Base de Datos

### Tablas Principales
- `usuarios` - Gestión de usuarios y autenticación
- `articulos` - Catálogo de productos
- `categorias` - Categorías de productos
- `proveedores` - Proveedores
- `logoperaciones` - Auditoría de operaciones

### Configuración en appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ferreteria;Username=postgres;Password=admin"
  }
}
```

---

## ?? Funcionalidades del Cliente

1. **Autenticación** - Login con JWT
2. **Insertar Artículo** - Agregar nuevos productos
3. **Consultar Artículo** - Buscar por código
4. **Listar Artículos** - Ver inventario completo
5. **Salir** - Cerrar aplicación

---

## ??? Tecnologías Utilizadas

- **.NET 9.0**
- **ASP.NET Core**
- **Entity Framework Core 9.0**
- **PostgreSQL (Npgsql)**
- **SoapCore** - Implementación SOAP
- **BCrypt.Net** - Hashing de contraseñas
- **JWT** - Autenticación con tokens

---

## ?? Datos de Prueba

### Artículos de Ejemplo
- `MART-001` - Martillo de Uña 16oz
- `TALAD-001` - Taladro Eléctrico 500W
- `PINT-001` - Pintura Látex Blanco 1GL
- `DEST-001` - Destornillador Phillips #2
- `CABLE-001` - Cable THW #12 AWG
- `TUBO-001` - Tubo PVC 1/2" x 3m

### Categorías
1. Herramientas Manuales
2. Herramientas Eléctricas
3. Materiales de Construcción
4. Pintura
5. Plomería
6. Electricidad

---

## ?? Flujo de Uso

1. **Ejecuta el servidor** (se abrirá automáticamente)
2. **Ejecuta el cliente** (verás el menú en consola)
3. **Autentícate** (opción 1) con usuario y contraseña
4. **Usa las funciones** (opciones 2-4) para gestionar el inventario
5. **Sal del sistema** (opción 5)

---

## ?? DOCUMENTACIÓN DEL PROYECTO

### Para Miembros del Equipo

| Documento | Responsable | Descripción |
|-----------|-------------|-------------|
| [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md) | **Anahy** ? | **Guía completa paso a paso para Anahy** |
| [`docs/GUIA_ANAHY.md`](docs/GUIA_ANAHY.md) | **Anahy** | Guía técnica detallada |
| [`docs/01-ERS/`](docs/01-ERS/) | **Anahy** | Especificación de Requisitos del Sistema |
| [`docs/02-DDA/`](docs/02-DDA/) | **Anahy** | Documento de Diseño Arquitectónico |
| [`docs/Scripts-SQL/`](docs/Scripts-SQL/) | **Anahy** | Scripts de base de datos |
| [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md) | **Anahy** | Solución de problemas de autenticación |

### Documentos Técnicos

- **ERS (Especificación de Requisitos):** `docs/01-ERS/ERS_InventarioFerreteria.md`
- **DDA (Diseño Arquitectónico):** `docs/02-DDA/DDA_InventarioFerreteria.md`
- **Scripts SQL:** `docs/Scripts-SQL/01-create-schema.sql`
- **Diagramas:** `docs/Diagramas/` (por crear)
- **Evidencias:** `docs/Evidencias/` (capturas de pruebas)

---

## ?? Troubleshooting

### Error: "No se puede conectar a PostgreSQL"
- Verifica que PostgreSQL esté ejecutándose
- Confirma las credenciales en `appsettings.json`
- Verifica que la base de datos `ferreteria` exista

### Error: "El servidor no responde"
- Asegúrate de ejecutar el servidor SOAP primero
- Espera 5 segundos antes de ejecutar el cliente
- Verifica que el puerto 5233 no esté en uso

### Error de autenticación
- **IMPORTANTE:** Las contraseñas deben estar hasheadas con BCrypt
- Ver guía completa: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)
- Ejecuta el script `docs/Scripts-SQL/02-hash-passwords.sql`

---

## ?? GUÍA RÁPIDA POR ROL

### **Si eres ANAHY (Documentación y BD):**
1. ? Lee **PRIMERO** ? [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)
2. Crea la base de datos (Paso 1-3 en LEEME_ANAHY.md)
3. Crea los diagramas UML (casos de uso, clases, secuencia)
4. Completa la documentación ERS y DDA
5. Toma capturas de pruebas (cuando el sistema esté listo)
6. Crea el informe final

### **Si eres OSCAR (Backend):**
1. Implementa la capa de negocio (Business)
2. Implementa los servicios SOAP (SoapService)
3. Crea pruebas unitarias
4. Coordina con Anahy para validar WSDL/XSD

### **Si eres CAMILA (Frontend/DevOps):**
1. Implementa el cliente de consola (Client)
2. Crea la interfaz de usuario (si aplica)
3. Configura CI/CD (GitHub Actions)
4. Crea el manual de despliegue

---

## ?? Soporte

### Problemas de Autenticación
Ver: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)

### Problemas de Base de Datos
Ver: [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md) - Paso 1-3

### Problemas de Ejecución
Ver sección "Troubleshooting" arriba

---

## ?? INICIO RÁPIDO (3 Pasos)

### 1?? Crear Base de Datos (Anahy)
```bash
# En pgAdmin, ejecuta:
docs/Scripts-SQL/01-create-schema.sql
```

### 2?? Hashear Contraseñas (Anahy)
```bash
# Sigue la guía en:
TROUBLESHOOTING_AUTH.md
```

### 3?? Ejecutar Sistema
```bash
# Opción simple:
.\run-both.ps1

# O en Visual Studio: F5
```

---

**Desarrollado por:** Anahy Herrera, Oscar [Apellido], Camila [Apellido]  
**Institución:** [Tu Universidad/Institución]  
**Fecha:** Octubre 2025  
**Tecnologías:** .NET 9, PostgreSQL, SOAP, Entity Framework Core

---

**?? Para empezar, Anahy lee:** [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)
