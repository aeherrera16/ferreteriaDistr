# Configuración del cliente de ferretería
import os

# Configuración del servicio SOAP (puede ser modificada dinámicamente)
SOAP_SERVICE_URL = "http://localhost:5000/InventarioService.asmx"
SOAP_WSDL_URL = f"{SOAP_SERVICE_URL}?wsdl"

# Configuración de la base de datos (informativa - el backend C# maneja la conexión)
DB_CONFIG = {
    "host": "localhost",
    "port": "5432",
    "database": "postgres",
    "user": "postgres"
}

# Configuración de la aplicación
APP_NAME = "Sistema de Inventario - Ferretería"
APP_VERSION = "1.0.0"

# Tema de la aplicación
THEME_MODE = "dark"  # "dark" o "light"
COLOR_THEME = "blue"  # "blue", "green", "dark-blue"

# Directorio de recursos
BASE_DIR = os.path.dirname(os.path.abspath(__file__))
ASSETS_DIR = os.path.join(BASE_DIR, "assets")
ICONS_DIR = os.path.join(ASSETS_DIR, "icons")

# Configuración de la ventana principal
WINDOW_WIDTH = 1400
WINDOW_HEIGHT = 850
MIN_WINDOW_WIDTH = 1200
MIN_WINDOW_HEIGHT = 700

# Auto-actualización
AUTO_REFRESH_INTERVAL = 5000  # milisegundos (5 segundos)
