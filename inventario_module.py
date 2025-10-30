"""
Módulo de Inventario - Diseño Moderno y Responsivo
"""
import customtkinter as ctk

class InventarioModule(ctk.CTkFrame):
    """Módulo de inventario"""
    
    def __init__(self, parent, soap_client=None):
        super().__init__(parent, fg_color="#F5F7FA")
        self.pack(fill="both", expand=True)
        
        self.soap_client = soap_client
        self.create_widgets()
    
    def create_widgets(self):
        """Crea los widgets"""
        # Header
        header = ctk.CTkFrame(self, fg_color="#1976D2", corner_radius=0, height=100)
        header.pack(fill="x")
        header.pack_propagate(False)

        title = ctk.CTkLabel(
            header,
            text="📊 Módulo de Inventario",
            font=('Segoe UI', 32, 'bold'),
            text_color="white"
        )
        title.pack(pady=30, padx=40)

        # Contenido
        content = ctk.CTkFrame(self, fg_color="white", corner_radius=12)
        content.pack(fill="both", expand=True, padx=40, pady=30)

        # Estadísticas básicas (carga desde SOAP cuando sea posible)
        stats_frame = ctk.CTkFrame(content, fg_color="transparent")
        stats_frame.pack(fill="x", pady=(10, 20))

        self.total_articulos_label = ctk.CTkLabel(stats_frame, text="Total Artículos: -", font=('Segoe UI', 18, 'bold'), text_color="#333333")
        self.total_articulos_label.pack(anchor='w', padx=20, pady=(10, 5))

        self.total_stock_label = ctk.CTkLabel(stats_frame, text="Stock Total: -", font=('Segoe UI', 16), text_color="#666666")
        self.total_stock_label.pack(anchor='w', padx=20, pady=(0, 5))

        self.valor_total_label = ctk.CTkLabel(stats_frame, text="Valor Total: -", font=('Segoe UI', 16), text_color="#666666")
        self.valor_total_label.pack(anchor='w', padx=20, pady=(0, 5))

        # Lista rápida de artículos (si hay)
        list_frame = ctk.CTkFrame(content, fg_color="transparent")
        list_frame.pack(fill="both", expand=True, pady=(10,0))

        self.articles_scroll = ctk.CTkScrollableFrame(list_frame, fg_color="white")
        self.articles_scroll.pack(fill="both", expand=True, padx=10, pady=10)

        # Botón refrescar
        btn_refresh = ctk.CTkButton(content, text="� Refrescar", command=self.refresh_data, fg_color="#1976D2", hover_color="#1565C0")
        btn_refresh.pack(side='right', padx=30, pady=20)

        # Cargar datos iniciales
        self.refresh_data()

    def refresh_data(self):
        """Carga/actualiza los datos del inventario usando el soap_client si está disponible"""
        # Limpiar lista
        for w in self.articles_scroll.winfo_children():
            w.destroy()

        total_art = 0
        total_stock = 0
        total_valor = 0.0

        try:
            if self.soap_client:
                res = self.soap_client.obtener_todos_articulos()
                if res and res.get('exito'):
                    articulos = res.get('dato', []) or []
                else:
                    articulos = []
            else:
                articulos = [
                    {"codigo": "A001", "nombre": "Martillo", "categoria": "Herramientas", "stock": 12, "precio": 9.5},
                    {"codigo": "A002", "nombre": "Clavo 2''", "categoria": "Ferretería", "stock": 200, "precio": 0.05},
                ]

            for art in articulos:
                # admitir dicts o objetos
                if not isinstance(art, dict):
                    codigo = getattr(art, 'Codigo', '')
                    nombre = getattr(art, 'Nombre', '')
                    stock = getattr(art, 'Stock', 0)
                    precio = float(getattr(art, 'PrecioVenta', 0))
                else:
                    codigo = art.get('codigo', '-')
                    nombre = art.get('nombre', '-')
                    stock = art.get('stock', 0)
                    precio = float(art.get('precio', 0))

                total_art += 1
                total_stock += int(stock)
                total_valor += float(stock) * float(precio)

                row = ctk.CTkFrame(self.articles_scroll, fg_color="#FFFFFF", height=40)
                row.pack(fill='x', pady=4, padx=6)
                ctk.CTkLabel(row, text=f"{codigo}", width=120, anchor='w').pack(side='left', padx=6)
                ctk.CTkLabel(row, text=f"{nombre}", anchor='w').pack(side='left', padx=6)
                ctk.CTkLabel(row, text=f"Stock: {stock}").pack(side='right', padx=12)

            self.total_articulos_label.configure(text=f"Total Artículos: {total_art}")
            self.total_stock_label.configure(text=f"Stock Total: {total_stock}")
            self.valor_total_label.configure(text=f"Valor Total: ${total_valor:.2f}")

        except Exception as e:
            # Mostrar error y dejar valores por defecto
            self.total_articulos_label.configure(text=f"Total Artículos: -")
            self.total_stock_label.configure(text=f"Stock Total: -")
            self.valor_total_label.configure(text=f"Valor Total: -")
            ctk.CTkLabel(self.articles_scroll, text=f"Error cargando datos: {str(e)}", text_color="#d32f2f").pack(pady=20)
