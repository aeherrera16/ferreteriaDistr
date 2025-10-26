# ?? Sistema de Inventario Ferreter�a - SOAP Service

Sistema de gesti�n de inventario para ferreter�a usando arquitectura SOAP con .NET 9.

## ?? Arquitectura del Proyecto

```
InventarioFerreteria/
??? ??? InventarioFerreteria.SoapService   (Servidor SOAP - ASP.NET Core Web)
??? ?? InventarioFerreteria.Client        (Cliente de Consola)
??? ?? InventarioFerreteria.Business      (Capa de Negocio)
??? ??? InventarioFerreteria.DataAccess    (Capa de Datos - EF Core)
??? ?? InventarioFerreteria.Entities      (Modelos y DTOs)
??? ?? docs/                              (Documentaci�n t�cnica)
```

## ?? EQUIPO DEL PROYECTO

| Miembro | Responsabilidad | Archivos Importantes |
|---------|----------------|---------------------|
| **Anahy** | Documentaci�n, Base de Datos, ERS/DDA | `docs/LEEME_ANAHY.md` ? |
| **Oscar** | Backend, Servicios SOAP, L�gica de Negocio | Business/, SoapService/ |
| **Camila** | Frontend, Cliente, DevOps | Client/, README.md |

### ?? **ANAHY: Lee esto primero ?** [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)

---

## ?? C�mo Ejecutar el Sistema

### **Opci�n 1: Visual Studio (Recomendado)**

1. Abre `InventarioFerreteria.sln` en Visual Studio
2. Click derecho en la **Soluci�n** ? **Configurar proyectos de inicio**
3. Selecciona **"Varios proyectos de inicio"**
4. Marca como **"Iniciar"**:
   - ? InventarioFerreteria.SoapService
   - ? InventarioFerreteria.Client
5. Click **Aceptar**
6. Presiona **F5**

### **Opci�n 2: Script Automatizado (Windows)**

**PowerShell:**
```powershell
.\run-both.ps1
```

**Batch:**
```cmd
run-both.bat
```

### **Opci�n 3: Manualmente (2 Terminales)**

**Terminal 1 - Servidor:**
```bash
dotnet run --project InventarioFerreteria.SoapService
```

**Terminal 2 - Cliente (espera 5 segundos):**
```bash
dotnet run --project InventarioFerreteria.Client
```

---

## ??? CONFIGURACI�N DE BASE DE DATOS (Para Anahy)

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

### **Paso 3: Hashear Contrase�as (IMPORTANTE)**

Ver gu�a completa en: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)

**Opci�n r�pida:**
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
- **Contrase�a:** `Admin123!`

### Usuario Normal
- **Usuario:** `usuario`
- **Contrase�a:** `Usuario123!`

---

## ?? Endpoints

- **Servicio SOAP:** http://localhost:5233/InventarioService.asmx
- **WSDL:** http://localhost:5233/InventarioService.asmx?wsdl

---

## ??? Estructura de la Base de Datos

### Tablas Principales
- `usuarios` - Gesti�n de usuarios y autenticaci�n
- `articulos` - Cat�logo de productos
- `categorias` - Categor�as de productos
- `proveedores` - Proveedores
- `logoperaciones` - Auditor�a de operaciones

### Configuraci�n en appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ferreteria;Username=postgres;Password=admin"
  }
}
```

---

## ?? Funcionalidades del Cliente

1. **Autenticaci�n** - Login con JWT
2. **Insertar Art�culo** - Agregar nuevos productos
3. **Consultar Art�culo** - Buscar por c�digo
4. **Listar Art�culos** - Ver inventario completo
5. **Salir** - Cerrar aplicaci�n

---

## ??? Tecnolog�as Utilizadas

- **.NET 9.0**
- **ASP.NET Core**
- **Entity Framework Core 9.0**
- **PostgreSQL (Npgsql)**
- **SoapCore** - Implementaci�n SOAP
- **BCrypt.Net** - Hashing de contrase�as
- **JWT** - Autenticaci�n con tokens

---

## ?? Datos de Prueba

### Art�culos de Ejemplo
- `MART-001` - Martillo de U�a 16oz
- `TALAD-001` - Taladro El�ctrico 500W
- `PINT-001` - Pintura L�tex Blanco 1GL
- `DEST-001` - Destornillador Phillips #2
- `CABLE-001` - Cable THW #12 AWG
- `TUBO-001` - Tubo PVC 1/2" x 3m

### Categor�as
1. Herramientas Manuales
2. Herramientas El�ctricas
3. Materiales de Construcci�n
4. Pintura
5. Plomer�a
6. Electricidad

---

## ?? Flujo de Uso

1. **Ejecuta el servidor** (se abrir� autom�ticamente)
2. **Ejecuta el cliente** (ver�s el men� en consola)
3. **Autent�cate** (opci�n 1) con usuario y contrase�a
4. **Usa las funciones** (opciones 2-4) para gestionar el inventario
5. **Sal del sistema** (opci�n 5)

---

## ?? DOCUMENTACI�N DEL PROYECTO

### Para Miembros del Equipo

| Documento | Responsable | Descripci�n |
|-----------|-------------|-------------|
| [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md) | **Anahy** ? | **Gu�a completa paso a paso para Anahy** |
| [`docs/GUIA_ANAHY.md`](docs/GUIA_ANAHY.md) | **Anahy** | Gu�a t�cnica detallada |
| [`docs/01-ERS/`](docs/01-ERS/) | **Anahy** | Especificaci�n de Requisitos del Sistema |
| [`docs/02-DDA/`](docs/02-DDA/) | **Anahy** | Documento de Dise�o Arquitect�nico |
| [`docs/Scripts-SQL/`](docs/Scripts-SQL/) | **Anahy** | Scripts de base de datos |
| [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md) | **Anahy** | Soluci�n de problemas de autenticaci�n |

### Documentos T�cnicos

- **ERS (Especificaci�n de Requisitos):** `docs/01-ERS/ERS_InventarioFerreteria.md`
- **DDA (Dise�o Arquitect�nico):** `docs/02-DDA/DDA_InventarioFerreteria.md`
- **Scripts SQL:** `docs/Scripts-SQL/01-create-schema.sql`
- **Diagramas:** `docs/Diagramas/` (por crear)
- **Evidencias:** `docs/Evidencias/` (capturas de pruebas)

---

## ?? Troubleshooting

### Error: "No se puede conectar a PostgreSQL"
- Verifica que PostgreSQL est� ejecut�ndose
- Confirma las credenciales en `appsettings.json`
- Verifica que la base de datos `ferreteria` exista

### Error: "El servidor no responde"
- Aseg�rate de ejecutar el servidor SOAP primero
- Espera 5 segundos antes de ejecutar el cliente
- Verifica que el puerto 5233 no est� en uso

### Error de autenticaci�n
- **IMPORTANTE:** Las contrase�as deben estar hasheadas con BCrypt
- Ver gu�a completa: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)
- Ejecuta el script `docs/Scripts-SQL/02-hash-passwords.sql`

---

## ?? GU�A R�PIDA POR ROL

### **Si eres ANAHY (Documentaci�n y BD):**
1. ? Lee **PRIMERO** ? [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)
2. Crea la base de datos (Paso 1-3 en LEEME_ANAHY.md)
3. Crea los diagramas UML (casos de uso, clases, secuencia)
4. Completa la documentaci�n ERS y DDA
5. Toma capturas de pruebas (cuando el sistema est� listo)
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

### Problemas de Autenticaci�n
Ver: [`TROUBLESHOOTING_AUTH.md`](TROUBLESHOOTING_AUTH.md)

### Problemas de Base de Datos
Ver: [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md) - Paso 1-3

### Problemas de Ejecuci�n
Ver secci�n "Troubleshooting" arriba

---

## ?? INICIO R�PIDO (3 Pasos)

### 1?? Crear Base de Datos (Anahy)
```bash
# En pgAdmin, ejecuta:
docs/Scripts-SQL/01-create-schema.sql
```

### 2?? Hashear Contrase�as (Anahy)
```bash
# Sigue la gu�a en:
TROUBLESHOOTING_AUTH.md
```

### 3?? Ejecutar Sistema
```bash
# Opci�n simple:
.\run-both.ps1

# O en Visual Studio: F5
```

---

**Desarrollado por:** Anahy Herrera, Oscar [Apellido], Camila [Apellido]  
**Instituci�n:** [Tu Universidad/Instituci�n]  
**Fecha:** Octubre 2025  
**Tecnolog�as:** .NET 9, PostgreSQL, SOAP, Entity Framework Core

---

**?? Para empezar, Anahy lee:** [`docs/LEEME_ANAHY.md`](docs/LEEME_ANAHY.md)
