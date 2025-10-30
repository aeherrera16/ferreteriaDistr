"""
Punto de entrada principal del sistema - Diseño Moderno
"""
import customtkinter as ctk
from login_window import LoginWindow

def main():
    """Función principal"""
    # Configurar apariencia global
    ctk.set_appearance_mode("light")
    ctk.set_default_color_theme("blue")
    
    # Crear y ejecutar ventana de login
    app = LoginWindow()
    app.mainloop()

if __name__ == "__main__":
    main()
