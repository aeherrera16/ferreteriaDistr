// InventarioFerreteria.Business/Services/CategoriaService.cs
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _categoriaRepository;

    public CategoriaService(ICategoriaRepository categoriaRepository)
    {
        _categoriaRepository = categoriaRepository;
    }

    public async Task<RespuestaDTO<Categoria>> InsertarCategoriaAsync(string nombre, string? descripcion)
    {
        var respuesta = new RespuestaDTO<Categoria>();

        try
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "El nombre es requerido";
                return respuesta;
            }

            var categoria = new Categoria
            {
                Nombre = nombre.Trim(),
                Descripcion = descripcion?.Trim()
            };

            var id = await _categoriaRepository.InsertarAsync(categoria);
            var categoriaInsertada = await _categoriaRepository.ObtenerPorIdAsync(id);

            respuesta.Exito = true;
            respuesta.Mensaje = "Categoría insertada correctamente";
            respuesta.Datos = categoriaInsertada;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar la categoría";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<List<Categoria>>> ObtenerTodosAsync()
    {
        var respuesta = new RespuestaDTO<List<Categoria>>();

        try
        {
            var categorias = await _categoriaRepository.ObtenerTodosAsync();

            respuesta.Exito = true;
            respuesta.Mensaje = $"Se encontraron {categorias.Count} categorías";
            respuesta.Datos = categorias;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener las categorías";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<Categoria>> ActualizarCategoriaAsync(int id, string nombre, string? descripcion)
    {
        var respuesta = new RespuestaDTO<Categoria>();

        try
        {
            var categoriaExistente = await _categoriaRepository.ObtenerPorIdAsync(id);

            if (categoriaExistente == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Categoría no encontrada";
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "El nombre es requerido";
                return respuesta;
            }

            categoriaExistente.Nombre = nombre.Trim();
            categoriaExistente.Descripcion = descripcion?.Trim();

            await _categoriaRepository.ActualizarAsync(categoriaExistente);

            respuesta.Exito = true;
            respuesta.Mensaje = "Categoría actualizada correctamente";
            respuesta.Datos = categoriaExistente;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar la categoría";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<bool>> EliminarCategoriaAsync(int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            var eliminado = await _categoriaRepository.EliminarAsync(id);

            if (eliminado)
            {
                respuesta.Exito = true;
                respuesta.Mensaje = "Categoría eliminada correctamente";
                respuesta.Datos = true;
            }
            else
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Categoría no encontrada";
                respuesta.Datos = false;
            }
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar la categoría";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }
}