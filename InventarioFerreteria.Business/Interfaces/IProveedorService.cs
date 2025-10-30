// InventarioFerreteria.Business/Interfaces/IProveedorService.cs
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Interfaces;

public interface IProveedorService
{
    Task<RespuestaDTO<Proveedor>> InsertarProveedorAsync(string nombre, string? telefono, string? email, string? direccion);
    Task<RespuestaDTO<List<Proveedor>>> ObtenerTodosAsync();
    Task<RespuestaDTO<Proveedor>> ActualizarProveedorAsync(int id, string nombre, string? telefono, string? email, string? direccion);
    Task<RespuestaDTO<bool>> EliminarProveedorAsync(int id);
}