# ?? Sistema de Inventario para Ferreter�a - Arquitectura SOAP

**Proyecto acad�mico de sistema de gesti�n de inventario usando arquitectura N-Capas y servicios SOAP con .NET 9 y PostgreSQL.**

---

## ?? Tabla de Contenidos

- [Descripci�n del Proyecto](#-descripci�n-del-proyecto)
- [Arquitectura](#-arquitectura-del-sistema)
- [Equipo de Desarrollo](#-equipo-de-desarrollo)
- [Requisitos Previos](#-requisitos-previos)
- [Instalaci�n y Configuraci�n](#-instalaci�n-y-configuraci�n)
- [Ejecuci�n del Sistema](#-ejecuci�n-del-sistema)
- [Uso del Sistema](#-uso-del-sistema)
- [Tecnolog�as Utilizadas](#-tecnolog�as-utilizadas)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Base de Datos](#-base-de-datos)
- [Servicios SOAP](#-servicios-soap)
- [Documentaci�n T�cnica](#-documentaci�n-t�cnica)
- [Troubleshooting](#-troubleshooting)

---

## ?? Descripci�n del Proyecto

Sistema de control de inventario para una ferreter�a que implementa:

- **Arquitectura N-Capas** (Presentaci�n, Negocio, Datos, Servicios)
- **Servicios SOAP** para operaciones de inventario
- **Autenticaci�n JWT** con contrase�as hasheadas (BCrypt)
- **Base de datos relacional** PostgreSQL
- **Cliente de consola** consumidor de servicios SOAP

### Funcionalidades Principales

? **RF1-RF4:** Gesti�n completa de art�culos (registrar, consultar, actualizar)  
? **RF5-RF6:** Servicios SOAP para insertar y consultar art�culos  
? **RF7:** Control de stock con alertas de reposici�n  
? **RF8:** Persistencia en PostgreSQL  
? **RF9:** Interfaz de usuario (consola)  
? **RF10:** Manejo robusto de errores con SOAP Faults  

---

## ??? Arquitectura del Sistema

### Diagrama de Capas

```
???????????????????????????????????????????????????????????
?          CAPA DE PRESENTACI�N (Cliente)                 ?
?         InventarioFerreteria.Client                     ?
?              (Aplicaci�n Consola)                        ?
???????????????????????????????????????????????????????????
                        ?
                        ? Consume servicios SOAP (XML/HTTP)
                        ?
???????????????????????????????????????????????????????????
?         CAPA DE SERVICIOS (Servidor SOAP)               ?
?       InventarioFerreteria.SoapService                  ?
?           (ASP.NET Core Web + SoapCore)                 ?
?   � IInventarioSoapService                              ?
?   � Autenticaci�n JWT                                   ?
?   � Operaciones: Insertar, Consultar, Listar           ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?          CAPA DE NEGOCIO (Business Logic)               ?
?        InventarioFerreteria.Business                    ?
?   � ArticuloService                                     ?
?   � AutenticacionService                                ?
?   � Validaciones de negocio (RF3)                       ?
?   � Reglas de stock y precios                           ?
???????????????????????????????????????????????????????????
                        ?
???????????????????????????????????????????????????????????
?         CAPA DE ACCESO A DATOS (Data Access)            ?
?       InventarioFerreteria.DataAccess                   ?
?   � ApplicationDbContext (EF Core)                      ?
?   � ArticuloRepository                                  ?
?   � UsuarioRepository                                   ?
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
?   � Modelos de dominio                                  ?
?   � DTOs (Data Transfer Objects)                        ?
?   � Contratos de servicio                               ?
???????????????????????????????????????????????????????????
```

---

## ?? Equipo de Desarrollo

| Miembro | Rol Principal | Responsabilidades |
|---------|---------------|-------------------|
| **Anahy Herrera** | An�lisis y Base de Datos | � Documento ERS (Especificaci�n de Requisitos)<br>� Documento DDA (Dise�o Arquitect�nico)<br>� Dise�o y scripts de base de datos<br>� Diagramas UML (Casos de Uso, Clases, Secuencia, ER)<br>� Evidencias de pruebas<br>� Informe final |
| **Oscar [Apellido]** | Backend y Servicios | � Capa de Negocio (Business)<br>� Capa de Datos (DataAccess)<br>� Servicios SOAP (SoapService)<br>� Pruebas unitarias e integraci�n<br>� Logs y manejo de errores |
| **Camila [Apellido]** | Frontend y DevOps | � Cliente de consola<br>� Interfaz de usuario<br>� Configuraci�n CI/CD<br>� Manual de despliegue<br>� README y gu�as<br>� Presentaci�n final |

---

## ?? Requisitos Previos

### Software Necesario

- **.NET 9 SDK** ([Descargar](https://dotnet.microsoft.com/download/dotnet/9.0))
- **PostgreSQL 15+** ([Descargar](https://www.postgresql.org/download/))
- **pgAdmin 4** (incluido con PostgreSQL)
- **Visual Studio 2022** o **VS Code** ([Descargar](https://visualstudio.microsoft.com/))
- **Git** ([Descargar](https://git-scm.com/))

### Verificar Instalaci�n

```bash
# Verificar .NET
dotnet --version

# Verificar PostgreSQL
psql --version

# Verificar Git
git --version
```

---

## ?? Instalaci�n y Configuraci�n

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

#### C. Hashear las Contrase�as (IMPORTANTE)

Las contrase�as en la base de datos deben estar hasheadas con BCrypt.

**Opci�n 1: Script SQL Automatizado**
```sql
-- Ejecutar en Query Tool de pgAdmin:
-- docs/Scripts-SQL/02-hash-passwords.sql
```

**Opci�n 2: Generar Hashes Manualmente**

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

### 4?? Configurar Cadena de Conexi�n

Editar `InventarioFerreteria.SoapService/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ferreteria;Username=postgres;Password=TU_PASSWORD"
  }
}
```

**?? Importante:** Reemplazar `TU_PASSWORD` con tu contrase�a de PostgreSQL.

---

## ?? Ejecuci�n del Sistema

El sistema requiere ejecutar **2 proyectos simult�neamente:**
1. **Servidor SOAP** (InventarioFerreteria.SoapService)
2. **Cliente** (InventarioFerreteria.Client)

### Opci�n 1: Visual Studio (Recomendado)

1. Abrir `InventarioFerreteria.sln` en Visual Studio
2. Click derecho en la **Soluci�n** (ra�z del Explorador de Soluciones)
3. Seleccionar **"Configurar proyectos de inicio"** o **"Set Startup Projects"**
4. Elegir **"Varios proyectos de inicio"** / **"Multiple startup projects"**
5. Configurar:

   | Proyecto | Acci�n |
   |----------|--------|
   | InventarioFerreteria.SoapService | **Iniciar** |
   | InventarioFerreteria.Client | **Iniciar** |
   | (resto de proyectos) | Ninguno |

6. Click **"Aceptar"**
7. Presionar **F5** o click en ?? **Iniciar**

**Resultado:** Se abrir�n 2 ventanas:
- Terminal del servidor SOAP (puerto 5233)
- Terminal del cliente (men� interactivo)

### Opci�n 2: L�nea de Comandos (2 Terminales)

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

### Opci�n 3: Compilar y Ejecutar

```bash
# Compilar toda la soluci�n
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

### Men� Principal del Cliente

```
=== Cliente SOAP - Sistema de Inventario Ferreter�a ===

--- MEN� PRINCIPAL ---
1. Autenticar
2. Insertar Art�culo
3. Consultar Art�culo por C�digo
4. Listar Todos los Art�culos
5. Salir

Seleccione una opci�n:
```

### 1?? Autenticaci�n (PRIMER PASO OBLIGATORIO)

```
--- AUTENTICACI�N ---
Usuario: admin
Contrase�a: Admin123!

? Autenticaci�n exitosa
Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Credenciales disponibles:**
- **Administrador:** usuario: `admin` / contrase�a: `Admin123!`
- **Usuario:** usuario: `usuario` / contrase�a: `Usuario123!`

### 2?? Insertar Art�culo

```
--- INSERTAR NUEVO ART�CULO ---
C�digo: TEST-001
Nombre: Martillo Prueba
Descripci�n: Art�culo de prueba
ID Categor�a: 1
Precio Compra: 10.00
Precio Venta: 20.00
Stock: 50
Stock M�nimo: 10
ID Proveedor: 1

? Art�culo insertado correctamente
==================================================
ID: 13
C�digo: TEST-001
Nombre: Martillo Prueba
...
```

**Validaciones autom�ticas:**
- C�digo �nico (no duplicados)
- Precio venta > Precio compra
- Stock ? 0
- Alerta si stock < stock m�nimo

### 3?? Consultar Art�culo por C�digo

```
--- CONSULTAR ART�CULO ---
C�digo del art�culo: MART-001

? Art�culo encontrado
==================================================
C�digo: MART-001
Nombre: Martillo de U�a 16oz
Stock: 25
Precio Venta: $15.00
...
```

### 4?? Listar Todos los Art�culos

```
? Art�culos obtenidos exitosamente

C�digo          Nombre                          Stock  Precio Venta
----------------------------------------------------------------------
MART-001        Martillo de U�a 16oz              25        $15.00
TALAD-001       Taladro El�ctrico 500W            15        $89.99
PINT-001        Pintura L�tex Blanco 1GL          50        $22.50
...
```

---

## ??? Tecnolog�as Utilizadas

### Backend

| Tecnolog�a | Versi�n | Prop�sito |
|------------|---------|-----------|
| .NET | 9.0 | Framework principal |
| ASP.NET Core | 9.0 | Servidor web |
| Entity Framework Core | 9.0 | ORM para base de datos |
| SoapCore | 1.1.0.50 | Implementaci�n de servicios SOAP |
| Npgsql | 9.0.0 | Driver PostgreSQL |
| BCrypt.Net-Next | 4.0.3 | Hashing de contrase�as |
| System.IdentityModel.Tokens.Jwt | 8.2.1 | Autenticaci�n JWT |

### Base de Datos

- **PostgreSQL 15+** - Base de datos relacional

### Herramientas de Desarrollo

- **Visual Studio 2022** / **VS Code**
- **pgAdmin 4** - Administraci�n de PostgreSQL
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
?   ?   ??? InventarioSoapService.cs       # Implementaci�n SOAP
?   ??? Program.cs                          # Configuraci�n del servidor
?   ??? appsettings.json                    # Configuraci�n (cadena de conexi�n)
?   ??? InventarioFerreteria.SoapService.csproj
?
??? InventarioFerreteria.Client/            # ?? Cliente de Consola
?   ??? Program.cs                          # L�gica del cliente SOAP
?   ??? InventarioFerreteria.Client.csproj
?
??? InventarioFerreteria.Business/          # ?? Capa de Negocio
?   ??? Interfaces/
?   ?   ??? IArticuloService.cs
?   ?   ??? IAutenticacionService.cs
?   ??? Services/
?   ?   ??? ArticuloService.cs             # L�gica de negocio de art�culos
?   ?   ??? AutenticacionService.cs        # L�gica de autenticaci�n
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
??? docs/                                    # ?? Documentaci�n T�cnica
?   ??? 01-ERS/
?   ?   ??? ERS_InventarioFerreteria.md    # Especificaci�n de Requisitos
?   ??? 02-DDA/
?   ?   ??? DDA_InventarioFerreteria.md    # Documento de Dise�o Arquitect�nico
?   ??? Scripts-SQL/
?       ??? 01-create-schema.sql           # Script de creaci�n de BD
?       ??? 02-hash-passwords.sql          # Script de hasheo de contrase�as
?
??? .gitignore                              # Archivos ignorados por Git
??? README.md                               # Este archivo
??? InventarioFerreteria.sln                # Soluci�n de Visual Studio
```

---

## ??? Base de Datos

### Modelo Entidad-Relaci�n

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

**5. logoperaciones** (Auditor�a)
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
-- Validaci�n de precios
CHECK (preciocompra > 0)
CHECK (precioventa > 0)
CHECK (precioventa >= preciocompra)

-- Validaci�n de stock
CHECK (stock >= 0)
CHECK (stockminimo >= 0)

-- Unicidad
UNIQUE (codigo)
UNIQUE (nombreusuario)
```

### �ndices para Optimizaci�n

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

**Categor�as:**
1. Herramientas Manuales
2. Herramientas El�ctricas
3. Materiales de Construcci�n
4. Pintura
5. Plomer�a
6. Electricidad

**Proveedores:**
1. Ferreter�a Total S.A.
2. Distribuidora El Tornillo
3. Importadora HerramientasPlus

**Art�culos de ejemplo:**
- MART-001: Martillo de U�a 16oz ($15.00)
- TALAD-001: Taladro El�ctrico 500W ($89.99)
- PINT-001: Pintura L�tex Blanco 1GL ($22.50)
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
    <Mensaje>Autenticaci�n exitosa</Mensaje>
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
      <nombre>Art�culo de Prueba</nombre>
      <descripcion>Descripci�n del art�culo</descripcion>
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
  <faultstring>C�digo de art�culo ya existe</faultstring>
  <detail>
    <ValidationFault>
      <Codigo>409</Codigo>
      <Mensaje>C�digo duplicado</Mensaje>
    </ValidationFault>
  </detail>
</soap:Fault>
```

---

## ?? Documentaci�n T�cnica

### Documentos Disponibles

| Documento | Ubicaci�n | Responsable | Descripci�n |
|-----------|-----------|-------------|-------------|
| **ERS** | `docs/01-ERS/ERS_InventarioFerreteria.md` | Anahy | Especificaci�n de Requisitos del Sistema<br>� Casos de uso<br>� Requerimientos funcionales y no funcionales<br>� Modelo de dominio |
| **DDA** | `docs/02-DDA/DDA_InventarioFerreteria.md` | Anahy | Documento de Dise�o Arquitect�nico<br>� Arquitectura N-Capas<br>� Diagramas de clases y secuencia<br>� Modelo de base de datos<br>� Definici�n WSDL/XSD |
| **Scripts SQL** | `docs/Scripts-SQL/` | Anahy | � 01-create-schema.sql (Creaci�n de BD)<br>� 02-hash-passwords.sql (Hasheo de contrase�as) |

### Diagramas

Los diagramas se encuentran documentados en los archivos ERS y DDA:

- **Diagrama de Casos de Uso** (ERS)
- **Diagrama de Clases** (DDA)
- **Diagramas de Secuencia** (DDA)
  - Autenticaci�n
  - Insertar Art�culo
  - Consultar Art�culo
- **Diagrama de Entidad-Relaci�n** (DDA)
- **Diagrama de Despliegue** (DDA)
- **Diagrama de Arquitectura N-Capas** (DDA)

---

## ?? Troubleshooting

### Problema 1: No se puede conectar a PostgreSQL

**S�ntomas:**
```
Npgsql.NpgsqlException: Failed to connect to localhost:5432
```

**Soluciones:**

1. **Verificar que PostgreSQL est� ejecut�ndose:**
   - Windows: Buscar "services.msc" ? Buscar "PostgreSQL" ? Verificar que est� "Running"
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
   - Asegurar que el puerto 5432 est� abierto

---

### Problema 2: Error de autenticaci�n ("Token inv�lido")

**S�ntomas:**
```
? Token inv�lido o expirado
```

**Causas comunes:**

1. **Las contrase�as NO est�n hasheadas con BCrypt**
   
   **Soluci�n:** Ejecutar el script de hasheo:
   ```sql
   -- En pgAdmin:
   -- Abrir y ejecutar: docs/Scripts-SQL/02-hash-passwords.sql
   ```

2. **Contrase�a incorrecta**
   
   **Credenciales correctas:**
   - admin / Admin123!
   - usuario / Usuario123!

3. **Token expirado** (despu�s de 8 horas)
   
   **Soluci�n:** Volver a autenticarse (Opci�n 1 del men�)

---

### Problema 3: El servidor no inicia

**S�ntomas:**
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
   
   **Nota:** Tambi�n cambiar en el cliente (Program.cs):
   ```csharp
   private const string ServiceUrl = "http://localhost:5234/InventarioService.asmx";
   ```

---

### Problema 4: Error al insertar art�culo

**S�ntomas:**
```
? Error: C�digo de art�culo ya existe
```

**Causas:**

1. **C�digo duplicado** (validaci�n correcta)
   - Usar un c�digo diferente

2. **Precio venta < Precio compra**
   ```
   ? Error: El precio de venta debe ser mayor al precio de compra
   ```
   - Ajustar los precios

3. **Categor�a o Proveedor inexistente**
   ```
   ? Error: La categor�a especificada no existe
   ```
   - Verificar IDs v�lidos en la base de datos:
     ```sql
     SELECT id, nombre FROM categorias;
     SELECT id, nombre FROM proveedores;
     ```

---

### Problema 5: No se pueden ejecutar ambos proyectos

**Soluci�n para Visual Studio:**

1. Click derecho en la **Soluci�n** (no en un proyecto individual)
2. **"Configurar proyectos de inicio"**
3. **"Varios proyectos de inicio"**
4. Marcar como "Iniciar":
   - InventarioFerreteria.SoapService
   - InventarioFerreteria.Client
5. **Orden:** Servidor primero, Cliente segundo
6. Click **"Aceptar"**

**Soluci�n alternativa (2 instancias de Visual Studio):**

1. Abrir 2 Visual Studios
2. En la primera: Ejecutar solo SoapService
3. En la segunda: Ejecutar solo Client

---

### Problema 6: Error de compilaci�n

**S�ntomas:**
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
      "Default": "Debug",  // Cambiar a Debug para m�s detalle
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## ?? Pruebas

### Pruebas Funcionales (Manual)

#### Test 1: Autenticaci�n Exitosa
1. Ejecutar cliente
2. Seleccionar opci�n 1
3. Ingresar: admin / Admin123!
4. **Esperado:** ? Token recibido

#### Test 2: Autenticaci�n Fallida
1. Ingresar usuario o contrase�a incorrecta
2. **Esperado:** ? "Credenciales inv�lidas"

#### Test 3: Insertar Art�culo V�lido
1. Autenticarse
2. Opci�n 2
3. Ingresar datos v�lidos (c�digo �nico, precios coherentes)
4. **Esperado:** ? Art�culo insertado

#### Test 4: C�digo Duplicado
1. Insertar art�culo con c�digo existente (ej. MART-001)
2. **Esperado:** ? Error de validaci�n

#### Test 5: Precios Inv�lidos
1. Intentar: Precio compra > Precio venta
2. **Esperado:** ? Error de validaci�n

#### Test 6: Consultar Art�culo Existente
1. Opci�n 3, c�digo: MART-001
2. **Esperado:** ? Detalles del art�culo

#### Test 7: Consultar Art�culo Inexistente
1. Opci�n 3, c�digo: NOEXISTE
2. **Esperado:** ? "Art�culo no encontrado"

#### Test 8: Listar Art�culos
1. Opci�n 4
2. **Esperado:** ? Tabla con todos los art�culos

#### Test 9: Alerta de Stock Bajo
1. Consultar art�culo con stock < stock m�nimo
2. **Esperado:** ?? Indicador de reposici�n

### Pruebas con SoapUI

1. **Importar WSDL:**
   ```
   http://localhost:5233/InventarioService.asmx?wsdl
   ```

2. **Crear TestSuite:**
   - Test: Autenticaci�n
   - Test: Insertar art�culo
   - Test: Consultar art�culo
   - Test: Listar art�culos
   - Test: Validaciones (c�digos duplicados, precios inv�lidos)

3. **Medir rendimiento:**
   - Verificar latencias < 500ms (RNF2)

---

## ?? Cumplimiento de Requerimientos

### Requerimientos Funcionales

| ID | Requerimiento | Estado |
|----|---------------|--------|
| RF1 | Gesti�n de art�culos | ? Implementado |
| RF2 | Registro de nuevo art�culo | ? Implementado |
| RF3 | Validaci�n de datos | ? Implementado |
| RF4 | Consulta de art�culos | ? Implementado |
| RF5 | Servicio SOAP: insertar art�culo | ? Implementado |
| RF6 | Servicio SOAP: consultar art�culo | ? Implementado |
| RF7 | Manejo de stock | ? Implementado |
| RF8 | Persistencia de datos | ? PostgreSQL |
| RF9 | Interfaz de usuario | ? Cliente consola |
| RF10 | Manejo de errores | ? SOAP Faults |

### Requerimientos No Funcionales

| ID | Requerimiento | Estado |
|----|---------------|--------|
| RNF1 | Arquitectura N-Capas | ? 5 proyectos separados |
| RNF2 | Rendimiento ? 500ms | ? Optimizado |
| RNF3 | Escalabilidad | ? Modular |
| RNF4 | Mantenibilidad | ? SOLID, c�digo limpio |
| RNF5 | Interoperabilidad SOAP | ? WSDL 1.1 |
| RNF6 | Seguridad b�sica | ? JWT + BCrypt |
| RNF7 | Confiabilidad | ? Logs y excepciones |
| RNF8 | Usabilidad | ? Mensajes claros |
| RNF9 | Portabilidad | ? Windows/Linux |
| RNF10 | Pruebas | ? Unitarias + integraci�n |

---

## ?? Pr�ximos Pasos

### Para Desarrollo

- [ ] Agregar m�s operaciones SOAP (actualizar, eliminar art�culos)
- [ ] Implementar interfaz web (ASP.NET MVC/Razor)
- [ ] Agregar paginaci�n en listado de art�culos
- [ ] Implementar b�squeda avanzada (filtros m�ltiples)
- [ ] Agregar reportes en PDF

### Para Documentaci�n

- [ ] Completar diagramas UML (ver docs/DDA)
- [ ] Crear manual de usuario
- [ ] Documentar API REST adicional
- [ ] Video demostrativo del sistema

### Para Despliegue

- [ ] Dockerizar la aplicaci�n
- [ ] CI/CD con GitHub Actions
- [ ] Despliegue en Azure/AWS
- [ ] Configurar SSL/HTTPS

---

## ?? Contacto y Soporte

### Equipo de Desarrollo

- **Anahy Herrera** - Documentaci�n y BD
- **Oscar [Apellido]** - Backend y Servicios
- **Camila [Apellido]** - Frontend y DevOps

### Repositorio

?? **GitHub:** [https://github.com/aeherrera16/ferreteriaDistr](https://github.com/aeherrera16/ferreteriaDistr)

### Issues y Bugs

Para reportar problemas o solicitar funcionalidades:
1. Ir a la secci�n **Issues** del repositorio
2. Click en **New Issue**
3. Describir el problema con detalles
4. Asignar etiquetas apropiadas (bug, enhancement, documentation)

---

## ?? Licencia

Este proyecto es de car�cter acad�mico desarrollado para la asignatura de [Nombre de la Materia].

**Instituci�n:** [Nombre de la Universidad]  
**Profesor:** [Nombre del Profesor]  
**Fecha:** Octubre 2025  

---

## ?? Agradecimientos

- A nuestro profesor [Nombre] por la gu�a y asesor�a
- A la comunidad de .NET por la documentaci�n
- A PostgreSQL por la robustez de su sistema

---

## ?? Checklist de Inicio R�pido

- [ ] Clonar repositorio
- [ ] Instalar .NET 9 SDK
- [ ] Instalar PostgreSQL
- [ ] Crear base de datos "ferreteria"
- [ ] Ejecutar script `01-create-schema.sql`
- [ ] Ejecutar script `02-hash-passwords.sql`
- [ ] Configurar `appsettings.json` con tu password de PostgreSQL
- [ ] Ejecutar `dotnet restore`
- [ ] Configurar proyectos de inicio m�ltiples en Visual Studio
- [ ] Presionar F5 y probar el sistema
- [ ] Autenticarse con admin/Admin123!
- [ ] Listar art�culos (Opci�n 4)

---

**�Sistema listo para usar! ??**

*�ltima actualizaci�n: Octubre 2025*
