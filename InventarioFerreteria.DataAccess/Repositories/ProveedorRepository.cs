// InventarioFerreteria.DataAccess/Repositories/ProveedorRepository.cs
using Microsoft.EntityFrameworkCore;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess.Repositories;

public class ProveedorRepository : IProveedorRepository
{
    private readonly ApplicationDbContext _context;

    public ProveedorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Proveedor?> ObtenerPorIdAsync(int id)
    {
        return await _context.Proveedores.FindAsync(id);
    }

    public async Task<List<Proveedor>> ObtenerTodosAsync()
    {
        return await _context.Proveedores.ToListAsync();
    }

    public async Task<int> InsertarAsync(Proveedor proveedor)
    {
        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
        return proveedor.Id;
    }

    public async Task<bool> ActualizarAsync(Proveedor proveedor)
    {
        _context.Proveedores.Update(proveedor);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return false;

        _context.Proveedores.Remove(proveedor);
        return await _context.SaveChangesAsync() > 0;
    }
}