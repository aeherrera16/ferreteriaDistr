using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
}