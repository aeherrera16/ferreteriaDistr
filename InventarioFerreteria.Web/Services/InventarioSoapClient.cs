// InventarioFerreteria.Web/Services/InventarioSoapClient.cs
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace InventarioFerreteria.Web.Services;

public class InventarioSoapClient
{
    private readonly HttpClient _httpClient;
    private const string ServiceUrl = "http://localhost:5000/InventarioService.asmx";
    
    public string? Token { get; set; }

    public InventarioSoapClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // ============ AUTENTICACIÓN ============
    public async Task<RespuestaDto<string>> AutenticarAsync(string usuario, string password)
    {
        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <Autenticar xmlns=""http://tempuri.org/"">
      <nombreUsuario>{usuario}</nombreUsuario>
      <password>{password}</password>
    </Autenticar>
  </soap:Body>
</soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "Autenticar");
        
        if (resultado.Exito)
        {
            Token = resultado.Datos;
            return new RespuestaDto<string> { Exito = true, Mensaje = resultado.Mensaje, Datos = Token };
        }
        
        return new RespuestaDto<string> { Exito = false, Mensaje = resultado.Mensaje };
    }

    // ============ ARTÍCULOS ============
    public async Task<RespuestaDto<ArticuloDto>> InsertarArticuloAsync(ArticuloInputDto articulo)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<ArticuloDto> { Exito = false, Mensaje = "No autenticado" };

        var categoriaIdXml = articulo.CategoriaId.HasValue 
            ? $"<categoriaId>{articulo.CategoriaId.Value}</categoriaId>" 
            : "<categoriaId xsi:nil=\"true\" />";
            
        var proveedorIdXml = articulo.ProveedorId.HasValue 
            ? $"<proveedorId>{articulo.ProveedorId.Value}</proveedorId>" 
            : "<proveedorId xsi:nil=\"true\" />";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <soap:Body>
    <InsertarArticulo xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
      <codigo>{articulo.Codigo}</codigo>
      <nombre>{articulo.Nombre}</nombre>
      <descripcion>{articulo.Descripcion}</descripcion>
      {categoriaIdXml}
      <precioCompra>{articulo.PrecioCompra.ToString(CultureInfo.InvariantCulture)}</precioCompra>
      <precioVenta>{articulo.PrecioVenta.ToString(CultureInfo.InvariantCulture)}</precioVenta>
      <stock>{articulo.Stock}</stock>
      <stockMinimo>{articulo.StockMinimo}</stockMinimo>
      {proveedorIdXml}
    </InsertarArticulo>
  </soap:Body>
</soap:Envelope>";

        return await EjecutarSoapConDatosAsync<ArticuloDto>(soapRequest, "InsertarArticulo", ParsearArticulo);
    }

    public async Task<RespuestaDto<List<ArticuloDto>>> ObtenerTodosArticulosAsync()
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<List<ArticuloDto>> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerTodosArticulos xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
    </ObtenerTodosArticulos>
  </soap:Body>
</soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "ObtenerTodosArticulos");
        
        if (resultado.Exito && resultado.XmlRespuesta != null)
        {
            var articulos = ParsearListaArticulos(resultado.XmlRespuesta);
            return new RespuestaDto<List<ArticuloDto>> 
            { 
                Exito = true, 
                Mensaje = resultado.Mensaje, 
                Datos = articulos 
            };
        }
        
        return new RespuestaDto<List<ArticuloDto>> { Exito = false, Mensaje = resultado.Mensaje };
    }

    public async Task<RespuestaDto<ArticuloDto>> ActualizarArticuloAsync(int id, ArticuloInputDto articulo)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<ArticuloDto> { Exito = false, Mensaje = "No autenticado" };

        var categoriaIdXml = articulo.CategoriaId.HasValue 
            ? $"<categoriaId>{articulo.CategoriaId.Value}</categoriaId>" 
            : "<categoriaId xsi:nil=\"true\" />";
            
        var proveedorIdXml = articulo.ProveedorId.HasValue 
            ? $"<proveedorId>{articulo.ProveedorId.Value}</proveedorId>" 
            : "<proveedorId xsi:nil=\"true\" />";

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <soap:Body>
    <ActualizarArticulo xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
      <id>{id}</id>
      <codigo>{articulo.Codigo}</codigo>
      <nombre>{articulo.Nombre}</nombre>
      <descripcion>{articulo.Descripcion}</descripcion>
      {categoriaIdXml}
      <precioCompra>{articulo.PrecioCompra.ToString(CultureInfo.InvariantCulture)}</precioCompra>
      <precioVenta>{articulo.PrecioVenta.ToString(CultureInfo.InvariantCulture)}</precioVenta>
      <stock>{articulo.Stock}</stock>
      <stockMinimo>{articulo.StockMinimo}</stockMinimo>
      {proveedorIdXml}
    </ActualizarArticulo>
  </soap:Body>
</soap:Envelope>";

        return await EjecutarSoapConDatosAsync<ArticuloDto>(soapRequest, "ActualizarArticulo", ParsearArticulo);
    }

    public async Task<RespuestaDto<bool>> EliminarArticuloAsync(int id)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<bool> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <EliminarArticulo xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
      <id>{id}</id>
    </EliminarArticulo>
  </soap:Body>
</soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "EliminarArticulo");
        return new RespuestaDto<bool> { Exito = resultado.Exito, Mensaje = resultado.Mensaje, Datos = resultado.Exito };
    }

    // ============ CATEGORÍAS ============
    public async Task<RespuestaDto<CategoriaDto>> InsertarCategoriaAsync(string nombre, string? descripcion)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<CategoriaDto> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <InsertarCategoria xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
      <nombre>{nombre}</nombre>
      <descripcion>{descripcion}</descripcion>
    </InsertarCategoria>
  </soap:Body>
</soap:Envelope>";

        return await EjecutarSoapConDatosAsync<CategoriaDto>(soapRequest, "InsertarCategoria", ParsearCategoria);
    }

    public async Task<RespuestaDto<List<CategoriaDto>>> ObtenerTodasCategoriasAsync()
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<List<CategoriaDto>> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerTodasCategorias xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
    </ObtenerTodasCategorias>
  </soap:Body>
</soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "ObtenerTodasCategorias");
        
        if (resultado.Exito && resultado.XmlRespuesta != null)
        {
            var categorias = ParsearListaCategorias(resultado.XmlRespuesta);
            return new RespuestaDto<List<CategoriaDto>> { Exito = true, Mensaje = resultado.Mensaje, Datos = categorias };
        }
        
        return new RespuestaDto<List<CategoriaDto>> { Exito = false, Mensaje = resultado.Mensaje };
    }

        

    public async Task<RespuestaDto<CategoriaDto>> ActualizarCategoriaAsync(int id, string nombre, string? descripcion)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<CategoriaDto> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <ActualizarCategoria xmlns=""http://tempuri.org/"">
        <token>{Token}</token>
        <id>{id}</id>
        <nombre>{nombre}</nombre>
        <descripcion>{descripcion}</descripcion>
        </ActualizarCategoria>
    </soap:Body>
    </soap:Envelope>";

        return await EjecutarSoapConDatosAsync<CategoriaDto>(soapRequest, "ActualizarCategoria", ParsearCategoria);
    }

    public async Task<RespuestaDto<bool>> EliminarCategoriaAsync(int id)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<bool> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <EliminarCategoria xmlns=""http://tempuri.org/"">
        <token>{Token}</token>
        <id>{id}</id>
        </EliminarCategoria>
    </soap:Body>
    </soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "EliminarCategoria");
        return new RespuestaDto<bool> { Exito = resultado.Exito, Mensaje = resultado.Mensaje, Datos = resultado.Exito };
    }

    // ============ PROVEEDORES ============
    public async Task<RespuestaDto<ProveedorDto>> InsertarProveedorAsync(string nombre, string? telefono, string? email, string? direccion)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<ProveedorDto> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <InsertarProveedor xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
      <nombre>{nombre}</nombre>
      <telefono>{telefono}</telefono>
      <email>{email}</email>
      <direccion>{direccion}</direccion>
    </InsertarProveedor>
  </soap:Body>
</soap:Envelope>";

        return await EjecutarSoapConDatosAsync<ProveedorDto>(soapRequest, "InsertarProveedor", ParsearProveedor);
    }

    public async Task<RespuestaDto<List<ProveedorDto>>> ObtenerTodosProveedoresAsync()
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<List<ProveedorDto>> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerTodosProveedores xmlns=""http://tempuri.org/"">
      <token>{Token}</token>
    </ObtenerTodosProveedores>
  </soap:Body>
</soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "ObtenerTodosProveedores");
        
        if (resultado.Exito && resultado.XmlRespuesta != null)
        {
            var proveedores = ParsearListaProveedores(resultado.XmlRespuesta);
            return new RespuestaDto<List<ProveedorDto>> { Exito = true, Mensaje = resultado.Mensaje, Datos = proveedores };
        }
        
        return new RespuestaDto<List<ProveedorDto>> { Exito = false, Mensaje = resultado.Mensaje };
    }

    // Agregar después del método ObtenerTodosProveedoresAsync()

    public async Task<RespuestaDto<ProveedorDto>> ActualizarProveedorAsync(int id, string nombre, string? telefono, string? email, string? direccion)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<ProveedorDto> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <ActualizarProveedor xmlns=""http://tempuri.org/"">
        <token>{Token}</token>
        <id>{id}</id>
        <nombre>{nombre}</nombre>
        <telefono>{telefono}</telefono>
        <email>{email}</email>
        <direccion>{direccion}</direccion>
        </ActualizarProveedor>
    </soap:Body>
    </soap:Envelope>";

        return await EjecutarSoapConDatosAsync<ProveedorDto>(soapRequest, "ActualizarProveedor", ParsearProveedor);
    }

    public async Task<RespuestaDto<bool>> EliminarProveedorAsync(int id)
    {
        if (string.IsNullOrEmpty(Token))
            return new RespuestaDto<bool> { Exito = false, Mensaje = "No autenticado" };

        var soapRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
    <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <EliminarProveedor xmlns=""http://tempuri.org/"">
        <token>{Token}</token>
        <id>{id}</id>
        </EliminarProveedor>
    </soap:Body>
    </soap:Envelope>";

        var resultado = await EjecutarSoapAsync(soapRequest, "EliminarProveedor");
        return new RespuestaDto<bool> { Exito = resultado.Exito, Mensaje = resultado.Mensaje, Datos = resultado.Exito };
    }

    // ============ MÉTODOS PRIVADOS ============
    private async Task<(bool Exito, string Mensaje, string? Datos, XDocument? XmlRespuesta)> EjecutarSoapAsync(string soapRequest, string operacion)
    {
        try
        {
            var content = new StringContent(soapRequest, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", $"http://tempuri.org/IInventarioSoapService/{operacion}");

            var response = await _httpClient.PostAsync(ServiceUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            var xml = XDocument.Parse(responseContent);
            
            XNamespace[] namespaces = {
                XNamespace.Get("http://tempuri.org/"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities.DTOs"),
                XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
                XNamespace.None
            };

            var exito = ObtenerValorXml(xml, "Exito", namespaces);
            var mensaje = ObtenerValorXml(xml, "Mensaje", namespaces);
            var datos = ObtenerValorXml(xml, "Datos", namespaces);

            return (exito == "true", mensaje ?? "Sin mensaje", datos, xml);
        }
        catch (Exception ex)
        {
            return (false, $"Error: {ex.Message}", null, null);
        }
    }

    private async Task<RespuestaDto<T>> EjecutarSoapConDatosAsync<T>(string soapRequest, string operacion, Func<XDocument, T?> parseFunc)
    {
        var resultado = await EjecutarSoapAsync(soapRequest, operacion);
        
        if (resultado.Exito && resultado.XmlRespuesta != null)
        {
            var datos = parseFunc(resultado.XmlRespuesta);
            return new RespuestaDto<T> { Exito = true, Mensaje = resultado.Mensaje, Datos = datos };
        }
        
        return new RespuestaDto<T> { Exito = false, Mensaje = resultado.Mensaje };
    }

    private string? ObtenerValorXml(XDocument xml, string nombreElemento, XNamespace[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            var valor = xml.Descendants(ns + nombreElemento).FirstOrDefault()?.Value;
            if (valor != null) return valor;
        }
        return null;
    }

    private string? ObtenerValor(XElement elemento, string nombreElemento, XNamespace[] namespaces)
    {
        foreach (var ns in namespaces)
        {
            var valor = elemento.Element(ns + nombreElemento)?.Value ?? 
                       elemento.Descendants(ns + nombreElemento).FirstOrDefault()?.Value;
            if (valor != null) return valor;
        }
        return null;
    }

    // ============ PARSEADORES ============
    private ArticuloDto? ParsearArticulo(XDocument xml)
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

        if (datos == null) return null;

        return new ArticuloDto
        {
            Id = int.Parse(ObtenerValor(datos, "Id", namespaces) ?? "0"),
            Codigo = ObtenerValor(datos, "Codigo", namespaces) ?? "",
            Nombre = ObtenerValor(datos, "Nombre", namespaces) ?? "",
            Descripcion = ObtenerValor(datos, "Descripcion", namespaces),
            CategoriaId = int.TryParse(ObtenerValor(datos, "CategoriaId", namespaces), out var catId) ? catId : null,
            PrecioCompra = decimal.Parse(ObtenerValor(datos, "PrecioCompra", namespaces) ?? "0", CultureInfo.InvariantCulture),
            PrecioVenta = decimal.Parse(ObtenerValor(datos, "PrecioVenta", namespaces) ?? "0", CultureInfo.InvariantCulture),
            Stock = int.Parse(ObtenerValor(datos, "Stock", namespaces) ?? "0"),
            StockMinimo = int.Parse(ObtenerValor(datos, "StockMinimo", namespaces) ?? "0"),
            ProveedorId = int.TryParse(ObtenerValor(datos, "ProveedorId", namespaces), out var provId) ? provId : null,
            RequiereReposicion = ObtenerValor(datos, "RequiereReposicion", namespaces) == "true"
        };
    }

    private List<ArticuloDto> ParsearListaArticulos(XDocument xml)
    {
        var articulos = new List<ArticuloDto>();
        
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        IEnumerable<XElement>? elementos = null;
        foreach (var ns in namespaces)
        {
            elementos = xml.Descendants(ns + "Articulo");
            if (elementos.Any()) break;
        }

        if (elementos != null)
        {
            foreach (var elem in elementos)
            {
                articulos.Add(new ArticuloDto
                {
                    Id = int.Parse(ObtenerValor(elem, "Id", namespaces) ?? "0"),
                    Codigo = ObtenerValor(elem, "Codigo", namespaces) ?? "",
                    Nombre = ObtenerValor(elem, "Nombre", namespaces) ?? "",
                    Descripcion = ObtenerValor(elem, "Descripcion", namespaces),
                    CategoriaId = int.TryParse(ObtenerValor(elem, "CategoriaId", namespaces), out var catId) ? catId : null,
                    PrecioCompra = decimal.Parse(ObtenerValor(elem, "PrecioCompra", namespaces) ?? "0", CultureInfo.InvariantCulture),
                    PrecioVenta = decimal.Parse(ObtenerValor(elem, "PrecioVenta", namespaces) ?? "0", CultureInfo.InvariantCulture),
                    Stock = int.Parse(ObtenerValor(elem, "Stock", namespaces) ?? "0"),
                    StockMinimo = int.Parse(ObtenerValor(elem, "StockMinimo", namespaces) ?? "0"),
                    ProveedorId = int.TryParse(ObtenerValor(elem, "ProveedorId", namespaces), out var provId) ? provId : null,
                    RequiereReposicion = ObtenerValor(elem, "RequiereReposicion", namespaces) == "true"
                });
            }
        }

        return articulos;
    }

    private CategoriaDto? ParsearCategoria(XDocument xml)
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

        if (datos == null) return null;

        return new CategoriaDto
        {
            Id = int.Parse(ObtenerValor(datos, "Id", namespaces) ?? "0"),
            Nombre = ObtenerValor(datos, "Nombre", namespaces) ?? "",
            Descripcion = ObtenerValor(datos, "Descripcion", namespaces)
        };
    }

    private List<CategoriaDto> ParsearListaCategorias(XDocument xml)
    {
        var categorias = new List<CategoriaDto>();
        
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        IEnumerable<XElement>? elementos = null;
        foreach (var ns in namespaces)
        {
            elementos = xml.Descendants(ns + "Categoria");
            if (elementos.Any()) break;
        }

        if (elementos != null)
        {
            foreach (var elem in elementos)
            {
                categorias.Add(new CategoriaDto
                {
                    Id = int.Parse(ObtenerValor(elem, "Id", namespaces) ?? "0"),
                    Nombre = ObtenerValor(elem, "Nombre", namespaces) ?? "",
                    Descripcion = ObtenerValor(elem, "Descripcion", namespaces)
                });
            }
        }

        return categorias;
    }

    private ProveedorDto? ParsearProveedor(XDocument xml)
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

        if (datos == null) return null;

        return new ProveedorDto
        {
            Id = int.Parse(ObtenerValor(datos, "Id", namespaces) ?? "0"),
            Nombre = ObtenerValor(datos, "Nombre", namespaces) ?? "",
            Telefono = ObtenerValor(datos, "Telefono", namespaces),
            Email = ObtenerValor(datos, "Email", namespaces),
            Direccion = ObtenerValor(datos, "Direccion", namespaces)
        };
    }

    private List<ProveedorDto> ParsearListaProveedores(XDocument xml)
    {
        var proveedores = new List<ProveedorDto>();
        
        XNamespace[] namespaces = {
            XNamespace.Get("http://tempuri.org/"),
            XNamespace.Get("http://schemas.datacontract.org/2004/07/InventarioFerreteria.Entities"),
            XNamespace.None
        };

        IEnumerable<XElement>? elementos = null;
        foreach (var ns in namespaces)
        {
            elementos = xml.Descendants(ns + "Proveedor");
            if (elementos.Any()) break;
        }

        if (elementos != null)
        {
            foreach (var elem in elementos)
            {
                proveedores.Add(new ProveedorDto
                {
                    Id = int.Parse(ObtenerValor(elem, "Id", namespaces) ?? "0"),
                    Nombre = ObtenerValor(elem, "Nombre", namespaces) ?? "",
                    Telefono = ObtenerValor(elem, "Telefono", namespaces),
                    Email = ObtenerValor(elem, "Email", namespaces),
                    Direccion = ObtenerValor(elem, "Direccion", namespaces)
                });
            }
        }

        return proveedores;
    }
}

// ============ MODELOS DTO ============
public class RespuestaDto<T>
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public T? Datos { get; set; }
}

public class ArticuloDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int? CategoriaId { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public int? ProveedorId { get; set; }
    public bool RequiereReposicion { get; set; }
}

public class ArticuloInputDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int? CategoriaId { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; } = 5;
    public int? ProveedorId { get; set; }
}

public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
}

public class ProveedorDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
}