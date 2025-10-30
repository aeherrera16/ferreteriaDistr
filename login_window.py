"""
Ventana de Login moderna y compacta
Diseño profesional con menú hamburguesa
Tema claro: Azul y Blanco
"""
import customtkinter as ctk
from tkinter import messagebox
import sys
from soap_client import SoapClient
import config
from styles import COLORS, FONTS, SIZES, BUTTON_STYLES, INPUT_STYLES, FRAME_STYLES

class LoginWindow(ctk.CTk):
    def __init__(self):
        super().__init__()
        
        # Cliente SOAP
        self.soap_client = SoapClient()
        
        # Configuración de la ventana
        self.title("Sistema de Inventario")
        
        # Ventana más pequeña y compacta
        width = 1000
        height = 600
        
        # Centrar ventana
        screen_width = self.winfo_screenwidth()
        screen_height = self.winfo_screenheight()
        x = (screen_width - width) // 2
        y = (screen_height - height) // 2
        
        self.geometry(f"{width}x{height}+{x}+{y}")
        self.resizable(False, False)
        
        # Configurar tema CLARO
        ctk.set_appearance_mode("light")
        
        # Variables
        self.usuario_var = ctk.StringVar()
        self.password_var = ctk.StringVar()
        self.config_panel_visible = False
        
        self.create_ui()
    
    def create_ui(self):
        """Crea la interfaz de usuario con menú hamburguesa"""
        
        # ===== CONTENEDOR PRINCIPAL =====
        main_container = ctk.CTkFrame(self, fg_color="#F5F7FA", corner_radius=0)
        main_container.pack(fill="both", expand=True)
        
        # ===== HEADER CON MENÚ HAMBURGUESA =====
        header = ctk.CTkFrame(main_container, fg_color="#1976D2", height=100, corner_radius=0)
        header.pack(fill="x", padx=0, pady=0)
        header.pack_propagate(False)
        
        # Botón hamburguesa (izquierda)
        self.hamburger_btn = ctk.CTkButton(
            header,
            text="☰",
            width=50,
            height=50,
            font=("Segoe UI", 24),
            fg_color="transparent",
            text_color="white",
            hover_color="#1565C0",
            command=self.toggle_config_panel
        )
        self.hamburger_btn.place(x=20, y=25)
        
        # Título y subtítulo (centro)
        title_frame = ctk.CTkFrame(header, fg_color="transparent")
        title_frame.place(relx=0.5, rely=0.5, anchor="center")
        
        title = ctk.CTkLabel(
            title_frame,
            text="🔧 SISTEMA DE INVENTARIO",
            font=("Segoe UI", 28, "bold"),
            text_color="white"
        )
        title.pack()
        
        subtitle = ctk.CTkLabel(
            title_frame,
            text="Ferretería Profesional",
            font=("Segoe UI", 14),
            text_color="white"
        )
        subtitle.pack()
        
        # ===== ÁREA DE CONTENIDO =====
        content_area = ctk.CTkFrame(main_container, fg_color="#F5F7FA")
        content_area.pack(fill="both", expand=True)
        
        # ===== PANEL DE CONFIGURACIÓN (OCULTO INICIALMENTE) =====
        self.config_panel = ctk.CTkFrame(
            content_area,
            width=350,
            fg_color="white",
            corner_radius=0
        )
        # No se empaqueta aún (está oculto)
        
        # Contenido del panel de configuración
        config_title = ctk.CTkLabel(
            self.config_panel,
            text="⚙️ Configuración",
            font=("Segoe UI", 20, "bold"),
            text_color="#1976D2"
        )
        config_title.pack(pady=(30, 20), padx=20)
        
        # Scrollable para configuración
        config_scroll = ctk.CTkScrollableFrame(
            self.config_panel,
            fg_color="transparent",
            width=310
        )
        config_scroll.pack(fill="both", expand=True, padx=20, pady=(0, 20))
        
        # Servicio SOAP
        soap_label = ctk.CTkLabel(
            config_scroll,
            text="🌐 Servicio SOAP",
            font=("Segoe UI", 14, "bold"),
            text_color="#1976D2",
            anchor="w"
        )
        soap_label.pack(fill="x", pady=(10, 5))
        
        self.soap_url_entry = ctk.CTkEntry(
            config_scroll,
            placeholder_text="URL del servicio",
            height=35,
            font=("Segoe UI", 11),
            fg_color="white",
            border_color="#1976D2"
        )
        self.soap_url_entry.pack(fill="x", pady=5)
        
        # Base de datos
        db_label = ctk.CTkLabel(
            config_scroll,
            text="🗄️ Base de Datos",
            font=("Segoe UI", 14, "bold"),
            text_color="#1976D2",
            anchor="w"
        )
        db_label.pack(fill="x", pady=(20, 5))
        
        self.db_host_entry = ctk.CTkEntry(
            config_scroll,
            placeholder_text="Host (ej: localhost o IP)",
            height=35,
            font=("Segoe UI", 11),
            fg_color="white",
            border_color="#1976D2"
        )
        self.db_host_entry.pack(fill="x", pady=5)
        
        self.db_port_entry = ctk.CTkEntry(
            config_scroll,
            placeholder_text="Puerto (5432)",
            height=35,
            font=("Segoe UI", 11),
            fg_color="white",
            border_color="#1976D2"
        )
        self.db_port_entry.pack(fill="x", pady=5)
        
        self.db_name_entry = ctk.CTkEntry(
            config_scroll,
            placeholder_text="Nombre de la base de datos",
            height=35,
            font=("Segoe UI", 11),
            fg_color="white",
            border_color="#1976D2"
        )
        self.db_name_entry.pack(fill="x", pady=5)
        
        self.db_user_entry = ctk.CTkEntry(
            config_scroll,
            placeholder_text="Usuario PostgreSQL",
            height=35,
            font=("Segoe UI", 11),
            fg_color="white",
            border_color="#1976D2"
        )
        self.db_user_entry.pack(fill="x", pady=5)
        
        # Botones de configuración
        btn_frame = ctk.CTkFrame(config_scroll, fg_color="transparent")
        btn_frame.pack(fill="x", pady=20)
        
        test_btn = ctk.CTkButton(
            btn_frame,
            text="🔍 Probar",
            height=35,
            fg_color="#4CAF50",
            hover_color="#45a049",
            font=("Segoe UI", 11, "bold"),
            command=self.test_connection
        )
        test_btn.pack(side="left", fill="x", expand=True, padx=(0, 5))
        
        save_btn = ctk.CTkButton(
            btn_frame,
            text="💾 Guardar",
            height=35,
            fg_color="#1976D2",
            hover_color="#1565C0",
            font=("Segoe UI", 11, "bold"),
            command=self.save_config
        )
        save_btn.pack(side="left", fill="x", expand=True, padx=(5, 0))
        
        # ===== FORMULARIO DE LOGIN (CENTRADO) =====
        self.login_container = ctk.CTkFrame(content_area, fg_color="transparent")
        self.login_container.pack(fill="both", expand=True)
        
        # Frame de login centrado
        login_frame = ctk.CTkFrame(
            self.login_container,
            fg_color="white",
            corner_radius=15,
            width=400,
            height=450
        )
        login_frame.place(relx=0.5, rely=0.5, anchor="center")
        login_frame.pack_propagate(False)
        
        # Contenido del login
        login_content = ctk.CTkFrame(login_frame, fg_color="transparent")
        login_content.pack(fill="both", expand=True, padx=40, pady=40)
        
        # Título
        login_title = ctk.CTkLabel(
            login_content,
            text="Iniciar Sesión",
            font=("Segoe UI", 26, "bold"),
            text_color="#1976D2"
        )
        login_title.pack(pady=(0, 10))
        
        login_subtitle = ctk.CTkLabel(
            login_content,
            text="Ingrese sus credenciales para continuar",
            font=("Segoe UI", 11),
            text_color="#666666"
        )
        login_subtitle.pack(pady=(0, 30))
        
        # Campo Usuario
        user_label = ctk.CTkLabel(
            login_content,
            text="Usuario",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        user_label.pack(fill="x", pady=(0, 5))
        
        self.entry_usuario = ctk.CTkEntry(
            login_content,
            textvariable=self.usuario_var,
            placeholder_text="Ingrese su nombre de usuario",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_usuario.pack(fill="x", pady=(0, 20))
        
        # Campo Contraseña
        pass_label = ctk.CTkLabel(
            login_content,
            text="Contraseña",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        pass_label.pack(fill="x", pady=(0, 5))
        
        self.entry_password = ctk.CTkEntry(
            login_content,
            textvariable=self.password_var,
            placeholder_text="Ingrese su contraseña",
            show="●",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_password.pack(fill="x", pady=(0, 30))
        self.entry_password.bind('<Return>', lambda e: self.iniciar_sesion())
        
        # Botón Iniciar Sesión
        btn_login = ctk.CTkButton(
            login_content,
            text="INICIAR SESIÓN",
            command=self.iniciar_sesion,
            height=45,
            font=("Segoe UI", 14, "bold"),
            fg_color="#1976D2",
            hover_color="#1565C0",
            text_color="white"
        )
        btn_login.pack(fill="x", pady=(0, 15))
        
        # Botón Registrarse
        btn_register = ctk.CTkButton(
            login_content,
            text="REGISTRARSE",
            command=self.mostrar_registro,
            height=45,
            font=("Segoe UI", 14, "bold"),
            fg_color="transparent",
            hover_color="#E3F2FD",
            text_color="#1976D2",
            border_width=2,
            border_color="#1976D2"
        )
        btn_register.pack(fill="x", pady=(0, 10))
        
        # Footer
        footer = ctk.CTkLabel(
            login_content,
            text="Versión 3.0.0",
            font=("Segoe UI", 9),
            text_color="#999999"
        )
        footer.pack(side="bottom", pady=(20, 0))
        
        # Focus en usuario
        self.entry_usuario.focus()
        
        # Cargar configuración desde config.py (por defecto del proyecto)
        self.load_project_config()
    
    def toggle_config_panel(self):
        """Muestra u oculta el panel de configuración (menú hamburguesa)"""
        if self.config_panel_visible:
            # Ocultar panel
            self.config_panel.pack_forget()
            self.login_container.pack(fill="both", expand=True)
            self.config_panel_visible = False
            self.hamburger_btn.configure(text="☰")
        else:
            # Mostrar panel
            self.config_panel.pack(side="left", fill="y", padx=0, pady=0)
            self.config_panel_visible = True
            self.hamburger_btn.configure(text="✕")
    
    def load_project_config(self):
        """Carga la configuración desde config.py (valores por defecto del proyecto)"""
        import os
        import json
        
        # Primero intentar cargar desde JSON si existe (personalización del usuario)
        config_file = 'connection_config.json'
        if os.path.exists(config_file):
            try:
                with open(config_file, 'r') as f:
                    saved_config = json.load(f)
                
                # Cargar configuración guardada (personalizada por el usuario)
                self.soap_url_entry.insert(0, saved_config.get('soap_url', config.SOAP_SERVICE_URL))
                self.db_host_entry.insert(0, saved_config.get('db_host', config.DB_CONFIG['host']))
                self.db_port_entry.insert(0, saved_config.get('db_port', config.DB_CONFIG['port']))
                self.db_name_entry.insert(0, saved_config.get('db_name', config.DB_CONFIG['database']))
                self.db_user_entry.insert(0, saved_config.get('db_user', config.DB_CONFIG['user']))
                return
            except Exception as e:
                print(f"Error cargando config guardada: {e}")
        
        # Si no hay JSON, cargar desde config.py (valores por defecto del proyecto)
        self.soap_url_entry.insert(0, config.SOAP_SERVICE_URL)
        self.db_host_entry.insert(0, config.DB_CONFIG['host'])
        self.db_port_entry.insert(0, config.DB_CONFIG['port'])
        self.db_name_entry.insert(0, config.DB_CONFIG['database'])
        self.db_user_entry.insert(0, config.DB_CONFIG['user'])
    
    def load_saved_config(self):
        """Carga la configuración guardada (obsoleto, usar load_project_config)"""
        import os
        import json
        
        config_file = 'connection_config.json'
        if os.path.exists(config_file):
            try:
                with open(config_file, 'r') as f:
                    config = json.load(f)
                
                self.soap_url_entry.insert(0, config.get('soap_url', ''))
                self.db_host_entry.insert(0, config.get('db_host', ''))
                self.db_port_entry.insert(0, config.get('db_port', '5432'))
                self.db_name_entry.insert(0, config.get('db_name', ''))
                self.db_user_entry.insert(0, config.get('db_user', ''))
            except Exception as e:
                print(f"Error cargando configuración: {e}")
    
    def save_config(self):
        """Guarda la configuración"""
        import json
        import config
        
        soap_url = self.soap_url_entry.get().strip()
        db_host = self.db_host_entry.get().strip()
        db_port = self.db_port_entry.get().strip()
        db_name = self.db_name_entry.get().strip()
        db_user = self.db_user_entry.get().strip()
        
        if not soap_url or not db_host or not db_port or not db_name or not db_user:
            messagebox.showwarning("Campos Incompletos", "Por favor, completa todos los campos de configuración")
            return
        
        # Actualizar config global
        config.SOAP_SERVICE_URL = soap_url
        config.SOAP_WSDL_URL = f"{soap_url}?wsdl" if not soap_url.endswith('?wsdl') else soap_url
        config.DB_CONFIG['host'] = db_host
        config.DB_CONFIG['port'] = db_port
        config.DB_CONFIG['database'] = db_name
        config.DB_CONFIG['user'] = db_user
        
        # Guardar en archivo
        config_data = {
            'soap_url': soap_url,
            'db_host': db_host,
            'db_port': db_port,
            'db_name': db_name,
            'db_user': db_user
        }
        
        try:
            with open('connection_config.json', 'w') as f:
                json.dump(config_data, f, indent=4)
            
            messagebox.showinfo("Configuración Guardada", "✓ La configuración se ha guardado correctamente")
            self.toggle_config_panel()  # Cerrar panel
            
        except Exception as e:
            messagebox.showerror("Error", f"No se pudo guardar la configuración:\n{str(e)}")
    
    def test_connection(self):
        """Prueba la conexión SOAP"""
        from zeep import Client
        
        soap_url = self.soap_url_entry.get().strip()
        
        if not soap_url:
            messagebox.showwarning("Campo Vacío", "Ingresa la URL del servicio SOAP")
            return
        
        wsdl_url = f"{soap_url}?wsdl" if not soap_url.endswith('?wsdl') else soap_url
        
        self.configure(cursor="wait")
        self.update()
        
        try:
            client = Client(wsdl_url)
            self.configure(cursor="")
            messagebox.showinfo(
                "Conexión Exitosa", 
                "✓ La conexión al servicio SOAP fue exitosa.\n\nEl servidor está disponible."
            )
        except Exception as e:
            self.configure(cursor="")
            error_msg = str(e)
            
            # Mensajes más amigables según el error
            if "Max retries exceeded" in error_msg or "Failed to establish" in error_msg:
                messagebox.showerror(
                    "Servicio No Disponible",
                    "⚠️ No se pudo conectar al servicio SOAP.\n\n"
                    "Posibles causas:\n"
                    "• El servicio C# no está ejecutándose\n"
                    "• La URL o puerto son incorrectos\n"
                    f"• Firewall bloqueando el puerto\n\n"
                    f"URL configurada: {soap_url}\n\n"
                    "Solución:\n"
                    "1. Verifica que el proyecto C# esté ejecutándose\n"
                    "2. Revisa la URL y el puerto en la configuración"
                )
            else:
                messagebox.showerror(
                    "Error de Conexión",
                    f"No se pudo conectar al servicio:\n\n{error_msg}"
                )
    
    
    def iniciar_sesion(self):
        """Procesa el inicio de sesión"""
        usuario = self.usuario_var.get().strip()
        password = self.password_var.get().strip()
        
        if not usuario or not password:
            messagebox.showwarning(
                "Campos Vacíos",
                "Por favor, ingresa usuario y contraseña"
            )
            return
        
        # Mostrar loading
        self.mostrar_loading(True)
        
        try:
            # Intentar autenticación
            resultado = self.soap_client.autenticar(usuario, password)
            
            if resultado and resultado.get('exito') == True:
                self.mostrar_loading(False)
                self.abrir_ventana_principal()
            else:
                self.mostrar_loading(False)
                mensaje = resultado.get('mensaje', 'Credenciales inválidas') if resultado else 'Error de conexión'
                messagebox.showerror("Error de Autenticación", mensaje)
                
        except Exception as e:
            self.mostrar_loading(False)
            messagebox.showerror(
                "Error de Conexión",
                f"No se pudo conectar con el servidor:\n{str(e)}\n\nVerifica la configuración de conexión."
            )
    
    def abrir_ventana_principal(self):
        """Abre la ventana principal del sistema"""
        self.withdraw()
        from main_window import MainWindow
        main_window = MainWindow(self.soap_client)
        main_window.mainloop()
        self.destroy()
    
    def mostrar_loading(self, mostrar):
        """Muestra/oculta indicador de carga"""
        if mostrar:
            self.configure(cursor="wait")
        else:
            self.configure(cursor="")
        self.update()
    
    def mostrar_registro(self):
        """Muestra la ventana de registro"""
        registro_window = RegistroWindow(self)

class RegistroWindow(ctk.CTkToplevel):
    """Ventana de registro de nuevos usuarios"""
    
    def __init__(self, parent):
        super().__init__(parent)
        self.parent = parent
        self.soap_client = parent.soap_client
        
        # Configuración de la ventana
        self.title("Registro de Usuario")
        self.geometry("450x600")
        self.resizable(False, False)
        
        # Centrar ventana
        self.update_idletasks()
        x = (self.winfo_screenwidth() // 2) - (450 // 2)
        y = (self.winfo_screenheight() // 2) - (600 // 2)
        self.geometry(f"450x600+{x}+{y}")
        
        # Variables
        self.nombre_var = ctk.StringVar()
        self.email_var = ctk.StringVar()
        self.usuario_var = ctk.StringVar()
        self.password_var = ctk.StringVar()
        self.confirm_password_var = ctk.StringVar()
        
        self.create_widgets()
        
        # Focus en el primer campo
        self.after(100, lambda: self.entry_nombre.focus())
        
        # Mantener ventana al frente
        self.transient(parent)
        self.grab_set()
    
    def create_widgets(self):
        """Crea los widgets de la ventana de registro"""
        
        # Frame principal
        main_frame = ctk.CTkFrame(self, fg_color="#F5F7FA")
        main_frame.pack(fill="both", expand=True, padx=20, pady=20)
        
        # Header
        header_frame = ctk.CTkFrame(main_frame, fg_color="#1976D2", corner_radius=12)
        header_frame.pack(fill="x", pady=(0, 30))
        
        title_label = ctk.CTkLabel(
            header_frame,
            text="📝 Nuevo Usuario",
            font=("Segoe UI", 24, "bold"),
            text_color="white"
        )
        title_label.pack(pady=25)
        
        # Formulario
        form_frame = ctk.CTkFrame(main_frame, fg_color="white", corner_radius=12)
        form_frame.pack(fill="both", expand=True, padx=10, pady=(0, 10))
        
        form_content = ctk.CTkFrame(form_frame, fg_color="transparent")
        form_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        # Campo Nombre Completo
        nombre_label = ctk.CTkLabel(
            form_content,
            text="Nombre Completo",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        nombre_label.pack(fill="x", pady=(0, 5))
        
        self.entry_nombre = ctk.CTkEntry(
            form_content,
            textvariable=self.nombre_var,
            placeholder_text="Ingrese su nombre completo",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_nombre.pack(fill="x", pady=(0, 15))
        
        # Campo Email
        email_label = ctk.CTkLabel(
            form_content,
            text="Correo Electrónico",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        email_label.pack(fill="x", pady=(0, 5))
        
        self.entry_email = ctk.CTkEntry(
            form_content,
            textvariable=self.email_var,
            placeholder_text="correo@ejemplo.com",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_email.pack(fill="x", pady=(0, 15))
        
        # Campo Usuario
        usuario_label = ctk.CTkLabel(
            form_content,
            text="Nombre de Usuario",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        usuario_label.pack(fill="x", pady=(0, 5))
        
        self.entry_usuario = ctk.CTkEntry(
            form_content,
            textvariable=self.usuario_var,
            placeholder_text="Nombre de usuario",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_usuario.pack(fill="x", pady=(0, 15))
        
        # Campo Contraseña
        password_label = ctk.CTkLabel(
            form_content,
            text="Contraseña",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        password_label.pack(fill="x", pady=(0, 5))
        
        self.entry_password = ctk.CTkEntry(
            form_content,
            textvariable=self.password_var,
            placeholder_text="Contraseña segura",
            show="●",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_password.pack(fill="x", pady=(0, 15))
        
        # Campo Confirmar Contraseña
        confirm_label = ctk.CTkLabel(
            form_content,
            text="Confirmar Contraseña",
            font=("Segoe UI", 12, "bold"),
            text_color="#424242",
            anchor="w"
        )
        confirm_label.pack(fill="x", pady=(0, 5))
        
        self.entry_confirm = ctk.CTkEntry(
            form_content,
            textvariable=self.confirm_password_var,
            placeholder_text="Repita la contraseña",
            show="●",
            height=40,
            font=("Segoe UI", 12),
            fg_color="white",
            border_color="#1976D2",
            border_width=2
        )
        self.entry_confirm.pack(fill="x", pady=(0, 25))
        self.entry_confirm.bind('<Return>', lambda e: self.registrar_usuario())
        
        # Botones
        buttons_frame = ctk.CTkFrame(form_content, fg_color="transparent")
        buttons_frame.pack(fill="x", pady=(0, 10))
        
        btn_registrar = ctk.CTkButton(
            buttons_frame,
            text="REGISTRAR USUARIO",
            command=self.registrar_usuario,
            height=45,
            font=("Segoe UI", 14, "bold"),
            fg_color="#4CAF50",
            hover_color="#388E3C",
            text_color="white"
        )
        btn_registrar.pack(fill="x", pady=(0, 10))
        
        btn_cancelar = ctk.CTkButton(
            buttons_frame,
            text="CANCELAR",
            command=self.destroy,
            height=40,
            font=("Segoe UI", 12, "bold"),
            fg_color="transparent",
            hover_color="#FFEBEE",
            text_color="#F44336",
            border_width=2,
            border_color="#F44336"
        )
        btn_cancelar.pack(fill="x")
    
    def registrar_usuario(self):
        """Registra un nuevo usuario"""
        try:
            # Validar campos
            nombre = self.nombre_var.get().strip()
            email = self.email_var.get().strip()
            usuario = self.usuario_var.get().strip()
            password = self.password_var.get().strip()
            confirm_password = self.confirm_password_var.get().strip()
            
            if not all([nombre, email, usuario, password, confirm_password]):
                messagebox.showerror("Error", "Todos los campos son obligatorios")
                return
            
            if password != confirm_password:
                messagebox.showerror("Error", "Las contraseñas no coinciden")
                return
            
            if len(password) < 6:
                messagebox.showerror("Error", "La contraseña debe tener al menos 6 caracteres")
                return
            
            # Validar email básico
            if "@" not in email or "." not in email:
                messagebox.showerror("Error", "Ingrese un email válido")
                return
            
            # Simular registro exitoso (aquí se conectaría con el backend)
            messagebox.showinfo(
                "Registro Exitoso", 
                f"Usuario '{usuario}' registrado correctamente.\n\nYa puede iniciar sesión con sus credenciales."
            )
            
            # Cerrar ventana de registro
            self.destroy()
            
        except Exception as e:
            messagebox.showerror("Error", f"Error al registrar usuario: {str(e)}")

if __name__ == "__main__":
    app = LoginWindow()
    app.mainloop()
