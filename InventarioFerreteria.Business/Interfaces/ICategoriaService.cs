// InventarioFerreteria.Business/Interfaces/ICategoriaService.cs
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Interfaces;

public interface ICategoriaService
{
    Task<RespuestaDTO<Categoria>> InsertarCategoriaAsync(string nombre, string? descripcion);
    Task<RespuestaDTO<List<Categoria>>> ObtenerTodosAsync();
    Task<RespuestaDTO<Categoria>> ActualizarCategoriaAsync(int id, string nombre, string? descripcion);
    Task<RespuestaDTO<bool>> EliminarCategoriaAsync(int id);
}