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

    // InventarioFerreteria.DataAccess/Repositories/ArticuloRepository.cs
    public async Task<bool> ActualizarAsync(Articulo articulo)
    {
        try
        {
            // Obtener la entidad rastreada
            var articuloExistente = await _context.Articulos.FindAsync(articulo.Id);
            
            if (articuloExistente == null)
                return false;

            // Actualizar solo las propiedades
            articuloExistente.Codigo = articulo.Codigo;
            articuloExistente.Nombre = articulo.Nombre;
            articuloExistente.Descripcion = articulo.Descripcion;
            articuloExistente.CategoriaId = articulo.CategoriaId;
            articuloExistente.PrecioCompra = articulo.PrecioCompra;
            articuloExistente.PrecioVenta = articulo.PrecioVenta;
            articuloExistente.Stock = articulo.Stock;
            articuloExistente.StockMinimo = articulo.StockMinimo;
            articuloExistente.ProveedorId = articulo.ProveedorId;
            articuloExistente.FechaActualizacion = DateTime.UtcNow;
            // NO actualizar: FechaCreacion, Activo, Id

            return await _context.SaveChangesAsync() > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ActualizarAsync: {ex.Message}");
            throw;
        }
    }

    // InventarioFerreteria.DataAccess/Repositories/ArticuloRepository.cs
    public async Task<bool> EliminarAsync(int id)
    {
        try
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo == null) 
            {
                Console.WriteLine($"[INFO] Artículo con ID {id} no encontrado");
                return false;
            }

            // Eliminación física - borra permanentemente de la base de datos
            _context.Articulos.Remove(articulo);
            
            var resultado = await _context.SaveChangesAsync() > 0;
            Console.WriteLine($"[INFO] Artículo {id} eliminado permanentemente de la BD: {resultado}");
            
            return resultado;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] EliminarAsync: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"[ERROR] InnerException: {ex.InnerException.Message}");
            throw;
        }
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excluirId = null)
    {
        var query = _context.Articulos.Where(a => a.Codigo == codigo);
        
        if (excluirId.HasValue)
            query = query.Where(a => a.Id != excluirId.Value);

        return await query.AnyAsync();
    }
}