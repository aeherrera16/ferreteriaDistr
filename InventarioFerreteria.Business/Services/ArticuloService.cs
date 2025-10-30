using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Services;

public class ArticuloService : IArticuloService
{
    private readonly IArticuloRepository _articuloRepository;

    public ArticuloService(IArticuloRepository articuloRepository)
    {
        _articuloRepository = articuloRepository;
    }

    public async Task<RespuestaDTO<Articulo>> InsertarArticuloAsync(ArticuloDTO articuloDto)
    {
        var respuesta = new RespuestaDTO<Articulo>();
        var errores = new List<string>();

        try
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(articuloDto.Codigo))
                errores.Add("El código es requerido");

            if (string.IsNullOrWhiteSpace(articuloDto.Nombre))
                errores.Add("El nombre es requerido");

            if (articuloDto.PrecioCompra <= 0)
                errores.Add("El precio de compra debe ser mayor a cero");

            if (articuloDto.PrecioVenta <= 0)
                errores.Add("El precio de venta debe ser mayor a cero");

            if (articuloDto.PrecioVenta < articuloDto.PrecioCompra)
                errores.Add("El precio de venta debe ser mayor o igual al precio de compra");

            if (articuloDto.Stock < 0)
                errores.Add("El stock no puede ser negativo");

            // Verificar si el código ya existe
            if (await _articuloRepository.ExisteCodigoAsync(articuloDto.Codigo))
                errores.Add($"El código '{articuloDto.Codigo}' ya está registrado");

            if (errores.Any())
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Errores de validación";
                respuesta.Errores = errores;
                return respuesta;
            }

            // Mapear DTO a entidad
            var articulo = new Articulo
            {
                Codigo = articuloDto.Codigo.Trim(),
                Nombre = articuloDto.Nombre.Trim(),
                Descripcion = articuloDto.Descripcion?.Trim(),
                CategoriaId = articuloDto.CategoriaId,
                PrecioCompra = articuloDto.PrecioCompra,
                PrecioVenta = articuloDto.PrecioVenta,
                Stock = articuloDto.Stock,
                StockMinimo = articuloDto.StockMinimo,
                ProveedorId = articuloDto.ProveedorId,
                Activo = true
            };

            var id = await _articuloRepository.InsertarAsync(articulo);
            var articuloInsertado = await _articuloRepository.ObtenerPorIdAsync(id);

            respuesta.Exito = true;
            respuesta.Mensaje = "Artículo insertado correctamente";
            respuesta.Datos = articuloInsertado;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar el artículo";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<Articulo>> ObtenerArticuloPorCodigoAsync(string codigo)
    {
        var respuesta = new RespuestaDTO<Articulo>();

        try
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "El código es requerido";
                return respuesta;
            }

            var articulo = await _articuloRepository.ObtenerPorCodigoAsync(codigo);

            if (articulo == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = $"No se encontró el artículo con código '{codigo}'";
                return respuesta;
            }

            respuesta.Exito = true;
            respuesta.Mensaje = "Artículo encontrado";
            respuesta.Datos = articulo;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al consultar el artículo";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    // InventarioFerreteria.Business/Services/ArticuloService.cs
    // ... (después del método ObtenerArticuloPorCodigoAsync, agrega estos dos)

    public async Task<RespuestaDTO<Articulo>> ObtenerArticuloPorIdAsync(int id)
    {
        var respuesta = new RespuestaDTO<Articulo>();

        try
        {
            var articulo = await _articuloRepository.ObtenerPorIdAsync(id);

            if (articulo == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = $"No se encontró el artículo con ID {id}";
                return respuesta;
            }

            respuesta.Exito = true;
            respuesta.Mensaje = "Artículo encontrado";
            respuesta.Datos = articulo;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al consultar el artículo";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<bool>> EliminarArticuloAsync(int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            var articuloExistente = await _articuloRepository.ObtenerPorIdAsync(id);

            if (articuloExistente == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Artículo no encontrado";
                respuesta.Datos = false;
                return respuesta;
            }

            var eliminado = await _articuloRepository.EliminarAsync(id);

            respuesta.Exito = eliminado;
            respuesta.Mensaje = eliminado ? "Artículo eliminado correctamente" : "No se pudo eliminar el artículo";
            respuesta.Datos = eliminado;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar el artículo";
            respuesta.Errores = new List<string> { ex.Message };
            respuesta.Datos = false;
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<List<Articulo>>> ObtenerTodosAsync()
    {
        var respuesta = new RespuestaDTO<List<Articulo>>();

        try
        {
            var articulos = await _articuloRepository.ObtenerTodosAsync();

            respuesta.Exito = true;
            respuesta.Mensaje = $"Se encontraron {articulos.Count} artículos";
            respuesta.Datos = articulos;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener los artículos";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<Articulo>> ActualizarArticuloAsync(int id, ArticuloDTO articuloDto)
    {
        var respuesta = new RespuestaDTO<Articulo>();
        var errores = new List<string>();

        try
        {
            var articuloExistente = await _articuloRepository.ObtenerPorIdAsync(id);

            if (articuloExistente == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Artículo no encontrado";
                return respuesta;
            }

            // Validaciones
            if (articuloDto.PrecioVenta < articuloDto.PrecioCompra)
                errores.Add("El precio de venta debe ser mayor o igual al precio de compra");

            if (await _articuloRepository.ExisteCodigoAsync(articuloDto.Codigo, id))
                errores.Add($"El código '{articuloDto.Codigo}' ya está registrado en otro artículo");

            if (errores.Any())
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Errores de validación";
                respuesta.Errores = errores;
                return respuesta;
            }

            // Actualizar propiedades
            articuloExistente.Codigo = articuloDto.Codigo.Trim();
            articuloExistente.Nombre = articuloDto.Nombre.Trim();
            articuloExistente.Descripcion = articuloDto.Descripcion?.Trim();
            articuloExistente.CategoriaId = articuloDto.CategoriaId;
            articuloExistente.PrecioCompra = articuloDto.PrecioCompra;
            articuloExistente.PrecioVenta = articuloDto.PrecioVenta;
            articuloExistente.Stock = articuloDto.Stock;
            articuloExistente.StockMinimo = articuloDto.StockMinimo;
            articuloExistente.ProveedorId = articuloDto.ProveedorId;

            await _articuloRepository.ActualizarAsync(articuloExistente);

            respuesta.Exito = true;
            respuesta.Mensaje = "Artículo actualizado correctamente";
            respuesta.Datos = articuloExistente;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar el artículo";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }
}