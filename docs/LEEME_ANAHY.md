# ?? RESUMEN EJECUTIVO - �QU� DEBO HACER EXACTAMENTE?

## Hola Anahy! ??

Te voy a explicar **paso a paso y SIN tecnicismos** qu� debes hacer para cumplir con tu parte del proyecto.

---

## ?? TU RESPONSABILIDAD EN UNA FRASE:

**"Eres la encargada de DOCUMENTAR TODO y CREAR LA BASE DE DATOS"**

Mientras Oscar programa el c�digo y Camila hace la interfaz, t� te encargas de:
1. Escribir todos los documentos t�cnicos
2. Crear diagramas
3. Dise�ar y crear la base de datos PostgreSQL
4. Tomar capturas de pantalla de las pruebas
5. Hacer el informe final

---

## ? PASO A PASO - QU� HACER HOY

### **PASO 1: Crear la Base de Datos** (30 minutos)

1. **Abre pgAdmin** (el programa de PostgreSQL)
2. **Click derecho en "Databases"** ? **Create** ? **Database**
3. **Nombre:** `ferreteria`
4. **Click "Save"**
5. **Click derecho en la base de datos "ferreteria"** ? **Query Tool**
6. **Copia y pega TODO el contenido** del archivo:
   ```
   C:\Users\Anahy\Desktop\InventarioFerreteria\docs\Scripts-SQL\01-create-schema.sql
   ```
7. **Click en el bot�n ?? (Execute)**
8. **Deber�as ver:** "Script ejecutado correctamente"

**? Listo! Ya tienes la base de datos creada con tablas y datos de ejemplo**

---

### **PASO 2: Arreglar las Contrase�as** (15 minutos)

Hay un problema: las contrase�as est�n en texto plano y no van a funcionar.

**Soluci�n R�pida:**

1. **Abre Visual Studio Code**
2. **Abre la terminal** (Ctrl + `)
3. **Ejecuta estos comandos uno por uno:**

```bash
cd C:\Users\Anahy\Desktop
dotnet new console -n HashGenerator
cd HashGenerator
dotnet add package BCrypt.Net-Next
```

4. **Abre el archivo `Program.cs`** que se cre�
5. **Borra todo** y pega esto:

```csharp
using BCrypt.Net;

string adminPassword = "Admin123!";
string usuarioPassword = "Usuario123!";

string adminHash = BCrypt.HashPassword(adminPassword);
string usuarioHash = BCrypt.HashPassword(usuarioPassword);

Console.WriteLine("=== COPIA ESTOS HASHES ===\n");
Console.WriteLine($"Admin hash:\n{adminHash}\n");
Console.WriteLine($"Usuario hash:\n{usuarioHash}\n");
Console.WriteLine("=== Presiona Enter para salir ===");
Console.ReadLine();
```

6. **Guarda el archivo** (Ctrl + S)
7. **Ejecuta en la terminal:**
```bash
dotnet run
```

8. **Copia los dos hashes** que aparecen (son largos, empiezan con `$2a$`)

9. **Vuelve a pgAdmin** ? **Query Tool** en la base de datos `ferreteria`

10. **Pega y ejecuta esto** (reemplaza los HASH_AQUI con los que copiaste):

```sql
UPDATE usuarios 
SET passwordhash = 'HASH_QUE_COPIASTE_PARA_ADMIN'
WHERE nombreusuario = 'admin';

UPDATE usuarios 
SET passwordhash = 'HASH_QUE_COPIASTE_PARA_USUARIO'
WHERE nombreusuario = 'usuario';
```

**? Listo! Las contrase�as ya est�n correctas**

---

### **PASO 3: Probar que Funciona** (10 minutos)

1. **Abre Visual Studio** (no VS Code, el Visual Studio grande)
2. **Abre el archivo** `InventarioFerreteria.sln`
3. **Click derecho en la SOLUCI�N** (arriba en el explorador de soluciones)
4. **Selecciona "Configurar proyectos de inicio"** o "Set Startup Projects"
5. **Selecciona "Varios proyectos de inicio"**
6. **Marca como "Iniciar"** estos dos:
   - InventarioFerreteria.SoapService
   - InventarioFerreteria.Client
7. **Click "Aceptar"**
8. **Presiona F5** (o click en el bot�n de play ??)

**Deber�an abrirse 2 ventanas:**
- Una que dice "Now listening on: http://localhost:5233"
- Otra con un men� que dice "1. Autenticar, 2. Insertar..."

9. **En la ventana del men�:**
   - Escribe `1` y presiona Enter
   - Usuario: `admin`
   - Contrase�a: `Admin123!`
   - **Deber�as ver:** "? Autenticaci�n exitosa" y un token largo

**? Si ves eso, TODO FUNCIONA PERFECTO!**

---

## ?? LO QUE DEBES HACER DESPU�S (Los pr�ximos d�as)

### **D�A 1-2: Documentos de Requisitos**

Ya est�n creados en:
```
C:\Users\Anahy\Desktop\InventarioFerreteria\docs\01-ERS\ERS_InventarioFerreteria.md
```

**Tu trabajo:**
1. **Abre el archivo** con VS Code o cualquier editor
2. **L�elo completo** - est� casi terminado
3. **Crea el diagrama de casos de uso:**
   - Ve a https://app.diagrams.net/
   - Crea un diagrama UML con:
     - 3 actores: Administrador, Usuario, Cliente SOAP
     - 5 casos de uso: Registrar Art�culo, Consultar Art�culo, Actualizar Art�culo, Insertar SOAP, Consultar SOAP
   - Gu�rdalo como PNG en: `docs/Diagramas/casos-de-uso.png`

### **D�A 3-4: Documentos de Dise�o**

Ya est� creado en:
```
C:\Users\Anahy\Desktop\InventarioFerreteria\docs\02-DDA\DDA_InventarioFerreteria.md
```

**Tu trabajo:**
1. **Crear diagramas** (usa https://app.diagrams.net/ o PlantUML):
   - Diagrama de arquitectura de capas
   - Diagrama de componentes
   - Diagramas de clases
   - Diagramas de secuencia (3)
   - Diagrama de despliegue

2. **Crear diagrama de base de datos:**
   - Ve a https://dbdiagram.io/
   - Copia el c�digo que est� en la gu�a
   - Exporta como PNG
   - Guarda en: `docs/Diagramas/modelo-er-fisico.png`

### **D�A 5: Tomar Capturas de Pantalla**

Cuando Oscar y Camila terminen su parte:

1. **Prueba la aplicaci�n** (como hiciste en el Paso 3)
2. **Toma capturas** de:
   - Autenticaci�n exitosa
   - Insertar un art�culo
   - Consultar un art�culo
   - Error cuando el c�digo est� duplicado
   - La interfaz de usuario (si Camila la hace)

3. **Gu�rdalas en:**
```
docs/Evidencias/01-autenticacion.png
docs/Evidencias/02-insertar-articulo.png
docs/Evidencias/03-consultar-articulo.png
... etc
```

### **D�A 6: Manual T�cnico**

Crear un documento que explique c�mo instalar y usar el sistema.
B�sicamente, escribir los pasos que hiciste t� (Paso 1, 2, 3) pero m�s detallado.

### **D�A 7: Informe Final**

Juntar TODO en un documento PDF profesional:
- Portada
- Introducci�n
- Los documentos que creaste (ERS, DDA)
- Las capturas de pantalla
- Conclusiones

Usa Word o Google Docs, luego exporta a PDF.

---

## ?? HERRAMIENTAS QUE VAS A USAR

| Herramienta | Para qu� sirve | Link |
|-------------|----------------|------|
| **pgAdmin** | Gestionar PostgreSQL | Ya lo tienes instalado |
| **Visual Studio** | Ejecutar el proyecto | Ya lo tienes instalado |
| **Draw.io** | Crear diagramas UML | https://app.diagrams.net/ |
| **DBDiagram** | Crear diagramas de BD | https://dbdiagram.io/ |
| **VS Code** | Editar archivos Markdown | Ya lo tienes instalado |
| **Word/Google Docs** | Informe final | Ya lo tienes |

---

## ?? �CU�NDO PEDIR AYUDA A OSCAR Y CAMILA?

**Pide ayuda a Oscar cuando:**
- Necesites que te explique c�mo funciona el c�digo
- Necesites el archivo WSDL (cuando �l termine el servidor SOAP)
- Necesites hacer pruebas del servicio SOAP

**Pide ayuda a Camila cuando:**
- Necesites capturas de la interfaz de usuario
- Necesites probar el sistema completo
- Necesites ayuda con Git (subir cambios a GitHub)

**Pide ayuda al profesor cuando:**
- Tengas dudas sobre qu� debe llevar el informe final
- Necesites validar que los diagramas est�n correctos
- Tengas problemas con PostgreSQL que no puedas resolver

---

## ?? PROBLEMAS COMUNES Y SOLUCIONES

### **Problema 1: "No puedo conectar a PostgreSQL"**
**Soluci�n:**
- Abre "Servicios" en Windows (busca "services.msc")
- Busca "PostgreSQL"
- Click derecho ? Iniciar

### **Problema 2: "El servidor no inicia"**
**Soluci�n:**
- Verifica que PostgreSQL est� corriendo (Problema 1)
- Verifica que la base de datos "ferreteria" exista
- Verifica las credenciales en `appsettings.json`:
  - Database: ferreteria
  - Username: postgres
  - Password: admin

### **Problema 3: "No puedo autenticarme"**
**Soluci�n:**
- Verifica que hayas ejecutado el Paso 2 (hashear contrase�as)
- Las contrase�as correctas son:
  - admin / Admin123!
  - usuario / Usuario123!

---

## ? CHECKLIST - �Qu� Ya Est� Hecho?

- ? Estructura de carpetas `docs/` creada
- ? ERS (Especificaci�n de Requisitos) creado
- ? DDA (Documento de Dise�o) creado
- ? Script SQL de creaci�n de base de datos creado
- ? Script de hasheo de contrase�as creado
- ? README.md con instrucciones creado
- ? TROUBLESHOOTING_AUTH.md creado
- ? Scripts de ejecuci�n (run-both.ps1 y run-both.bat) creados
- ? Archivo .gitignore configurado
- ? appsettings.json configurado para tu BD

## ?? CHECKLIST - �Qu� Te Falta?

- [ ] Ejecutar el script SQL (Paso 1)
- [ ] Hashear las contrase�as (Paso 2)
- [ ] Probar que funcione (Paso 3)
- [ ] Crear diagramas (9 diagramas en total)
- [ ] Tomar capturas de pruebas (cuando est� listo)
- [ ] Crear Manual T�cnico
- [ ] Crear Informe Final PDF
- [ ] Crear Presentaci�n PowerPoint

---

## ?? PRIORIDAD PARA HOY (26 de Octubre)

1. **URGENTE:** Ejecutar Paso 1 (crear base de datos) - 30 min
2. **URGENTE:** Ejecutar Paso 2 (hashear contrase�as) - 15 min
3. **IMPORTANTE:** Ejecutar Paso 3 (probar que funciona) - 10 min
4. **IMPORTANTE:** Empezar a crear diagramas de casos de uso - 1-2 horas

**Tiempo total estimado hoy: 2-3 horas**

---

## ?? �TODO EST� LISTO PARA QUE EMPIECES!

Los archivos que necesitas est�n en:
```
C:\Users\Anahy\Desktop\InventarioFerreteria\docs\
```

Los documentos principales ya est�n creados y casi completos.
Solo necesitas:
1. Crear la base de datos (Paso 1, 2, 3)
2. Crear los diagramas
3. Tomar capturas
4. Hacer el informe final

**�Mucho �xito, Anahy! Tienes todo lo necesario para hacer un excelente trabajo. ??**

---

**Creado por:** GitHub Copilot  
**Fecha:** 26 de Octubre, 2025  
**Para:** Anahy Herrera - Responsable de Documentaci�n y Base de Datos
