# ?? GU�A COMPLETA PARA ANAHY - Responsable de Documentaci�n y Base de Datos

---

## ?? TU MISI�N

Eres la **Arquitecta de Datos y Documentalista del Proyecto**. Mientras Oscar programa el backend y Camila la UI, t�:

1. ? Dise�as y documentas la arquitectura completa
2. ? Creas y mantienes la base de datos
3. ? Produces toda la documentaci�n t�cnica (ERS, DDA, informes)
4. ? Coordinas las evidencias de pruebas
5. ? Consolidas el informe final y presentaci�n

---

## ?? CHECKLIST DE TAREAS

### **FASE 1: An�lisis y Dise�o (D�as 1-2)**

#### ? **Tarea 1.1: ERS - Especificaci�n de Requisitos**
**Archivo:** `docs/01-ERS/ERS_InventarioFerreteria.md` (YA CREADO ?)

**Acciones:**
- [ ] Revisar y completar casos de uso (UC01-UC05)
- [ ] Crear diagrama de casos de uso (UML) con herramienta:
  - Draw.io: https://app.diagrams.net/
  - Lucidchart: https://www.lucidchart.com/
  - PlantUML: https://plantuml.com/
- [ ] Guardar diagrama como `docs/Diagramas/casos-de-uso.png`
- [ ] Validar que todos los RF y RNF est�n completos
- [ ] Agregar criterios de aceptaci�n espec�ficos

**Herramientas recomendadas:**
```bash
# Opci�n 1: Draw.io (online o desktop)
https://app.diagrams.net/

# Opci�n 2: PlantUML (c�digo a diagrama)
@startuml
left to right direction
actor "Administrador" as admin
actor "Usuario" as user
actor "Cliente SOAP" as soap

rectangle "Sistema Inventario" {
  usecase "Registrar Art�culo" as UC01
  usecase "Consultar Art�culo" as UC02
  usecase "Actualizar Art�culo" as UC03
  usecase "Insertar Art�culo (SOAP)" as UC04
  usecase "Consultar por C�digo (SOAP)" as UC05
}

admin --> UC01
admin --> UC02
admin --> UC03
user --> UC02
soap --> UC04
soap --> UC05
@enduml
```

#### ? **Tarea 1.2: Modelo de Datos**
**Archivos:** 
- `docs/Diagramas/modelo-er-logico.png`
- `docs/Diagramas/modelo-er-fisico.png`
- `docs/Scripts-SQL/01-create-schema.sql` (YA CREADO ?)

**Acciones:**
- [ ] Crear diagrama ER l�gico (entidades sin tipos de datos)
- [ ] Crear diagrama ER f�sico (tablas con tipos, PKs, FKs)
- [ ] Herramienta recomendada: **DBDiagram.io** o **pgModeler**

**Ejemplo para DBDiagram.io:**
```sql
// dbdiagram.io
Table usuarios {
  id integer [pk, increment]
  nombreusuario varchar(50) [unique, not null]
  passwordhash varchar(255) [not null]
  rol varchar(20) [not null, default: 'Usuario']
  fechacreacion timestamp [not null, default: `now()`]
  activo boolean [not null, default: true]
}

Table articulos {
  id integer [pk, increment]
  codigo varchar(50) [unique, not null]
  nombre varchar(200) [not null]
  categoriaid integer [ref: > categorias.id]
  proveedorid integer [ref: > proveedores.id]
  preciocompra decimal(10,2) [not null]
  precioventa decimal(10,2) [not null]
  stock integer [not null, default: 0]
  stockminimo integer [not null, default: 5]
  activo boolean [not null, default: true]
}

Table categorias {
  id integer [pk, increment]
  nombre varchar(100) [unique, not null]
  descripcion text
}

Table proveedores {
  id integer [pk, increment]
  nombre varchar(150) [unique, not null]
  telefono varchar(20)
  email varchar(100)
  direccion text
}

Table logoperaciones {
  id integer [pk, increment]
  operacion varchar(50) [not null]
  entidad varchar(50) [not null]
  usuarioid integer [ref: > usuarios.id]
  fechahora timestamp [not null, default: `now()`]
}
```

Luego exporta como PNG y guarda en `docs/Diagramas/`.

#### ? **Tarea 1.3: Crear Base de Datos en PostgreSQL**

**Pasos:**

1. **Abrir pgAdmin o psql**
```bash
# Opci�n 1: psql (terminal)
psql -U postgres

# Opci�n 2: pgAdmin (GUI)
# Abrir pgAdmin ? Create Database
```

2. **Crear la base de datos**
```sql
CREATE DATABASE ferreteria
    WITH 
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'Spanish_Ecuador.1252'
    LC_CTYPE = 'Spanish_Ecuador.1252'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;
```

3. **Ejecutar el script de creaci�n**
```bash
# En psql:
\c ferreteria
\i 'C:/Users/Anahy/Desktop/InventarioFerreteria/docs/Scripts-SQL/01-create-schema.sql'

# O desde terminal:
psql -U postgres -d ferreteria -f "C:\Users\Anahy\Desktop\InventarioFerreteria\docs\Scripts-SQL\01-create-schema.sql"
```

4. **Verificar creaci�n**
```sql
-- Ver tablas
\dt

-- Ver datos
SELECT * FROM categorias;
SELECT * FROM proveedores;
SELECT * FROM articulos;
SELECT * FROM usuarios;
```

5. **?? IMPORTANTE: Hashear Contrase�as**

**Las contrase�as actuales est�n en texto plano. Necesitas hashearlas con BCrypt.**

**Opci�n A: Usar herramienta online (temporal)**
```
https://bcrypt-generator.com/
Input: Admin123!
Output: $2a$12$XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

**Opci�n B: Crear script C# (recomendado)**
```bash
cd C:\Users\Anahy\Desktop
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

Edita `Program.cs`:
```csharp
using BCrypt.Net;

Console.WriteLine("=== Generador de Hashes BCrypt ===\n");

string[] passwords = { "Admin123!", "Usuario123!" };

foreach (var pwd in passwords)
{
    string hash = BCrypt.HashPassword(pwd);
    Console.WriteLine($"Password: {pwd}");
    Console.WriteLine($"Hash BCrypt: {hash}\n");
}
```

Ejecuta:
```bash
dotnet run
```

Copia los hashes y ejecuta en PostgreSQL:
```sql
UPDATE usuarios 
SET passwordhash = '$2a$12$HASH_ADMIN_AQUI'
WHERE nombreusuario = 'admin';

UPDATE usuarios 
SET passwordhash = '$2a$12$HASH_USUARIO_AQUI'
WHERE nombreusuario = 'usuario';

-- Verificar
SELECT nombreusuario, LENGTH(passwordhash) as hash_length FROM usuarios;
-- Deber�a mostrar hash_length = 60
```

---

### **FASE 2: Dise�o Arquitect�nico (D�as 3-4)**

#### ? **Tarea 2.1: DDA - Documento de Dise�o Arquitect�nico**
**Archivo:** `docs/02-DDA/DDA_InventarioFerreteria.md` (YA CREADO ?)

**Acciones:**
- [ ] Crear diagrama de arquitectura N-Capas
- [ ] Crear diagrama de componentes (UML)
- [ ] Crear diagramas de clases para cada capa
- [ ] Crear diagramas de secuencia (UC01-UC05)
- [ ] Crear diagrama de despliegue
- [ ] Documentar especificaci�n WSDL/XSD completa

**Diagramas a crear (gu�rdalos en `docs/Diagramas/`):**
1. `arquitectura-n-capas.png` - Diagrama de capas
2. `diagrama-componentes.png` - Componentes UML
3. `diagrama-clases-entidades.png`
4. `diagrama-clases-negocio.png`
5. `diagrama-clases-soap.png`
6. `secuencia-autenticacion.png`
7. `secuencia-insertar-articulo.png`
8. `secuencia-consultar-articulo.png`
9. `diagrama-despliegue.png`

**Herramienta recomendada: PlantUML + VS Code**

Instala extensi�n PlantUML en VS Code:
```
Ctrl+P -> ext install plantuml
```

Ejemplo de diagrama de secuencia:
```plantuml
@startuml
actor Cliente
participant "SoapService" as SS
participant "AuthService" as AS
participant "UsuarioRepo" as UR
database PostgreSQL as DB

Cliente -> SS: Autenticar(usuario, password)
SS -> AS: Autenticar(usuario, password)
AS -> UR: ObtenerPorNombreUsuario(usuario)
UR -> DB: SELECT * FROM usuarios WHERE...
DB -> UR: Usuario con passwordhash
UR -> AS: Usuario
AS -> AS: VerificarPassword(BCrypt)
alt Contrase�a v�lida
  AS -> AS: GenerarTokenJWT()
  AS -> SS: RespuestaDTO<string>(token)
  SS -> Cliente: ResultadoOperacion(Exito=true, Datos=token)
else Contrase�a inv�lida
  AS -> SS: RespuestaDTO<string>(error)
  SS -> Cliente: ResultadoOperacion(Exito=false)
end
@enduml
```

#### ? **Tarea 2.2: Definici�n WSDL/XSD**

El archivo DDA ya incluye la especificaci�n XML Schema. Ahora debes:

1. **Generar el WSDL real del servicio**
   
Una vez que Oscar tenga el servicio SOAP corriendo:
```bash
# Accede a:
http://localhost:5233/InventarioService.asmx?wsdl

# Guarda el contenido en:
# docs/02-DDA/InventarioService.wsdl
```

2. **Guardar WSDL en archivo**
```bash
# En navegador, guarda como:
docs/02-DDA/InventarioService.wsdl
```

3. **Extraer XSD del WSDL**
Busca la secci�n `<types>` en el WSDL y gu�rdala en:
```
docs/02-DDA/InventarioService.xsd
```

---

### **FASE 3: Pruebas y Evidencias (D�as 5-6)**

#### ? **Tarea 3.1: Plan de Pruebas**
**Archivo:** `docs/03-PlanPruebas/PlanDePruebas.md`

**Contenido:**
- Casos de prueba unitarios (coordinaci�n con Oscar)
- Casos de prueba de integraci�n SOAP (con SoapUI)
- Casos de prueba de UI (coordinaci�n con Camila)
- Criterios de aceptaci�n por cada RF/RNF

#### ? **Tarea 3.2: Capturar Evidencias**

**Durante las pruebas (cuando Oscar y Camila terminen), captura:**

1. **Autenticaci�n exitosa**
   - Captura: Cliente enviando credenciales
   - Captura: Respuesta con token JWT
   - Guardar en: `docs/Evidencias/01-autenticacion-exitosa.png`

2. **Insertar art�culo (SOAP)**
   - Captura: Request SOAP desde SoapUI
   - Captura: Response con art�culo creado
   - Guardar en: `docs/Evidencias/02-insertar-articulo-soap.png`

3. **C�digo duplicado (SOAP Fault)**
   - Captura: Request con c�digo existente
   - Captura: SOAP Fault ValidationFault
   - Guardar en: `docs/Evidencias/03-codigo-duplicado-fault.png`

4. **Consultar art�culo por c�digo**
   - Captura: Request SOAP
   - Captura: Response con datos del art�culo
   - Guardar en: `docs/Evidencias/04-consultar-articulo.png`

5. **Alerta de stock bajo (RF7)**
   - Captura: Art�culo con RequiereReposicion=true
   - Guardar en: `docs/Evidencias/05-alerta-stock-bajo.png`

6. **Rendimiento (RNF2)**
   - Captura de SoapUI mostrando latencias < 500ms
   - Guardar en: `docs/Evidencias/06-rendimiento-soap.png`

7. **Interfaz de usuario (Camila)**
   - Capturas de pantalla de la UI
   - Guardar en: `docs/Evidencias/07-ui-*.png`

---

### **FASE 4: Manual T�cnico (D�a 6)**

#### ? **Tarea 4.1: Manual de Instalaci�n y Despliegue**
**Archivo:** `docs/04-ManualTecnico/ManualTecnico.md`

**Contenido:**
```markdown
# Manual T�cnico - Sistema Inventario Ferreter�a

## 1. Requisitos del Sistema
- .NET 9 SDK
- PostgreSQL 15+
- Windows 10/11 o Linux (Ubuntu 20.04+)
- 4 GB RAM m�nimo
- 500 MB espacio en disco

## 2. Instalaci�n de Dependencias

### 2.1 Instalar .NET 9 SDK
... (instrucciones)

### 2.2 Instalar PostgreSQL
... (instrucciones)

## 3. Configuraci�n de Base de Datos
... (paso a paso con screenshots)

## 4. Configuraci�n del Proyecto
... (appsettings.json, variables de entorno)

## 5. Ejecuci�n
... (c�mo ejecutar servidor y cliente)

## 6. Pruebas
... (c�mo ejecutar tests)

## 7. Troubleshooting
... (problemas comunes y soluciones)
```

---

### **FASE 5: Informe Final (D�a 7)**

#### ? **Tarea 5.1: Informe Final del Proyecto**
**Archivo:** `docs/05-InformeFinal/InformeFinal.pdf`

**Estructura sugerida:**

```
1. PORTADA
   - T�tulo del proyecto
   - Nombres del equipo (Anahy, Oscar, Camila)
   - Fecha
   - Logo de la instituci�n

2. RESUMEN EJECUTIVO (1 p�gina)
   - Objetivo del proyecto
   - Tecnolog�as utilizadas
   - Resultados alcanzados
   - Conclusiones principales

3. INTRODUCCI�N (2 p�ginas)
   - Contexto del proyecto
   - Problem�tica
   - Objetivos generales y espec�ficos
   - Alcance

4. MARCO TE�RICO (3 p�ginas)
   - Arquitectura N-Capas
   - Servicios SOAP
   - PostgreSQL
   - .NET 9

5. AN�LISIS Y DISE�O (10 p�ginas)
   - Requisitos (RF y RNF)
   - Casos de uso (con diagramas)
   - Modelo de datos (diagramas ER)
   - Arquitectura del sistema (diagramas)
   - Dise�o de clases
   - Dise�o de servicios SOAP (WSDL/XSD)

6. IMPLEMENTACI�N (5 p�ginas)
   - Tecnolog�as y herramientas
   - Estructura de proyectos
   - Componentes principales
   - Fragmentos de c�digo relevantes

7. PRUEBAS (5 p�ginas)
   - Plan de pruebas
   - Casos de prueba ejecutados
   - Resultados (con capturas)
   - Cumplimiento de RNF2 (rendimiento)

8. MANUAL DE USUARIO (3 p�ginas)
   - Gu�a de uso del cliente
   - Pantallas principales
   - Operaciones disponibles

9. CONCLUSIONES Y RECOMENDACIONES (2 p�ginas)
   - Cumplimiento de requisitos
   - Lecciones aprendidas
   - Mejoras futuras
   - Posibles extensiones

10. BIBLIOGRAF�A
    - Referencias utilizadas

11. ANEXOS
    - Scripts SQL completos
    - WSDL completo
    - C�digo fuente destacado
```

**Herramientas recomendadas:**
- Microsoft Word / Google Docs
- LaTeX (para formato acad�mico profesional)

---

## ??? C�MO EJECUTAR SERVIDOR + CLIENTE

### **M�todo Recomendado: Visual Studio**

1. **Abre Visual Studio**
2. **Abre la soluci�n:** `InventarioFerreteria.sln`
3. **Configura proyectos de inicio m�ltiples:**
   - Click derecho en la **Soluci�n** (no en un proyecto individual)
   - Selecciona **"Propiedades"**
   - En el panel izquierdo: **"Proyectos de inicio"**
   - Selecciona **"Varios proyectos de inicio"**
   - Establece **"Acci�n"** = **"Iniciar"** para:
     - ? `InventarioFerreteria.SoapService`
     - ? `InventarioFerreteria.Client`
   - Deja los dem�s como **"Ninguno"**
   - Click **"Aceptar"**

4. **Ejecuta:** Presiona `F5` o click en ?? **Iniciar**

Se abrir�n dos ventanas:
- **Ventana 1:** Servidor SOAP (consola mostrando logs)
- **Ventana 2:** Cliente (men� interactivo)

### **M�todo Alternativo: Scripts**

**Windows PowerShell:**
```powershell
# Ejecuta el script creado
.\run-both.ps1
```

**Windows Batch:**
```cmd
run-both.bat
```

**Linux/Mac:**
```bash
# Terminal 1
dotnet run --project InventarioFerreteria.SoapService

# Terminal 2 (espera 5 segundos)
dotnet run --project InventarioFerreteria.Client
```

---

## ?? COORDINACI�N CON EL EQUIPO

### **Con Oscar (Backend):**
- [ ] Validar que el modelo de datos coincida con las entidades C#
- [ ] Solicitar acceso al WSDL generado por SoapCore
- [ ] Coordinar pruebas de integraci�n SOAP
- [ ] Solicitar logs de errores para documentar en RNF7

### **Con Camila (Frontend/DevOps):**
- [ ] Coordinar capturas de pantalla de la UI
- [ ] Validar que la gu�a de despliegue funcione
- [ ] Solicitar evidencias de CI/CD (si implementa)
- [ ] Coordinar presentaci�n final

---

## ?? CRONOGRAMA SUGERIDO

| D�a | Tarea Principal | Entregables |
|-----|-----------------|-------------|
| **D�a 1** | ERS completo + Diagrama casos de uso | ERS.md, casos-de-uso.png |
| **D�a 2** | Modelo de datos + Crear BD | modelo-er-*.png, BD creada, datos cargados |
| **D�a 3** | DDA: Arquitectura + Componentes | arquitectura-n-capas.png, componentes.png |
| **D�a 4** | DDA: Clases + Secuencias + WSDL | diagramas-clases-*.png, secuencias-*.png |
| **D�a 5** | Plan de pruebas + Evidencias (con equipo) | PlanDePruebas.md, capturas |
| **D�a 6** | Manual T�cnico | ManualTecnico.md |
| **D�a 7** | Informe Final + Presentaci�n | InformeFinal.pdf, Presentacion.pptx |

---

## ? CHECKLIST FINAL ANTES DE ENTREGAR

### Documentos:
- [ ] ERS completo con todos los diagramas
- [ ] DDA completo con arquitectura, clases, secuencias
- [ ] Plan de pruebas con casos de prueba
- [ ] Manual t�cnico con gu�a de instalaci�n
- [ ] Informe final en PDF profesional
- [ ] Presentaci�n PowerPoint/Google Slides

### Diagramas (m�nimo 9):
- [ ] Casos de uso
- [ ] Modelo ER l�gico
- [ ] Modelo ER f�sico
- [ ] Arquitectura N-Capas
- [ ] Diagrama de componentes
- [ ] Diagramas de clases (Entidades, Negocio, SOAP)
- [ ] Diagramas de secuencia (Autenticaci�n, Insertar, Consultar)
- [ ] Diagrama de despliegue

### Scripts SQL:
- [ ] Script de creaci�n de esquema
- [ ] Script de datos iniciales
- [ ] Contrase�as hasheadas con BCrypt

### Evidencias (capturas):
- [ ] Autenticaci�n exitosa
- [ ] Insertar art�culo SOAP
- [ ] C�digo duplicado (SOAP Fault)
- [ ] Consultar art�culo
- [ ] Alerta stock bajo
- [ ] Pruebas de rendimiento (< 500ms)
- [ ] Capturas de UI

### Base de Datos:
- [ ] BD creada y funcionando
- [ ] Tablas con constraints correctos
- [ ] �ndices aplicados
- [ ] Triggers funcionando
- [ ] Datos de prueba cargados
- [ ] Contrase�as hasheadas

### Verificaci�n T�cnica:
- [ ] El servidor SOAP inicia correctamente
- [ ] El cliente consume servicios SOAP
- [ ] WSDL accesible en /InventarioService.asmx?wsdl
- [ ] Autenticaci�n JWT funciona
- [ ] Validaciones de negocio (RF3) funcionan
- [ ] Stock bajo genera alerta (RF7)

---

## ?? CONSEJOS PROFESIONALES

### 1. **Organizaci�n de Archivos**
Mant�n esta estructura:
```
docs/
??? 01-ERS/
?   ??? ERS_InventarioFerreteria.md
?   ??? anexos/
??? 02-DDA/
?   ??? DDA_InventarioFerreteria.md
?   ??? InventarioService.wsdl
?   ??? InventarioService.xsd
??? 03-PlanPruebas/
?   ??? PlanDePruebas.md
??? 04-ManualTecnico/
?   ??? ManualTecnico.md
??? 05-InformeFinal/
?   ??? InformeFinal.pdf
?   ??? Presentacion.pptx
??? Diagramas/
?   ??? casos-de-uso.png
?   ??? modelo-er-logico.png
?   ??? modelo-er-fisico.png
?   ??? arquitectura-n-capas.png
?   ??? diagrama-componentes.png
?   ??? diagrama-clases-*.png
?   ??? secuencia-*.png
?   ??? diagrama-despliegue.png
??? Scripts-SQL/
?   ??? 01-create-schema.sql
?   ??? 02-insert-data.sql
?   ??? 03-hash-passwords.sql
??? Evidencias/
    ??? 01-autenticacion-exitosa.png
    ??? 02-insertar-articulo-soap.png
    ??? 03-codigo-duplicado-fault.png
    ??? 04-consultar-articulo.png
    ??? 05-alerta-stock-bajo.png
    ??? 06-rendimiento-soap.png
    ??? 07-ui-*.png
```

### 2. **Control de Versiones (Git)**
```bash
# Crea una rama para tu trabajo
git checkout -b feature/anahy-docs

# A�ade tus cambios
git add docs/

# Commitea frecuentemente
git commit -m "docs: Agrega ERS completo con casos de uso"

# Push peri�dicamente
git push origin feature/anahy-docs
```

### 3. **Reuniones con el Equipo**
- **Reuni�n inicial:** Alinear visi�n del proyecto
- **Reuni�n D�a 3:** Validar arquitectura con Oscar
- **Reuni�n D�a 5:** Coordinar pruebas y evidencias
- **Reuni�n D�a 6:** Revisi�n final antes de entrega

### 4. **Herramientas Recomendadas**
- **Diagramas UML:** PlantUML + VS Code o Draw.io
- **Diagramas ER:** DBDiagram.io o pgModeler
- **Documentaci�n:** Markdown ? Pandoc ? PDF
- **Presentaci�n:** PowerPoint o Google Slides
- **Control de versiones:** Git + GitHub Desktop

---

## ?? RESOLUCI�N DE PROBLEMAS

### **Problema: No puedo conectar a PostgreSQL**
**Soluci�n:**
```bash
# Verifica que PostgreSQL est� ejecut�ndose
# Windows:
services.msc -> PostgreSQL -> Estado: Iniciado

# Verifica puerto:
netstat -an | findstr :5432

# Prueba conexi�n:
psql -U postgres -h localhost
```

### **Problema: Las contrase�as no funcionan**
**Soluci�n:**
Ver archivo `TROUBLESHOOTING_AUTH.md` en la ra�z del proyecto.
Las contrase�as deben estar hasheadas con BCrypt (ver Tarea 1.3).

### **Problema: No se generan los diagramas**
**Soluci�n:**
- Usa herramientas online si las locales fallan
- Draw.io funciona sin instalaci�n
- DBDiagram.io genera SQL y diagramas autom�ticamente

---

## ?? CONTACTO CON EL EQUIPO

**Anahy (t�):** Documentaci�n, Base de Datos  
**Oscar:** Backend, Servicios SOAP, L�gica de Negocio  
**Camila:** Frontend, Cliente, DevOps

**Canales de comunicaci�n:**
- WhatsApp del grupo
- Google Drive compartido para documentos
- GitHub para c�digo y documentaci�n t�cnica

---

��xito en tu trabajo, Anahy! Esta gu�a deber�a cubrir todo lo que necesitas. ??

**�ltima actualizaci�n:** 26 de Octubre, 2025
