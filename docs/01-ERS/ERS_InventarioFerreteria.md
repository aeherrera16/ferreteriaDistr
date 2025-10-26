# ?? ERS - Especificación de Requisitos del Sistema
## Sistema de Control de Inventario para Ferretería

---

## 1. INTRODUCCIÓN

### 1.1 Propósito del Documento
Este documento describe los requisitos funcionales y no funcionales del Sistema de Control de Inventario para Ferretería, desarrollado con arquitectura N-Capas y servicios SOAP.

### 1.2 Alcance del Sistema
El sistema permitirá gestionar el inventario de una ferretería mediante:
- Registro, consulta y actualización de artículos
- Servicios web SOAP para integración con sistemas externos
- Control de stock y alertas de reposición
- Autenticación de usuarios

### 1.3 Objetivos del Proyecto
- Implementar arquitectura N-Capas siguiendo principios SOLID
- Exponer servicios SOAP cumpliendo estándares WSDL 1.1
- Gestionar inventario con validaciones de negocio robustas
- Proporcionar interfaz intuitiva para operaciones CRUD

---

## 2. DESCRIPCIÓN GENERAL

### 2.1 Perspectiva del Producto
Sistema standalone con base de datos PostgreSQL y servicios SOAP para interoperabilidad.

### 2.2 Funciones del Producto
- Gestión completa de artículos (CRUD)
- Control de stock con alertas automáticas
- Servicios SOAP para inserción y consulta
- Autenticación y autorización básica

### 2.3 Características de los Usuarios

| Tipo de Usuario | Descripción | Permisos |
|-----------------|-------------|----------|
| Administrador | Usuario con permisos completos | Crear, leer, actualizar artículos |
| Usuario | Operador de inventario | Consultar artículos |
| Cliente SOAP | Sistema externo | Insertar y consultar vía SOAP |

---

## 3. ACTORES DEL SISTEMA

### 3.1 Administrador
**Descripción:** Usuario interno con privilegios completos
**Responsabilidades:**
- Registrar nuevos artículos
- Actualizar información de inventario
- Consultar estado del stock
- Gestionar proveedores y categorías

### 3.2 Usuario/Operador
**Descripción:** Usuario interno con permisos de consulta
**Responsabilidades:**
- Consultar artículos por código o nombre
- Ver alertas de stock bajo

### 3.3 Cliente SOAP (Sistema Externo)
**Descripción:** Aplicación externa que consume servicios SOAP
**Responsabilidades:**
- Insertar artículos mediante servicio SOAP
- Consultar artículos por código vía SOAP

---

## 4. CASOS DE USO

### 4.1 Diagrama de Casos de Uso
```
[INSTRUCCIÓN: Crear diagrama con herramienta UML mostrando:]
- Actor: Administrador
  - UC01: Registrar Artículo
  - UC02: Consultar Artículo
  - UC03: Actualizar Artículo
  
- Actor: Usuario
  - UC02: Consultar Artículo
  
- Actor: Cliente SOAP
  - UC04: Insertar Artículo (SOAP)
  - UC05: Consultar Artículo por Código (SOAP)
```

### 4.2 Especificación de Casos de Uso

#### **UC01: Registrar Artículo**

| Campo | Descripción |
|-------|-------------|
| **ID** | UC01 |
| **Nombre** | Registrar Artículo |
| **Actor Principal** | Administrador |
| **Precondiciones** | Usuario autenticado como Administrador |
| **Postcondiciones** | Artículo registrado en la base de datos |

**Flujo Principal:**
1. El administrador accede a la función "Registrar Artículo"
2. El sistema muestra formulario de registro
3. El administrador ingresa:
   - Código (único)
   - Nombre
   - Descripción (opcional)
   - Categoría
   - Precio de compra
   - Precio de venta
   - Stock inicial
   - Stock mínimo
   - Proveedor (opcional)
4. El sistema valida:
   - Código único (no existente)
   - Precios > 0
   - Precio venta > Precio compra
   - Stock ? 0
5. El sistema registra el artículo
6. El sistema muestra mensaje de confirmación

**Flujos Alternos:**
- **4a. Código duplicado:**
  - 4a.1 El sistema muestra error "El código ya existe"
  - 4a.2 Retorna al paso 3

- **4b. Datos inválidos:**
  - 4b.1 El sistema muestra errores de validación específicos
  - 4b.2 Retorna al paso 3

#### **UC02: Consultar Artículo**

| Campo | Descripción |
|-------|-------------|
| **ID** | UC02 |
| **Nombre** | Consultar Artículo |
| **Actor Principal** | Administrador, Usuario |
| **Precondiciones** | Usuario autenticado |
| **Postcondiciones** | Información del artículo mostrada |

**Flujo Principal:**
1. El usuario accede a "Consultar Artículo"
2. El sistema ofrece opciones de búsqueda:
   - Por código (búsqueda exacta)
   - Por nombre (búsqueda parcial)
3. El usuario ingresa criterio de búsqueda
4. El sistema busca en la base de datos
5. El sistema muestra resultados con:
   - Código, nombre, descripción
   - Precios (compra y venta)
   - Stock actual y mínimo
   - ?? Alerta si stock < stock mínimo
6. Fin del caso de uso

**Flujos Alternos:**
- **4a. No se encuentra artículo:**
  - 4a.1 El sistema muestra "No se encontraron resultados"
  - 4a.2 Retorna al paso 2

#### **UC03: Actualizar Artículo**

| Campo | Descripción |
|-------|-------------|
| **ID** | UC03 |
| **Nombre** | Actualizar Artículo |
| **Actor Principal** | Administrador |
| **Precondiciones** | Usuario autenticado como Administrador; Artículo existe |
| **Postcondiciones** | Artículo actualizado en base de datos |

**Flujo Principal:**
1. El administrador consulta el artículo (UC02)
2. Selecciona opción "Editar"
3. El sistema muestra formulario con datos actuales
4. El administrador modifica campos permitidos:
   - Nombre, descripción, categoría
   - Precios (compra/venta)
   - Stock, stock mínimo
   - Proveedor
5. El sistema valida datos (mismas reglas que UC01)
6. El sistema actualiza el artículo
7. El sistema muestra confirmación

**Flujos Alternos:**
- **5a. Validación falla:** Similar a UC01.4b

#### **UC04: Insertar Artículo (SOAP)**

| Campo | Descripción |
|-------|-------------|
| **ID** | UC04 |
| **Nombre** | Insertar Artículo vía SOAP |
| **Actor Principal** | Cliente SOAP (Sistema Externo) |
| **Precondiciones** | Cliente autenticado con token JWT válido |
| **Postcondiciones** | Artículo registrado en base de datos |

**Flujo Principal:**
1. Cliente envía petición SOAP `InsertarArticulo` con:
   - Token JWT
   - DTO del artículo (código, nombre, precios, etc.)
2. El servicio valida el token
3. El servicio valida datos del artículo (RF3)
4. El servicio registra el artículo en BD
5. El servicio retorna `ResultadoOperacion` con:
   - Exito = true
   - Mensaje = "Artículo insertado correctamente"
   - Datos = ArticuloDTO completo

**Flujos Alternos:**
- **2a. Token inválido:**
  - 2a.1 El servicio retorna SOAP Fault `UnauthorizedFault`
  - 2a.2 Mensaje: "Token inválido o expirado"

- **3a. Código duplicado:**
  - 3a.1 El servicio retorna SOAP Fault `ValidationFault`
  - 3a.2 Mensaje: "El código ya existe"

- **3b. Datos inválidos:**
  - 3b.1 El servicio retorna SOAP Fault `ValidationFault`
  - 3b.2 Mensaje: Lista de errores de validación

#### **UC05: Consultar Artículo por Código (SOAP)**

| Campo | Descripción |
|-------|-------------|
| **ID** | UC05 |
| **Nombre** | Consultar Artículo por Código vía SOAP |
| **Actor Principal** | Cliente SOAP |
| **Precondiciones** | Cliente autenticado |
| **Postcondiciones** | Información del artículo retornada |

**Flujo Principal:**
1. Cliente envía petición SOAP `ConsultarArticuloPorCodigo` con:
   - Token JWT
   - Código del artículo
2. El servicio valida el token
3. El servicio busca el artículo en BD
4. El servicio retorna `ResultadoOperacion` con:
   - Exito = true
   - Mensaje = "Artículo encontrado"
   - Datos = ArticuloDTO (incluye alerta si stock < mínimo)

**Flujos Alternos:**
- **3a. Artículo no encontrado:**
  - 3a.1 El servicio retorna SOAP Fault `NotFoundFault`
  - 3a.2 Mensaje: "Artículo con código X no encontrado"

---

## 5. REQUERIMIENTOS FUNCIONALES

| ID | Requerimiento | Descripción | Prioridad |
|----|---------------|-------------|-----------|
| **RF1** | Gestión de artículos | Permitir registrar, consultar y actualizar artículos del inventario | Alta |
| **RF2** | Registro de nuevo artículo | Ingresar datos: código, nombre, categoría, precios, stock, proveedor | Alta |
| **RF3** | Validación de datos | Validar unicidad código, precios positivos, coherencia precios | Alta |
| **RF4** | Consulta de artículos | Buscar artículos por código (exacto) o nombre (parcial) | Alta |
| **RF5** | Servicio SOAP: insertar | Exponer operación SOAP para agregar artículos a BD | Alta |
| **RF6** | Servicio SOAP: consultar | Exponer operación SOAP para consultar artículo por código | Alta |
| **RF7** | Manejo de stock | Registrar stock disponible; alertar si stock < mínimo | Media |
| **RF8** | Persistencia de datos | Guardar registros en PostgreSQL | Alta |
| **RF9** | Interfaz de usuario | Interfaz web/escritorio simple para operaciones básicas | Media |
| **RF10** | Manejo de errores | Mostrar mensajes claros o SOAP Fault ante errores | Alta |

### 5.1 Reglas de Negocio

#### **RN1: Unicidad de Código**
- El código de artículo debe ser único en el sistema
- Comparación case-insensitive
- Formato: alfanumérico, hasta 30 caracteres

#### **RN2: Validación de Precios**
- Precio de compra > 0
- Precio de venta > 0
- Precio de venta debe ser mayor que precio de compra
- Formato: numérico con 2 decimales

#### **RN3: Validación de Stock**
- Stock actual ? 0
- Stock mínimo ? 0
- Stock mínimo no puede ser mayor que stock actual al registrar

#### **RN4: Alerta de Reposición**
- El sistema debe marcar artículos con stock < stock_minimo
- Bandera RequiereReposicion en ArticuloDTO

#### **RN5: Autenticación SOAP**
- Todas las operaciones SOAP requieren token JWT válido
- Token debe incluir en header de petición SOAP
- Expiración: 8 horas (configurable)

---

## 6. REQUERIMIENTOS NO FUNCIONALES

| ID | Requerimiento | Descripción | Criterio de Aceptación |
|----|---------------|-------------|------------------------|
| **RNF1** | Arquitectura N-Capas | Código organizado en capas con responsabilidades separadas | Proyectos separados: Presentación, Servicios, Negocio, Datos, Entidades |
| **RNF2** | Rendimiento | Operaciones SOAP ? 500 ms bajo carga normal | Pruebas con SoapUI muestran latencia promedio < 500ms |
| **RNF3** | Escalabilidad | Permitir agregar funcionalidades sin afectar existentes | Arquitectura desacoplada con interfaces |
| **RNF4** | Mantenibilidad | Código limpio, comentado, SOLID | Code review aprueba adherencia a SOLID |
| **RNF5** | Interoperabilidad | SOAP cumple WSDL 1.1 y XML Schema | WSDL válido y consumible desde SoapUI |
| **RNF6** | Seguridad básica | Acceso restringido a admin | Login requerido; contraseñas con BCrypt |
| **RNF7** | Confiabilidad | Manejo de errores con mensajes y logs | Excepciones capturadas; logs en consola/archivo |
| **RNF8** | Usabilidad | Interfaz intuitiva con validaciones visuales | Mensajes descriptivos; validaciones en cliente |
| **RNF9** | Portabilidad | Ejecutable en Windows/Linux con .NET + PostgreSQL | Pruebas exitosas en ambos OS |
| **RNF10** | Pruebas | Pruebas unitarias + integración SOAP | Cobertura ? 70% en capa negocio; tests SOAP OK |

---

## 7. MODELO DE DOMINIO

### 7.1 Diagrama Entidad-Relación (Lógico)

```
[INSTRUCCIÓN: Crear diagrama ER con herramienta como draw.io, Lucidchart o DBDiagram.io]

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

| Campo | Tipo | Restricciones | Descripción |
|-------|------|---------------|-------------|
| id | INTEGER | PK, AUTO_INCREMENT | Identificador único |
| codigo | VARCHAR(50) | NOT NULL, UNIQUE | Código del artículo |
| nombre | VARCHAR(200) | NOT NULL | Nombre del producto |
| descripcion | TEXT | NULL | Descripción detallada |
| categoriaid | INTEGER | FK -> categorias.id | Categoría del artículo |
| preciocompra | NUMERIC(10,2) | NOT NULL, > 0 | Precio de compra |
| precioventa | NUMERIC(10,2) | NOT NULL, > 0, > preciocompra | Precio de venta |
| stock | INTEGER | NOT NULL, ? 0 | Stock actual |
| stockminimo | INTEGER | NOT NULL, ? 0 | Stock mínimo para alerta |
| proveedorid | INTEGER | FK -> proveedores.id | Proveedor principal |
| fechacreacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | Fecha de registro |
| fechaactualizacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | Última modificación |
| activo | BOOLEAN | NOT NULL, DEFAULT TRUE | Estado del artículo |

#### **Tabla: usuarios**

| Campo | Tipo | Restricciones | Descripción |
|-------|------|---------------|-------------|
| id | INTEGER | PK, AUTO_INCREMENT | Identificador único |
| nombreusuario | VARCHAR(50) | NOT NULL, UNIQUE | Nombre de usuario |
| passwordhash | VARCHAR(255) | NOT NULL | Contraseña hasheada (BCrypt) |
| rol | VARCHAR(20) | NOT NULL, DEFAULT 'Usuario' | Rol: Admin/Usuario |
| fechacreacion | TIMESTAMP | NOT NULL, DEFAULT NOW() | Fecha de creación |
| activo | BOOLEAN | NOT NULL, DEFAULT TRUE | Estado de la cuenta |

_(Completar tablas categorias, proveedores, logoperaciones)_

---

## 8. CRITERIOS DE ACEPTACIÓN

### 8.1 Funcionalidad Core
- [ ] Se pueden registrar artículos con todas las validaciones (RF1-RF3)
- [ ] Se pueden consultar artículos por código y nombre (RF4)
- [ ] Servicios SOAP funcionan correctamente (RF5-RF6)
- [ ] Alertas de stock funcionan (RF7)
- [ ] Datos persisten en PostgreSQL (RF8)

### 8.2 Calidad y Arquitectura
- [ ] Arquitectura N-Capas implementada (RNF1)
- [ ] Operaciones SOAP < 500ms (RNF2)
- [ ] Código cumple SOLID (RNF4)
- [ ] WSDL 1.1 válido (RNF5)
- [ ] Autenticación funcional (RNF6)

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
- Autenticación JWT

### 9.2 Supuestos
- PostgreSQL está instalado y configurado
- Los usuarios tienen permisos de administrador
- El servidor SOAP es accesible en red local

---

## 10. APÉNDICES

### 10.1 Glosario
- **SOAP:** Simple Object Access Protocol
- **WSDL:** Web Services Description Language
- **DTO:** Data Transfer Object
- **JWT:** JSON Web Token
- **BCrypt:** Algoritmo de hashing de contraseñas

### 10.2 Referencias
- Estándar WSDL 1.1: https://www.w3.org/TR/wsdl
- .NET 9 Documentation: https://learn.microsoft.com/dotnet
- PostgreSQL Documentation: https://www.postgresql.org/docs/

---

**Elaborado por:** Anahy Herrera  
**Fecha:** 26 de Octubre, 2025  
**Versión:** 1.0  
**Estado:** En Revisión

