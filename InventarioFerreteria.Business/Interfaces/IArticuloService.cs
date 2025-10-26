// InventarioFerreteria.Business/Interfaces/IArticuloService.cs
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Interfaces;

public interface IArticuloService
{
    Task<RespuestaDTO<Articulo>> InsertarArticuloAsync(ArticuloDTO articuloDto);
    Task<RespuestaDTO<Articulo>> ObtenerArticuloPorIdAsync(int id);  // NUEVO
    Task<RespuestaDTO<Articulo>> ObtenerArticuloPorCodigoAsync(string codigo);
    Task<RespuestaDTO<List<Articulo>>> ObtenerTodosAsync();
    Task<RespuestaDTO<Articulo>> ActualizarArticuloAsync(int id, ArticuloDTO articuloDto);
    Task<RespuestaDTO<bool>> EliminarArticuloAsync(int id);  // NUEVO
}