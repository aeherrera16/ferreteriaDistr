// InventarioFerreteria.SoapService/Services/InventarioSoapService.cs
using System.IdentityModel.Tokens.Jwt;
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.SoapService.Services;

public class InventarioSoapService : IInventarioSoapService
{
    private readonly IArticuloService _articuloService;
    private readonly IAutenticacionService _autenticacionService;

    public InventarioSoapService(
        IArticuloService articuloService,
        IAutenticacionService autenticacionService)
    {
        _articuloService = articuloService;
        _autenticacionService = autenticacionService;
    }

    public async Task<RespuestaDTO<string>> AutenticarAsync(string nombreUsuario, string password)
{
    var respuesta = new RespuestaDTO<string>();

    try
    {
        var autenticacionDto = new AutenticacionDTO
        {
            NombreUsuario = nombreUsuario,
            Password = password
        };

        var resultadoAuth = await _autenticacionService.AutenticarAsync(autenticacionDto);

        if (!resultadoAuth.Exito || resultadoAuth.Datos == null)
        {
            Console.WriteLine($"[SOAP] Autenticación falló");
            respuesta.Exito = false;
            respuesta.Mensaje = resultadoAuth.Mensaje;
            respuesta.Errores = resultadoAuth.Errores;
            return respuesta;
        }

        var token = _autenticacionService.GenerarToken(resultadoAuth.Datos);

        respuesta.Exito = true;
        respuesta.Mensaje = "Token generado exitosamente";
        respuesta.Datos = token;
        
        Console.WriteLine($"[SOAP] Respuesta preparada:");
        Console.WriteLine($"  Exito: {respuesta.Exito}");
        Console.WriteLine($"  Mensaje: {respuesta.Mensaje}");
        Console.WriteLine($"  Datos (token): {respuesta.Datos?[..Math.Min(50, respuesta.Datos?.Length ?? 0)]}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[SOAP] Excepción: {ex.Message}");
        respuesta.Exito = false;
        respuesta.Mensaje = "Error en autenticación";
        respuesta.Errores = new List<string> { ex.Message };
    }

    return respuesta;
}

    public async Task<RespuestaDTO<Articulo>> InsertarArticuloAsync(
        string token,
        string codigo,
        string nombre,
        string? descripcion,
        int? categoriaId,
        decimal precioCompra,
        decimal precioVenta,
        int stock,
        int stockMinimo,
        int? proveedorId)
    {
        var respuesta = new RespuestaDTO<Articulo>();

        try
        {
            // Validar token
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            var articuloDto = new ArticuloDTO
            {
                Codigo = codigo,
                Nombre = nombre,
                Descripcion = descripcion,
                CategoriaId = categoriaId,
                PrecioCompra = precioCompra,
                PrecioVenta = precioVenta,
                Stock = stock,
                StockMinimo = stockMinimo,
                ProveedorId = proveedorId
            };

            return await _articuloService.InsertarArticuloAsync(articuloDto);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar artículo";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<Articulo>> ConsultarArticuloPorCodigoAsync(
        string token,
        string codigo)
    {
        var respuesta = new RespuestaDTO<Articulo>();

        try
        {
            // Validar token
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _articuloService.ObtenerArticuloPorCodigoAsync(codigo);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al consultar artículo";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<List<Articulo>>> ObtenerTodosArticulosAsync(string token)
    {
        var respuesta = new RespuestaDTO<List<Articulo>>();

        try
        {
            // Validar token
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _articuloService.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener artículos";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    private bool ValidarToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            
            // Verificar si el token ha expirado
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}