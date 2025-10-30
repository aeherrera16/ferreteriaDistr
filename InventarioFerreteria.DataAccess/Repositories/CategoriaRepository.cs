// InventarioFerreteria.DataAccess/Repositories/CategoriaRepository.cs
using Microsoft.EntityFrameworkCore;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ApplicationDbContext _context;

    public CategoriaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Categoria?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categorias.FindAsync(id);
    }

    public async Task<List<Categoria>> ObtenerTodosAsync()
    {
        return await _context.Categorias.ToListAsync();
    }

    public async Task<int> InsertarAsync(Categoria categoria)
    {
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return categoria.Id;
    }

    public async Task<bool> ActualizarAsync(Categoria categoria)
    {
        _context.Categorias.Update(categoria);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var categoria = await _context.Categorias.FindAsync(id);
        if (categoria == null) return false;

        _context.Categorias.Remove(categoria);
        return await _context.SaveChangesAsync() > 0;
    }
}