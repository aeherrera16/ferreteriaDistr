"""
Módulo de Gestión de Artículos - Diseño Moderno y Responsivo
Tema: Azul y Blanco
"""
import customtkinter as ctk
from tkinter import messagebox
from soap_client import SoapClient

class ArticulosModule(ctk.CTkFrame):
    """Módulo de gestión de artículos"""
    
    def __init__(self, parent, soap_client=None):
        super().__init__(parent, fg_color="#F5F7FA")
        self.pack(fill="both", expand=True)
        
        self.soap_client = soap_client
        self.articulos_data = []
        
        # Colores
        self.colors = {
            'primary': '#1976D2',
            'primary_hover': '#1565C0',
            'bg': '#F5F7FA',
            'white': '#FFFFFF',
            'text_dark': '#1a1a1a',
            'text_gray': '#666666',
            'success': '#4CAF50',
            'error': '#F44336',
            'border': '#E0E0E0'
        }
        
        self.create_widgets()
        self.cargar_articulos()
    
    def create_widgets(self):
        """Crea los widgets del módulo"""
        
        # ===== HEADER =====
        header = ctk.CTkFrame(
            self,
            fg_color=self.colors['primary'],
            corner_radius=0,
            height=100
        )
        header.pack(fill="x", padx=0, pady=0)
        header.pack_propagate(False)
        
        header_content = ctk.CTkFrame(header, fg_color="transparent")
        header_content.pack(fill="both", expand=True, padx=40, pady=25)
        
        title = ctk.CTkLabel(
            header_content,
            text="📦 Gestión de Artículos",
            font=('Segoe UI', 32, 'bold'),
            text_color=self.colors['white']
        )
        title.pack(side="left")
        
        # Botones de acción
        btn_frame = ctk.CTkFrame(header_content, fg_color="transparent")
        btn_frame.pack(side="right")
        
        btn_nuevo = ctk.CTkButton(
            btn_frame,
            text="➕ Nuevo Artículo",
            command=self.nuevo_articulo,
            fg_color=self.colors['success'],
            hover_color="#388E3C",
            height=42,
            font=('Segoe UI', 14, 'bold'),
            corner_radius=8
        )
        btn_nuevo.pack(side="left", padx=5)
        
        btn_refresh = ctk.CTkButton(
            btn_frame,
            text="🔄 Refrescar",
            command=self.cargar_articulos,
            fg_color=self.colors['white'],
            hover_color="#E0E0E0",
            text_color=self.colors['primary'],
            height=42,
            font=('Segoe UI', 14, 'bold'),
            corner_radius=8
        )
        btn_refresh.pack(side="left", padx=5)
        
        # ===== CONTENIDO =====
        content = ctk.CTkFrame(self, fg_color="transparent")
        content.pack(fill="both", expand=True, padx=40, pady=30)
        
        # ===== BARRA DE BÚSQUEDA =====
        search_frame = ctk.CTkFrame(content, fg_color=self.colors['white'], corner_radius=10)
        search_frame.pack(fill="x", pady=(0, 20))
        
        search_content = ctk.CTkFrame(search_frame, fg_color="transparent")
        search_content.pack(fill="x", padx=25, pady=20)
        
        search_label = ctk.CTkLabel(
            search_content,
            text="🔍 Buscar:",
            font=('Segoe UI', 14),
            text_color=self.colors['text_dark']
        )
        search_label.pack(side="left", padx=(0, 15))
        
        self.search_entry = ctk.CTkEntry(
            search_content,
            placeholder_text="Buscar por código, nombre o descripción...",
            height=40,
            font=('Segoe UI', 13),
            border_color=self.colors['border'],
            fg_color=self.colors['white']
        )
        self.search_entry.pack(side="left", fill="x", expand=True, padx=(0, 15))
        self.search_entry.bind('<KeyRelease>', lambda e: self.buscar_articulos())
        
        btn_search = ctk.CTkButton(
            search_content,
            text="Buscar",
            command=self.buscar_articulos,
            fg_color=self.colors['primary'],
            hover_color=self.colors['primary_hover'],
            height=40,
            width=120,
            font=('Segoe UI', 13, 'bold'),
            corner_radius=8
        )
        btn_search.pack(side="left")
        
        # ===== TABLA DE ARTÍCULOS =====
        table_frame = ctk.CTkFrame(content, fg_color=self.colors['white'], corner_radius=10)
        table_frame.pack(fill="both", expand=True)
        
        # Header de tabla
        table_header = ctk.CTkFrame(table_frame, fg_color=self.colors['primary'], height=50, corner_radius=0)
        table_header.pack(fill="x")
        table_header.pack_propagate(False)
        
        headers = ["Código", "Nombre", "Categoría", "Stock", "Precio", "Acciones"]
        header_widths = [100, 250, 150, 100, 120, 150]
        
        for i, (header, width) in enumerate(zip(headers, header_widths)):
            label = ctk.CTkLabel(
                table_header,
                text=header,
                font=('Segoe UI', 13, 'bold'),
                text_color=self.colors['white'],
                width=width
            )
            label.pack(side="left", padx=10, pady=12)
        
        # Scroll para datos
        self.table_scroll = ctk.CTkScrollableFrame(
            table_frame,
            fg_color=self.colors['white'],
            scrollbar_button_color=self.colors['primary']
        )
        self.table_scroll.pack(fill="both", expand=True, padx=0, pady=0)
        
        # Mensaje si no hay datos
        self.no_data_label = ctk.CTkLabel(
            self.table_scroll,
            text="📭 No hay artículos registrados\n\nClick en 'Nuevo Artículo' para agregar uno",
            font=('Segoe UI', 16),
            text_color=self.colors['text_gray']
        )
        self.no_data_label.pack(pady=100)
    
    def cargar_articulos(self):
        """Carga los artículos desde el servidor"""
        try:
            if self.soap_client:
                res = self.soap_client.obtener_todos_articulos()
                if res and res.get('exito'):
                    self.articulos_data = res.get('dato', []) or []
                else:
                    self.articulos_data = []
            else:
                # Sin cliente SOAP, mostrar datos de ejemplo (modo offline)
                self.articulos_data = [
                    {"codigo": "A001", "nombre": "Martillo", "categoria": "Herramientas", "stock": 12, "precio": 9.5},
                    {"codigo": "A002", "nombre": "Clavo 2''", "categoria": "Ferretería", "stock": 200, "precio": 0.05},
                ]

            self.mostrar_articulos(self.articulos_data)

        except Exception as e:
            messagebox.showerror(
                "Error",
                f"No se pudieron cargar los artículos:\n{str(e)}"
            )
    
    def mostrar_articulos(self, articulos):
        """Muestra la lista de artículos en la interfaz"""
        # Limpiar tabla
        for widget in self.table_scroll.winfo_children():
            widget.destroy()
        
        if len(articulos) == 0:
            self.no_data_label = ctk.CTkLabel(
                self.table_scroll,
                text="📭 No se encontraron artículos\n\nIntenta con otros términos de búsqueda",
                font=('Segoe UI', 16),
                text_color=self.colors['text_gray']
            )
            self.no_data_label.pack(pady=100)
        else:
            for articulo in articulos:
                if isinstance(articulo, dict):
                    self.create_article_row(articulo)
                else:
                    # Si es un objeto zeep, convertir
                    self.create_article_row({
                        'codigo': getattr(articulo, 'Codigo', ''),
                        'nombre': getattr(articulo, 'Nombre', ''),
                        'categoria': getattr(articulo, 'Categoria', ''),
                        'stock': getattr(articulo, 'Stock', 0),
                        'precio': float(getattr(articulo, 'PrecioVenta', 0))
                    })
    
    def create_article_row(self, articulo):
        """Crea una fila en la tabla de artículos"""
        row = ctk.CTkFrame(
            self.table_scroll,
            fg_color=self.colors['white'],
            height=60,
            corner_radius=0
        )
        row.pack(fill="x", pady=1)
        row.pack_propagate(False)
        
        # Datos del artículo
        data = [
            articulo.get('codigo', '-'),
            articulo.get('nombre', '-'),
            articulo.get('categoria', '-'),
            str(articulo.get('stock', 0)),
            f"${articulo.get('precio', 0):.2f}",
        ]
        
        widths = [100, 250, 150, 100, 120]
        
        for i, (value, width) in enumerate(zip(data, widths)):
            label = ctk.CTkLabel(
                row,
                text=value,
                font=('Segoe UI', 12),
                text_color=self.colors['text_dark'],
                width=width,
                anchor="w"
            )
            label.pack(side="left", padx=10)
        
        # Botones de acciones
        actions_frame = ctk.CTkFrame(row, fg_color="transparent", width=150)
        actions_frame.pack(side="left", padx=10)
        
        btn_edit = ctk.CTkButton(
            actions_frame,
            text="✏️",
            command=lambda: self.editar_articulo(articulo),
            fg_color=self.colors['primary'],
            hover_color=self.colors['primary_hover'],
            width=40,
            height=32,
            corner_radius=6
        )
        btn_edit.pack(side="left", padx=2)
        
        btn_delete = ctk.CTkButton(
            actions_frame,
            text="🗑️",
            command=lambda: self.eliminar_articulo(articulo),
            fg_color=self.colors['error'],
            hover_color="#D32F2F",
            width=40,
            height=32,
            corner_radius=6
        )
        btn_delete.pack(side="left", padx=2)
    
    def buscar_articulos(self):
        """Busca artículos según el texto ingresado"""
        texto = self.search_entry.get().lower().strip()
        
        # Si no hay texto, mostrar todos los artículos
        if not texto:
            self.cargar_articulos()
            return
        
        # Filtrar artículos que coincidan con el texto
        articulos_filtrados = []
        for articulo in self.articulos_data:
            codigo = articulo.get('codigo', '').lower()
            nombre = articulo.get('nombre', '').lower()
            descripcion = articulo.get('descripcion', '').lower()
            categoria = articulo.get('categoria', '').lower()
            
            if (texto in codigo or 
                texto in nombre or 
                texto in descripcion or 
                texto in categoria):
                articulos_filtrados.append(articulo)
        
        # Mostrar resultados filtrados
        self.mostrar_articulos(articulos_filtrados)
    
    def generar_codigo_automatico(self):
        """Genera un código automático para nuevos artículos"""
        try:
            # Obtener todos los artículos para encontrar el último código
            if self.soap_client:
                res = self.soap_client.obtener_todos_articulos()
                if res and res.get('exito'):
                    articulos = res.get('dato', [])
                    
                    # Buscar el número más alto en códigos existentes
                    max_num = 0
                    for articulo in articulos:
                        codigo = articulo.get('codigo', '')
                        # Buscar códigos que empiecen con ART- seguido de números
                        if codigo.startswith('ART-'):
                            try:
                                num = int(codigo.split('-')[1])
                                max_num = max(max_num, num)
                            except:
                                continue
                    
                    # Generar el siguiente código
                    siguiente_num = max_num + 1
                    return f"ART-{siguiente_num:03d}"  # Formato ART-001, ART-002, etc.
            
            # Si no hay conexión o no hay artículos, empezar con ART-001
            return "ART-001"
            
        except Exception as e:
            print(f"Error generando código: {e}")
            return "ART-001"
    
    def nuevo_articulo(self):
        """Abre diálogo para crear nuevo artículo"""
        self.ventana_articulo()
    
    def editar_articulo(self, articulo):
        """Abre diálogo para editar artículo"""
        self.ventana_articulo(articulo)
    
    def eliminar_articulo(self, articulo):
        """Elimina un artículo"""
        if messagebox.askyesno(
            "Confirmar Eliminación",
            f"¿Estás seguro de eliminar el artículo '{articulo.get('nombre', '')}'?"
        ):
            try:
                # Usar el cliente SOAP existente (con token de autenticación)
                if not self.soap_client:
                    messagebox.showerror("Error", "No hay conexión con el servidor")
                    return
                
                resultado = self.soap_client.eliminar_articulo(articulo.get('codigo', ''))
                
                # Manejar respuesta en formato diccionario
                if isinstance(resultado, dict):
                    if resultado.get("exito", False):
                        messagebox.showinfo("Éxito", resultado.get("mensaje", "Artículo eliminado correctamente"))
                        self.cargar_articulos()  # Recargar la lista
                    else:
                        messagebox.showerror("Error", resultado.get("mensaje", "No se pudo eliminar el artículo"))
                elif resultado:  # Para compatibilidad con respuestas booleanas
                    messagebox.showinfo("Éxito", "Artículo eliminado correctamente")
                    self.cargar_articulos()  # Recargar la lista
                else:
                    messagebox.showerror("Error", "No se pudo eliminar el artículo")
            except Exception as e:
                messagebox.showerror("Error", f"Error al eliminar artículo: {str(e)}")
    
    def ventana_articulo(self, articulo=None):
        """Ventana para crear o editar artículo"""
        # Determinar si es edición o creación
        es_edicion = articulo is not None
        titulo = "Editar Artículo" if es_edicion else "Nuevo Artículo"
        
        # Crear ventana modal
        ventana = ctk.CTkToplevel(self.master)
        ventana.title(titulo)
        ventana.geometry("500x600")
        ventana.transient(self.master)
        ventana.grab_set()
        ventana.resizable(False, False)
        
        # Centrar ventana
        ventana.geometry("+%d+%d" % (
            self.master.winfo_rootx() + 200,
            self.master.winfo_rooty() + 100
        ))
        
        # Frame principal
        main_frame = ctk.CTkFrame(ventana)
        main_frame.pack(fill="both", expand=True, padx=20, pady=20)
        
        # Título
        title_label = ctk.CTkLabel(
            main_frame,
            text=titulo,
            font=ctk.CTkFont(size=24, weight="bold"),
            text_color="#1976D2"
        )
        title_label.pack(pady=(0, 20))
        
        # Frame para campos
        campos_frame = ctk.CTkFrame(main_frame)
        campos_frame.pack(fill="both", expand=True, padx=10, pady=10)
        
        # Variables para los campos
        if es_edicion:
            # Para edición, usar los datos existentes
            codigo_inicial = articulo.get('codigo', '')
            nombre_inicial = articulo.get('nombre', '')
            descripcion_inicial = articulo.get('descripcion', '')
            categoria_inicial = articulo.get('categoria', '')
            # Manejar precio con diferentes posibles keys
            precio_inicial = str(articulo.get('precio', articulo.get('precio_venta', '0.00')))
            stock_inicial = str(articulo.get('stock', '0'))
        else:
            # Para nuevo artículo, generar código automático
            codigo_inicial = self.generar_codigo_automatico()
            nombre_inicial = ''
            descripcion_inicial = ''
            categoria_inicial = ''
            precio_inicial = '0.00'
            stock_inicial = '0'
        
        codigo_var = ctk.StringVar(value=codigo_inicial)
        nombre_var = ctk.StringVar(value=nombre_inicial)
        descripcion_var = ctk.StringVar(value=descripcion_inicial)
        categoria_var = ctk.StringVar(value=categoria_inicial)
        precio_var = ctk.StringVar(value=precio_inicial)
        stock_var = ctk.StringVar(value=stock_inicial)
        
        # Campo Código
        codigo_label_text = "Código:" if es_edicion else "Código: (Generado automáticamente)"
        ctk.CTkLabel(campos_frame, text=codigo_label_text, font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(10, 5))
        entry_codigo = ctk.CTkEntry(
            campos_frame,
            textvariable=codigo_var,
            placeholder_text="Código único del artículo" if es_edicion else "Se genera automáticamente",
            height=40
        )
        entry_codigo.pack(fill="x", pady=(0, 5))
        if es_edicion:
            entry_codigo.configure(state="disabled")  # No permitir cambiar código en edición
        else:
            entry_codigo.configure(state="disabled")  # Código automático no editable
        
        # Información adicional para código automático
        if not es_edicion:
            info_label = ctk.CTkLabel(
                campos_frame,
                text="💡 El código se genera automáticamente (ART-001, ART-002, etc.)",
                font=ctk.CTkFont(size=12),
                text_color="#666666"
            )
            info_label.pack(anchor="w", pady=(0, 10))
        else:
            # Espacio para mantener consistencia
            ctk.CTkLabel(campos_frame, text="", height=5).pack(pady=(0, 10))
        
        # Campo Nombre
        ctk.CTkLabel(campos_frame, text="Nombre:", font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(0, 5))
        entry_nombre = ctk.CTkEntry(
            campos_frame,
            textvariable=nombre_var,
            placeholder_text="Nombre del artículo",
            height=40
        )
        entry_nombre.pack(fill="x", pady=(0, 10))
        
        # Campo Descripción
        ctk.CTkLabel(campos_frame, text="Descripción:", font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(0, 5))
        entry_descripcion = ctk.CTkEntry(
            campos_frame,
            textvariable=descripcion_var,
            placeholder_text="Descripción del artículo",
            height=40
        )
        entry_descripcion.pack(fill="x", pady=(0, 10))
        
        # Campo Categoría
        ctk.CTkLabel(campos_frame, text="Categoría:", font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(0, 5))
        entry_categoria = ctk.CTkEntry(
            campos_frame,
            textvariable=categoria_var,
            placeholder_text="Categoría del artículo",
            height=40
        )
        entry_categoria.pack(fill="x", pady=(0, 10))
        
        # Campo Precio
        ctk.CTkLabel(campos_frame, text="Precio:", font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(0, 5))
        entry_precio = ctk.CTkEntry(
            campos_frame,
            textvariable=precio_var,
            placeholder_text="0.00",
            height=40
        )
        entry_precio.pack(fill="x", pady=(0, 10))
        
        # Campo Stock
        ctk.CTkLabel(campos_frame, text="Stock:", font=ctk.CTkFont(weight="bold")).pack(anchor="w", pady=(0, 5))
        entry_stock = ctk.CTkEntry(
            campos_frame,
            textvariable=stock_var,
            placeholder_text="0",
            height=40
        )
        entry_stock.pack(fill="x", pady=(0, 15))
        
        # Frame para botones
        botones_frame = ctk.CTkFrame(main_frame, fg_color="transparent")
        botones_frame.pack(fill="x", pady=10)
        
        def guardar_articulo():
            """Guarda el artículo (crear o actualizar)"""
            # Validar campos obligatorios
            if not codigo_var.get().strip():
                messagebox.showerror("Error", "El código es obligatorio")
                return
            if not nombre_var.get().strip():
                messagebox.showerror("Error", "El nombre es obligatorio")
                entry_nombre.focus()
                return
            
            try:
                # Validar y convertir precio
                precio_texto = precio_var.get().strip()
                if not precio_texto:
                    precio = 0.0
                else:
                    precio = float(precio_texto)
                    if precio < 0:
                        messagebox.showerror("Error", "El precio no puede ser negativo")
                        entry_precio.focus()
                        return
                
                # Validar y convertir stock
                stock_texto = stock_var.get().strip()
                if not stock_texto:
                    stock = 0
                else:
                    stock = int(stock_texto)
                    if stock < 0:
                        messagebox.showerror("Error", "El stock no puede ser negativo")
                        entry_stock.focus()
                        return
                
                # Preparar datos del artículo
                datos_articulo = {
                    'codigo': codigo_var.get().strip(),
                    'nombre': nombre_var.get().strip(),
                    'descripcion': descripcion_var.get().strip(),
                    'categoria_id': 1,  # General por defecto
                    'precio': precio,
                    'precio_compra': precio * 0.6,  # 60% del precio de venta
                    'stock': stock,
                    'stock_minimo': max(1, stock // 10),  # 10% del stock
                    'proveedor_id': 1  # Proveedor por defecto
                }
                
                # Si es edición, agregar el ID del artículo
                if es_edicion:
                    datos_articulo['id'] = articulo.get('id')
                    datos_articulo['Id'] = articulo.get('Id') or articulo.get('id')
                
                # Usar el cliente SOAP existente (con token de autenticación)
                if not self.soap_client:
                    messagebox.showerror("Error", "No hay conexión con el servidor")
                    return
                
                if es_edicion:
                    resultado = self.soap_client.actualizar_articulo(datos_articulo)
                    mensaje_exito = f"Artículo '{datos_articulo['nombre']}' actualizado correctamente"
                else:
                    resultado = self.soap_client.crear_articulo(datos_articulo)
                    mensaje_exito = f"Artículo '{datos_articulo['nombre']}' creado correctamente con código {datos_articulo['codigo']}"
                
                # Manejar respuesta en formato diccionario
                if isinstance(resultado, dict):
                    if resultado.get("exito", False):
                        messagebox.showinfo("Éxito", resultado.get("mensaje", mensaje_exito))
                        ventana.destroy()
                        self.cargar_articulos()  # Recargar la lista
                    else:
                        messagebox.showerror("Error", resultado.get("mensaje", "No se pudo guardar el artículo"))
                elif resultado:  # Para compatibilidad con respuestas booleanas
                    messagebox.showinfo("Éxito", mensaje_exito)
                    ventana.destroy()
                    self.cargar_articulos()  # Recargar la lista
                else:
                    messagebox.showerror("Error", "No se pudo guardar el artículo. Verifique la conexión con el servidor.")
                    
            except ValueError as ve:
                if "precio" in str(ve).lower():
                    messagebox.showerror("Error", "El precio debe ser un número válido")
                    entry_precio.focus()
                elif "stock" in str(ve).lower():
                    messagebox.showerror("Error", "El stock debe ser un número entero válido")
                    entry_stock.focus()
                else:
                    messagebox.showerror("Error", "Precio y stock deben ser números válidos")
            except Exception as e:
                messagebox.showerror("Error", f"Error al guardar artículo: {str(e)}")
        
        def cancelar():
            """Cierra la ventana sin guardar"""
            ventana.destroy()
        
        # Botón Guardar
        btn_guardar = ctk.CTkButton(
            botones_frame,
            text="Guardar",
            command=guardar_articulo,
            fg_color="#4CAF50",
            hover_color="#45a049",
            height=40,
            font=ctk.CTkFont(weight="bold")
        )
        btn_guardar.pack(side="right", padx=(10, 0))
        
        # Botón Cancelar
        btn_cancelar = ctk.CTkButton(
            botones_frame,
            text="Cancelar",
            command=cancelar,
            fg_color="#f44336",
            hover_color="#da190b",
            height=40,
            font=ctk.CTkFont(weight="bold")
        )
        btn_cancelar.pack(side="right")
        
        # Enfocar primer campo
        if not es_edicion:
            entry_codigo.focus()
        else:
            entry_nombre.focus()


if __name__ == "__main__":
    root = ctk.CTk()
    root.title("Artículos Test")
    root.geometry("1200x800")
    ctk.set_appearance_mode("light")
    
    articulos = ArticulosModule(root)
    root.mainloop()
