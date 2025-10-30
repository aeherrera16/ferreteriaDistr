"""
Módulo de Configuración - Diseño Moderno y Responsivo
"""
import customtkinter as ctk
from tkinter import messagebox
import json
import os
from datetime import datetime
import config

class ConfiguracionModule(ctk.CTkFrame):
    """Módulo de configuración del sistema"""
    
    def __init__(self, parent, soap_client=None):
        super().__init__(parent, fg_color="#F5F7FA")
        self.pack(fill="both", expand=True)
        
        self.soap_client = soap_client
        self.colors = {
            'primary': '#1976D2',
            'bg': '#F5F7FA',
            'white': '#FFFFFF',
            'text_dark': '#1a1a1a',
            'text_gray': '#666666',
            'success': '#4CAF50',
            'warning': '#FF9800',
            'error': '#F44336',
            'info': '#2196F3'
        }
        
        # Variables de configuración
        self.soap_url_var = ctk.StringVar()
        self.db_host_var = ctk.StringVar()
        self.db_port_var = ctk.StringVar()
        self.db_name_var = ctk.StringVar()
        self.db_user_var = ctk.StringVar()
        self.tema_var = ctk.StringVar()
        self.idioma_var = ctk.StringVar()
        
        self.create_widgets()
        self.cargar_configuracion()
    
    def create_widgets(self):
        """Crea los widgets del módulo"""
        
        # Header
        header = ctk.CTkFrame(self, fg_color=self.colors['primary'], corner_radius=0, height=100)
        header.pack(fill="x")
        header.pack_propagate(False)
        
        header_content = ctk.CTkFrame(header, fg_color="transparent")
        header_content.pack(fill="both", expand=True, padx=40, pady=25)
        
        title = ctk.CTkLabel(
            header_content,
            text="⚙️ Configuración del Sistema",
            font=('Segoe UI', 32, 'bold'),
            text_color="white"
        )
        title.pack(side="left")
        
        # Botón guardar en header
        btn_guardar_header = ctk.CTkButton(
            header_content,
            text="💾 Guardar Configuración",
            command=self.guardar_configuracion,
            height=45,
            font=('Segoe UI', 14, 'bold'),
            fg_color="white",
            text_color=self.colors['primary'],
            hover_color="#F5F5F5"
        )
        btn_guardar_header.pack(side="right")
        
        # Contenido principal con scroll
        main_scroll = ctk.CTkScrollableFrame(
            self,
            fg_color=self.colors['bg']
        )
        main_scroll.pack(fill="both", expand=True, padx=20, pady=20)
        
        # ===== CONFIGURACIÓN DE CONEXIÓN =====
        conexion_frame = ctk.CTkFrame(main_scroll, fg_color=self.colors['white'], corner_radius=12)
        conexion_frame.pack(fill="x", pady=(0, 20))
        
        conexion_content = ctk.CTkFrame(conexion_frame, fg_color="transparent")
        conexion_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        # Título de sección
        conexion_title = ctk.CTkLabel(
            conexion_content,
            text="🌐 Configuración de Conexión",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        conexion_title.pack(anchor="w", pady=(0, 20))
        
        # URL del servicio SOAP
        soap_label = ctk.CTkLabel(
            conexion_content,
            text="URL del Servicio SOAP:",
            font=('Segoe UI', 12, 'bold'),
            text_color=self.colors['text_dark']
        )
        soap_label.pack(anchor="w", pady=(0, 5))
        
        self.soap_entry = ctk.CTkEntry(
            conexion_content,
            textvariable=self.soap_url_var,
            placeholder_text="http://localhost:5000/InventarioService.asmx",
            height=40,
            font=('Segoe UI', 12),
            fg_color="white",
            border_color=self.colors['primary'],
            border_width=2
        )
        self.soap_entry.pack(fill="x", pady=(0, 15))
        
        # Configuración de Base de Datos
        db_title = ctk.CTkLabel(
            conexion_content,
            text="🗄️ Configuración de Base de Datos:",
            font=('Segoe UI', 16, 'bold'),
            text_color=self.colors['text_dark']
        )
        db_title.pack(anchor="w", pady=(15, 10))
        
        # Grid para campos de BD
        db_grid = ctk.CTkFrame(conexion_content, fg_color="transparent")
        db_grid.pack(fill="x", pady=(0, 15))
        db_grid.columnconfigure((0, 1), weight=1)
        
        # Host
        host_label = ctk.CTkLabel(db_grid, text="Host:", font=('Segoe UI', 12, 'bold'))
        host_label.grid(row=0, column=0, sticky="w", pady=(0, 5))
        
        self.host_entry = ctk.CTkEntry(
            db_grid,
            textvariable=self.db_host_var,
            placeholder_text="localhost",
            height=35,
            font=('Segoe UI', 11)
        )
        self.host_entry.grid(row=1, column=0, sticky="ew", padx=(0, 10), pady=(0, 10))
        
        # Puerto
        port_label = ctk.CTkLabel(db_grid, text="Puerto:", font=('Segoe UI', 12, 'bold'))
        port_label.grid(row=0, column=1, sticky="w", pady=(0, 5))
        
        self.port_entry = ctk.CTkEntry(
            db_grid,
            textvariable=self.db_port_var,
            placeholder_text="5432",
            height=35,
            font=('Segoe UI', 11)
        )
        self.port_entry.grid(row=1, column=1, sticky="ew", pady=(0, 10))
        
        # Nombre de BD
        dbname_label = ctk.CTkLabel(db_grid, text="Base de Datos:", font=('Segoe UI', 12, 'bold'))
        dbname_label.grid(row=2, column=0, sticky="w", pady=(0, 5))
        
        self.dbname_entry = ctk.CTkEntry(
            db_grid,
            textvariable=self.db_name_var,
            placeholder_text="ferreteria",
            height=35,
            font=('Segoe UI', 11)
        )
        self.dbname_entry.grid(row=3, column=0, sticky="ew", padx=(0, 10), pady=(0, 10))
        
        # Usuario
        user_label = ctk.CTkLabel(db_grid, text="Usuario:", font=('Segoe UI', 12, 'bold'))
        user_label.grid(row=2, column=1, sticky="w", pady=(0, 5))
        
        self.user_entry = ctk.CTkEntry(
            db_grid,
            textvariable=self.db_user_var,
            placeholder_text="postgres",
            height=35,
            font=('Segoe UI', 11)
        )
        self.user_entry.grid(row=3, column=1, sticky="ew", pady=(0, 10))
        
        # Botón probar conexión
        btn_probar = ctk.CTkButton(
            conexion_content,
            text="🔍 Probar Conexión",
            command=self.probar_conexion,
            height=40,
            font=('Segoe UI', 12, 'bold'),
            fg_color=self.colors['info'],
            hover_color="#1565C0"
        )
        btn_probar.pack(anchor="w", pady=(0, 10))
        
        # ===== CONFIGURACIÓN DE INTERFAZ =====
        interfaz_frame = ctk.CTkFrame(main_scroll, fg_color=self.colors['white'], corner_radius=12)
        interfaz_frame.pack(fill="x", pady=(0, 20))
        
        interfaz_content = ctk.CTkFrame(interfaz_frame, fg_color="transparent")
        interfaz_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        interfaz_title = ctk.CTkLabel(
            interfaz_content,
            text="🎨 Configuración de Interfaz",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        interfaz_title.pack(anchor="w", pady=(0, 20))
        
        # Tema
        tema_label = ctk.CTkLabel(
            interfaz_content,
            text="Tema de la Aplicación:",
            font=('Segoe UI', 12, 'bold'),
            text_color=self.colors['text_dark']
        )
        tema_label.pack(anchor="w", pady=(0, 5))
        
        self.tema_combo = ctk.CTkOptionMenu(
            interfaz_content,
            variable=self.tema_var,
            values=["light", "dark", "system"],
            height=40,
            font=('Segoe UI', 12),
            fg_color=self.colors['primary'],
            button_color=self.colors['primary'],
            button_hover_color="#1565C0"
        )
        self.tema_combo.pack(fill="x", pady=(0, 15))
        
        # Idioma
        idioma_label = ctk.CTkLabel(
            interfaz_content,
            text="Idioma:",
            font=('Segoe UI', 12, 'bold'),
            text_color=self.colors['text_dark']
        )
        idioma_label.pack(anchor="w", pady=(0, 5))
        
        self.idioma_combo = ctk.CTkOptionMenu(
            interfaz_content,
            variable=self.idioma_var,
            values=["Español", "English", "Português"],
            height=40,
            font=('Segoe UI', 12),
            fg_color=self.colors['primary'],
            button_color=self.colors['primary'],
            button_hover_color="#1565C0"
        )
        self.idioma_combo.pack(fill="x", pady=(0, 15))
        
        # ===== CONFIGURACIÓN AVANZADA =====
        avanzada_frame = ctk.CTkFrame(main_scroll, fg_color=self.colors['white'], corner_radius=12)
        avanzada_frame.pack(fill="x", pady=(0, 20))
        
        avanzada_content = ctk.CTkFrame(avanzada_frame, fg_color="transparent")
        avanzada_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        avanzada_title = ctk.CTkLabel(
            avanzada_content,
            text="🔧 Configuración Avanzada",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        avanzada_title.pack(anchor="w", pady=(0, 20))
        
        # Switches de configuración
        self.auto_backup_var = ctk.BooleanVar(value=True)
        auto_backup_switch = ctk.CTkSwitch(
            avanzada_content,
            text="Respaldo automático de datos",
            variable=self.auto_backup_var,
            font=('Segoe UI', 12),
            progress_color=self.colors['success']
        )
        auto_backup_switch.pack(anchor="w", pady=(0, 10))
        
        self.auto_update_var = ctk.BooleanVar(value=False)
        auto_update_switch = ctk.CTkSwitch(
            avanzada_content,
            text="Actualización automática de datos",
            variable=self.auto_update_var,
            font=('Segoe UI', 12),
            progress_color=self.colors['success']
        )
        auto_update_switch.pack(anchor="w", pady=(0, 10))
        
        self.notificaciones_var = ctk.BooleanVar(value=True)
        notif_switch = ctk.CTkSwitch(
            avanzada_content,
            text="Mostrar notificaciones del sistema",
            variable=self.notificaciones_var,
            font=('Segoe UI', 12),
            progress_color=self.colors['success']
        )
        notif_switch.pack(anchor="w", pady=(0, 10))
        
        # ===== ACCIONES DEL SISTEMA =====
        acciones_frame = ctk.CTkFrame(main_scroll, fg_color=self.colors['white'], corner_radius=12)
        acciones_frame.pack(fill="x")
        
        acciones_content = ctk.CTkFrame(acciones_frame, fg_color="transparent")
        acciones_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        acciones_title = ctk.CTkLabel(
            acciones_content,
            text="🛠️ Acciones del Sistema",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        acciones_title.pack(anchor="w", pady=(0, 20))
        
        # Botones de acciones
        acciones_grid = ctk.CTkFrame(acciones_content, fg_color="transparent")
        acciones_grid.pack(fill="x")
        acciones_grid.columnconfigure((0, 1, 2), weight=1)
        
        btn_resetear = ctk.CTkButton(
            acciones_grid,
            text="🔄 Resetear Configuración",
            command=self.resetear_configuracion,
            height=45,
            font=('Segoe UI', 12, 'bold'),
            fg_color=self.colors['warning'],
            hover_color="#F57C00"
        )
        btn_resetear.grid(row=0, column=0, padx=(0, 10), sticky="ew")
        
        btn_exportar = ctk.CTkButton(
            acciones_grid,
            text="📤 Exportar Configuración",
            command=self.exportar_configuracion,
            height=45,
            font=('Segoe UI', 12, 'bold'),
            fg_color=self.colors['info'],
            hover_color="#1565C0"
        )
        btn_exportar.grid(row=0, column=1, padx=5, sticky="ew")
        
        btn_importar = ctk.CTkButton(
            acciones_grid,
            text="📥 Importar Configuración",
            command=self.importar_configuracion,
            height=45,
            font=('Segoe UI', 12, 'bold'),
            fg_color=self.colors['success'],
            hover_color="#388E3C"
        )
        btn_importar.grid(row=0, column=2, padx=(10, 0), sticky="ew")
    
    def cargar_configuracion(self):
        """Carga la configuración actual"""
        try:
            # Cargar desde config.py
            self.soap_url_var.set(getattr(config, 'SOAP_SERVICE_URL', 'http://localhost:5000/InventarioService.asmx'))
            
            db_config = getattr(config, 'DB_CONFIG', {})
            self.db_host_var.set(db_config.get('host', 'localhost'))
            self.db_port_var.set(str(db_config.get('port', 5432)))
            self.db_name_var.set(db_config.get('database', 'ferreteria'))
            self.db_user_var.set(db_config.get('user', 'postgres'))
            
            self.tema_var.set("light")
            self.idioma_var.set("Español")
            
            # Cargar desde archivo de configuración si existe
            if os.path.exists('connection_config.json'):
                with open('connection_config.json', 'r') as f:
                    saved_config = json.load(f)
                    self.soap_url_var.set(saved_config.get('soap_url', self.soap_url_var.get()))
                    
        except Exception as e:
            print(f"Error cargando configuración: {e}")
    
    def guardar_configuracion(self):
        """Guarda la configuración actual"""
        try:
            # Guardar configuración de conexión
            config_data = {
                'soap_url': self.soap_url_var.get(),
                'db_host': self.db_host_var.get(),
                'db_port': self.db_port_var.get(),
                'db_name': self.db_name_var.get(),
                'db_user': self.db_user_var.get(),
                'tema': self.tema_var.get(),
                'idioma': self.idioma_var.get(),
                'auto_backup': self.auto_backup_var.get(),
                'auto_update': self.auto_update_var.get(),
                'notificaciones': self.notificaciones_var.get()
            }
            
            # Guardar en archivo
            with open('connection_config.json', 'w') as f:
                json.dump(config_data, f, indent=2)
            
            # Actualizar configuración global
            config.SOAP_SERVICE_URL = self.soap_url_var.get()
            config.SOAP_WSDL_URL = self.soap_url_var.get() + "?wsdl"
            
            # Aplicar tema
            if self.tema_var.get() != "system":
                ctk.set_appearance_mode(self.tema_var.get())
            
            messagebox.showinfo(
                "Configuración Guardada",
                "La configuración se ha guardado correctamente.\n\nAlgunos cambios pueden requerir reiniciar la aplicación."
            )
            
        except Exception as e:
            messagebox.showerror("Error", f"Error al guardar configuración: {str(e)}")
    
    def probar_conexion(self):
        """Prueba la conexión con el servidor SOAP"""
        try:
            if not self.soap_client:
                messagebox.showwarning("Advertencia", "Cliente SOAP no disponible")
                return
            
            # Actualizar URL temporalmente para la prueba
            original_url = getattr(config, 'SOAP_SERVICE_URL', '')
            config.SOAP_SERVICE_URL = self.soap_url_var.get()
            config.SOAP_WSDL_URL = self.soap_url_var.get() + "?wsdl"
            
            # Intentar conectar
            if self.soap_client.connect():
                messagebox.showinfo(
                    "Conexión Exitosa",
                    f"✅ Conexión establecida correctamente\n\nServidor: {self.soap_url_var.get()}\nEstado: Activo"
                )
            else:
                messagebox.showerror(
                    "Error de Conexión",
                    f"❌ No se pudo conectar al servidor\n\nVerifica que:\n• El servidor esté funcionando\n• La URL sea correcta\n• No haya problemas de red"
                )
            
            # Restaurar URL original
            config.SOAP_SERVICE_URL = original_url
            
        except Exception as e:
            messagebox.showerror("Error", f"Error al probar conexión: {str(e)}")
    
    def resetear_configuracion(self):
        """Resetea la configuración a valores por defecto"""
        if messagebox.askyesno(
            "Confirmar Reset",
            "¿Está seguro que desea resetear toda la configuración?\n\nEsta acción no se puede deshacer."
        ):
            try:
                # Valores por defecto
                self.soap_url_var.set("http://localhost:5000/InventarioService.asmx")
                self.db_host_var.set("localhost")
                self.db_port_var.set("5432")
                self.db_name_var.set("ferreteria")
                self.db_user_var.set("postgres")
                self.tema_var.set("light")
                self.idioma_var.set("Español")
                self.auto_backup_var.set(True)
                self.auto_update_var.set(False)
                self.notificaciones_var.set(True)
                
                # Eliminar archivo de configuración
                if os.path.exists('connection_config.json'):
                    os.remove('connection_config.json')
                
                messagebox.showinfo("Reset Completo", "Configuración reseteada a valores por defecto")
                
            except Exception as e:
                messagebox.showerror("Error", f"Error al resetear: {str(e)}")
    
    def exportar_configuracion(self):
        """Exporta la configuración actual a un archivo"""
        try:
            from tkinter import filedialog
            filename = filedialog.asksaveasfilename(
                defaultextension=".json",
                filetypes=[("Archivos JSON", "*.json"), ("Todos los archivos", "*.*")],
                title="Exportar configuración"
            )
            
            if filename:
                config_data = {
                    'soap_url': self.soap_url_var.get(),
                    'db_host': self.db_host_var.get(),
                    'db_port': self.db_port_var.get(),
                    'db_name': self.db_name_var.get(),
                    'db_user': self.db_user_var.get(),
                    'tema': self.tema_var.get(),
                    'idioma': self.idioma_var.get(),
                    'auto_backup': self.auto_backup_var.get(),
                    'auto_update': self.auto_update_var.get(),
                    'notificaciones': self.notificaciones_var.get(),
                    'fecha_exportacion': str(datetime.now())
                }
                
                with open(filename, 'w') as f:
                    json.dump(config_data, f, indent=2)
                
                messagebox.showinfo("Éxito", f"Configuración exportada a:\n{filename}")
                
        except Exception as e:
            messagebox.showerror("Error", f"Error al exportar: {str(e)}")
    
    def importar_configuracion(self):
        """Importa configuración desde un archivo"""
        try:
            from tkinter import filedialog
            filename = filedialog.askopenfilename(
                filetypes=[("Archivos JSON", "*.json"), ("Todos los archivos", "*.*")],
                title="Importar configuración"
            )
            
            if filename:
                with open(filename, 'r') as f:
                    config_data = json.load(f)
                
                # Aplicar configuración importada
                self.soap_url_var.set(config_data.get('soap_url', ''))
                self.db_host_var.set(config_data.get('db_host', ''))
                self.db_port_var.set(config_data.get('db_port', ''))
                self.db_name_var.set(config_data.get('db_name', ''))
                self.db_user_var.set(config_data.get('db_user', ''))
                self.tema_var.set(config_data.get('tema', 'light'))
                self.idioma_var.set(config_data.get('idioma', 'Español'))
                self.auto_backup_var.set(config_data.get('auto_backup', True))
                self.auto_update_var.set(config_data.get('auto_update', False))
                self.notificaciones_var.set(config_data.get('notificaciones', True))
                
                messagebox.showinfo(
                    "Importación Exitosa", 
                    f"Configuración importada desde:\n{filename}\n\nRecuerde guardar los cambios."
                )
                
        except Exception as e:
            messagebox.showerror("Error", f"Error al importar: {str(e)}")


if __name__ == "__main__":
    # Prueba del módulo
    root = ctk.CTk()
    root.title("Configuración Test")
    root.geometry("1000x800")
    ctk.set_appearance_mode("light")
    
    config_module = ConfiguracionModule(root)
    root.mainloop()
