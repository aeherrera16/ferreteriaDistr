using Microsoft.EntityFrameworkCore;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Repositories;

public class ArticuloRepository : IArticuloRepository
{
    private readonly ApplicationDbContext _context;

    public ArticuloRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Articulo?> ObtenerPorIdAsync(int id)
    {
        return await _context.Articulos
            .Include(a => a.Categoria)
            .Include(a => a.Proveedor)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Articulo?> ObtenerPorCodigoAsync(string codigo)
    {
        return await _context.Articulos
            .Include(a => a.Categoria)
            .Include(a => a.Proveedor)
            .FirstOrDefaultAsync(a => a.Codigo == codigo);
    }

    public async Task<List<Articulo>> ObtenerTodosAsync()
    {
        return await _context.Articulos
            .Include(a => a.Categoria)
            .Include(a => a.Proveedor)
            .Where(a => a.Activo)
            .ToListAsync();
    }

    public async Task<List<Articulo>> BuscarPorNombreAsync(string nombre)
    {
        return await _context.Articulos
            .Include(a => a.Categoria)
            .Include(a => a.Proveedor)
            .Where(a => a.Activo && a.Nombre.ToLower().Contains(nombre.ToLower()))
            .ToListAsync();
    }

    public async Task<int> InsertarAsync(Articulo articulo)
    {
        articulo.FechaCreacion = DateTime.UtcNow;
        articulo.FechaActualizacion = DateTime.UtcNow;
        
        _context.Articulos.Add(articulo);
        await _context.SaveChangesAsync();
        return articulo.Id;
    }

    public async Task<bool> ActualizarAsync(Articulo articulo)
    {
        articulo.FechaActualizacion = DateTime.UtcNow;
        _context.Articulos.Update(articulo);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var articulo = await _context.Articulos.FindAsync(id);
        if (articulo == null) return false;

        articulo.Activo = false;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null)
    {
        var query = _context.Articulos.Where(a => a.Codigo == codigo);
        
        if (excluirId.HasValue)
            query = query.Where(a => a.Id != excluirId.Value);

        return await query.AnyAsync();
    }
}