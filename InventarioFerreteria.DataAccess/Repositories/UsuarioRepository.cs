using Microsoft.EntityFrameworkCore;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
    {
    Console.WriteLine($"[DEBUG REPO] Buscando usuario: {nombreUsuario}");
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Activo);
    Console.WriteLine($"[DEBUG REPO] Usuario encontrado: {usuario != null}");
    return usuario;
    }
}