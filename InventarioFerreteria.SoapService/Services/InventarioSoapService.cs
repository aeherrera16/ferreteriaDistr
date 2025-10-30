// InventarioFerreteria.SoapService/Services/InventarioSoapServiceImpl.cs
using System.IdentityModel.Tokens.Jwt;
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.SoapService.Services;

public class InventarioSoapService : IInventarioSoapService
{
    private readonly IArticuloService _articuloService;
    private readonly IAutenticacionService _autenticacionService;
    private readonly ICategoriaService _categoriaService;
    private readonly IProveedorService _proveedorService;

    public InventarioSoapService(
        IArticuloService articuloService,
        IAutenticacionService autenticacionService,
        ICategoriaService categoriaService,
        IProveedorService proveedorService)
    {
        _articuloService = articuloService;
        _autenticacionService = autenticacionService;
        _categoriaService = categoriaService;
        _proveedorService = proveedorService;
    }

    // ============ AUTENTICACIÓN ============
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
                respuesta.Exito = false;
                respuesta.Mensaje = resultadoAuth.Mensaje;
                respuesta.Errores = resultadoAuth.Errores;
                return respuesta;
            }

            var token = _autenticacionService.GenerarToken(resultadoAuth.Datos);

            respuesta.Exito = true;
            respuesta.Mensaje = "Token generado exitosamente";
            respuesta.Datos = token;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error en autenticación";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    // ============ ARTÍCULOS ============
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

    public async Task<RespuestaDTO<Articulo>> ConsultarArticuloPorCodigoAsync(string token, string codigo)
    {
        var respuesta = new RespuestaDTO<Articulo>();

        try
        {
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

    public async Task<RespuestaDTO<Articulo>> ActualizarArticuloAsync(
        string token,
        int id,
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

            return await _articuloService.ActualizarArticuloAsync(id, articuloDto);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar artículo";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<bool>> EliminarArticuloAsync(string token, int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            // CORREGIDO: Ahora llama al método correcto
            return await _articuloService.EliminarArticuloAsync(id);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar artículo";
            respuesta.Errores = new List<string> { ex.Message };
            respuesta.Datos = false;
        }

        return respuesta;
    }

    // ============ CATEGORÍAS ============
    public async Task<RespuestaDTO<Categoria>> InsertarCategoriaAsync(
        string token,
        string nombre,
        string? descripcion)
    {
        var respuesta = new RespuestaDTO<Categoria>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _categoriaService.InsertarCategoriaAsync(nombre, descripcion);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar categoría";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<List<Categoria>>> ObtenerTodasCategoriasAsync(string token)
    {
        var respuesta = new RespuestaDTO<List<Categoria>>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _categoriaService.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener categorías";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<Categoria>> ActualizarCategoriaAsync(
        string token,
        int id,
        string nombre,
        string? descripcion)
    {
        var respuesta = new RespuestaDTO<Categoria>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _categoriaService.ActualizarCategoriaAsync(id, nombre, descripcion);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar categoría";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<bool>> EliminarCategoriaAsync(string token, int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _categoriaService.EliminarCategoriaAsync(id);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar categoría";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    // ============ PROVEEDORES ============
    public async Task<RespuestaDTO<Proveedor>> InsertarProveedorAsync(
        string token,
        string nombre,
        string? telefono,
        string? email,
        string? direccion)
    {
        var respuesta = new RespuestaDTO<Proveedor>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _proveedorService.InsertarProveedorAsync(nombre, telefono, email, direccion);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar proveedor";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<List<Proveedor>>> ObtenerTodosProveedoresAsync(string token)
    {
        var respuesta = new RespuestaDTO<List<Proveedor>>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _proveedorService.ObtenerTodosAsync();
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener proveedores";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<Proveedor>> ActualizarProveedorAsync(
        string token,
        int id,
        string nombre,
        string? telefono,
        string? email,
        string? direccion)
    {
        var respuesta = new RespuestaDTO<Proveedor>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _proveedorService.ActualizarProveedorAsync(id, nombre, telefono, email, direccion);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar proveedor";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    public async Task<RespuestaDTO<bool>> EliminarProveedorAsync(string token, int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            if (!ValidarToken(token))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Token inválido o expirado";
                return respuesta;
            }

            return await _proveedorService.EliminarProveedorAsync(id);
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar proveedor";
            respuesta.Errores = new List<string> { ex.Message };
            return respuesta;
        }
    }

    // ============ VALIDACIÓN ============
    private bool ValidarToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}