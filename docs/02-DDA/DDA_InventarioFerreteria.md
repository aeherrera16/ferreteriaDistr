# ??? DDA - Documento de Dise�o Arquitect�nico
## Sistema de Control de Inventario para Ferreter�a

---

## 1. INTRODUCCI�N

### 1.1 Prop�sito
Este documento describe el dise�o arquitect�nico del Sistema de Inventario, incluyendo la estructura de capas, componentes, patrones de dise�o y especificaci�n de servicios SOAP.

### 1.2 Alcance
Abarca el dise�o de:
- Arquitectura N-Capas
- Componentes y sus relaciones
- Modelo de datos
- Servicios SOAP (WSDL/XSD)
- Diagrama de despliegue

---

## 2. ARQUITECTURA DEL SISTEMA

### 2.1 Estilo Arquitect�nico: N-Capas

El sistema implementa arquitectura en capas con separaci�n de responsabilidades:

```
???????????????????????????????????????????????????????????
?              CAPA DE PRESENTACI�N                        ?
?        (InventarioFerreteria.Client)                    ?
?                                                          ?
?  - UI de Consola                                        ?
?  - Validaciones de entrada                              ?
?  - Consumo de servicios SOAP                            ?
????????????????????????????????????????????????????????????
                     ? SOAP/HTTP
                     ?
???????????????????????????????????????????????????????????
?              CAPA DE SERVICIOS                           ?
?        (InventarioFerreteria.SoapService)               ?
?                                                          ?
?  - Endpoints SOAP (SoapCore)                            ?
?  - Autenticaci�n JWT                                     ?
?  - Mapeo DTO ? Entidad                                  ?
?  - Manejo de SOAP Faults                                ?
????????????????????????????????????????????????????????????
                     ? Llamadas directas
                     ?
???????????????????????????????????????????????????????????
?              CAPA DE NEGOCIO                             ?
?        (InventarioFerreteria.Business)                  ?
?                                                          ?
?  - Servicios de dominio                                 ?
?  - Reglas de negocio (RF3)                              ?
?  - Validaciones (FluentValidation)                      ?
?  - L�gica de autenticaci�n (BCrypt, JWT)                ?
????????????????????????????????????????????????????????????
                     ? Interfaces de repositorio
                     ?
???????????????????????????????????????????????????????????
?              CAPA DE DATOS                               ?
?        (InventarioFerreteria.DataAccess)                ?
?                                                          ?
?  - Entity Framework Core                                ?
?  - DbContext                                             ?
?  - Repositorios (patr�n Repository)                     ?
?  - Npgsql Provider                                       ?
????????????????????????????????????????????????????????????
                     ? SQL/ADO.NET
                     ?
???????????????????????????????????????????????????????????
?                  POSTGRESQL                              ?
?            Base de Datos Relacional                      ?
?                                                          ?
?  - Tablas: articulos, usuarios, categorias, etc.        ?
?  - Constraints, Triggers, �ndices                        ?
???????????????????????????????????????????????????????????

  CAPA TRANSVERSAL (InventarioFerreteria.Entities)
  - DTOs, Modelos, Interfaces compartidas
```

### 2.2 Principios Aplicados

#### **2.2.1 SOLID**
- **S (Single Responsibility):** Cada capa tiene una responsabilidad �nica
- **O (Open/Closed):** Extensible mediante interfaces
- **L (Liskov Substitution):** Repositorios intercambiables
- **I (Interface Segregation):** Interfaces espec�ficas (IArticuloService, IUsuarioRepository)
- **D (Dependency Inversion):** Dependencias mediante interfaces e inyecci�n

#### **2.2.2 Patrones de Dise�o**
- **Repository Pattern:** Abstracci�n de acceso a datos
- **Service Layer:** L�gica de negocio encapsulada
- **DTO Pattern:** Transferencia de datos entre capas
- **Dependency Injection:** Inyecci�n de dependencias (ASP.NET Core DI)
- **Factory Pattern:** Generaci�n de tokens JWT

---

## 3. COMPONENTES DEL SISTEMA

### 3.1 Diagrama de Componentes

```
[INSTRUCCI�N: Crear diagrama UML de componentes mostrando:]

Componentes principales:
1. InventarioFerreteria.Client
   - Interfaces: IConsolaPrincipal
   - Usa: InventarioSoapServiceClient

2. InventarioFerreteria.SoapService
   - Provee: IInventarioSoapService (SOAP)
   - Usa: IArticuloService, IAutenticacionService

3. InventarioFerreteria.Business
   - Provee: IArticuloService, IAutenticacionService
   - Usa: IArticuloRepository, IUsuarioRepository

4. InventarioFerreteria.DataAccess
   - Provee: IArticuloRepository, IUsuarioRepository
   - Usa: ApplicationDbContext (EF Core)

5. InventarioFerreteria.Entities
   - Provee: DTOs, Entidades, Interfaces

Relaciones:
- Client -> SoapService [SOAP/HTTP]
- SoapService -> Business [Llamadas directas]
- Business -> DataAccess [Interfaces]
- DataAccess -> PostgreSQL [SQL]
```

### 3.2 Descripci�n de Componentes

#### **3.2.1 InventarioFerreteria.Client**
**Tipo:** Aplicaci�n de consola  
**Responsabilidad:** Interfaz de usuario para consumir servicios SOAP  
**Tecnolog�as:** .NET 9, HttpClient, System.Xml.Linq  

**Clases principales:**
- `Program`: Punto de entrada, men� principal
- M�todos: `AutenticarAsync()`, `InsertarArticuloAsync()`, `ConsultarArticuloAsync()`, `ListarArticulosAsync()`

#### **3.2.2 InventarioFerreteria.SoapService**
**Tipo:** ASP.NET Core Web Application  
**Responsabilidad:** Exponer servicios SOAP con SoapCore  
**Tecnolog�as:** ASP.NET Core 9, SoapCore, System.ServiceModel  

**Clases principales:**
- `InventarioSoapService`: Implementaci�n del servicio SOAP
- `IInventarioSoapService`: Contrato del servicio
- `Program`: Configuraci�n de host y endpoints

**Endpoints:**
- `/InventarioService.asmx` - Endpoint SOAP principal
- `/InventarioService.asmx?wsdl` - Definici�n WSDL

#### **3.2.3 InventarioFerreteria.Business**
**Tipo:** Class Library  
**Responsabilidad:** L�gica de negocio y validaciones  
**Tecnolog�as:** .NET 9, BCrypt.Net, System.IdentityModel.Tokens.Jwt  

**Servicios:**
- `ArticuloService` (impl. `IArticuloService`)
  - `InsertarArticulo(ArticuloDTO)`: Validar y registrar
  - `ConsultarPorCodigo(string)`: Buscar por c�digo
  - `ConsultarPorNombre(string)`: Buscar por nombre
  - `ActualizarArticulo(ArticuloDTO)`: Actualizar datos
  
- `AutenticacionService` (impl. `IAutenticacionService`)
  - `Autenticar(usuario, password)`: Validar credenciales y generar JWT
  - `ValidarToken(string)`: Verificar token JWT

#### **3.2.4 InventarioFerreteria.DataAccess**
**Tipo:** Class Library  
**Responsabilidad:** Acceso a base de datos  
**Tecnolog�as:** Entity Framework Core 9, Npgsql.EntityFrameworkCore.PostgreSQL  

**Componentes:**
- `ApplicationDbContext`: DbContext de EF Core
- `ArticuloRepository` (impl. `IArticuloRepository`)
- `UsuarioRepository` (impl. `IUsuarioRepository`)

#### **3.2.5 InventarioFerreteria.Entities**
**Tipo:** Class Library  
**Responsabilidad:** Definiciones compartidas  

**Contenido:**
- Entidades: `Articulo`, `Usuario`, `Categoria`, `Proveedor`
- DTOs: `ArticuloDTO`, `AutenticacionDTO`, `RespuestaDTO<T>`
- Interfaces: `IArticuloService`, `IArticuloRepository`, etc.

---

## 4. DIAGRAMAS DE CLASES

### 4.1 Capa de Entidades

```
[INSTRUCCI�N: Crear diagrama UML de clases para:]

class Articulo {
  + int Id
  + string Codigo
  + string Nombre
  + string? Descripcion
  + int? CategoriaId
  + decimal PrecioCompra
  + decimal PrecioVenta
  + int Stock
  + int StockMinimo
  + int? ProveedorId
  + DateTime FechaCreacion
  + DateTime FechaActualizacion
  + bool Activo
  + Categoria? Categoria
  + Proveedor? Proveedor
}

class Categoria {
  + int Id
  + string Nombre
  + string? Descripcion
  + ICollection<Articulo> Articulos
}

class Proveedor {
  + int Id
  + string Nombre
  + string? Telefono
  + string? Email
  + string? Direccion
  + ICollection<Articulo> Articulos
}

class Usuario {
  + int Id
  + string NombreUsuario
  + string PasswordHash
  + string Rol
  + DateTime FechaCreacion
  + bool Activo
}

Articulo "*" --> "0..1" Categoria
Articulo "*" --> "0..1" Proveedor
```

### 4.2 Capa de Negocio

```
[INSTRUCCI�N: Crear diagrama UML mostrando:]

<<interface>> IArticuloService {
  + Task<RespuestaDTO<ArticuloDTO>> InsertarArticulo(ArticuloDTO)
  + Task<RespuestaDTO<ArticuloDTO>> ConsultarPorCodigo(string)
  + Task<RespuestaDTO<List<ArticuloDTO>>> ConsultarPorNombre(string)
  + Task<RespuestaDTO<ArticuloDTO>> ActualizarArticulo(ArticuloDTO)
}

class ArticuloService implements IArticuloService {
  - IArticuloRepository _repository
  + ArticuloService(IArticuloRepository)
  + async Task<RespuestaDTO> InsertarArticulo(ArticuloDTO)
  - void ValidarArticulo(ArticuloDTO)
  - bool ValidarUnicidad(string codigo)
}

<<interface>> IAutenticacionService {
  + Task<RespuestaDTO<string>> Autenticar(string, string)
  + bool ValidarToken(string)
}

class AutenticacionService implements IAutenticacionService {
  - IUsuarioRepository _repository
  - IConfiguration _config
  + async Task<RespuestaDTO<string>> Autenticar(usuario, pwd)
  - string GenerarTokenJWT(Usuario)
  - bool VerificarPassword(string pwd, string hash)
}
```

### 4.3 Capa de Servicios SOAP

```
[INSTRUCCI�N: Crear diagrama UML mostrando:]

<<ServiceContract>>
interface IInventarioSoapService {
  + ResultadoOperacion InsertarArticulo(token, ArticuloDTO)
  + ResultadoOperacion ConsultarArticuloPorCodigo(token, codigo)
  + ResultadoOperacion ObtenerTodosArticulos(token)
}

class InventarioSoapService implements IInventarioSoapService {
  - IArticuloService _articuloService
  - IAutenticacionService _authService
  + InventarioSoapService(services...)
  + ResultadoOperacion InsertarArticulo(token, dto)
  + ResultadoOperacion ConsultarArticuloPorCodigo(token, codigo)
  - void ValidarToken(string)
}

class ResultadoOperacion {
  + bool Exito
  + string Mensaje
  + object? Datos
  + List<string>? Errores
}

class ArticuloDTO {
  + int? Id
  + string Codigo
  + string Nombre
  + string? Descripcion
  + int? CategoriaId
  + decimal PrecioCompra
  + decimal PrecioVenta
  + int Stock
  + int StockMinimo
  + int? ProveedorId
  + bool RequiereReposicion
}
```

---

## 5. DIAGRAMAS DE SECUENCIA

### 5.1 Secuencia: Autenticaci�n

```
[INSTRUCCI�N: Crear diagrama de secuencia UML para:]

Actor: Cliente
Participantes: SoapService, AuthService, UsuarioRepo, PostgreSQL

1. Cliente -> SoapService: Autenticar(usuario, password)
2. SoapService -> AuthService: Autenticar(usuario, password)
3. AuthService -> UsuarioRepo: ObtenerPorNombreUsuario(usuario)
4. UsuarioRepo -> PostgreSQL: SELECT * FROM usuarios WHERE nombreusuario = ?
5. PostgreSQL -> UsuarioRepo: Usuario (con passwordhash)
6. UsuarioRepo -> AuthService: Usuario
7. AuthService: VerificarPassword(password, passwordhash) con BCrypt
8. AuthService: GenerarTokenJWT(usuario)
9. AuthService -> SoapService: RespuestaDTO<string>(token)
10. SoapService -> Cliente: ResultadoOperacion (Exito=true, Datos=token)
```

### 5.2 Secuencia: Insertar Art�culo (SOAP)

```
[INSTRUCCI�N: Crear diagrama de secuencia UML para:]

1. Cliente -> SoapService: InsertarArticulo(token, ArticuloDTO)
2. SoapService -> AuthService: ValidarToken(token)
3. alt Token v�lido
   4. SoapService -> ArticuloService: InsertarArticulo(ArticuloDTO)
   5. ArticuloService: ValidarArticulo(dto)
   6. ArticuloService -> ArticuloRepo: ExisteCodigo(dto.Codigo)
   7. ArticuloRepo -> PostgreSQL: SELECT COUNT(*) WHERE codigo = ?
   8. PostgreSQL -> ArticuloRepo: 0 (no existe)
   9. ArticuloService: MapearDTOaEntidad(dto)
   10. ArticuloService -> ArticuloRepo: InsertarAsync(Articulo)
   11. ArticuloRepo -> PostgreSQL: INSERT INTO articulos...
   12. PostgreSQL -> ArticuloRepo: OK
   13. ArticuloRepo -> ArticuloService: Articulo (con ID generado)
   14. ArticuloService -> SoapService: RespuestaDTO<ArticuloDTO>(Exito=true)
   15. SoapService -> Cliente: ResultadoOperacion (Exito, Mensaje, Datos)
3. else Token inv�lido
   16. SoapService -> Cliente: SOAP Fault (UnauthorizedFault)
```

### 5.3 Secuencia: Consultar Art�culo

```
[INSTRUCCI�N: Crear diagrama similar para ConsultarArticuloPorCodigo]
Incluir:
- Validaci�n de token
- B�squeda en repositorio
- C�lculo de RequiereReposicion (stock < stockminimo)
- Retorno de ArticuloDTO con alerta
```

---

## 6. MODELO DE DATOS

### 6.1 Diagrama Entidad-Relaci�n (F�sico)

```
[INSTRUCCI�N: Crear diagrama ER con notaci�n crow's foot mostrando:]

Tablas:
- usuarios (PK: id)
- articulos (PK: id, FK: categoriaid, proveedorid)
- categorias (PK: id)
- proveedores (PK: id)
- logoperaciones (PK: id, FK: usuarioid)

Relaciones:
- articulos.categoriaid -> categorias.id (N:1, opcional)
- articulos.proveedorid -> proveedores.id (N:1, opcional)
- logoperaciones.usuarioid -> usuarios.id (N:1, opcional)

Constraints:
- articulos.codigo UNIQUE
- usuarios.nombreusuario UNIQUE
- articulos.preciocompra > 0
- articulos.precioventa > preciocompra
- articulos.stock >= 0
```

### 6.2 Script DDL (Creaci�n de Tablas)

**Ver archivo:** `docs/Scripts-SQL/01-create-schema.sql`

### 6.3 �ndices y Optimizaciones

```sql
-- �ndices para mejorar rendimiento (RNF2)
CREATE INDEX idx_articulos_codigo ON articulos(codigo);
CREATE INDEX idx_articulos_nombre ON articulos(nombre);
CREATE INDEX idx_articulos_categoria ON articulos(categoriaid);
CREATE INDEX idx_articulos_activo ON articulos(activo);
```

---

## 7. ESPECIFICACI�N DE SERVICIOS SOAP

### 7.1 Definici�n WSDL 1.1

**TargetNamespace:** `http://tempuri.org/`  
**Binding:** BasicHttpBinding  
**Endpoint:** `http://localhost:5233/InventarioService.asmx`

#### **7.1.1 Operaciones del Servicio**

| Operaci�n | Input | Output | Faults |
|-----------|-------|--------|--------|
| `Autenticar` | `string nombreUsuario, string password` | `ResultadoOperacion` | - |
| `InsertarArticulo` | `string token, ArticuloDTO articulo` | `ResultadoOperacion` | `ValidationFault`, `UnauthorizedFault` |
| `ConsultarArticuloPorCodigo` | `string token, string codigo` | `ResultadoOperacion` | `NotFoundFault`, `UnauthorizedFault` |
| `ObtenerTodosArticulos` | `string token` | `ResultadoOperacion` | `UnauthorizedFault` |

### 7.2 Definici�n XML Schema (XSD)

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema"
           targetNamespace="http://tempuri.org/"
           xmlns:tns="http://tempuri.org/"
           elementFormDefault="qualified">

  <!-- Tipo: ArticuloDTO -->
  <xs:complexType name="ArticuloDTO">
    <xs:sequence>
      <xs:element name="Id" type="xs:int" minOccurs="0"/>
      <xs:element name="Codigo" type="xs:string"/>
      <xs:element name="Nombre" type="xs:string"/>
      <xs:element name="Descripcion" type="xs:string" minOccurs="0"/>
      <xs:element name="CategoriaId" type="xs:int" minOccurs="0"/>
      <xs:element name="PrecioCompra" type="xs:decimal"/>
      <xs:element name="PrecioVenta" type="xs:decimal"/>
      <xs:element name="Stock" type="xs:int"/>
      <xs:element name="StockMinimo" type="xs:int"/>
      <xs:element name="ProveedorId" type="xs:int" minOccurs="0"/>
      <xs:element name="RequiereReposicion" type="xs:boolean"/>
    </xs:sequence>
  </xs:complexType>

  <!-- Tipo: ResultadoOperacion -->
  <xs:complexType name="ResultadoOperacion">
    <xs:sequence>
      <xs:element name="Exito" type="xs:boolean"/>
      <xs:element name="Mensaje" type="xs:string"/>
      <xs:element name="Datos" type="xs:anyType" minOccurs="0"/>
      <xs:element name="Errores" minOccurs="0">
        <xs:complexType>
          <xs:sequence>
            <xs:element name="string" type="xs:string" maxOccurs="unbounded"/>
          </xs:sequence>
        </xs:complexType>
      </xs:element>
    </xs:sequence>
  </xs:complexType>

  <!-- Fault: ValidationFault -->
  <xs:complexType name="ValidationFault">
    <xs:sequence>
      <xs:element name="Codigo" type="xs:int"/>
      <xs:element name="Mensaje" type="xs:string"/>
      <xs:element name="Detalles" type="xs:string" minOccurs="0"/>
    </xs:sequence>
  </xs:complexType>

  <!-- Fault: NotFoundFault -->
  <xs:complexType name="NotFoundFault">
    <xs:sequence>
      <xs:element name="Codigo" type="xs:int"/>
      <xs:element name="Mensaje" type="xs:string"/>
    </xs:sequence>
  </xs:complexType>

  <!-- Fault: UnauthorizedFault -->
  <xs:complexType name="UnauthorizedFault">
    <xs:sequence>
      <xs:element name="Codigo" type="xs:int"/>
      <xs:element name="Mensaje" type="xs:string"/>
    </xs:sequence>
  </xs:complexType>

</xs:schema>
```

### 7.3 Ejemplo de Mensajes SOAP

#### **Request: InsertarArticulo**

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <InsertarArticulo xmlns="http://tempuri.org/">
      <token>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...</token>
      <codigo>MART-002</codigo>
      <nombre>Martillo de Goma</nombre>
      <descripcion>Martillo de goma para trabajos delicados</descripcion>
      <categoriaId>1</categoriaId>
      <precioCompra>7.50</precioCompra>
      <precioVenta>12.00</precioVenta>
      <stock>30</stock>
      <stockMinimo>10</stockMinimo>
      <proveedorId>1</proveedorId>
    </InsertarArticulo>
  </soap:Body>
</soap:Envelope>
```

#### **Response: InsertarArticulo (�xito)**

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <InsertarArticuloResponse xmlns="http://tempuri.org/">
      <InsertarArticuloResult>
        <Exito>true</Exito>
        <Mensaje>Art�culo insertado correctamente</Mensaje>
        <Datos>
          <Id>13</Id>
          <Codigo>MART-002</Codigo>
          <Nombre>Martillo de Goma</Nombre>
          <PrecioCompra>7.50</PrecioCompra>
          <PrecioVenta>12.00</PrecioVenta>
          <Stock>30</Stock>
          <StockMinimo>10</StockMinimo>
          <RequiereReposicion>false</RequiereReposicion>
        </Datos>
      </InsertarArticuloResult>
    </InsertarArticuloResponse>
  </soap:Body>
</soap:Envelope>
```

#### **Response: SOAP Fault (C�digo Duplicado)**

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <soap:Fault>
      <faultcode>soap:Client</faultcode>
      <faultstring>Validation Error</faultstring>
      <detail>
        <ValidationFault xmlns="http://tempuri.org/">
          <Codigo>400</Codigo>
          <Mensaje>El c�digo MART-002 ya existe</Mensaje>
          <Detalles>El art�culo no puede ser registrado porque el c�digo est� duplicado</Detalles>
        </ValidationFault>
      </detail>
    </soap:Fault>
  </soap:Body>
</soap:Envelope>
```

---

## 8. DIAGRAMA DE DESPLIEGUE

```
[INSTRUCCI�N: Crear diagrama UML de despliegue mostrando:]

Nodos:
1. <<device>> M�quina Cliente
   - <<artifact>> Cliente SOAP (.NET 9 Console App)
   - <<artifact>> Navegador Web (opcional para WSDL)
   
2. <<device>> Servidor de Aplicaci�n
   - <<execution environment>> .NET 9 Runtime
   - <<artifact>> SoapService (ASP.NET Core)
   - Puerto: 5233
   
3. <<device>> Servidor de Base de Datos
   - <<execution environment>> PostgreSQL 15+
   - <<artifact>> InventarioFerreteriaDB
   - Puerto: 5432

Conexiones:
- Cliente -> Servidor App [HTTP/SOAP] (puerto 5233)
- Servidor App -> Servidor BD [TCP/IP] (puerto 5432, Npgsql)

Notas:
- Puede ejecutarse en un solo nodo (localhost) en desarrollo
- En producci�n: separar Servidor App y Servidor BD
- Compatible con Windows y Linux
```

### 8.1 Configuraci�n de Entorno

#### **Desarrollo (localhost)**
```
Cliente (consola) ---> SoapService (http://localhost:5233)
                              |
                              v
                        PostgreSQL (localhost:5432)
```

#### **Producci�n (sugerido)**
```
Cliente (red interna) ---> SoapService (http://app-server:5233)
                                   |
                                   v
                              PostgreSQL (db-server:5432)
```

---

## 9. SEGURIDAD

### 9.1 Autenticaci�n
- **Mecanismo:** JWT (JSON Web Tokens)
- **Hashing:** BCrypt para contrase�as
- **Expiraci�n:** 8 horas (configurable)

### 9.2 Validaci�n de Entrada
- Sanitizaci�n en capa de Presentaci�n
- Validaciones de negocio en capa Business
- Constraints en base de datos

### 9.3 Manejo de Errores
- SOAP Faults con c�digos espec�ficos
- Logs de errores (no exposici�n de detalles sensibles)

---

## 10. RENDIMIENTO (RNF2)

### 10.1 Objetivos
- Operaciones SOAP ? 500 ms bajo carga normal
- Consultas a BD optimizadas con �ndices

### 10.2 Estrategias
- �ndices en columnas `codigo`, `nombre`, `categoriaid`
- Connection pooling (Npgsql autom�tico)
- DTOs ligeros para transferencia

---

## 11. PATRONES Y PR�CTICAS

### 11.1 Patrones Utilizados
- **Repository Pattern:** IArticuloRepository, IUsuarioRepository
- **Service Layer:** IArticuloService, IAutenticacionService
- **DTO Pattern:** ArticuloDTO, RespuestaDTO<T>
- **Dependency Injection:** Constructor injection en todos los servicios
- **Factory:** Generaci�n de tokens JWT

### 11.2 Convenciones de C�digo
- Nombres de m�todos async terminan en `Async`
- Interfaces prefijadas con `I`
- DTOs sufijadas con `DTO`
- Servicios sufijados con `Service`
- Repositorios sufijados con `Repository`

---

## 12. AP�NDICES

### 12.1 Matriz de Trazabilidad (Requisitos ? Componentes)

| Requisito | Componente(s) Responsable(s) |
|-----------|------------------------------|
| RF1-RF4 | Business, DataAccess, Client |
| RF5-RF6 | SoapService, Business |
| RF7 | Business (RequiereReposicion) |
| RF8 | DataAccess (EF Core + Npgsql) |
| RF9 | Client (Consola) |
| RF10 | SoapService (SOAP Faults), Business (Validaciones) |
| RNF1 | Estructura de proyectos N-Capas |
| RNF2 | �ndices en BD, optimizaci�n de consultas |
| RNF4 | Interfaces, inyecci�n de dependencias |
| RNF5 | SoapCore, WSDL/XSD |
| RNF6 | AutenticacionService (BCrypt, JWT) |

### 12.2 Referencias
- SoapCore Documentation: https://github.com/DigDes/SoapCore
- Entity Framework Core: https://learn.microsoft.com/ef/core
- WSDL 1.1 Specification: https://www.w3.org/TR/wsdl

---

**Elaborado por:** Anahy Herrera  
**Fecha:** 26 de Octubre, 2025  
**Versi�n:** 1.0  
**Estado:** En Revisi�n

