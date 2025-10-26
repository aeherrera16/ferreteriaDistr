# ?? ERS - Especificaci�n de Requisitos del Sistema
## Sistema de Control de Inventario para Ferreter�a

---

## 1. INTRODUCCI�N

### 1.1 Prop�sito del Documento
Este documento describe los requisitos funcionales y no funcionales del Sistema de Control de Inventario para Ferreter�a, desarrollado con arquitectura N-Capas y servicios SOAP.

### 1.2 Alcance del Sistema
El sistema permitir� gestionar el inventario de una ferreter�a mediante:
- Registro, consulta y actualizaci�n de art�culos
- Servicios web SOAP para integraci�n con sistemas externos
- Control de stock y alertas de reposici�n
- Autenticaci�n de usuarios

### 1.3 Objetivos del Proyecto
- Implementar arquitectura N-Capas siguiendo principios SOLID
- Exponer servicios SOAP cumpliendo est�ndares WSDL 1.1
- Gestionar inventario con validaciones de negocio robustas
- Proporcionar interfaz intuitiva para operaciones CRUD

---

## 2. DESCRIPCI�N GENERAL

### 2.1 Perspectiva del Producto
Sistema standalone con base de datos PostgreSQL y servicios SOAP para interoperabilidad.

### 2.2 Funciones del Producto
- Gesti�n completa de art�culos (CRUD)
- Control de stock con alertas autom�ticas
- Servicios SOAP para inserci�n y consulta
- Autenticaci�n y autorizaci�n b�sica

### 2.3 Caracter�sticas de los Usuarios

| Tipo de Usuario | Descripci�n | Permisos |
|-----------------|-------------|----------|
| Administrador | Usuario con permisos completos | Crear, leer, actualizar art�culos |
| Usuario | Operador de inventario | Consultar art�culos |
| Cliente SOAP | Sistema externo | Insertar y consultar v�a SOAP |

---

## 3. ACTORES DEL SISTEMA

### 3.1 Administrador
**Descripci�n:** Usuario interno con privilegios completos
**Responsabilidades:**
- Registrar nuevos art�culos
- Actualizar informaci�n de inventario
- Consultar estado del stock
- Gestionar proveedores y categor�as

### 3.2 Usuario/Operador
**Descripci�n:** Usuario interno con permisos de consulta
**Responsabilidades:**
- Consultar art�culos por c�digo o nombre
- Ver alertas de stock bajo

### 3.3 Cliente SOAP (Sistema Externo)
**Descripci�n:** Aplicaci�n externa que consume servicios SOAP
**Responsabilidades:**
- Insertar art�culos mediante servicio SOAP
- Consultar art�culos por c�digo v�a SOAP

---

## 4. CASOS DE USO

### 4.1 Diagrama de Casos de Uso
```
[INSTRUCCI�N: Crear diagrama con herramienta UML mostrando:]
- Actor: Administrador
  - UC01: Registrar Art�culo
  - UC02: Consultar Art�culo
  - UC03: Actualizar Art�culo
  
- Actor: Usuario
  - UC02: Consultar Art�culo
  
- Actor: Cliente SOAP
  - UC04: Insertar Art�culo (SOAP)
  - UC05: Consultar Art�culo por C�digo (SOAP)
```

### 4.2 Especificaci�n de Casos de Uso

#### **UC01: Registrar Art�culo**

| Campo | Descripci�n |
|-------|-------------|
| **ID** | UC01 |
| **Nombre** | Registrar Art�culo |
| **Actor Principal** | Administrador |
| **Precondiciones** | Usuario autenticado como Administrador |
| **Postcondiciones** | Art�culo registrado en la base de datos |

**Flujo Principal:**
1. El administrador accede a la funci�n "Registrar Art�culo"
2. El sistema muestra formulario de registro
3. El administrador ingresa:
   - C�digo (�nico)
   - Nombre
   - Descripci�n (opcional)
   - Categor�a
   - Precio de compra
   - Precio de venta
   - Stock inicial
   - Stock m�nimo
   - Proveedor (opcional)
4. El sistema valida:
   - C�digo �nico (no existente)
   - Precios > 0
   - Precio venta > Precio compra
   - Stock ? 0
5. El sistema registra el art�culo
6. El sistema muestra mensaje de confirmaci�n

**Flujos Alternos:**
- **4a. C�digo duplicado:**
  - 4a.1 El sistema muestra error "El c�digo ya existe"
  - 4a.2 Retorna al paso 3

- **4b. Datos inv�lidos:**
  - 4b.1 El sistema muestra errores de validaci�n espec�ficos
  - 4b.2 Retorna al paso 3

#### **UC02: Consultar Art�culo**

| Campo | Descripci�n |
|-------|-------------|
| **ID** | UC02 |
| **Nombre** | Consultar Art�culo |
| **Actor Principal** | Administrador, Usuario |
| **Precondiciones** | Usuario autenticado |
| **Postcondiciones** | Informaci�n del art�culo mostrada |

**Flujo Principal:**
1. El usuario accede a "Consultar Art�culo"
2. El sistema ofrece opciones de b�squeda:
   - Por c�digo (b�squeda exacta)
   - Por nombre (b�squeda parcial)
3. El usuario ingresa criterio de b�squeda
4. El sistema busca en la base de datos
5. El sistema muestra resultados con:
   - C�digo, nombre, descripci�n
   - Precios (compra y venta)
   - Stock actual y m�nimo
   - ?? Alerta si stock < stock m�nimo
6. Fin del caso de uso

**Flujos Alternos:**
- **4a. No se encuentra art�culo:**
  - 4a.1 El sistema muestra "No se encontraron resultados"
  - 4a.2 Retorna al paso 2

#### **UC03: Actualizar Art�culo**

| Campo | Descripci�n |
|-------|-------------|
| **ID** | UC03 |
| **Nombre** | Actualizar Art�culo |
| **Actor Principal** | Administrador |
| **Precondiciones** | Usuario autenticado como Administrador; Art�culo existe |
| **Postcondiciones** | Art�culo actualizado en base de datos |

**Flujo Principal:**
1. El administrador consulta el art�culo (UC02)
2. Selecciona opci�n "Editar"
3. El sistema muestra formulario con datos actuales
4. El administrador modifica campos permitidos:
   - Nombre, descripci�n, categor�a
   - Precios (compra/venta)
   - Stock, stock m�nimo
   - Proveedor
5. El sistema valida datos (mismas reglas que UC01)
6. El sistema actualiza el art�culo
7. El sistema muestra confirmaci�n

**Flujos Alternos:**
- **5a. Validaci�n falla:** Similar a UC01.4b

#### **UC04: Insertar Art�culo (SOAP)**

| Campo | Descripci�n |
|-------|-------------|
| **ID** | UC04 |
| **Nombre** | Insertar Art�culo v�a SOAP |
| **Actor Principal** | Cliente SOAP (Sistema Externo) |
| **Precondiciones** | Cliente autenticado con token JWT v�lido |
| **Postcondiciones** | Art�culo registrado en base de datos |

**Flujo Principal:**
1. Cliente env�a petici�n SOAP `InsertarArticulo` con:
   - Token JWT
   - DTO del art�culo (c�digo, nombre, precios, etc.)
2. El servicio valida el token
3. El servicio valida datos del art�culo (RF3)
4. El servicio registra el art�culo en BD
5. El servicio retorna `ResultadoOperacion` con:
   - Exito = true
   - Mensaje = "Art�culo insertado correctamente"
   - Datos = ArticuloDTO completo

**Flujos Alternos:**
- **2a. Token inv�lido:**
  - 2a.1 El servicio retorna SOAP Fault `UnauthorizedFault`
  - 2a.2 Mensaje: "Token inv�lido o expirado"

- **3a. C�digo duplicado:**
  - 3a.1 El servicio retorna SOAP Fault `ValidationFault`
  - 3a.2 Mensaje: "El c�digo ya existe"

- **3b. Datos inv�lidos:**
  - 3b.1 El servicio retorna SOAP Fault `ValidationFault`
  - 3b.2 Mensaje: Lista de errores de validaci�n

#### **UC05: Consultar Art�culo por C�digo (SOAP)**

| Campo | Descripci�n |
|-------|-------------|
| **ID** | UC05 |
| **Nombre** | Consultar Art�culo por C�digo v�a SOAP |
| **Actor Principal** | Cliente SOAP |
| **Precondiciones** | Cliente autenticado |
| **Postcondiciones** | Informaci�n del art�culo retornada |

**Flujo Principal:**
1. Cliente env�a petici�n SOAP `ConsultarArticuloPorCodigo` con:
   - Token JWT
   - C�digo del art�culo
2. El servicio valida el token
3. El servicio busca el art�culo en BD
4. El servicio retorna `ResultadoOperacion` con:
   - Exito = true
   - Mensaje = "Art�culo encontrado"
   - Datos = ArticuloDTO (incluye alerta si stock < m�nimo)

**Flujos Alternos:**
- **3a. Art�culo no encontrado:**
  - 3a.1 El servicio retorna SOAP Fault `NotFoundFault`
  - 3a.2 Mensaje: "Art�culo con c�digo X no encontrado"

---

## 5. REQUERIMIENTOS FUNCIONALES

| ID | Requerimiento | Descripci�n | Prioridad |
|----|---------------|-------------|-----------|
| **RF1** | Gesti�n de art�culos | Permitir registrar, consultar y actualizar art�culos del inventario | Alta |
| **RF2** | Registro de nuevo art�culo | Ingresar datos: c�digo, nombre, categor�a, precios, stock, proveedor | Alta |
| **RF3** | Validaci�n de datos | Validar unicidad c�digo, precios positivos, coherencia precios | Alta |
| **RF4** | Consulta de art�culos | Buscar art�culos por c�digo (exacto) o nombre (parcial) | Alta |
| **RF5** | Servicio SOAP: insertar | Exponer operaci�n SOAP para agregar art�culos a BD | Alta |
| **RF6** | Servicio SOAP: consultar | Exponer operaci�n SOAP para consultar art�culo por c�digo | Alta |
| **RF7** | Manejo de stock | Registrar stock disponible; alertar si stock < m�nimo | Media |
| **RF8** | Persistencia de datos | Guardar registros en PostgreSQL | Alta |
| **RF9** | Interfaz de usuario | Interfaz web/escritorio simple para operaciones b�sicas | Media |
| **RF10** | Manejo de errores | Mostrar mensajes claros o SOAP Fault ante errores | Alta |

### 5.1 Reglas de Negocio

#### **RN1: Unicidad de C�digo**
- El c�digo de art�culo debe ser �nico en el sistema
- Comparaci�n case-insensitive
- Formato: alfanum�rico, hasta 30 caracteres

#### **RN2: Validaci�n de Precios**
- Precio de compra > 0
- Precio de venta > 0
- Precio de venta debe ser mayor que precio de compra
- Formato: num�rico con 2 decimales

#### **RN3: Validaci�n de Stock**
- Stock actual ? 0
- Stock m�nimo ? 0
- Stock m�nimo no puede ser mayor que stock actual al registrar

#### **RN4: Alerta de Reposici�n**
- El sistema debe marcar art�culos con stock < stock_minimo
- Bandera RequiereReposicion en ArticuloDTO

#### **RN5: Autenticaci�n SOAP**
- Todas las operaciones SOAP requieren token JWT v�lido
- Token debe incluir en header de petici�n SOAP
- Expiraci�n: 8 horas (configurable)

---

## 6. REQUERIMIENTOS NO FUNCIONALES

| ID | Requerimiento | Descripci�n | Criterio de Aceptaci�n |
|----|---------------|-------------|------------------------|
| **RNF1** | Arquitectura N-Capas | C�digo organizado en capas con responsabilidades separadas | Proyectos separados: Presentaci�n, Servicios, Negocio, Datos, Entidades |
| **RNF2** | Rendimiento | Operaciones SOAP ? 500 ms bajo carga normal | Pruebas con SoapUI muestran latencia promedio < 500ms |
| **RNF3** | Escalabilidad | Permitir agregar funcionalidades sin afectar existentes | Arquitectura desacoplada con interfaces |
| **RNF4** | Mantenibilidad | C�digo limpio, comentado, SOLID | Code review aprueba adherencia a SOLID |
| **RNF5** | Interoperabilidad | SOAP cumple WSDL 1.1 y XML Schema | WSDL v�lido y consumible desde SoapUI |
| **RNF6** | Seguridad b�sica | Acceso restringido a admin | Login requerido; contrase�as con BCrypt |
| **RNF7** | Confiabilidad | Manejo de errores con mensajes y logs | Excepciones capturadas; logs en consola/archivo |
| **RNF8** | Usabilidad | Interfaz intuitiva con validaciones visuales | Mensajes descriptivos; validaciones en cliente |
| **RNF9** | Portabilidad | Ejecutable en Windows/Linux con .NET + PostgreSQL | Pruebas exitosas en ambos OS |
| **RNF10** | Pruebas | Pruebas unitarias + integraci�n SOAP | Cobertura ? 70% en capa negocio; tests SOAP OK |

---

## 7. MODELO DE DOMINIO

### 7.1 Diagrama Entidad-Relaci�n (L�gico)

```
[INSTRUCCI�N: Crear diagrama ER con herramienta como draw.io, Lucidchart o DBDiagram.io]

Entidades principales:
- USUARIO (id, nombreusuario*, passwordhash, rol, fechacreacion, activo)
- ARTICULO (id, codigo*, nombre, descripcion, categoriaid, preciocompra, precioventa, stock, stockminimo, proveedorid, fechacreacion, fechaactualizacion, activo)
- CATEGORIA (id, nombre*, descripcion)
- PROVEEDOR (id, nombre*, telefono, email, direccion)
- LOGOPERACIONES (id, operacion, entidad, entidadid, usuarioid, detalles, fechahora)

Relaciones:
- ARTICULO -> CATEGORIA (N:1)
- ARTICULO -> PROVEEDOR (N:1)
- LOGOPERACIONES -> USUARIO (N:1)
```

### 7.2 Diccionario de Datos

#### **Tabla: articulos**

| Campo | Tipo | Restricciones | Descripci�n |
|-------|------|---------------|-------------|
| id | INTEGER | PK, AUTO_INCREMENT | Identificador �nico |
| codigo | VARCHAR(50) | NOT NULL, UNIQUE | C�digo del art�culo |
| nombre | VARCHAR(200) | NOT NULL | Nombre del producto |
| descripcion | TEXT | NULL | Descripci�n detallada |
| categoriaid | INTEGER | FK -> categorias.id | Categor�a del art�culo |
| preciocompra | NUMERIC(10,2) | NOT NULL, > 0 | Precio de compra |
| precioventa | NUMERIC(10,2) | NOT NULL, > 0, > preciocompra | Precio de venta |
| stock | INTEGER | NOT NULL, ? 0 | Stock actual |
| stockminimo | INTEGER | NOT NULL, ? 0 | Stock m�nimo para alerta |
| proveedorid | INTEGER | FK -> proveedores.id | Proveedor principal |
| fechacreacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | Fecha de registro |
| fechaactualizacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | �ltima modificaci�n |
| activo | BOOLEAN | NOT NULL, DEFAULT TRUE | Estado del art�culo |

#### **Tabla: usuarios**

| Campo | Tipo | Restricciones | Descripci�n |
|-------|------|---------------|-------------|
| id | INTEGER | PK, AUTO_INCREMENT | Identificador �nico |
| nombreusuario | VARCHAR(50) | NOT NULL, UNIQUE | Nombre de usuario |
| passwordhash | VARCHAR(255) | NOT NULL | Contrase�a hasheada (BCrypt) |
| rol | VARCHAR(20) | NOT NULL, DEFAULT 'Usuario' | Rol: Admin/Usuario |
| fechacreacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | Fecha de creaci�n |
| activo | BOOLEAN | NOT NULL, DEFAULT TRUE | Estado de la cuenta |

_(Completar tablas categorias, proveedores, logoperaciones)_

---

## 8. CRITERIOS DE ACEPTACI�N

### 8.1 Funcionalidad Core
- [ ] Se pueden registrar art�culos con todas las validaciones (RF1-RF3)
- [ ] Se pueden consultar art�culos por c�digo y nombre (RF4)
- [ ] Servicios SOAP funcionan correctamente (RF5-RF6)
- [ ] Alertas de stock funcionan (RF7)
- [ ] Datos persisten en PostgreSQL (RF8)

### 8.2 Calidad y Arquitectura
- [ ] Arquitectura N-Capas implementada (RNF1)
- [ ] Operaciones SOAP < 500ms (RNF2)
- [ ] C�digo cumple SOLID (RNF4)
- [ ] WSDL 1.1 v�lido (RNF5)
- [ ] Autenticaci�n funcional (RNF6)

### 8.3 Pruebas
- [ ] Pruebas unitarias ? 70% cobertura (RNF10)
- [ ] Pruebas SOAP con SoapUI exitosas (RNF10)
- [ ] Manejo de errores probado (RF10, RNF7)

---

## 9. RESTRICCIONES Y SUPUESTOS

### 9.1 Restricciones
- Debe usar .NET 9
- Base de datos PostgreSQL
- Protocolo SOAP (no REST)
- Autenticaci�n JWT

### 9.2 Supuestos
- PostgreSQL est� instalado y configurado
- Los usuarios tienen permisos de administrador
- El servidor SOAP es accesible en red local

---

## 10. AP�NDICES

### 10.1 Glosario
- **SOAP:** Simple Object Access Protocol
- **WSDL:** Web Services Description Language
- **DTO:** Data Transfer Object
- **JWT:** JSON Web Token
- **BCrypt:** Algoritmo de hashing de contrase�as

### 10.2 Referencias
- Est�ndar WSDL 1.1: https://www.w3.org/TR/wsdl
- .NET 9 Documentation: https://learn.microsoft.com/dotnet
- PostgreSQL Documentation: https://www.postgresql.org/docs/

---

**Elaborado por:** Anahy Herrera  
**Fecha:** 26 de Octubre, 2025  
**Versi�n:** 1.0  
**Estado:** En Revisi�n

