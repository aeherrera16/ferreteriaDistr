// InventarioFerreteria.DataAccess/Interfaces/IProveedorRepository.cs
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Interfaces;

public interface IProveedorRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(int id);
    Task<List<Proveedor>> ObtenerTodosAsync();
    Task<int> InsertarAsync(Proveedor proveedor);
    Task<bool> ActualizarAsync(Proveedor proveedor);
    Task<bool> EliminarAsync(int id);
}