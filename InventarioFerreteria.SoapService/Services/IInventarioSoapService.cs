// InventarioFerreteria.SoapService/Services/IInventarioSoapService.cs
using System.ServiceModel;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.SoapService.Services;

[ServiceContract]
public interface IInventarioSoapService
{
    // === AUTENTICACIÓN ===
    [OperationContract(Name = "Autenticar")]
    Task<RespuestaDTO<string>> AutenticarAsync(string nombreUsuario, string password);

    // === ARTÍCULOS ===
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

    [OperationContract]
    Task<RespuestaDTO<Articulo>> ActualizarArticuloAsync(
        string token,
        int id,
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
    Task<RespuestaDTO<bool>> EliminarArticuloAsync(string token, int id);

    // === CATEGORÍAS ===
    [OperationContract]
    Task<RespuestaDTO<Categoria>> InsertarCategoriaAsync(
        string token,
        string nombre,
        string? descripcion);

    [OperationContract]
    Task<RespuestaDTO<List<Categoria>>> ObtenerTodasCategoriasAsync(string token);

    [OperationContract]
    Task<RespuestaDTO<Categoria>> ActualizarCategoriaAsync(
        string token,
        int id,
        string nombre,
        string? descripcion);

    [OperationContract]
    Task<RespuestaDTO<bool>> EliminarCategoriaAsync(string token, int id);

    // === PROVEEDORES ===
    [OperationContract]
    Task<RespuestaDTO<Proveedor>> InsertarProveedorAsync(
        string token,
        string nombre,
        string? telefono,
        string? email,
        string? direccion);

    [OperationContract]
    Task<RespuestaDTO<List<Proveedor>>> ObtenerTodosProveedoresAsync(string token);

    [OperationContract]
    Task<RespuestaDTO<Proveedor>> ActualizarProveedorAsync(
        string token,
        int id,
        string nombre,
        string? telefono,
        string? email,
        string? direccion);

    [OperationContract]
    Task<RespuestaDTO<bool>> EliminarProveedorAsync(string token, int id);
}