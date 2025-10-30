# Sistema de Inventario - Ferretería

Sistema de gestión de inventario desarrollado en Python con interfaz CustomTkinter y backend SOAP en C#.

## 📁 Estructura del Proyecto

```
cliente-python/
├── dist/
│   └── InventarioFerreteria.exe    # Ejecutable final
├── articulos_module.py              # Módulo CRUD de artículos
├── config.py                        # Configuración del sistema
├── configuracion_module.py          # Módulo de configuración UI
├── connection_config.json           # Configuración de conexión
├── dashboard_module.py              # Módulo del dashboard principal
├── inventario_module.py             # Módulo de inventario
├── login_window.py                  # Ventana de login
├── main.py                          # Punto de entrada principal
├── main_window.py                   # Ventana principal
├── reportes_module.py               # Módulo de reportes
├── soap_client.py                   # Cliente SOAP optimizado
└── styles.py                        # Estilos y temas UI
```

## 🚀 Características

- ✅ **Autenticación JWT** con servicio SOAP C#
- ✅ **Dashboard en tiempo real** con estadísticas
- ✅ **CRUD completo de artículos**
  - Crear nuevos artículos con código automático
  - Editar artículos existentes
  - Eliminar artículos
  - Listar todos los artículos
- ✅ **Interfaz moderna** con CustomTkinter
- ✅ **Tema claro**: Azul y Blanco
- ✅ **Validación de formularios**
- ✅ **Mensajes de confirmación**

## 🔧 Requisitos

### Servidor (C#)
- .NET 6.0+
- PostgreSQL 15+
- Servicio SOAP ejecutándose en `http://localhost:5000`

### Cliente (Python)
El ejecutable ya incluye todas las dependencias necesarias.

## 📝 Credenciales por Defecto

El sistema utiliza las siguientes credenciales configuradas en la base de datos:

- **Administrador**: `admin` / `Admin123!`
- **Usuario normal**: `usuario` / `Usuario123!`

## 🎯 Uso del Ejecutable

1. **Asegúrate de que el servicio SOAP C# esté ejecutándose**
   ```
   http://localhost:5000/InventarioService.asmx
   ```

2. **Ejecuta el archivo**
   ```
   dist/InventarioFerreteria.exe
   ```

3. **El sistema se autentica automáticamente** usando las credenciales por defecto

4. **Navega por los módulos:**
   - 🏠 **Dashboard**: Vista general con estadísticas
   - 📦 **Artículos**: Gestión CRUD completa
   - 📊 **Inventario**: Vista de stock
   - 📋 **Reportes**: Generación de reportes
   - ⚙️ **Configuración**: Configuración del sistema

## 🔐 Autenticación

El cliente Python implementa autenticación automática:
- Al iniciar, intenta autenticarse con `admin/Admin123!`
- Si falla, intenta con `usuario/Usuario123!`
- Obtiene un token JWT válido del servicio
- El token se usa en todas las operaciones CRUD

## 📡 Operaciones SOAP

Todos los métodos CRUD utilizan el token JWT:

- `ObtenerTodosArticulos(token)` - Listar artículos
- `InsertarArticulo(token, ...)` - Crear artículo
- `ActualizarArticulo(token, ...)` - Actualizar artículo
- `EliminarArticulo(token, id)` - Eliminar artículo

## 🛠️ Desarrollo

### Reconstruir el ejecutable

```bash
cd cliente-python
pyinstaller --onefile --windowed --name "InventarioFerreteria" --add-data "connection_config.json;." main.py
```

### Estructura de archivos Python

- **main.py**: Punto de entrada, configura el tema
- **login_window.py**: Ventana de login (autenticación automática)
- **main_window.py**: Ventana principal con navegación
- **soap_client.py**: Cliente SOAP optimizado con gestión de tokens
- **articulos_module.py**: CRUD de artículos con validación
- **dashboard_module.py**: Dashboard con estadísticas reales
- **styles.py**: Constantes de estilo y temas

## 📋 Notas Técnicas

### Mapeo de Códigos a IDs

El sistema mantiene un mapeo de códigos conocidos a IDs de base de datos para optimizar las operaciones:

```python
codigo_a_id = {
    "MART-001": 1,
    "TALAD-001": 2,
    "PINT-001": 3,
    # ...
}
```

Si un código no está en el mapeo, el sistema hace búsqueda dinámica.

### Gestión de Tokens

El cliente verifica automáticamente el token antes de cada operación:
- Si el token no existe, intenta autenticación automática
- Si el token expira, re-autentica automáticamente
- Muestra mensajes de error claros si falla la autenticación

## ✨ Mejoras Implementadas

- ✅ Código limpio y optimizado
- ✅ Eliminación de código redundante
- ✅ Mensajes de log mejorados con símbolos (✓, ✗, ⚠)
- ✅ Gestión automática de autenticación
- ✅ Validación robusta de datos
- ✅ Manejo de errores completo

## 📞 Soporte

Para problemas o preguntas:
1. Verifica que el servicio SOAP esté ejecutándose
2. Verifica la conexión a la base de datos PostgreSQL
3. Revisa las credenciales en la base de datos

---

**Versión**: 1.0.0  
**Fecha**: Octubre 2025  
**Estado**: ✅ Producción
