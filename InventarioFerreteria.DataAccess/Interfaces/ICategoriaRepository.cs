// InventarioFerreteria.DataAccess/Interfaces/ICategoriaRepository.cs
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Interfaces;

public interface ICategoriaRepository
{
    Task<Categoria?> ObtenerPorIdAsync(int id);
    Task<List<Categoria>> ObtenerTodosAsync();
    Task<int> InsertarAsync(Categoria categoria);
    Task<bool> ActualizarAsync(Categoria categoria);
    Task<bool> EliminarAsync(int id);
}