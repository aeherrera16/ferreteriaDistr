using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Interfaces;

public interface IArticuloRepository
{
    Task<Articulo?> ObtenerPorIdAsync(int id);
    Task<Articulo?> ObtenerPorCodigoAsync(string codigo);
    Task<List<Articulo>> ObtenerTodosAsync();
    Task<List<Articulo>> BuscarPorNombreAsync(string nombre);
    Task<int> InsertarAsync(Articulo articulo);
    Task<bool> ActualizarAsync(Articulo articulo);
    Task<bool> EliminarAsync(int id);
    Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null);
}