// InventarioFerreteria.SoapService/Services/IInventarioSoapService.cs
using System.ServiceModel;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.SoapService.Services;

[ServiceContract]
public interface IInventarioSoapService
{
    [OperationContract]
    Task<RespuestaDTO<string>> AutenticarAsync(string nombreUsuario, string password);

    [OperationContract]
    Task<RespuestaDTO<Articulo>> InsertarArticuloAsync(
        string token,
        string codigo,
        string nombre,
        string? descripcion,
        int? categoriaId,
        decimal precioCompra,
        decimal precioVenta,
        int stock,
        int stockMinimo,
        int? proveedorId);

    [OperationContract]
    Task<RespuestaDTO<Articulo>> ConsultarArticuloPorCodigoAsync(
        string token,
        string codigo);

    [OperationContract]
    Task<RespuestaDTO<List<Articulo>>> ObtenerTodosArticulosAsync(string token);
}