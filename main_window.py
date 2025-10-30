
import customtkinter as ctk
from tkinter import messagebox

class MainWindow(ctk.CTk):
    def __init__(self, soap_client=None):
        super().__init__()
        self.soap_client = soap_client
        self.title("Sistema de Inventario - Ferretería")
        self.state('zoomed')
        ctk.set_appearance_mode("light")
        self.current_module_frame = None
        self.usuario_actual = "admin"
        self.colors = {
            'primary': '#1976D2',
            'sidebar_bg': '#0D47A1',
            'bg_main': '#F5F7FA',
            'white': '#FFFFFF',
            'error': '#F44336'
        }
        self.create_ui()

    def create_ui(self):
        self.sidebar = ctk.CTkFrame(self, width=260, fg_color=self.colors['sidebar_bg'], corner_radius=0)
        self.sidebar.pack(side="left", fill="y")
        self.sidebar.pack_propagate(False)

        sidebar_header = ctk.CTkFrame(self.sidebar, fg_color=self.colors['sidebar_bg'], height=80)
        sidebar_header.pack(fill="x")
        sidebar_header.pack_propagate(False)

        logo = ctk.CTkLabel(sidebar_header, text="🏪 FERRETERÍA", font=('Segoe UI', 20, 'bold'), text_color=self.colors['white'])
        logo.pack(pady=25)

        btn_frame = ctk.CTkFrame(self.sidebar, fg_color="transparent")
        btn_frame.pack(fill="both", expand=True, padx=15, pady=10)

        self.menu_buttons = {}
        menus = [
            ("🏠", "Dashboard", self.show_dashboard),
            ("📦", "Artículos", self.show_articulos),
            ("📊", "Inventario", self.show_inventario),
            ("📈", "Reportes", self.show_reportes),
            ("⚙️", "Configuración", self.show_configuracion)
        ]

        for icon, text, cmd in menus:
            btn = ctk.CTkButton(btn_frame, text=f"{icon}  {text}", command=cmd, fg_color="transparent", hover_color=self.colors['primary'], text_color=self.colors['white'], anchor="w", height=48, font=('Segoe UI', 14), corner_radius=8)
            btn.pack(fill="x", pady=4)
            self.menu_buttons[text] = btn

        ctk.CTkFrame(btn_frame, fg_color="transparent").pack(fill="both", expand=True)

        logout = ctk.CTkButton(btn_frame, text="🚪  Cerrar Sesión", command=self.cerrar_sesion, fg_color=self.colors['error'], height=45, font=('Segoe UI', 13, 'bold'), corner_radius=8)
        logout.pack(fill="x", pady=(10, 0))

        user_frame = ctk.CTkFrame(self.sidebar, fg_color="#0A3A8C", height=60)
        user_frame.pack(side="bottom", fill="x")
        user_frame.pack_propagate(False)
        ctk.CTkLabel(user_frame, text=f"👤 {self.usuario_actual}", font=('Segoe UI', 12), text_color=self.colors['white']).pack(pady=20)

        self.main_area = ctk.CTkFrame(self, fg_color=self.colors['bg_main'], corner_radius=0)
        self.main_area.pack(side="left", fill="both", expand=True)

        self.show_dashboard()

    def clear_main_area(self):
        if self.current_module_frame:
            self.current_module_frame.destroy()
            self.current_module_frame = None

    def highlight_button(self, name):
        for text, btn in self.menu_buttons.items():
            if text == name:
                btn.configure(fg_color=self.colors['primary'], font=('Segoe UI', 14, 'bold'))
            else:
                btn.configure(fg_color="transparent", font=('Segoe UI', 14))

    def show_dashboard(self):
        self.clear_main_area()
        self.highlight_button("Dashboard")
        try:
            # Mostrar loading mientras carga
            loading_frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            loading_frame.pack(fill='both', expand=True)
            loading_label = ctk.CTkLabel(loading_frame, text="⏳ Cargando Dashboard...", 
                                       font=('Segoe UI', 16), text_color=self.colors['primary'])
            loading_label.pack(expand=True)
            
            # Actualizar la interfaz
            self.update()
            
            # Importar y crear el módulo
            from dashboard_module import DashboardModule
            loading_frame.destroy()
            self.current_module_frame = DashboardModule(self.main_area, self.soap_client)
        except Exception as e:
            print(f"Error cargando dashboard: {e}")
            # Fallback: simple label
            if 'loading_frame' in locals():
                loading_frame.destroy()
            frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            ctk.CTkLabel(frame, text=f"Error cargando Dashboard: {str(e)}", 
                        font=('Segoe UI', 16), text_color=self.colors['error']).pack(padx=20, pady=20)
            frame.pack(fill='both', expand=True)
            self.current_module_frame = frame

    def show_articulos(self):
        self.clear_main_area()
        self.highlight_button("Artículos")
        try:
            # Mostrar loading
            loading_frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            loading_frame.pack(fill='both', expand=True)
            loading_label = ctk.CTkLabel(loading_frame, text="⏳ Cargando Artículos...", 
                                       font=('Segoe UI', 16), text_color=self.colors['primary'])
            loading_label.pack(expand=True)
            self.update()
            
            from articulos_module import ArticulosModule
            loading_frame.destroy()
            self.current_module_frame = ArticulosModule(self.main_area, self.soap_client)
        except Exception as e:
            print(f"Error cargando artículos: {e}")
            if 'loading_frame' in locals():
                loading_frame.destroy()
            frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            ctk.CTkLabel(frame, text=f"Error cargando Artículos: {str(e)}", 
                        font=('Segoe UI', 16), text_color=self.colors['error']).pack(padx=20, pady=20)
            frame.pack(fill='both', expand=True)
            self.current_module_frame = frame

    def show_inventario(self):
        self.clear_main_area()
        self.highlight_button("Inventario")
        try:
            # Mostrar loading
            loading_frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            loading_frame.pack(fill='both', expand=True)
            loading_label = ctk.CTkLabel(loading_frame, text="⏳ Cargando Inventario...", 
                                       font=('Segoe UI', 16), text_color=self.colors['primary'])
            loading_label.pack(expand=True)
            self.update()
            
            from inventario_module import InventarioModule
            loading_frame.destroy()
            self.current_module_frame = InventarioModule(self.main_area, self.soap_client)
        except Exception as e:
            print(f"Error cargando inventario: {e}")
            if 'loading_frame' in locals():
                loading_frame.destroy()
            frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            ctk.CTkLabel(frame, text=f"Error cargando Inventario: {str(e)}", 
                        font=('Segoe UI', 16), text_color=self.colors['error']).pack(padx=20, pady=20)
            frame.pack(fill='both', expand=True)
            self.current_module_frame = frame

    def show_reportes(self):
        self.clear_main_area()
        self.highlight_button("Reportes")
        try:
            from reportes_module import ReportesModule
            self.current_module_frame = ReportesModule(self.main_area, self.soap_client)
        except Exception:
            frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            ctk.CTkLabel(frame, text="Reportes", font=('Segoe UI', 24, 'bold')).pack(padx=20, pady=20)
            frame.pack(fill='both', expand=True)
            self.current_module_frame = frame

    def show_configuracion(self):
        self.clear_main_area()
        self.highlight_button("Configuración")
        try:
            from configuracion_module import ConfiguracionModule
            self.current_module_frame = ConfiguracionModule(self.main_area, self.soap_client)
        except Exception:
            frame = ctk.CTkFrame(self.main_area, fg_color=self.colors['bg_main'])
            ctk.CTkLabel(frame, text="Configuración", font=('Segoe UI', 24, 'bold')).pack(padx=20, pady=20)
            frame.pack(fill='both', expand=True)
            self.current_module_frame = frame

    def cerrar_sesion(self):
        if messagebox.askyesno("Cerrar Sesión", "¿Estás seguro que deseas cerrar sesión?"):
            self.destroy()
            from login_window import LoginWindow
            login = LoginWindow()
            login.mainloop()
