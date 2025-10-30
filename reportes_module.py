"""
Módulo de Reportes - Diseño Moderno y Responsivo
"""
import customtkinter as ctk
from tkinter import messagebox, filedialog
import os
from datetime import datetime
import json

class ReportesModule(ctk.CTkFrame):
    """Módulo de reportes"""
    
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
        self.create_widgets()
    
    def create_widgets(self):
        """Crea los widgets"""
        # Header
        header = ctk.CTkFrame(self, fg_color=self.colors['primary'], corner_radius=0, height=100)
        header.pack(fill="x")
        header.pack_propagate(False)
        
        header_content = ctk.CTkFrame(header, fg_color="transparent")
        header_content.pack(fill="both", expand=True, padx=40, pady=25)
        
        title = ctk.CTkLabel(
            header_content,
            text="📈 Módulo de Reportes",
            font=('Segoe UI', 32, 'bold'),
            text_color="white"
        )
        title.pack(side="left")
        
        # Fecha actual
        fecha_label = ctk.CTkLabel(
            header_content,
            text=datetime.now().strftime("📅 %d/%m/%Y - %H:%M"),
            font=('Segoe UI', 14),
            text_color="white"
        )
        fecha_label.pack(side="right")
        
        # Contenido principal
        main_content = ctk.CTkScrollableFrame(
            self,
            fg_color=self.colors['bg']
        )
        main_content.pack(fill="both", expand=True, padx=20, pady=20)
        
        # ===== SECCIÓN DE REPORTES DISPONIBLES =====
        reportes_frame = ctk.CTkFrame(main_content, fg_color=self.colors['white'], corner_radius=12)
        reportes_frame.pack(fill="x", pady=(0, 20))
        
        reportes_content = ctk.CTkFrame(reportes_frame, fg_color="transparent")
        reportes_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        reportes_title = ctk.CTkLabel(
            reportes_content,
            text="📋 Reportes Disponibles",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        reportes_title.pack(anchor="w", pady=(0, 20))
        
        # Grid de reportes
        reportes_grid = ctk.CTkFrame(reportes_content, fg_color="transparent")
        reportes_grid.pack(fill="x", pady=(0, 10))
        reportes_grid.columnconfigure((0, 1), weight=1)
        
        # Reporte de Inventario
        self.create_report_card(
            reportes_grid,
            "📦 Reporte de Inventario",
            "Listado completo de artículos con stock y valores",
            self.generar_reporte_inventario,
            0, 0
        )
        
        # Reporte de Stock Bajo
        self.create_report_card(
            reportes_grid,
            "⚠️ Reporte de Stock Bajo",
            "Artículos con stock por debajo del mínimo",
            self.generar_reporte_stock_bajo,
            0, 1
        )
        
        # Reporte por Categorías
        self.create_report_card(
            reportes_grid,
            "🏷️ Reporte por Categorías",
            "Resumen de inventario agrupado por categorías",
            self.generar_reporte_categorias,
            1, 0
        )
        
        # Reporte de Valores
        self.create_report_card(
            reportes_grid,
            "💰 Reporte de Valores",
            "Análisis de valores totales del inventario",
            self.generar_reporte_valores,
            1, 1
        )
        
        # ===== SECCIÓN DE EXPORTACIÓN =====
        export_frame = ctk.CTkFrame(main_content, fg_color=self.colors['white'], corner_radius=12)
        export_frame.pack(fill="x", pady=(0, 20))
        
        export_content = ctk.CTkFrame(export_frame, fg_color="transparent")
        export_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        export_title = ctk.CTkLabel(
            export_content,
            text="💾 Opciones de Exportación",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        export_title.pack(anchor="w", pady=(0, 20))
        
        # Botones de exportación
        export_buttons = ctk.CTkFrame(export_content, fg_color="transparent")
        export_buttons.pack(fill="x")
        
        btn_export_txt = ctk.CTkButton(
            export_buttons,
            text="📄 Exportar a TXT",
            command=self.exportar_txt,
            height=45,
            font=('Segoe UI', 14, 'bold'),
            fg_color=self.colors['info'],
            hover_color="#1565C0"
        )
        btn_export_txt.pack(side="left", padx=(0, 15))
        
        btn_export_csv = ctk.CTkButton(
            export_buttons,
            text="📊 Exportar a CSV",
            command=self.exportar_csv,
            height=45,
            font=('Segoe UI', 14, 'bold'),
            fg_color=self.colors['success'],
            hover_color="#388E3C"
        )
        btn_export_csv.pack(side="left", padx=(0, 15))
        
        btn_export_json = ctk.CTkButton(
            export_buttons,
            text="🔧 Exportar a JSON",
            command=self.exportar_json,
            height=45,
            font=('Segoe UI', 14, 'bold'),
            fg_color=self.colors['warning'],
            hover_color="#F57C00"
        )
        btn_export_json.pack(side="left")
        
        # ===== SECCIÓN DE ESTADÍSTICAS RÁPIDAS =====
        stats_frame = ctk.CTkFrame(main_content, fg_color=self.colors['white'], corner_radius=12)
        stats_frame.pack(fill="x")
        
        stats_content = ctk.CTkFrame(stats_frame, fg_color="transparent")
        stats_content.pack(fill="both", expand=True, padx=30, pady=30)
        
        stats_title = ctk.CTkLabel(
            stats_content,
            text="📊 Estadísticas Rápidas",
            font=('Segoe UI', 20, 'bold'),
            text_color=self.colors['text_dark']
        )
        stats_title.pack(anchor="w", pady=(0, 20))
        
        self.stats_info = ctk.CTkLabel(
            stats_content,
            text="Cargando estadísticas...",
            font=('Segoe UI', 14),
            text_color=self.colors['text_gray'],
            justify="left"
        )
        self.stats_info.pack(anchor="w")
        
        # Cargar estadísticas
        self.cargar_estadisticas()
    
    def create_report_card(self, parent, title, description, command, row, col):
        """Crea una tarjeta de reporte"""
        card = ctk.CTkFrame(
            parent,
            fg_color=self.colors['bg'],
            corner_radius=12,
            border_width=2,
            border_color="#E0E0E0"
        )
        card.grid(row=row, column=col, padx=10, pady=10, sticky="nsew")
        
        card_content = ctk.CTkFrame(card, fg_color="transparent")
        card_content.pack(fill="both", expand=True, padx=20, pady=20)
        
        title_label = ctk.CTkLabel(
            card_content,
            text=title,
            font=('Segoe UI', 16, 'bold'),
            text_color=self.colors['text_dark']
        )
        title_label.pack(anchor="w", pady=(0, 10))
        
        desc_label = ctk.CTkLabel(
            card_content,
            text=description,
            font=('Segoe UI', 12),
            text_color=self.colors['text_gray'],
            wraplength=200
        )
        desc_label.pack(anchor="w", pady=(0, 15))
        
        btn_generar = ctk.CTkButton(
            card_content,
            text="Generar Reporte",
            command=command,
            height=35,
            font=('Segoe UI', 12, 'bold'),
            fg_color=self.colors['primary'],
            hover_color="#1565C0"
        )
        btn_generar.pack(anchor="w")
    
    def cargar_estadisticas(self):
        """Carga estadísticas rápidas del inventario"""
        try:
            if self.soap_client:
                res = self.soap_client.obtener_todos_articulos()
                if res and res.get('exito'):
                    articulos = res.get('dato', [])
                    
                    total_articulos = len(articulos)
                    total_stock = sum(int(art.get('stock', 0) or 0) for art in articulos)
                    total_valor = sum(
                        float(art.get('stock', 0) or 0) * float(art.get('precioVenta', 0) or art.get('precio', 0) or 0)
                        for art in articulos
                    )
                    stock_bajo = sum(
                        1 for art in articulos 
                        if int(art.get('stock', 0) or 0) <= int(art.get('stockMinimo', 5) or 5)
                    )
                    
                    categorias = set(art.get('categoria', 'Sin categoría') for art in articulos)
                    
                    stats_text = (
                        f"📦 Total de Artículos: {total_articulos}\n"
                        f"📊 Stock Total: {total_stock} unidades\n"
                        f"💰 Valor Total del Inventario: ${total_valor:.2f}\n"
                        f"⚠️ Artículos con Stock Bajo: {stock_bajo}\n"
                        f"🏷️ Categorías Registradas: {len(categorias)}\n"
                        f"📅 Última Actualización: {datetime.now().strftime('%d/%m/%Y %H:%M')}"
                    )
                    
                    self.stats_info.configure(text=stats_text)
                else:
                    self.stats_info.configure(text="❌ No se pudieron cargar las estadísticas")
            else:
                self.stats_info.configure(text="⚠️ No hay conexión con el servidor")
        except Exception as e:
            self.stats_info.configure(text=f"❌ Error: {str(e)}")
    
    def obtener_datos_inventario(self):
        """Obtiene los datos del inventario"""
        if not self.soap_client:
            messagebox.showwarning("Advertencia", "No hay conexión con el servidor")
            return []
        
        try:
            res = self.soap_client.obtener_todos_articulos()
            if res and res.get('exito'):
                return res.get('dato', [])
            else:
                messagebox.showerror("Error", "No se pudieron obtener los datos del inventario")
                return []
        except Exception as e:
            messagebox.showerror("Error", f"Error al obtener datos: {str(e)}")
            return []
    
    def generar_reporte_inventario(self):
        """Genera reporte completo de inventario"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        # Crear contenido del reporte
        contenido = "=== REPORTE COMPLETO DE INVENTARIO ===\n"
        contenido += f"Fecha de generación: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}\n"
        contenido += f"Total de artículos: {len(articulos)}\n\n"
        
        contenido += f"{'CÓDIGO':<12} {'NOMBRE':<25} {'CATEGORÍA':<15} {'STOCK':<8} {'PRECIO':<10} {'VALOR TOTAL':<12}\n"
        contenido += "=" * 90 + "\n"
        
        total_valor = 0
        for art in articulos:
            codigo = str(art.get('codigo', 'N/A'))[:11]
            nombre = str(art.get('nombre', 'N/A'))[:24]
            categoria = str(art.get('categoria', 'Sin categoría'))[:14]
            stock = int(art.get('stock', 0) or 0)
            precio = float(art.get('precioVenta', 0) or art.get('precio', 0) or 0)
            valor_total = stock * precio
            total_valor += valor_total
            
            contenido += f"{codigo:<12} {nombre:<25} {categoria:<15} {stock:<8} ${precio:<9.2f} ${valor_total:<11.2f}\n"
        
        contenido += "=" * 90 + "\n"
        contenido += f"VALOR TOTAL DEL INVENTARIO: ${total_valor:.2f}\n"
        
        self.mostrar_reporte("Reporte de Inventario", contenido)
    
    def generar_reporte_stock_bajo(self):
        """Genera reporte de artículos con stock bajo"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        # Filtrar artículos con stock bajo
        stock_bajo = [
            art for art in articulos 
            if int(art.get('stock', 0) or 0) <= int(art.get('stockMinimo', 5) or 5)
        ]
        
        contenido = "=== REPORTE DE STOCK BAJO ===\n"
        contenido += f"Fecha de generación: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}\n"
        contenido += f"Artículos con stock bajo: {len(stock_bajo)}\n\n"
        
        if stock_bajo:
            contenido += f"{'CÓDIGO':<12} {'NOMBRE':<25} {'STOCK':<8} {'MÍNIMO':<8} {'ESTADO':<10}\n"
            contenido += "=" * 70 + "\n"
            
            for art in stock_bajo:
                codigo = str(art.get('codigo', 'N/A'))[:11]
                nombre = str(art.get('nombre', 'N/A'))[:24]
                stock = int(art.get('stock', 0) or 0)
                minimo = int(art.get('stockMinimo', 5) or 5)
                estado = "CRÍTICO" if stock == 0 else "BAJO"
                
                contenido += f"{codigo:<12} {nombre:<25} {stock:<8} {minimo:<8} {estado:<10}\n"
        else:
            contenido += "✅ No hay artículos con stock bajo\n"
        
        self.mostrar_reporte("Reporte de Stock Bajo", contenido)
    
    def generar_reporte_categorias(self):
        """Genera reporte agrupado por categorías"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        # Agrupar por categorías
        categorias = {}
        for art in articulos:
            cat = art.get('categoria', 'Sin categoría')
            if cat not in categorias:
                categorias[cat] = []
            categorias[cat].append(art)
        
        contenido = "=== REPORTE POR CATEGORÍAS ===\n"
        contenido += f"Fecha de generación: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}\n"
        contenido += f"Categorías encontradas: {len(categorias)}\n\n"
        
        for categoria, arts in categorias.items():
            contenido += f"\n🏷️ CATEGORÍA: {categoria.upper()}\n"
            contenido += "-" * 50 + "\n"
            contenido += f"Artículos en esta categoría: {len(arts)}\n"
            
            total_stock = sum(int(art.get('stock', 0) or 0) for art in arts)
            total_valor = sum(
                float(art.get('stock', 0) or 0) * float(art.get('precioVenta', 0) or art.get('precio', 0) or 0)
                for art in arts
            )
            
            contenido += f"Stock total: {total_stock} unidades\n"
            contenido += f"Valor total: ${total_valor:.2f}\n"
            
            contenido += f"\n{'CÓDIGO':<12} {'NOMBRE':<25} {'STOCK':<8} {'PRECIO':<10}\n"
            contenido += "-" * 60 + "\n"
            
            for art in arts:
                codigo = str(art.get('codigo', 'N/A'))[:11]
                nombre = str(art.get('nombre', 'N/A'))[:24]
                stock = int(art.get('stock', 0) or 0)
                precio = float(art.get('precioVenta', 0) or art.get('precio', 0) or 0)
                
                contenido += f"{codigo:<12} {nombre:<25} {stock:<8} ${precio:<9.2f}\n"
        
        self.mostrar_reporte("Reporte por Categorías", contenido)
    
    def generar_reporte_valores(self):
        """Genera reporte de análisis de valores"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        # Calcular valores
        valores = []
        for art in articulos:
            stock = int(art.get('stock', 0) or 0)
            precio = float(art.get('precioVenta', 0) or art.get('precio', 0) or 0)
            valor_total = stock * precio
            valores.append((art, valor_total))
        
        # Ordenar por valor
        valores.sort(key=lambda x: x[1], reverse=True)
        
        contenido = "=== ANÁLISIS DE VALORES DEL INVENTARIO ===\n"
        contenido += f"Fecha de generación: {datetime.now().strftime('%d/%m/%Y %H:%M:%S')}\n\n"
        
        total_inventario = sum(v[1] for v in valores)
        contenido += f"💰 VALOR TOTAL DEL INVENTARIO: ${total_inventario:.2f}\n\n"
        
        # Top 10 más valiosos
        contenido += "🔝 TOP 10 ARTÍCULOS MÁS VALIOSOS:\n"
        contenido += f"{'POS':<4} {'CÓDIGO':<12} {'NOMBRE':<25} {'VALOR TOTAL':<12}\n"
        contenido += "=" * 60 + "\n"
        
        for i, (art, valor) in enumerate(valores[:10], 1):
            codigo = str(art.get('codigo', 'N/A'))[:11]
            nombre = str(art.get('nombre', 'N/A'))[:24]
            contenido += f"{i:<4} {codigo:<12} {nombre:<25} ${valor:<11.2f}\n"
        
        # Estadísticas adicionales
        contenido += f"\n📊 ESTADÍSTICAS:\n"
        contenido += f"Valor promedio por artículo: ${total_inventario/len(articulos):.2f}\n"
        contenido += f"Artículo más valioso: ${valores[0][1]:.2f}\n"
        contenido += f"Artículo menos valioso: ${valores[-1][1]:.2f}\n"
        
        self.mostrar_reporte("Análisis de Valores", contenido)
    
    def mostrar_reporte(self, titulo, contenido):
        """Muestra el reporte en una ventana"""
        reporte_window = ReporteWindow(self, titulo, contenido)
    
    def exportar_txt(self):
        """Exporta inventario a archivo TXT"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        filename = filedialog.asksaveasfilename(
            defaultextension=".txt",
            filetypes=[("Archivos de texto", "*.txt"), ("Todos los archivos", "*.*")],
            title="Guardar reporte como TXT"
        )
        
        if filename:
            try:
                with open(filename, 'w', encoding='utf-8') as f:
                    f.write(f"INVENTARIO FERRETERÍA - {datetime.now().strftime('%d/%m/%Y %H:%M')}\n")
                    f.write("=" * 80 + "\n\n")
                    
                    for art in articulos:
                        f.write(f"Código: {art.get('codigo', 'N/A')}\n")
                        f.write(f"Nombre: {art.get('nombre', 'N/A')}\n")
                        f.write(f"Categoría: {art.get('categoria', 'Sin categoría')}\n")
                        f.write(f"Stock: {art.get('stock', 0)}\n")
                        f.write(f"Precio: ${art.get('precioVenta', art.get('precio', 0))}\n")
                        f.write("-" * 40 + "\n")
                
                messagebox.showinfo("Éxito", f"Archivo exportado correctamente:\n{filename}")
            except Exception as e:
                messagebox.showerror("Error", f"Error al exportar archivo: {str(e)}")
    
    def exportar_csv(self):
        """Exporta inventario a archivo CSV"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        filename = filedialog.asksaveasfilename(
            defaultextension=".csv",
            filetypes=[("Archivos CSV", "*.csv"), ("Todos los archivos", "*.*")],
            title="Guardar reporte como CSV"
        )
        
        if filename:
            try:
                with open(filename, 'w', encoding='utf-8') as f:
                    f.write("Código,Nombre,Categoría,Stock,Precio,Valor Total\n")
                    
                    for art in articulos:
                        codigo = art.get('codigo', 'N/A')
                        nombre = art.get('nombre', 'N/A').replace(',', ';')
                        categoria = art.get('categoria', 'Sin categoría').replace(',', ';')
                        stock = art.get('stock', 0)
                        precio = art.get('precioVenta', art.get('precio', 0))
                        valor_total = float(stock or 0) * float(precio or 0)
                        
                        f.write(f"{codigo},{nombre},{categoria},{stock},{precio},{valor_total:.2f}\n")
                
                messagebox.showinfo("Éxito", f"Archivo CSV exportado correctamente:\n{filename}")
            except Exception as e:
                messagebox.showerror("Error", f"Error al exportar CSV: {str(e)}")
    
    def exportar_json(self):
        """Exporta inventario a archivo JSON"""
        articulos = self.obtener_datos_inventario()
        if not articulos:
            return
        
        filename = filedialog.asksaveasfilename(
            defaultextension=".json",
            filetypes=[("Archivos JSON", "*.json"), ("Todos los archivos", "*.*")],
            title="Guardar reporte como JSON"
        )
        
        if filename:
            try:
                data = {
                    "fecha_exportacion": datetime.now().isoformat(),
                    "total_articulos": len(articulos),
                    "articulos": articulos
                }
                
                with open(filename, 'w', encoding='utf-8') as f:
                    json.dump(data, f, indent=2, ensure_ascii=False)
                
                messagebox.showinfo("Éxito", f"Archivo JSON exportado correctamente:\n{filename}")
            except Exception as e:
                messagebox.showerror("Error", f"Error al exportar JSON: {str(e)}")


class ReporteWindow(ctk.CTkToplevel):
    """Ventana para mostrar reportes"""
    
    def __init__(self, parent, titulo, contenido):
        super().__init__(parent)
        self.title(f"Reporte: {titulo}")
        self.geometry("800x600")
        
        # Centrar ventana
        self.update_idletasks()
        x = (self.winfo_screenwidth() // 2) - (800 // 2)
        y = (self.winfo_screenheight() // 2) - (600 // 2)
        self.geometry(f"800x600+{x}+{y}")
        
        self.contenido = contenido
        self.create_widgets()
        
        # Mantener ventana al frente
        self.transient(parent)
        
    def create_widgets(self):
        """Crea los widgets de la ventana"""
        
        # Header
        header_frame = ctk.CTkFrame(self, fg_color="#1976D2", corner_radius=0)
        header_frame.pack(fill="x")
        
        header_content = ctk.CTkFrame(header_frame, fg_color="transparent")
        header_content.pack(fill="both", expand=True, padx=20, pady=15)
        
        title_label = ctk.CTkLabel(
            header_content,
            text="📄 Reporte Generado",
            font=("Segoe UI", 20, "bold"),
            text_color="white"
        )
        title_label.pack(side="left")
        
        # Área de texto
        text_frame = ctk.CTkFrame(self, fg_color="white")
        text_frame.pack(fill="both", expand=True, padx=20, pady=20)
        
        self.text_area = ctk.CTkTextbox(
            text_frame,
            font=("Courier", 11),
            wrap="none"
        )
        self.text_area.pack(fill="both", expand=True, padx=15, pady=15)
        
        # Insertar contenido
        self.text_area.insert("1.0", self.contenido)
        self.text_area.configure(state="disabled")
        
        # Botones
        buttons_frame = ctk.CTkFrame(self, fg_color="transparent")
        buttons_frame.pack(fill="x", padx=20, pady=(0, 20))
        
        btn_guardar = ctk.CTkButton(
            buttons_frame,
            text="💾 Guardar como TXT",
            command=self.guardar_reporte,
            height=40,
            font=("Segoe UI", 12, "bold"),
            fg_color="#4CAF50",
            hover_color="#388E3C"
        )
        btn_guardar.pack(side="left", padx=(0, 10))
        
        btn_cerrar = ctk.CTkButton(
            buttons_frame,
            text="✖️ Cerrar",
            command=self.destroy,
            height=40,
            font=("Segoe UI", 12, "bold"),
            fg_color="#F44336",
            hover_color="#D32F2F"
        )
        btn_cerrar.pack(side="right")
    
    def guardar_reporte(self):
        """Guarda el reporte como archivo TXT"""
        filename = filedialog.asksaveasfilename(
            defaultextension=".txt",
            filetypes=[("Archivos de texto", "*.txt"), ("Todos los archivos", "*.*")],
            title="Guardar reporte"
        )
        
        if filename:
            try:
                with open(filename, 'w', encoding='utf-8') as f:
                    f.write(self.contenido)
                messagebox.showinfo("Éxito", f"Reporte guardado correctamente:\n{filename}")
            except Exception as e:
                messagebox.showerror("Error", f"Error al guardar: {str(e)}")


if __name__ == "__main__":
    # Prueba del módulo
    root = ctk.CTk()
    root.title("Reportes Test")
    root.geometry("1200x800")
    ctk.set_appearance_mode("light")
    
    reportes = ReportesModule(root)
    root.mainloop()
