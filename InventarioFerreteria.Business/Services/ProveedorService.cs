// InventarioFerreteria.Business/Services/ProveedorService.cs
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Services;

public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;

    public ProveedorService(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    public async Task<RespuestaDTO<Proveedor>> InsertarProveedorAsync(string nombre, string? telefono, string? email, string? direccion)
    {
        var respuesta = new RespuestaDTO<Proveedor>();

        try
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "El nombre es requerido";
                return respuesta;
            }

            var proveedor = new Proveedor
            {
                Nombre = nombre.Trim(),
                Telefono = telefono?.Trim(),
                Email = email?.Trim(),
                Direccion = direccion?.Trim()
            };

            var id = await _proveedorRepository.InsertarAsync(proveedor);
            var proveedorInsertado = await _proveedorRepository.ObtenerPorIdAsync(id);

            respuesta.Exito = true;
            respuesta.Mensaje = "Proveedor insertado correctamente";
            respuesta.Datos = proveedorInsertado;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al insertar el proveedor";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<List<Proveedor>>> ObtenerTodosAsync()
    {
        var respuesta = new RespuestaDTO<List<Proveedor>>();

        try
        {
            var proveedores = await _proveedorRepository.ObtenerTodosAsync();

            respuesta.Exito = true;
            respuesta.Mensaje = $"Se encontraron {proveedores.Count} proveedores";
            respuesta.Datos = proveedores;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al obtener los proveedores";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<Proveedor>> ActualizarProveedorAsync(int id, string nombre, string? telefono, string? email, string? direccion)
    {
        var respuesta = new RespuestaDTO<Proveedor>();

        try
        {
            var proveedorExistente = await _proveedorRepository.ObtenerPorIdAsync(id);

            if (proveedorExistente == null)
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Proveedor no encontrado";
                return respuesta;
            }

            if (string.IsNullOrWhiteSpace(nombre))
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "El nombre es requerido";
                return respuesta;
            }

            proveedorExistente.Nombre = nombre.Trim();
            proveedorExistente.Telefono = telefono?.Trim();
            proveedorExistente.Email = email?.Trim();
            proveedorExistente.Direccion = direccion?.Trim();

            await _proveedorRepository.ActualizarAsync(proveedorExistente);

            respuesta.Exito = true;
            respuesta.Mensaje = "Proveedor actualizado correctamente";
            respuesta.Datos = proveedorExistente;
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al actualizar el proveedor";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }

    public async Task<RespuestaDTO<bool>> EliminarProveedorAsync(int id)
    {
        var respuesta = new RespuestaDTO<bool>();

        try
        {
            var eliminado = await _proveedorRepository.EliminarAsync(id);

            if (eliminado)
            {
                respuesta.Exito = true;
                respuesta.Mensaje = "Proveedor eliminado correctamente";
                respuesta.Datos = true;
            }
            else
            {
                respuesta.Exito = false;
                respuesta.Mensaje = "Proveedor no encontrado";
                respuesta.Datos = false;
            }
        }
        catch (Exception ex)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Error al eliminar el proveedor";
            respuesta.Errores = new List<string> { ex.Message };
        }

        return respuesta;
    }
}