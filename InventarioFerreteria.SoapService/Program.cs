// InventarioFerreteria.Client/Program.cs
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using System.Globalization;

namespace InventarioFerreteria.Client;

class Program
{
    private static string? _token;
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ServiceUrl = "http://localhost:5000/InventarioService.asmx";

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Cliente SOAP - Sistema de Inventario Ferretería ===\n");

        bool continuar = true;

        while (continuar)
        {
            try
            {
                Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
                Console.WriteLine("║           MENÚ PRINCIPAL                      ║");
                Console.WriteLine("╚═══════════════════════════════════════════════╝");
                Console.WriteLine("1.  Autenticar");
                Console.WriteLine("\n--- ARTÍCULOS ---");
                Console.WriteLine("2.  Insertar Artículo");
                Console.WriteLine("3.  Consultar Artículo por Código");
                Console.WriteLine("4.  Listar Todos los Artículos");
                Console.WriteLine("5.  Actualizar Artículo");
                Console.WriteLine("6.  Eliminar Artículo");
                Console.WriteLine("\n--- CATEGORÍAS ---");
                Console.WriteLine("7.  Insertar Categoría");
                Console.WriteLine("8.  Listar Categorías");
                Console.WriteLine("9.  Actualizar Categoría");
                Console.WriteLine("10. Eliminar Categoría");
                Console.WriteLine("\n--- PROVEEDORES ---");
                Console.WriteLine("11. Insertar Proveedor");
                Console.WriteLine("12. Listar Proveedores");
                Console.WriteLine("13. Actualizar Proveedor");
                Console.WriteLine("14. Eliminar Proveedor");
                Console.WriteLine("\n0.  Salir");
                Console.Write("\nSeleccione una opción: ");

                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        await AutenticarAsync();
                        break;
                    case "2":
                        await InsertarArticuloAsync();
                        break;
                    case "3":
                        await ConsultarArticuloAsync();
                        break;
                    case "4":
                        await ListarArticulosAsync();
                        break;
                    case "5":
                        await ActualizarArticuloAsync();
                        break;
                    case "6":
                        await EliminarArticuloAsync();
                        break;
                    case "7":
                        await InsertarCategoriaAsync();
                        break;
                    case "8":
                        await ListarCategoriasAsync();
                        break;
                    case "9":
                        await ActualizarCategoriaAsync();
                        break;
                    case "10":
                        await EliminarCategoriaAsync();
                        break;
                    case "11":
                        await InsertarProveedorAsync();
                        break;
                    case "12":
                        await ListarProveedoresAsync();
                        break;
                    case "13":
                        await ActualizarProveedorAsync();
                        break;
                    case "14":
                        await EliminarProveedorAsync();
                        break;
                    case "0":
                        continuar = false;
                        Console.WriteLine("\n¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("\n❌ Opción inválida.");
                        break;
                }

                if (continuar && opcion != "1")
                {
                    Console.WriteLine("\nPresione Enter para continuar...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }
        }
    }

    // ============ AUTENTICACIÓN ============
    static async Task AutenticarAsync()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           AUTENTICACIÓN                       ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.Write("Usuario: ");
        var usuario = Console.ReadLine() ?? "";
        
        Console.Write("Contraseña: ");
        var password = LeerPassword();

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <Autenticar xmlns=""http://tempuri.org/"">
      <nombreUsuario>{usuario}</nombreUsuario>
      <password>{password}</password>
    </Autenticar>
  </soap:Body>
</soap:Envelope>";

        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/IInventarioSoapService/Autenticar");

        var response = await _httpClient.PostAsync(ServiceUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            var xml = XDocument.Parse(responseContent);
            
            XNamespace[] namespaces = {
                XNamespace.Get("http://tempuri.org/"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
                XNamespace.None
            };

            string? exito = null;
            string? mensaje = null;
            string? datos = null;

            foreach (var ns in namespaces)
            {
                exito = exito ?? xml.Descendants(ns + "Exito").FirstOrDefault()?.Value;
                mensaje = mensaje ?? xml.Descendants(ns + "Mensaje").FirstOrDefault()?.Value;
                datos = datos ?? xml.Descendants(ns + "Datos").FirstOrDefault()?.Value;
            }

            if (exito == "true" && !string.IsNullOrEmpty(datos))
            {
                _token = datos;
                Console.WriteLine($"\n✅ {mensaje}");
                Console.WriteLine($"Token: {_token[..Math.Min(50, _token.Length)]}...");
            }
            else
            {
                Console.WriteLine($"\n❌ {mensaje ?? "Error en autenticación"}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    // ============ ARTÍCULOS ============
    static async Task InsertarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           INSERTAR ARTÍCULO                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("Código: ");
        var codigo = Console.ReadLine() ?? "";

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción (opcional): ");
        var descripcion = Console.ReadLine() ?? "";

        Console.Write("ID Categoría (opcional): ");
        var categoriaIdStr = Console.ReadLine();

        Console.Write("Precio Compra: ");
        var precioCompra = Console.ReadLine() ?? "0";

        Console.Write("Precio Venta: ");
        var precioVenta = Console.ReadLine() ?? "0";

        Console.Write("Stock: ");
        var stock = Console.ReadLine() ?? "0";

        Console.Write("Stock Mínimo: ");
        var stockMinimo = Console.ReadLine() ?? "5";

        Console.Write("ID Proveedor (opcional): ");
        var proveedorIdStr = Console.ReadLine();

        var categoriaIdXml = string.IsNullOrEmpty(categoriaIdStr) 
            ? "<categoriaId xsi:nil=\"true\" />" 
            : $"<categoriaId>{categoriaIdStr}</categoriaId>";
            
        var proveedorIdXml = string.IsNullOrEmpty(proveedorIdStr) 
            ? "<proveedorId xsi:nil=\"true\" />" 
            : $"<proveedorId>{proveedorIdStr}</proveedorId>";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <soap:Body>
    <InsertarArticulo xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <codigo>{codigo}</codigo>
      <nombre>{nombre}</nombre>
      <descripcion>{descripcion}</descripcion>
      {categoriaIdXml}
      <precioCompra>{precioCompra}</precioCompra>
      <precioVenta>{precioVenta}</precioVenta>
      <stock>{stock}</stock>
      <stockMinimo>{stockMinimo}</stockMinimo>
      {proveedorIdXml}
    </InsertarArticulo>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("InsertarArticulo", soapRequest, MostrarArticuloDesdeXml);
    }

    static async Task ConsultarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           CONSULTAR ARTÍCULO                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");
        Console.Write("Código del artículo: ");
        var codigo = Console.ReadLine() ?? "";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ConsultarArticuloPorCodigo xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <codigo>{codigo}</codigo>
    </ConsultarArticuloPorCodigo>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("ConsultarArticuloPorCodigo", soapRequest, MostrarArticuloDesdeXml);
    }

    static async Task ListarArticulosAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                    LISTA DE ARTÍCULOS                                                           ║");
        Console.WriteLine("╚═════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════════╝");

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <ObtenerTodosArticulos xmlns=""http://tempuri.org/"">
        <token>{_token}</token>
        </ObtenerTodosArticulos>
    </soap:Body>
    </soap:Envelope>";

        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/IInventarioSoapService/ObtenerTodosArticulos");

        var response = await _httpClient.PostAsync(ServiceUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            var xml = XDocument.Parse(responseContent);
            
            XNamespace[] namespaces = {
                XNamespace.Get("http://tempuri.org/"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
                XNamespace.None
            };

            var exito = ObtenerValorXml(xml, "Exito", namespaces);
            var mensaje = ObtenerValorXml(xml, "Mensaje", namespaces);

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}\n");
                
                // Encabezado completo de la tabla con TODAS las columnas
                Console.WriteLine($"{"ID",-4} {"Código",-12} {"Nombre",-25} {"Descripción",-20} {"Cat",-4} {"P.Compra",10} {"P.Venta",10} {"Stock",6} {"Mín",4} {"Prov",-5} {"F.Creación",-17} {"F.Actualiz",-17} {"Act",-4} {"Rep",-4}");
                Console.WriteLine(new string('─', 155));

                IEnumerable<XElement>? articulos = null;
                foreach (var ns in namespaces)
                {
                    articulos = xml.Descendants(ns + "Articulo");
                    if (articulos.Any()) break;
                }

                if (articulos != null && articulos.Any())
                {
                    foreach (var articulo in articulos)
                    {
                        var artId = ObtenerValor(articulo, "Id", namespaces) ?? "";
                        var artCodigo = ObtenerValor(articulo, "Codigo", namespaces) ?? "";
                        var artNombre = ObtenerValor(articulo, "Nombre", namespaces) ?? "";
                        var artDescripcion = ObtenerValor(articulo, "Descripcion", namespaces) ?? "N/A";
                        var artCategoriaId = ObtenerValor(articulo, "CategoriaId", namespaces) ?? "-";
                        var artPrecioCompra = ObtenerValor(articulo, "PrecioCompra", namespaces) ?? "0";
                        var artPrecioVenta = ObtenerValor(articulo, "PrecioVenta", namespaces) ?? "0";
                        var artStock = ObtenerValor(articulo, "Stock", namespaces) ?? "0";
                        var artStockMin = ObtenerValor(articulo, "StockMinimo", namespaces) ?? "0";
                        var artProveedorId = ObtenerValor(articulo, "ProveedorId", namespaces) ?? "-";
                        var artFechaCreacion = ObtenerValor(articulo, "FechaCreacion", namespaces) ?? "";
                        var artFechaActualizacion = ObtenerValor(articulo, "FechaActualizacion", namespaces) ?? "";
                        var artActivo = ObtenerValor(articulo, "Activo", namespaces) == "true" ? "Sí" : "No";
                        var artRequiere = ObtenerValor(articulo, "RequiereReposicion", namespaces) == "true" ? "⚠️" : "✅";

                        Console.WriteLine($"[DEBUG] PrecioCompra RAW: '{artPrecioCompra}'");
                        // Truncar textos largos
                        if (artNombre.Length > 23)
                            artNombre = artNombre.Substring(0, 22) + "…";
                        if (artDescripcion.Length > 18)
                            artDescripcion = artDescripcion.Substring(0, 17) + "…";
                        if (artCodigo.Length > 10)
                            artCodigo = artCodigo.Substring(0, 9) + "…";

                        // Formatear fechas
                        string fechaCreacionStr = "";
                        string fechaActualizStr = "";
                        
                        if (DateTime.TryParse(artFechaCreacion, out var fc))
                            fechaCreacionStr = fc.ToString("dd/MM/yy HH:mm");
                        else
                            fechaCreacionStr = artFechaCreacion;
                            
                        if (DateTime.TryParse(artFechaActualizacion, out var fa))
                            fechaActualizStr = fa.ToString("dd/MM/yy HH:mm");
                        else
                            fechaActualizStr = artFechaActualizacion;

                        // Formatear precios
                        string precioCompraStr = "";
                        string precioVentaStr = "";
                        
                        // Busca estas líneas y reemplázalas:
                        if (decimal.TryParse(artPrecioCompra, NumberStyles.Any, CultureInfo.InvariantCulture, out var pc))
                            precioCompraStr = $"${pc:F2}";
                        else
                            precioCompraStr = artPrecioCompra;
                            
                        if (decimal.TryParse(artPrecioVenta, NumberStyles.Any, CultureInfo.InvariantCulture, out var pv))
                            precioVentaStr = $"${pv:F2}";
                        else
                            precioVentaStr = artPrecioVenta;

                        // Imprimir fila completa
                        Console.WriteLine($"{artId,-4} {artCodigo,-12} {artNombre,-25} {artDescripcion,-20} {artCategoriaId,-4} {precioCompraStr,10} {precioVentaStr,10} {artStock,6} {artStockMin,4} {artProveedorId,-5} {fechaCreacionStr,-17} {fechaActualizStr,-17} {artActivo,-4} {artRequiere,-4}");
                    }
                    
                    Console.WriteLine(new string('─', 155));
                    Console.WriteLine($"Total: {articulos.Count()} artículo(s)");
                }
                else
                {
                    Console.WriteLine("No hay artículos registrados.");
                }
            }
            else
            {
                Console.WriteLine($"\n❌ {mensaje}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    static async Task ActualizarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ACTUALIZAR ARTÍCULO                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID del artículo a actualizar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("Código: ");
        var codigo = Console.ReadLine() ?? "";

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción (opcional): ");
        var descripcion = Console.ReadLine() ?? "";

        Console.Write("ID Categoría (opcional): ");
        var categoriaIdStr = Console.ReadLine();

        Console.Write("Precio Compra: ");
        var precioCompra = Console.ReadLine() ?? "0";

        Console.Write("Precio Venta: ");
        var precioVenta = Console.ReadLine() ?? "0";

        Console.Write("Stock: ");
        var stock = Console.ReadLine() ?? "0";

        Console.Write("Stock Mínimo: ");
        var stockMinimo = Console.ReadLine() ?? "5";

        Console.Write("ID Proveedor (opcional): ");
        var proveedorIdStr = Console.ReadLine();

        var categoriaIdXml = string.IsNullOrEmpty(categoriaIdStr) 
            ? "<categoriaId xsi:nil=\"true\" />" 
            : $"<categoriaId>{categoriaIdStr}</categoriaId>";
            
        var proveedorIdXml = string.IsNullOrEmpty(proveedorIdStr) 
            ? "<proveedorId xsi:nil=\"true\" />" 
            : $"<proveedorId>{proveedorIdStr}</proveedorId>";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <soap:Body>
    <ActualizarArticulo xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
      <codigo>{codigo}</codigo>
      <nombre>{nombre}</nombre>
      <descripcion>{descripcion}</descripcion>
      {categoriaIdXml}
      <precioCompra>{precioCompra}</precioCompra>
      <precioVenta>{precioVenta}</precioVenta>
      <stock>{stock}</stock>
      <stockMinimo>{stockMinimo}</stockMinimo>
      {proveedorIdXml}
    </ActualizarArticulo>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("ActualizarArticulo", soapRequest, MostrarArticuloDesdeXml);
    }

    static async Task EliminarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ELIMINAR ARTÍCULO                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID del artículo a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("¿Está seguro? (S/N): ");
        if (Console.ReadLine()?.ToUpper() != "S")
        {
            Console.WriteLine("Operación cancelada");
            return;
        }

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <EliminarArticulo xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
    </EliminarArticulo>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("EliminarArticulo", soapRequest);
    }

    // ============ CATEGORÍAS ============
    static async Task InsertarCategoriaAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           INSERTAR CATEGORÍA                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción (opcional): ");
        var descripcion = Console.ReadLine() ?? "";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <InsertarCategoria xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <nombre>{nombre}</nombre>
      <descripcion>{descripcion}</descripcion>
    </InsertarCategoria>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("InsertarCategoria", soapRequest, MostrarCategoriaDesdeXml);
    }

    static async Task ListarCategoriasAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           LISTA DE CATEGORÍAS                 ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerTodasCategorias xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
    </ObtenerTodasCategorias>
  </soap:Body>
</soap:Envelope>";

        await ListarEntidadesAsync("ObtenerTodasCategorias", soapRequest, "Categoria", 
            new[] { "Id", "Nombre", "Descripcion" }, 
            new[] { 10, 30, 50 });
    }

    static async Task ActualizarCategoriaAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ACTUALIZAR CATEGORÍA                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID de la categoría: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción (opcional): ");
        var descripcion = Console.ReadLine() ?? "";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ActualizarCategoria xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
      <nombre>{nombre}</nombre>
      <descripcion>{descripcion}</descripcion>
    </ActualizarCategoria>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("ActualizarCategoria", soapRequest, MostrarCategoriaDesdeXml);
    }

    static async Task EliminarCategoriaAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ELIMINAR CATEGORÍA                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID de la categoría a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("¿Está seguro? (S/N): ");
        if (Console.ReadLine()?.ToUpper() != "S")
        {
            Console.WriteLine("Operación cancelada");
            return;
        }

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <EliminarCategoria xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
    </EliminarCategoria>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("EliminarCategoria", soapRequest);
    }

    // ============ PROVEEDORES ============
    static async Task InsertarProveedorAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           INSERTAR PROVEEDOR                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Teléfono (opcional): ");
        var telefono = Console.ReadLine() ?? "";

        Console.Write("Email (opcional): ");
        var email = Console.ReadLine() ?? "";

        Console.Write("Dirección (opcional): ");
        var direccion = Console.ReadLine() ?? "";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <InsertarProveedor xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <nombre>{nombre}</nombre>
      <telefono>{telefono}</telefono>
      <email>{email}</email>
      <direccion>{direccion}</direccion>
    </InsertarProveedor>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("InsertarProveedor", soapRequest, MostrarProveedorDesdeXml);
    }

    static async Task ListarProveedoresAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           LISTA DE PROVEEDORES                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerTodosProveedores xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
    </ObtenerTodosProveedores>
  </soap:Body>
</soap:Envelope>";

        await ListarEntidadesAsync("ObtenerTodosProveedores", soapRequest, "Proveedor", 
            new[] { "Id", "Nombre", "Telefono", "Email" }, 
            new[] { 10, 30, 15, 30 });
    }

    static async Task ActualizarProveedorAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ACTUALIZAR PROVEEDOR                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID del proveedor: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Teléfono (opcional): ");
        var telefono = Console.ReadLine() ?? "";

        Console.Write("Email (opcional): ");
        var email = Console.ReadLine() ?? "";

        Console.Write("Dirección (opcional): ");
        var direccion = Console.ReadLine() ?? "";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ActualizarProveedor xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
      <nombre>{nombre}</nombre>
      <telefono>{telefono}</telefono>
      <email>{email}</email>
      <direccion>{direccion}</direccion>
    </ActualizarProveedor>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("ActualizarProveedor", soapRequest, MostrarProveedorDesdeXml);
    }

    static async Task EliminarProveedorAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
        Console.WriteLine("║           ELIMINAR PROVEEDOR                  ║");
        Console.WriteLine("╚═══════════════════════════════════════════════╝");

        Console.Write("ID del proveedor a eliminar: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("❌ ID inválido");
            return;
        }

        Console.Write("¿Está seguro? (S/N): ");
        if (Console.ReadLine()?.ToUpper() != "S")
        {
            Console.WriteLine("Operación cancelada");
            return;
        }

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <EliminarProveedor xmlns=""http://tempuri.org/"">
      <token>{_token}</token>
      <id>{id}</id>
    </EliminarProveedor>
  </soap:Body>
</soap:Envelope>";

        await EjecutarOperacionAsync("EliminarProveedor", soapRequest);
    }

    // ============ MÉTODOS AUXILIARES ============
    static async Task EjecutarOperacionAsync(string operacion, string soapRequest, Action<XDocument>? mostrarDatos = null)
    {
        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", $"http://tempuri.org/IInventarioSoapService/{operacion}");

        var response = await _httpClient.PostAsync(ServiceUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            var xml = XDocument.Parse(responseContent);
            
            XNamespace[] namespaces = {
                XNamespace.Get("http://tempuri.org/"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
                XNamespace.None
            };

            var exito = ObtenerValorXml(xml, "Exito", namespaces);
            var mensaje = ObtenerValorXml(xml, "Mensaje", namespaces);

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}");
                mostrarDatos?.Invoke(xml);
            }
            else
            {
                Console.WriteLine($"\n❌ {mensaje}");
                var errores = xml.Descendants().Where(e => e.Name.LocalName == "string");
                foreach (var error in errores)
                {
                    Console.WriteLine($"  - {error.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    static async Task ListarEntidadesAsync(string operacion, string soapRequest, string nombreEntidad, string[] campos, int[] anchos)
    {
        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", $"http://tempuri.org/IInventarioSoapService/{operacion}");

        var response = await _httpClient.PostAsync(ServiceUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        try
        {
            var xml = XDocument.Parse(responseContent);
            
            XNamespace[] namespaces = {
                XNamespace.Get("http://tempuri.org/"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
                XNamespace.None
            };

            var exito = ObtenerValorXml(xml, "Exito", namespaces);
            var mensaje = ObtenerValorXml(xml, "Mensaje", namespaces);

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}\n");
                
                // Imprimir encabezados
                for (int i = 0; i < campos.Length; i++)
                {
                    Console.Write($"{campos[i].PadRight(anchos[i])} ");
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', anchos.Sum() + campos.Length - 1));

                // Buscar entidades
                IEnumerable<XElement>? entidades = null;
                foreach (var ns in namespaces)
                {
                    entidades = xml.Descendants(ns + nombreEntidad);
                    if (entidades.Any()) break;
                }

                if (entidades != null)
                {
                    foreach (var entidad in entidades)
                    {
                        for (int i = 0; i < campos.Length; i++)
                        {
                            var valor = ObtenerValor(entidad, campos[i], namespaces) ?? "N/A";
                            Console.Write($"{valor.PadRight(anchos[i])} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                Console.WriteLine($"\n❌ {mensaje}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    static void MostrarArticuloDesdeXml(XDocument xml)
    {
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        XElement? datos = null;
        foreach (var ns in namespaces)
        {
            datos = xml.Descendants(ns + "Datos").FirstOrDefault();
            if (datos != null) break;
        }

        if (datos == null) return;

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine($"ID: {ObtenerValorDesdeElemento(datos, "Id", namespaces)}");
        Console.WriteLine($"Código: {ObtenerValorDesdeElemento(datos, "Codigo", namespaces)}");
        Console.WriteLine($"Nombre: {ObtenerValorDesdeElemento(datos, "Nombre", namespaces)}");
        Console.WriteLine($"Descripción: {ObtenerValorDesdeElemento(datos, "Descripcion", namespaces) ?? "N/A"}");
        
        var precioCompra = ObtenerValorDesdeElemento(datos, "PrecioCompra", namespaces);
        var precioVenta = ObtenerValorDesdeElemento(datos, "PrecioVenta", namespaces);
        
        if (decimal.TryParse(precioCompra, out var pc))
            Console.WriteLine($"Precio Compra: ${pc:F2}");
        if (decimal.TryParse(precioVenta, out var pv))
            Console.WriteLine($"Precio Venta: ${pv:F2}");
            
        Console.WriteLine($"Stock: {ObtenerValorDesdeElemento(datos, "Stock", namespaces)}");
        Console.WriteLine($"Stock Mínimo: {ObtenerValorDesdeElemento(datos, "StockMinimo", namespaces)}");
        Console.WriteLine($"Requiere Reposición: {(ObtenerValorDesdeElemento(datos, "RequiereReposicion", namespaces) == "true" ? "SÍ ⚠️" : "NO")}");
        Console.WriteLine(new string('=', 50));
    }

    static void MostrarCategoriaDesdeXml(XDocument xml)
    {
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        XElement? datos = null;
        foreach (var ns in namespaces)
        {
            datos = xml.Descendants(ns + "Datos").FirstOrDefault();
            if (datos != null) break;
        }

        if (datos == null) return;

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine($"ID: {ObtenerValorDesdeElemento(datos, "Id", namespaces)}");
        Console.WriteLine($"Nombre: {ObtenerValorDesdeElemento(datos, "Nombre", namespaces)}");
        Console.WriteLine($"Descripción: {ObtenerValorDesdeElemento(datos, "Descripcion", namespaces) ?? "N/A"}");
        Console.WriteLine(new string('=', 50));
    }

    static void MostrarProveedorDesdeXml(XDocument xml)
    {
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        XElement? datos = null;
        foreach (var ns in namespaces)
        {
            datos = xml.Descendants(ns + "Datos").FirstOrDefault();
            if (datos != null) break;
        }

        if (datos == null) return;

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine($"ID: {ObtenerValorDesdeElemento(datos, "Id", namespaces)}");
        Console.WriteLine($"Nombre: {ObtenerValorDesdeElemento(datos, "Nombre", namespaces)}");
        Console.WriteLine($"Teléfono: {ObtenerValorDesdeElemento(datos, "Telefono", namespaces) ?? "N/A"}");
        Console.WriteLine($"Email: {ObtenerValorDesdeElemento(datos, "Email", namespaces) ?? "N/A"}");
        Console.WriteLine($"Dirección: {ObtenerValorDesdeElemento(datos, "Direccion", namespaces) ?? "N/A"}");
        Console.WriteLine(new string('=', 50));
    }

    static string? ObtenerValor(XElement elemento, string nombreElemento, XNamespace[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            var valor = elemento.Element(ns + nombreElemento)?.Value;
            if (valor != null) return valor;
        }
        return null;
    }

    static string? ObtenerValorDesdeElemento(XElement elemento, string nombreElemento, XNamespace[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            var valor = elemento.Descendants(ns + nombreElemento).FirstOrDefault()?.Value;
            if (valor != null) return valor;
        }
        return null;
    }

    static string? ObtenerValorXml(XDocument xml, string nombreElemento, XNamespace[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            var valor = xml.Descendants(ns + nombreElemento).FirstOrDefault()?.Value;
            if (valor != null) return valor;
        }
        return null;
    }

    static bool ValidarToken()
    {
        if (string.IsNullOrEmpty(_token))
        {
            Console.WriteLine("\n❌ Debe autenticarse primero (opción 1)");
            return false;
        }
        return true;
    }

    static string LeerPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}