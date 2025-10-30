"""
Estilos y configuración de colores para el sistema
Inspirado en diseño corporativo moderno
"""

# Paleta de colores principal
COLORS = {
    # Colores primarios
    'primary': '#FFC107',           # Amarillo corporativo
    'primary_dark': '#FFA000',      # Amarillo oscuro
    'primary_light': '#FFECB3',     # Amarillo claro
    
    # Colores secundarios
    'secondary': '#1A237E',         # Azul oscuro
    'secondary_dark': '#0D1642',    # Azul más oscuro
    'secondary_light': '#303F9F',   # Azul medio
    
    # Colores de fondo
    'bg_main': '#F5F5F5',           # Gris muy claro
    'bg_card': '#FFFFFF',           # Blanco
    'bg_sidebar': '#1A237E',        # Azul oscuro
    'bg_header': '#FFFFFF',         # Blanco
    
    # Colores de texto
    'text_primary': '#212121',      # Negro suave
    'text_secondary': '#757575',    # Gris medio
    'text_light': '#FFFFFF',        # Blanco
    'text_disabled': '#BDBDBD',     # Gris claro
    
    # Colores de estado
    'success': '#4CAF50',           # Verde
    'warning': '#FF9800',           # Naranja
    'error': '#F44336',             # Rojo
    'info': '#2196F3',              # Azul
    
    # Bordes y sombras
    'border': '#E0E0E0',            # Gris borde
    'shadow': '#00000020',          # Sombra suave
    
    # Hover y active
    'hover': '#FFF8E1',             # Amarillo muy claro
    'active': '#FFD54F',            # Amarillo activo
}

# Configuración de fuentes
FONTS = {
    'title': ('Segoe UI', 24, 'bold'),
    'subtitle': ('Segoe UI', 18, 'bold'),
    'heading': ('Segoe UI', 16, 'bold'),
    'body': ('Segoe UI', 12),
    'body_bold': ('Segoe UI', 12, 'bold'),
    'small': ('Segoe UI', 10),
    'button': ('Segoe UI', 11, 'bold'),
}

# Configuración de tamaños
SIZES = {
    'login_window': (450, 600),
    'config_window': (500, 650),
    'main_window': (1400, 800),
    'sidebar_width': 250,
    'header_height': 70,
    'button_height': 45,
    'input_height': 40,
    'corner_radius': 10,
    'card_padding': 20,
}

# Configuración de botones
BUTTON_STYLES = {
    'primary': {
        'fg_color': COLORS['primary'],
        'hover_color': COLORS['primary_dark'],
        'text_color': COLORS['text_primary'],
        'border_width': 0,
        'corner_radius': SIZES['corner_radius'],
        'font': FONTS['button'],
        'height': SIZES['button_height'],
    },
    'secondary': {
        'fg_color': COLORS['secondary'],
        'hover_color': COLORS['secondary_light'],
        'text_color': COLORS['text_light'],
        'border_width': 0,
        'corner_radius': SIZES['corner_radius'],
        'font': FONTS['button'],
        'height': SIZES['button_height'],
    },
    'outline': {
        'fg_color': 'transparent',
        'hover_color': COLORS['hover'],
        'text_color': COLORS['secondary'],
        'border_width': 2,
        'border_color': COLORS['secondary'],
        'corner_radius': SIZES['corner_radius'],
        'font': FONTS['button'],
        'height': SIZES['button_height'],
    },
    'text': {
        'fg_color': 'transparent',
        'hover_color': COLORS['hover'],
        'text_color': COLORS['secondary'],
        'border_width': 0,
        'corner_radius': SIZES['corner_radius'],
        'font': FONTS['body'],
        'height': 35,
    },
}

# Configuración de inputs
INPUT_STYLES = {
    'fg_color': COLORS['bg_card'],
    'border_width': 1,
    'border_color': COLORS['border'],
    'corner_radius': SIZES['corner_radius'],
    'height': SIZES['input_height'],
    'font': FONTS['body'],
    'text_color': COLORS['text_primary'],
}

# Configuración de frames
FRAME_STYLES = {
    'card': {
        'fg_color': COLORS['bg_card'],
        'corner_radius': SIZES['corner_radius'],
        'border_width': 1,
        'border_color': COLORS['border'],
    },
    'sidebar': {
        'fg_color': COLORS['bg_sidebar'],
        'corner_radius': 0,
        'border_width': 0,
    },
    'header': {
        'fg_color': COLORS['bg_header'],
        'corner_radius': 0,
        'border_width': 0,
    },
}
