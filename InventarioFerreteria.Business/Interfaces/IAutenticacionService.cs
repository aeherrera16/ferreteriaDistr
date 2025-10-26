using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Interfaces;

public interface IAutenticacionService
{
    Task<RespuestaDTO<Usuario>> AutenticarAsync(AutenticacionDTO autenticacionDto);
    string GenerarToken(Usuario usuario);
}