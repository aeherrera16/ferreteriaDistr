# Sistema de Inventario para Ferretería - Arquitectura SOAP

**Proyecto académico de sistema de gestión de inventario usando arquitectura N-Capas y servicios SOAP con .NET 9 y PostgreSQL.**

---

## Tabla de Contenidos

- [Descripción del Proyecto](#-descripción-del-proyecto)
- [Arquitectura](#-arquitectura-del-sistema)
- [Equipo de Desarrollo](#-equipo-de-desarrollo)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Ejecución del Sistema](#-ejecución-del-sistema)
- [Uso del Sistema](#-uso-del-sistema)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Base de Datos](#-base-de-datos)
- [Servicios SOAP](#-servicios-soap)
- [Documentación Técnica](#-documentación-técnica)
- [Troubleshooting](#-troubleshooting)

---

## Descripción del Proyecto

Sistema de control de inventario para una ferretería que implementa:

- **Arquitectura N-Capas** (Presentación, Negocio, Datos, Servicios)
- **Servicios SOAP** para operaciones de inventario
- **Autenticación JWT** con contraseñas hasheadas (BCrypt)
- **Base de datos relacional** PostgreSQL
- **Cliente de consola** consumidor de servicios SOAP

### Funcionalidades Principales

**RF1-RF4:** Gestión completa de artículos (registrar, consultar, actualizar)  
 **RF5-RF6:** Servicios SOAP para insertar y consultar artículos  
 **RF7:** Control de stock con alertas de reposición  
 **RF8:** Persistencia en PostgreSQL  
 **RF9:** Interfaz de usuario (consola)  
 **RF10:** Manejo robusto de errores con SOAP Faults

---

## Arquitectura del Sistema

### Diagrama de Capas

```
???????????????????????????????????????????????????????????
?          CAPA DE PRESENTACIÓN (Cliente)                 ?
?         InventarioFerreteria.Client                     ?
?              (Aplicación Consola)                        ?
???????????????????????????????????????????????????????????
                        ?
                        ? Consume servicios SOAP (XML/HTTP)
                        ?
???????????????????????????????????????????????????????????
?         CAPA DE SERVICIOS (Servidor SOAP)               ?
?       InventarioFerreteria.SoapService                  ?
?           (ASP.NET Core Web + SoapCore)                 ?
?   • IInventarioSoapService                              ?
?   • Autenticación JWT                                   ?
?   • Operaciones: Insertar, Consultar, Listar           ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?          CAPA DE NEGOCIO (Business Logic)               ?
?        InventarioFerreteria.Business                    ?
?   • ArticuloService                                     ?
?   • AutenticacionService                                ?
?   • Validaciones de negocio (RF3)                       ?
?   • Reglas de stock y precios                           ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?         CAPA DE ACCESO A DATOS (Data Access)            ?
?       InventarioFerreteria.DataAccess                   ?
?   • ApplicationDbContext (EF Core)                      ?
?   • ArticuloRepository                                  ?
?   • UsuarioRepository                                   ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?              BASE DE DATOS (PostgreSQL)                 ?
?   Tablas: usuarios, articulos, categorias,             ?
?           proveedores, logoperaciones                   ?
???????????????????????????????????????????????????????????

???????????????????????????????????????????????????????????
?          CAPA DE ENTIDADES (Shared)                     ?
?        InventarioFerreteria.Entities                    ?
?   • Modelos de dominio                                  ?
?   • DTOs (Data Transfer Objects)                        ?
?   • Contratos de servicio                               ?
???????????????????????????????????????????????????????????
```

---

## ?? Equipo de Desarrollo

| Miembro               | Rol Principal            | Responsabilidades                                                                                                                                                                                                                       |
| --------------------- | ------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Anahy Herrera**     | Análisis y Base de Datos | • Documento ERS (Especificación de Requisitos)<br>• Documento DDA (Diseño Arquitectónico)<br>• Diseño y scripts de base de datos<br>• Diagramas UML (Casos de Uso, Clases, Secuencia, ER)<br>• Evidencias de pruebas<br>• Informe final |
| **Oscar [Apellido]**  | Backend y Servicios      | • Capa de Negocio (Business)<br>• Capa de Datos (DataAccess)<br>• Servicios SOAP (SoapService)<br>• Pruebas unitarias e integración<br>• Logs y manejo de errores                                                                       |
| **Camila [Apellido]** | Frontend y DevOps        | • Cliente de consola<br>• Interfaz de usuario<br>• Configuración CI/CD<br>• Manual de despliegue<br>• README y guías<br>• Presentación final                                                                                            |

---

## ?? Requisitos Previos

### Software Necesario

- **.NET 9 SDK** ([Descargar](https://dotnet.microsoft.com/download/dotnet/9.0))
- **PostgreSQL 15+** ([Descargar](https://www.postgresql.org/download/))
- **pgAdmin 4** (incluido con PostgreSQL)
- **Visual Studio 2022** o **VS Code** ([Descargar](https://visualstudio.microsoft.com/))
- **Git** ([Descargar](https://git-scm.com/))

### Verificar Instalación

```bash
# Verificar .NET
dotnet --version

# Verificar PostgreSQL
psql --version

# Verificar Git
git --version
```

---

## ?? Instalación y Configuración

### 1?? Clonar el Repositorio

```bash
git clone https://github.com/aeherrera16/ferreteriaDistr.git
cd ferreteriaDistr
```

### 2?? Restaurar Paquetes NuGet

```bash
dotnet restore
```

### 3?? Configurar la Base de Datos

#### A. Crear la Base de Datos en PostgreSQL

1. Abrir **pgAdmin**
2. Click derecho en **"Databases"** ? **Create** ? **Database**
3. Configurar:
   - **Database:** `ferreteria`
   - **Owner:** `postgres`
4. Click **"Save"**

#### B. Ejecutar Scripts SQL

1. Click derecho en la base de datos **"ferreteria"** ? **Query Tool**
2. Abrir y ejecutar el archivo:
   ```
   docs/Scripts-SQL/01-create-schema.sql
   ```
3. Verificar que se crearon las tablas:
   - usuarios
   - categorias
   - proveedores
   - articulos
   - logoperaciones

#### C. Hashear las Contraseñas (IMPORTANTE)

Las contraseñas en la base de datos deben estar hasheadas con BCrypt.

**Opción 1: Script SQL Automatizado**

```sql
-- Ejecutar en Query Tool de pgAdmin:
-- docs/Scripts-SQL/02-hash-passwords.sql
```

**Opción 2: Generar Hashes Manualmente**

```bash
# Crear proyecto temporal
cd C:\Users\[TuUsuario]\Desktop
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

Editar `Program.cs`:

```csharp
using BCrypt.Net;

string adminPassword = "Admin123!";
string usuarioPassword = "Usuario123!";

string adminHash = BCrypt.HashPassword(adminPassword);
string usuarioHash = BCrypt.HashPassword(usuarioPassword);

Console.WriteLine($"Admin: {adminHash}");
Console.WriteLine($"Usuario: {usuarioHash}");
```

Ejecutar y copiar los hashes:

```bash
dotnet run
```

Actualizar en pgAdmin:

```sql
UPDATE usuarios SET passwordhash = 'HASH_ADMIN_AQUI' WHERE nombreusuario = 'admin';
UPDATE usuarios SET passwordhash = 'HASH_USUARIO_AQUI' WHERE nombreusuario = 'usuario';
```

### 4?? Configurar Cadena de Conexión

Editar `InventarioFerreteria.SoapService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ferreteria;Username=postgres;Password=TU_PASSWORD"
  }
}
```

**?? Importante:** Reemplazar `TU_PASSWORD` con tu contraseña de PostgreSQL.

---

## ?? Ejecución del Sistema

El sistema requiere ejecutar **2 proyectos simultáneamente:**

1. **Servidor SOAP** (InventarioFerreteria.SoapService)
2. **Cliente** (InventarioFerreteria.Client)

### Opción 1: Visual Studio (Recomendado)

1. Abrir `InventarioFerreteria.sln` en Visual Studio
2. Click derecho en la **Solución** (raíz del Explorador de Soluciones)
3. Seleccionar **"Configurar proyectos de inicio"** o **"Set Startup Projects"**
4. Elegir **"Varios proyectos de inicio"** / **"Multiple startup projects"**
5. Configurar:

   | Proyecto                         | Acción      |
   | -------------------------------- | ----------- |
   | InventarioFerreteria.SoapService | **Iniciar** |
   | InventarioFerreteria.Client      | **Iniciar** |
   | (resto de proyectos)             | Ninguno     |

6. Click **"Aceptar"**
7. Presionar **F5** o click en ?? **Iniciar**

**Resultado:** Se abrirán 2 ventanas:

- Terminal del servidor SOAP (puerto 5233)
- Terminal del cliente (menú interactivo)

### Opción 2: Línea de Comandos (2 Terminales)

**Terminal 1 - Servidor SOAP:**

```bash
cd InventarioFerreteria.SoapService
dotnet run
```

Esperar a ver:

```
Now listening on: http://localhost:5233
```

**Terminal 2 - Cliente:**

```bash
cd InventarioFerreteria.Client
dotnet run
```

### Opción 3: Compilar y Ejecutar

```bash
# Compilar toda la solución
dotnet build

# Ejecutar servidor
cd InventarioFerreteria.SoapService/bin/Debug/net9.0
./InventarioFerreteria.SoapService.exe

# En otra terminal, ejecutar cliente
cd InventarioFerreteria.Client/bin/Debug/net9.0
./InventarioFerreteria.Client.exe
```

---

## ?? Uso del Sistema

### Menú Principal del Cliente

```
=== Cliente SOAP - Sistema de Inventario Ferretería ===

--- MENÚ PRINCIPAL ---
1. Autenticar
2. Insertar Artículo
3. Consultar Artículo por Código
4. Listar Todos los Artículos
5. Salir

Seleccione una opción:
```

### 1?? Autenticación (PRIMER PASO OBLIGATORIO)

```
--- AUTENTICACIÓN ---
Usuario: admin
Contraseña: Admin123!

? Autenticación exitosa
Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Credenciales disponibles:**

- **Administrador:** usuario: `admin` / contraseña: `Admin123!`
- **Usuario:** usuario: `usuario` / contraseña: `Usuario123!`

### 2?? Insertar Artículo

```
--- INSERTAR NUEVO ARTÍCULO ---
Código: TEST-001
Nombre: Martillo Prueba
Descripción: Artículo de prueba
ID Categoría: 1
Precio Compra: 10.00
Precio Venta: 20.00
Stock: 50
Stock Mínimo: 10
ID Proveedor: 1

? Artículo insertado correctamente
==================================================
ID: 13
Código: TEST-001
Nombre: Martillo Prueba
...
```

**Validaciones automáticas:**

- Código único (no duplicados)
- Precio venta > Precio compra
- Stock ? 0
- Alerta si stock < stock mínimo

### 3?? Consultar Artículo por Código

```
--- CONSULTAR ARTÍCULO ---
Código del artículo: MART-001

? Artículo encontrado
==================================================
Código: MART-001
Nombre: Martillo de Uña 16oz
Stock: 25
Precio Venta: $15.00
...
```

### 4?? Listar Todos los Artículos

```
? Artículos obtenidos exitosamente

Código          Nombre                          Stock  Precio Venta
----------------------------------------------------------------------
MART-001        Martillo de Uña 16oz              25        $15.00
TALAD-001       Taladro Eléctrico 500W            15        $89.99
PINT-001        Pintura Látex Blanco 1GL          50        $22.50
...
```

---

## ??? Tecnologías Utilizadas

### Backend

| Tecnología                      | Versión  | Propósito                        |
| ------------------------------- | -------- | -------------------------------- |
| .NET                            | 9.0      | Framework principal              |
| ASP.NET Core                    | 9.0      | Servidor web                     |
| Entity Framework Core           | 9.0      | ORM para base de datos           |
| SoapCore                        | 1.1.0.50 | Implementación de servicios SOAP |
| Npgsql                          | 9.0.0    | Driver PostgreSQL                |
| BCrypt.Net-Next                 | 4.0.3    | Hashing de contraseñas           |
| System.IdentityModel.Tokens.Jwt | 8.2.1    | Autenticación JWT                |

### Base de Datos

- **PostgreSQL 15+** - Base de datos relacional

### Herramientas de Desarrollo

- **Visual Studio 2022** / **VS Code**
- **pgAdmin 4** - Administración de PostgreSQL
- **Git** - Control de versiones
- **SoapUI** / **Postman** - Pruebas de servicios SOAP

---

## ?? Estructura del Proyecto

```
InventarioFerreteria/
?
??? InventarioFerreteria.SoapService/      # ??? Servidor SOAP (ASP.NET Core Web)
?   ??? Services/
?   ?   ??? IInventarioSoapService.cs      # Contrato SOAP
?   ?   ??? InventarioSoapService.cs       # Implementación SOAP
?   ??? Program.cs                          # Configuración del servidor
?   ??? appsettings.json                    # Configuración (cadena de conexión)
?   ??? InventarioFerreteria.SoapService.csproj
?
??? InventarioFerreteria.Client/            # ?? Cliente de Consola
?   ??? Program.cs                          # Lógica del cliente SOAP
?   ??? InventarioFerreteria.Client.csproj
?
??? InventarioFerreteria.Business/          # ?? Capa de Negocio
?   ??? Interfaces/
?   ?   ??? IArticuloService.cs
?   ?   ??? IAutenticacionService.cs
?   ??? Services/
?   ?   ??? ArticuloService.cs             # Lógica de negocio de artículos
?   ?   ??? AutenticacionService.cs        # Lógica de autenticación
?   ??? InventarioFerreteria.Business.csproj
?
??? InventarioFerreteria.DataAccess/        # ??? Capa de Acceso a Datos
?   ??? ApplicationDbContext.cs            # DbContext de EF Core
?   ??? Interfaces/
?   ?   ??? IArticuloRepository.cs
?   ?   ??? IUsuarioRepository.cs
?   ??? Repositories/
?   ?   ??? ArticuloRepository.cs
?   ?   ??? UsuarioRepository.cs
?   ??? InventarioFerreteria.DataAccess.csproj
?
??? InventarioFerreteria.Entities/          # ?? Modelos y DTOs
?   ??? Articulo.cs                         # Modelo de dominio
?   ??? Usuario.cs
?   ??? Categoria.cs
?   ??? Proveedor.cs
?   ??? DTOs/
?   ?   ??? ArticuloDTO.cs                 # Data Transfer Object
?   ?   ??? AutenticacionDTO.cs
?   ?   ??? RespuestaDTO.cs
?   ??? InventarioFerreteria.Entities.csproj
?
??? docs/                                    # ?? Documentación Técnica
?   ??? 01-ERS/
?   ?   ??? ERS_InventarioFerreteria.md    # Especificación de Requisitos
?   ??? 02-DDA/
?   ?   ??? DDA_InventarioFerreteria.md    # Documento de Diseño Arquitectónico
?   ??? Scripts-SQL/
?       ??? 01-create-schema.sql           # Script de creación de BD
?       ??? 02-hash-passwords.sql          # Script de hasheo de contraseñas
?
??? .gitignore                              # Archivos ignorados por Git
??? README.md                               # Este archivo
??? InventarioFerreteria.sln                # Solución de Visual Studio
```

---

## ??? Base de Datos

### Modelo Entidad-Relación

#### Tablas Principales

**1. usuarios**

```sql
- id (PK, serial)
- nombreusuario (varchar(50), unique)
- passwordhash (varchar(255))  -- BCrypt hash
- rol (varchar(20))             -- 'Administrador' o 'Usuario'
- fechacreacion (timestamp)
- activo (boolean)
```

**2. articulos**

```sql
- id (PK, serial)
- codigo (varchar(50), unique)
- nombre (varchar(200))
- descripcion (text)
- categoriaid (FK ? categorias)
- preciocompra (numeric(10,2))
- precioventa (numeric(10,2))
- stock (integer)
- stockminimo (integer)
- proveedorid (FK ? proveedores)
- fechacreacion (timestamp)
- fechaactualizacion (timestamp)
- activo (boolean)
```

**3. categorias**

```sql
- id (PK, serial)
- nombre (varchar(100))
- descripcion (text)
```

**4. proveedores**

```sql
- id (PK, serial)
- nombre (varchar(150))
- telefono (varchar(20))
- email (varchar(100))
- direccion (text)
```

**5. logoperaciones** (Auditoría)

```sql
- id (PK, serial)
- operacion (varchar(50))
- entidad (varchar(50))
- entidadid (integer)
- usuarioid (FK ? usuarios)
- detalles (text)
- fechahora (timestamp)
```

### Constraints y Validaciones

```sql
-- Validación de precios
CHECK (preciocompra > 0)
CHECK (precioventa > 0)
CHECK (precioventa >= preciocompra)

-- Validación de stock
CHECK (stock >= 0)
CHECK (stockminimo >= 0)

-- Unicidad
UNIQUE (codigo)
UNIQUE (nombreusuario)
```

### Índices para Optimización

```sql
CREATE INDEX idx_articulos_codigo ON articulos(codigo);
CREATE INDEX idx_articulos_nombre ON articulos(nombre);
CREATE INDEX idx_articulos_categoria ON articulos(categoriaid);
CREATE INDEX idx_articulos_activo ON articulos(activo);
```

### Datos de Ejemplo

Ver script completo en: `docs/Scripts-SQL/01-create-schema.sql`

**Usuarios:**

- `admin` / `Admin123!` (Administrador)
- `usuario` / `Usuario123!` (Usuario)

**Categorías:**

1. Herramientas Manuales
2. Herramientas Eléctricas
3. Materiales de Construcción
4. Pintura
5. Plomería
6. Electricidad

**Proveedores:**

1. Ferretería Total S.A.
2. Distribuidora El Tornillo
3. Importadora HerramientasPlus

**Artículos de ejemplo:**

- MART-001: Martillo de Uña 16oz ($15.00)
- TALAD-001: Taladro Eléctrico 500W ($89.99)
- PINT-001: Pintura Látex Blanco 1GL ($22.50)
- DEST-001: Destornillador Phillips #2 ($5.00)
- CABLE-001: Cable THW #12 AWG ($1.50/metro)
- TUBO-001: Tubo PVC 1/2" x 3m ($6.00)

---

## ?? Servicios SOAP

### Endpoint Base

```
http://localhost:5233/InventarioService.asmx
```

### WSDL (Web Services Description Language)

```
http://localhost:5233/InventarioService.asmx?wsdl
```

### Operaciones Disponibles

#### 1. Autenticar

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <Autenticar xmlns="http://tempuri.org/">
      <nombreUsuario>admin</nombreUsuario>
      <password>Admin123!</password>
    </Autenticar>
  </soap:Body>
</soap:Envelope>
```

**Respuesta exitosa:**

```xml
<Autenticar Response>
  <AutenticarResult>
    <Exito>true</Exito>
    <Mensaje>Autenticación exitosa</Mensaje>
    <Datos>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</Datos>
  </AutenticarResult>
</AutenticarResponse>
```

#### 2. InsertarArticulo

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <InsertarArticulo xmlns="http://tempuri.org/">
      <token>JWT_TOKEN_AQUI</token>
      <codigo>TEST-001</codigo>
      <nombre>Artículo de Prueba</nombre>
      <descripcion>Descripción del artículo</descripcion>
      <categoriaId>1</categoriaId>
      <precioCompra>10.00</precioCompra>
      <precioVenta>20.00</precioVenta>
      <stock>50</stock>
      <stockMinimo>10</stockMinimo>
      <proveedorId>1</proveedorId>
    </InsertarArticulo>
  </soap:Body>
</soap:Envelope>
```

#### 3. ConsultarArticuloPorCodigo

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <ConsultarArticuloPorCodigo xmlns="http://tempuri.org/">
      <token>JWT_TOKEN_AQUI</token>
      <codigo>MART-001</codigo>
    </ConsultarArticuloPorCodigo>
  </soap:Body>
</soap:Envelope>
```

#### 4. ObtenerTodosArticulos

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <ObtenerTodosArticulos xmlns="http://tempuri.org/">
      <token>JWT_TOKEN_AQUI</token>
    </ObtenerTodosArticulos>
  </soap:Body>
</soap:Envelope>
```

### SOAP Faults (Manejo de Errores)

```xml
<soap:Fault>
  <faultcode>soap:Client</faultcode>
  <faultstring>Código de artículo ya existe</faultstring>
  <detail>
    <ValidationFault>
      <Codigo>409</Codigo>
      <Mensaje>Código duplicado</Mensaje>
    </ValidationFault>
  </detail>
</soap:Fault>
```

---

## ?? Documentación Técnica

### Documentos Disponibles

| Documento       | Ubicación                                 | Responsable | Descripción                                                                                                                                             |
| --------------- | ----------------------------------------- | ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **ERS**         | `docs/01-ERS/ERS_InventarioFerreteria.md` | Anahy       | Especificación de Requisitos del Sistema<br>• Casos de uso<br>• Requerimientos funcionales y no funcionales<br>• Modelo de dominio                      |
| **DDA**         | `docs/02-DDA/DDA_InventarioFerreteria.md` | Anahy       | Documento de Diseño Arquitectónico<br>• Arquitectura N-Capas<br>• Diagramas de clases y secuencia<br>• Modelo de base de datos<br>• Definición WSDL/XSD |
| **Scripts SQL** | `docs/Scripts-SQL/`                       | Anahy       | • 01-create-schema.sql (Creación de BD)<br>• 02-hash-passwords.sql (Hasheo de contraseñas)                                                              |

### Diagramas

Los diagramas se encuentran documentados en los archivos ERS y DDA:

- **Diagrama de Casos de Uso** (ERS)
- **Diagrama de Clases** (DDA)
- **Diagramas de Secuencia** (DDA)
  - Autenticación
  - Insertar Artículo
  - Consultar Artículo
- **Diagrama de Entidad-Relación** (DDA)
- **Diagrama de Despliegue** (DDA)
- **Diagrama de Arquitectura N-Capas** (DDA)

---

## ?? Troubleshooting

### Problema 1: No se puede conectar a PostgreSQL

**Síntomas:**

```
Npgsql.NpgsqlException: Failed to connect to localhost:5432
```

**Soluciones:**

1. **Verificar que PostgreSQL esté ejecutándose:**

   - Windows: Buscar "services.msc" ? Buscar "PostgreSQL" ? Verificar que esté "Running"
   - Linux: `sudo systemctl status postgresql`

2. **Verificar credenciales en appsettings.json:**

   ```json
   "DefaultConnection": "Host=localhost;Port=5432;Database=ferreteria;Username=postgres;Password=TU_PASSWORD"
   ```

3. **Verificar que la base de datos exista:**

   ```sql
   -- En pgAdmin Query Tool:
   SELECT datname FROM pg_database WHERE datname = 'ferreteria';
   ```

4. **Verificar firewall:**
   - Asegurar que el puerto 5432 esté abierto

---

### Problema 2: Error de autenticación ("Token inválido")

**Síntomas:**

```
? Token inválido o expirado
```

**Causas comunes:**

1. **Las contraseñas NO están hasheadas con BCrypt**

   **Solución:** Ejecutar el script de hasheo:

   ```sql
   -- En pgAdmin:
   -- Abrir y ejecutar: docs/Scripts-SQL/02-hash-passwords.sql
   ```

2. **Contraseña incorrecta**

   **Credenciales correctas:**

   - admin / Admin123!
   - usuario / Usuario123!

3. **Token expirado** (después de 8 horas)

   **Solución:** Volver a autenticarse (Opción 1 del menú)

---

### Problema 3: El servidor no inicia

**Síntomas:**

```
Failed to bind to address http://localhost:5233
```

**Soluciones:**

1. **Puerto 5233 ocupado:**

   ```bash
   # Windows
   netstat -ano | findstr :5233

   # Matar el proceso
   taskkill /PID [PID_NUMBER] /F
   ```

2. **Cambiar el puerto:**

   Editar `InventarioFerreteria.SoapService/Properties/launchSettings.json`:

   ```json
   "applicationUrl": "http://localhost:5234"
   ```

   **Nota:** También cambiar en el cliente (Program.cs):

   ```csharp
   private const string ServiceUrl = "http://localhost:5234/InventarioService.asmx";
   ```

---

### Problema 4: Error al insertar artículo

**Síntomas:**

```
? Error: Código de artículo ya existe
```

**Causas:**

1. **Código duplicado** (validación correcta)

   - Usar un código diferente

2. **Precio venta < Precio compra**

   ```
   ? Error: El precio de venta debe ser mayor al precio de compra
   ```

   - Ajustar los precios

3. **Categoría o Proveedor inexistente**
   ```
   ? Error: La categoría especificada no existe
   ```
   - Verificar IDs válidos en la base de datos:
     ```sql
     SELECT id, nombre FROM categorias;
     SELECT id, nombre FROM proveedores;
     ```

---

### Problema 5: No se pueden ejecutar ambos proyectos

**Solución para Visual Studio:**

1. Click derecho en la **Solución** (no en un proyecto individual)
2. **"Configurar proyectos de inicio"**
3. **"Varios proyectos de inicio"**
4. Marcar como "Iniciar":
   - InventarioFerreteria.SoapService
   - InventarioFerreteria.Client
5. **Orden:** Servidor primero, Cliente segundo
6. Click **"Aceptar"**

**Solución alternativa (2 instancias de Visual Studio):**

1. Abrir 2 Visual Studios
2. En la primera: Ejecutar solo SoapService
3. En la segunda: Ejecutar solo Client

---

### Problema 6: Error de compilación

**Síntomas:**

```
Error CS0246: The type or namespace name 'X' could not be found
```

**Soluciones:**

1. **Restaurar paquetes NuGet:**

   ```bash
   dotnet restore
   ```

2. **Limpiar y reconstruir:**

   ```bash
   dotnet clean
   dotnet build
   ```

3. **Verificar referencias de proyectos:**
   - Todos los proyectos deben referenciar `InventarioFerreteria.Entities`
   - SoapService debe referenciar Business, DataAccess y Entities
   - Business debe referenciar DataAccess y Entities
   - DataAccess debe referenciar Entities

---

### Problema 7: Logs y Debugging

**Ver logs del servidor:**

Los logs aparecen en la consola del servidor:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5233
info: Microsoft.AspNetCore.Hosting.Diagnostics[1]
      Request starting HTTP/1.1 POST http://localhost:5233/InventarioService.asmx
```

**Nivel de logs:**

Editar `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug", // Cambiar a Debug para más detalle
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## ?? Pruebas

### Pruebas Funcionales (Manual)

#### Test 1: Autenticación Exitosa

1. Ejecutar cliente
2. Seleccionar opción 1
3. Ingresar: admin / Admin123!
4. **Esperado:** ? Token recibido

#### Test 2: Autenticación Fallida

1. Ingresar usuario o contraseña incorrecta
2. **Esperado:** ? "Credenciales inválidas"

#### Test 3: Insertar Artículo Válido

1. Autenticarse
2. Opción 2
3. Ingresar datos válidos (código único, precios coherentes)
4. **Esperado:** ? Artículo insertado

#### Test 4: Código Duplicado

1. Insertar artículo con código existente (ej. MART-001)
2. **Esperado:** ? Error de validación

#### Test 5: Precios Inválidos

1. Intentar: Precio compra > Precio venta
2. **Esperado:** ? Error de validación

#### Test 6: Consultar Artículo Existente

1. Opción 3, código: MART-001
2. **Esperado:** ? Detalles del artículo

#### Test 7: Consultar Artículo Inexistente

1. Opción 3, código: NOEXISTE
2. **Esperado:** ? "Artículo no encontrado"

#### Test 8: Listar Artículos

1. Opción 4
2. **Esperado:** ? Tabla con todos los artículos

#### Test 9: Alerta de Stock Bajo

1. Consultar artículo con stock < stock mínimo
2. **Esperado:** ?? Indicador de reposición

### Pruebas con SoapUI

1. **Importar WSDL:**

   ```
   http://localhost:5233/InventarioService.asmx?wsdl
   ```

2. **Crear TestSuite:**

   - Test: Autenticación
   - Test: Insertar artículo
   - Test: Consultar artículo
   - Test: Listar artículos
   - Test: Validaciones (códigos duplicados, precios inválidos)

3. **Medir rendimiento:**
   - Verificar latencias < 500ms (RNF2)

---

## ?? Cumplimiento de Requerimientos

### Requerimientos Funcionales

| ID   | Requerimiento                     | Estado            |
| ---- | --------------------------------- | ----------------- |
| RF1  | Gestión de artículos              | ? Implementado    |
| RF2  | Registro de nuevo artículo        | ? Implementado    |
| RF3  | Validación de datos               | ? Implementado    |
| RF4  | Consulta de artículos             | ? Implementado    |
| RF5  | Servicio SOAP: insertar artículo  | ? Implementado    |
| RF6  | Servicio SOAP: consultar artículo | ? Implementado    |
| RF7  | Manejo de stock                   | ? Implementado    |
| RF8  | Persistencia de datos             | ? PostgreSQL      |
| RF9  | Interfaz de usuario               | ? Cliente consola |
| RF10 | Manejo de errores                 | ? SOAP Faults     |

### Requerimientos No Funcionales

| ID    | Requerimiento          | Estado                    |
| ----- | ---------------------- | ------------------------- |
| RNF1  | Arquitectura N-Capas   | ? 5 proyectos separados   |
| RNF2  | Rendimiento ? 500ms    | ? Optimizado              |
| RNF3  | Escalabilidad          | ? Modular                 |
| RNF4  | Mantenibilidad         | ? SOLID, código limpio    |
| RNF5  | Interoperabilidad SOAP | ? WSDL 1.1                |
| RNF6  | Seguridad básica       | ? JWT + BCrypt            |
| RNF7  | Confiabilidad          | ? Logs y excepciones      |
| RNF8  | Usabilidad             | ? Mensajes claros         |
| RNF9  | Portabilidad           | ? Windows/Linux           |
| RNF10 | Pruebas                | ? Unitarias + integración |

---

## 🌐 Interfaz Web Node.js/EJS (Frontend)

Desde 2024, el sistema cuenta con una interfaz web moderna desarrollada en **Node.js** y **EJS** (Express + plantillas), que permite consultar artículos por código o nombre, visualizar listados y mejorar la experiencia de usuario respecto al cliente de consola.

### Tecnologías y Dependencias

| Tecnología      | Versión | Propósito                       |
| --------------- | ------- | ------------------------------- |
| Node.js         | 18+     | Entorno de ejecución JavaScript |
| Express         | ^4      | Framework web backend           |
| EJS             | ^3      | Motor de plantillas HTML        |
| express-session | ^1      | Manejo de sesiones              |
| dotenv          | ^16     | Variables de entorno            |
| body-parser     | ^1      | Parseo de formularios           |
| joi             | ^17     | Validación de datos             |
| pg              | ^8      | Conexión a PostgreSQL           |
| soap            | ^0.44   | Cliente SOAP para Node.js       |

### Estructura del Frontend

```
src/
├── app.js                 # App principal Express
├── routes/
│   └── articulos.routes.js # Rutas para artículos (búsqueda, listado)
├── soapClient.js          # Cliente SOAP (con manejo robusto de respuestas)
├── views/
│   ├── consultar.ejs      # Vista de búsqueda de artículos (código/nombre)
│   └── ...                # Otras vistas EJS
├── public/                # Archivos estáticos (CSS, JS, imágenes)
├── ...                    # Otros módulos y configuraciones
```

### Instalación y Ejecución del Frontend

1. **Requisitos previos:**

- Node.js 18+
- Acceso al backend SOAP corriendo (ver instrucciones previas)

2. **Instalar dependencias:**

```bash
cd src
npm install
```

3. **Configurar variables de entorno:**

- Crear un archivo `.env` en `src/` con el endpoint SOAP y otros parámetros:
  ```env
  SOAP_URL=http://localhost:5233/InventarioService.asmx?wsdl
  # Otros parámetros según sea necesario
  ```

4. **Ejecutar el servidor web:**

```bash
npm start
# o
node app.js
```

5. **Acceder a la interfaz:**

- Abrir navegador en: [http://localhost:3000](http://localhost:3000)

### Funcionalidades Web

- Buscar artículos por **código** o **nombre** (con selector tipo combo box)
- Listar todos los artículos
- Validaciones robustas en formularios
- Manejo de sesiones y mensajes de error claros
- Interfaz moderna y responsiva (EJS + CSS)

### Notas Técnicas

- El frontend se comunica exclusivamente con el backend SOAP, no accede directamente a la base de datos.
- El cliente SOAP en Node.js está preparado para manejar respuestas donde la lista de artículos puede venir como array o como objeto único.
- El código fuente del frontend se encuentra en la carpeta `src/`.

---

## ?? Próximos Pasos

### Para Desarrollo

- [ ] Agregar más operaciones SOAP (actualizar, eliminar artículos)
- [ ] Implementar interfaz web (ASP.NET MVC/Razor)
- [ ] Agregar paginación en listado de artículos
- [ ] Implementar búsqueda avanzada (filtros múltiples)
- [ ] Agregar reportes en PDF

### Para Documentación

- [ ] Completar diagramas UML (ver docs/DDA)
- [ ] Crear manual de usuario
- [ ] Documentar API REST adicional
- [ ] Video demostrativo del sistema

### Para Despliegue

- [ ] Dockerizar la aplicación
- [ ] CI/CD con GitHub Actions
- [ ] Despliegue en Azure/AWS
- [ ] Configurar SSL/HTTPS

---

## ?? Contacto y Soporte

### Equipo de Desarrollo

- **Anahy Herrera** - Documentación y BD
- **Oscar [Apellido]** - Backend y Servicios
- **Camila [Apellido]** - Frontend y DevOps

### Repositorio

?? **GitHub:** [https://github.com/aeherrera16/ferreteriaDistr](https://github.com/aeherrera16/ferreteriaDistr)

### Issues y Bugs

Para reportar problemas o solicitar funcionalidades:

1. Ir a la sección **Issues** del repositorio
2. Click en **New Issue**
3. Describir el problema con detalles
4. Asignar etiquetas apropiadas (bug, enhancement, documentation)

---

## ?? Licencia

Este proyecto es de carácter académico desarrollado para la asignatura de [Nombre de la Materia].

**Institución:** [Nombre de la Universidad]  
**Profesor:** [Nombre del Profesor]  
**Fecha:** Octubre 2025

---

## ?? Agradecimientos

- A nuestro profesor [Nombre] por la guía y asesoría
- A la comunidad de .NET por la documentación
- A PostgreSQL por la robustez de su sistema

---

## ?? Checklist de Inicio Rápido

- [ ] Clonar repositorio
- [ ] Instalar .NET 9 SDK
- [ ] Instalar PostgreSQL
- [ ] Crear base de datos "ferreteria"
- [ ] Ejecutar script `01-create-schema.sql`
- [ ] Ejecutar script `02-hash-passwords.sql`
- [ ] Configurar `appsettings.json` con tu password de PostgreSQL
- [ ] Ejecutar `dotnet restore`
- [ ] Configurar proyectos de inicio múltiples en Visual Studio
- [ ] Presionar F5 y probar el sistema
- [ ] Autenticarse con admin/Admin123!
- [ ] Listar artículos (Opción 4)

---

**¡Sistema listo para usar! ??**

_Última actualización: Octubre 2025_
