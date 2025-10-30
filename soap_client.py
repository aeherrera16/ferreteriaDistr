"""
Cliente SOAP para el sistema de inventario de ferretería
Versión limpia y optimizada con autenticación JWT
"""

from zeep import Client
from typing import Dict, Any


class SoapClient:
    """Cliente para comunicación con el servicio SOAP del inventario"""
    
    def __init__(self, wsdl_url: str = "http://localhost:5000/InventarioService.asmx?wsdl"):
        self.wsdl_url = wsdl_url
        self.client = None
        self.token = None
        self.authenticated = False
        self.connect()
        self._autenticar_automatico()
    
    def connect(self) -> bool:
        """Establece conexión con el servicio SOAP"""
        try:
            self.client = Client(self.wsdl_url)
            print("✓ Conexión SOAP establecida")
            return True
        except Exception as e:
            print(f"✗ Error al conectar con SOAP: {e}")
            return False
    
    def _autenticar_automatico(self):
        """Autenticación automática con credenciales del sistema"""
        try:
            # Intentar con usuario admin
            resultado = self.autenticar("admin", "Admin123!")
            if resultado.get("exito"):
                print("✓ Autenticación automática exitosa (admin)")
                return
            
            # Intentar con usuario normal
            resultado = self.autenticar("usuario", "Usuario123!")
            if resultado.get("exito"):
                print("✓ Autenticación automática exitosa (usuario)")
            else:
                print("✗ Autenticación automática falló")
                self.token = None
                self.authenticated = False
        except Exception as e:
            print(f"✗ Error en autenticación automática: {e}")
            self.token = None
            self.authenticated = False
    
    def _verificar_autenticacion(self) -> bool:
        """Verifica que hay token válido, intenta autenticar si no hay"""
        if not self.token or not self.authenticated:
            print("⚠ Token no disponible, intentando autenticación...")
            self._autenticar_automatico()
            return self.token is not None and self.authenticated
        return True
    
    def autenticar(self, usuario: str, password: str) -> Dict[str, Any]:
        """Autentica un usuario y obtiene token JWT"""
        try:
            if not self.client:
                self.connect()
            
            response = self.client.service.Autenticar(usuario, password)
            
            if response.Exito:
                token = getattr(response, 'Datos', None)
                if token:
                    self.token = token
                    self.authenticated = True
                    return {
                        "exito": True,
                        "mensaje": "Autenticación exitosa",
                        "token": self.token
                    }
                return {
                    "exito": False,
                    "mensaje": "Token no recibido"
                }
            
            mensaje = getattr(response, 'Mensaje', 'Error de autenticación')
            return {
                "exito": False,
                "mensaje": mensaje
            }
        except Exception as e:
            print(f"✗ Error en autenticación: {e}")
            return {
                "exito": False,
                "mensaje": f"Error: {str(e)}"
            }
    
    def obtener_todos_articulos(self) -> Dict[str, Any]:
        """Obtiene todos los artículos del inventario"""
        try:
            if not self._verificar_autenticacion():
                return {
                    "exito": False,
                    "mensaje": "No se pudo autenticar"
                }
            
            response = self.client.service.ObtenerTodosArticulos(self.token)
            
            if response.Exito:
                dato = getattr(response, 'Dato', None) or getattr(response, 'Datos', None)
                
                if dato:
                    lista_articulos = getattr(dato, 'Articulo', None)
                    
                    if lista_articulos:
                        articulos = [self._articulo_to_dict(art) for art in lista_articulos]
                        return {
                            "exito": True,
                            "mensaje": f"{len(articulos)} artículos encontrados",
                            "dato": articulos
                        }
                
                return {
                    "exito": True,
                    "mensaje": "No hay artículos",
                    "dato": []
                }
            
            mensaje = getattr(response, 'Mensaje', 'Error al obtener artículos')
            return {
                "exito": False,
                "mensaje": mensaje
            }
        except Exception as e:
            return {
                "exito": False,
                "mensaje": f"Error: {str(e)}"
            }
    
    def crear_articulo(self, datos_articulo: Dict[str, Any]) -> Dict[str, Any]:
        """Crea un nuevo artículo"""
        try:
            if not self._verificar_autenticacion():
                return {
                    "exito": False,
                    "mensaje": "No se pudo autenticar"
                }
            
            response = self.client.service.InsertarArticulo(
                token=self.token,
                codigo=datos_articulo.get('codigo', ''),
                nombre=datos_articulo.get('nombre', ''),
                descripcion=datos_articulo.get('descripcion', ''),
                categoriaId=datos_articulo.get('categoria_id', 1),
                precioCompra=float(datos_articulo.get('precio_compra', datos_articulo.get('precio', 0))),
                precioVenta=float(datos_articulo.get('precio', 0)),
                stock=int(datos_articulo.get('stock', 0)),
                stockMinimo=int(datos_articulo.get('stock_minimo', 0)),
                proveedorId=datos_articulo.get('proveedor_id', 1)
            )
            
            if response.Exito:
                return {
                    "exito": True,
                    "mensaje": "Artículo creado exitosamente"
                }
            
            mensaje = getattr(response, 'Mensaje', 'Error al crear artículo')
            return {
                "exito": False,
                "mensaje": mensaje
            }
        except Exception as e:
            return {
                "exito": False,
                "mensaje": f"Error: {str(e)}"
            }
    
    def eliminar_articulo(self, codigo: str) -> Dict[str, Any]:
        """Elimina un artículo por código"""
        try:
            if not self._verificar_autenticacion():
                return {
                    "exito": False,
                    "mensaje": "No se pudo autenticar"
                }
            
            # Buscar ID dinámicamente por código
            articulo_id = None
            articulos_response = self.obtener_todos_articulos()
            
            if articulos_response.get("exito"):
                for articulo in articulos_response.get("dato", []):
                    if str(articulo.get("codigo", "")).strip() == str(codigo).strip():
                        articulo_id = articulo.get("id")
                        break
            
            if not articulo_id:
                return {
                    "exito": False,
                    "mensaje": f"No se encontró artículo con código: {codigo}"
                }
            
            response = self.client.service.EliminarArticulo(
                token=self.token,
                id=int(articulo_id)
            )
            
            if hasattr(response, 'Exito') and response.Exito:
                return {
                    "exito": True,
                    "mensaje": "Artículo eliminado exitosamente"
                }
            
            mensaje = getattr(response, 'Mensaje', 'Error al eliminar')
            return {
                "exito": False,
                "mensaje": mensaje
            }
        except Exception as e:
            return {
                "exito": False,
                "mensaje": f"Error: {str(e)}"
            }
    
    def actualizar_articulo(self, datos_articulo: Dict[str, Any]) -> Dict[str, Any]:
        """Actualiza un artículo existente"""
        try:
            if not self._verificar_autenticacion():
                return {
                    "exito": False,
                    "mensaje": "No se pudo autenticar"
                }
            
            # Obtener ID del artículo (puede venir directamente o buscar por código)
            articulo_id = datos_articulo.get('id') or datos_articulo.get('Id')
            codigo = datos_articulo.get('codigo', '')
            
            # Si no hay ID, buscar dinámicamente por código
            if not articulo_id and codigo:
                articulos_response = self.obtener_todos_articulos()
                if articulos_response.get("exito"):
                    for articulo in articulos_response.get("dato", []):
                        if str(articulo.get("codigo", "")).strip() == str(codigo).strip():
                            articulo_id = articulo.get("id")
                            break
            
            if not articulo_id:
                return {
                    "exito": False,
                    "mensaje": f"No se encontró artículo con código: {codigo}"
                }
            
            response = self.client.service.ActualizarArticulo(
                token=self.token,
                id=int(articulo_id),
                codigo=datos_articulo.get('codigo', ''),
                nombre=datos_articulo.get('nombre', ''),
                descripcion=datos_articulo.get('descripcion', ''),
                categoriaId=datos_articulo.get('categoria_id', 1),
                precioCompra=float(datos_articulo.get('precio_compra', datos_articulo.get('precio', 0))),
                precioVenta=float(datos_articulo.get('precio', 0)),
                stock=int(datos_articulo.get('stock', 0)),
                stockMinimo=int(datos_articulo.get('stock_minimo', 0)),
                proveedorId=datos_articulo.get('proveedor_id', 1)
            )
            
            if hasattr(response, 'Exito') and response.Exito:
                return {
                    "exito": True,
                    "mensaje": "Artículo actualizado exitosamente"
                }
            
            mensaje = getattr(response, 'Mensaje', 'Error al actualizar')
            return {
                "exito": False,
                "mensaje": mensaje
            }
        except Exception as e:
            return {
                "exito": False,
                "mensaje": f"Error: {str(e)}"
            }
    
    def _articulo_to_dict(self, articulo) -> Dict[str, Any]:
        """Convierte un objeto artículo SOAP a diccionario con formato correcto para la UI"""
        return {
            # Formato para UI (minúsculas)
            "id": getattr(articulo, 'Id', 0),
            "codigo": getattr(articulo, 'Codigo', ''),
            "nombre": getattr(articulo, 'Nombre', ''),
            "descripcion": getattr(articulo, 'Descripcion', ''),
            "categoria_id": getattr(articulo, 'CategoriaId', None),
            "categoria": "General",  # Por ahora genérico, se puede mejorar
            "precio": float(getattr(articulo, 'PrecioVenta', 0)),
            "precio_compra": float(getattr(articulo, 'PrecioCompra', 0)),
            "stock": int(getattr(articulo, 'Stock', 0)),
            "stock_minimo": int(getattr(articulo, 'StockMinimo', 0)),
            "proveedor_id": getattr(articulo, 'ProveedorId', None),
            # También mantener formato PascalCase para compatibilidad con dashboard
            "Id": getattr(articulo, 'Id', 0),
            "Codigo": getattr(articulo, 'Codigo', ''),
            "Nombre": getattr(articulo, 'Nombre', ''),
            "Stock": int(getattr(articulo, 'Stock', 0)),
            "PrecioVenta": float(getattr(articulo, 'PrecioVenta', 0)),
            "StockMinimo": int(getattr(articulo, 'StockMinimo', 0)),
        }
