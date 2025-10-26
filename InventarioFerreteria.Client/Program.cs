// InventarioFerreteria.Client/Program.cs
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace InventarioFerreteria.Client;

class Program
{
    private static string? _token;
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ServiceUrl = "http://localhost:5233/InventarioService.asmx";

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Cliente SOAP - Sistema de Inventario Ferretería ===\n");

        bool continuar = true;

        while (continuar)
        {
            try
            {
                Console.WriteLine("\n--- MENÚ PRINCIPAL ---");
                Console.WriteLine("1. Autenticar");
                Console.WriteLine("2. Insertar Artículo");
                Console.WriteLine("3. Consultar Artículo por Código");
                Console.WriteLine("4. Listar Todos los Artículos");
                Console.WriteLine("5. Salir");
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
                        continuar = false;
                        Console.WriteLine("\n¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("\nOpción inválida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
            }
        }
    }

    static async Task AutenticarAsync()
    {
        Console.WriteLine("\n--- AUTENTICACIÓN ---");
        Console.Write("Usuario: ");
        var usuario = Console.ReadLine() ?? "";
        
        Console.Write("Contraseña: ");
        var password = LeerPassword();

        // CAMBIADO: AutenticarAsync -> Autenticar
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
            
            // Buscar en múltiples namespaces posibles
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

    static async Task InsertarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n--- INSERTAR NUEVO ARTÍCULO ---");

        Console.Write("Código: ");
        var codigo = Console.ReadLine() ?? "";

        Console.Write("Nombre: ");
        var nombre = Console.ReadLine() ?? "";

        Console.Write("Descripción (opcional): ");
        var descripcion = Console.ReadLine() ?? "";

        Console.Write("ID Categoría (opcional, Enter para omitir): ");
        var categoriaIdStr = Console.ReadLine();

        Console.Write("Precio Compra: ");
        var precioCompra = Console.ReadLine() ?? "0";

        Console.Write("Precio Venta: ");
        var precioVenta = Console.ReadLine() ?? "0";

        Console.Write("Stock: ");
        var stock = Console.ReadLine() ?? "0";

        Console.Write("Stock Mínimo: ");
        var stockMinimo = Console.ReadLine() ?? "5";

        Console.Write("ID Proveedor (opcional, Enter para omitir): ");
        var proveedorIdStr = Console.ReadLine();

        // Construir elementos opcionales
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

        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/IInventarioSoapService/InsertarArticulo");

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

            foreach (var ns in namespaces)
            {
                exito = exito ?? xml.Descendants(ns + "Exito").FirstOrDefault()?.Value;
                mensaje = mensaje ?? xml.Descendants(ns + "Mensaje").FirstOrDefault()?.Value;
            }

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}");
                MostrarArticuloDesdeXml(xml);
            }
            else
            {
                Console.WriteLine($"\n❌ {mensaje}");
                // Buscar errores
                foreach (var ns in namespaces)
                {
                    var errores = xml.Descendants(ns + "string");
                    foreach (var error in errores)
                    {
                        Console.WriteLine($"  - {error.Value}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
        }
    }

    static async Task ConsultarArticuloAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n--- CONSULTAR ARTÍCULO ---");
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

        var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tempuri.org/IInventarioSoapService/ConsultarArticuloPorCodigo");

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

            foreach (var ns in namespaces)
            {
                exito = exito ?? xml.Descendants(ns + "Exito").FirstOrDefault()?.Value;
                mensaje = mensaje ?? xml.Descendants(ns + "Mensaje").FirstOrDefault()?.Value;
            }

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}");
                MostrarArticuloDesdeXml(xml);
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

    static async Task ListarArticulosAsync()
    {
        if (!ValidarToken()) return;

        Console.WriteLine("\n--- LISTA DE ARTÍCULOS ---");

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

            string? exito = null;
            string? mensaje = null;

            foreach (var ns in namespaces)
            {
                exito = exito ?? xml.Descendants(ns + "Exito").FirstOrDefault()?.Value;
                mensaje = mensaje ?? xml.Descendants(ns + "Mensaje").FirstOrDefault()?.Value;
            }

            if (exito == "true")
            {
                Console.WriteLine($"\n✅ {mensaje}\n");
                Console.WriteLine($"{"Código",-15} {"Nombre",-30} {"Stock",10} {"Precio Venta",15}");
                Console.WriteLine(new string('-', 70));

                // Buscar artículos en todos los namespaces
                IEnumerable<XElement>? articulos = null;
                foreach (var ns in namespaces)
                {
                    articulos = xml.Descendants(ns + "Articulo");
                    if (articulos.Any()) break;
                }

                if (articulos != null)
                {
                    foreach (var articulo in articulos)
                    {
                        var artCodigo = ObtenerValor(articulo, "Codigo", namespaces);
                        var artNombre = ObtenerValor(articulo, "Nombre", namespaces);
                        var artStock = ObtenerValor(articulo, "Stock", namespaces);
                        var artPrecioVenta = ObtenerValor(articulo, "PrecioVenta", namespaces);
                        var artRequiere = ObtenerValor(articulo, "RequiereReposicion", namespaces) == "true" ? " ⚠️" : "";

                        if (decimal.TryParse(artPrecioVenta, out var precio))
                        {
                            Console.WriteLine($"{artCodigo,-15} {artNombre,-30} {artStock,10}{artRequiere} ${precio,14:F2}");
                        }
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