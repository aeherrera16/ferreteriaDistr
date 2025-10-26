using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.Entities;
using InventarioFerreteria.Entities.DTOs;

namespace InventarioFerreteria.Business.Services;

public class AutenticacionService : IAutenticacionService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;

    public AutenticacionService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
    }

    public async Task<RespuestaDTO<Usuario>> AutenticarAsync(AutenticacionDTO autenticacionDto)
    {
    var respuesta = new RespuestaDTO<Usuario>();

    try
    {
        Console.WriteLine($"[DEBUG] Intentando autenticar usuario: {autenticacionDto.NombreUsuario}");
        
        var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(autenticacionDto.NombreUsuario);

        if (usuario == null)
        {
            Console.WriteLine($"[DEBUG] Usuario no encontrado: {autenticacionDto.NombreUsuario}");
            respuesta.Exito = false;
            respuesta.Mensaje = "Usuario o contraseña incorrectos";
            return respuesta;
        }

        Console.WriteLine($"[DEBUG] Usuario encontrado. ID: {usuario.Id}, Hash en DB: {usuario.PasswordHash}");
        Console.WriteLine($"[DEBUG] Contraseña recibida: {autenticacionDto.Password}");

        // Verificar contraseña en texto plano
        bool esValido = autenticacionDto.Password == usuario.PasswordHash;

        Console.WriteLine($"[DEBUG] Validación: {esValido}");

        if (!esValido)
        {
            respuesta.Exito = false;
            respuesta.Mensaje = "Usuario o contraseña incorrectos";
            return respuesta;
        }

        respuesta.Exito = true;
        respuesta.Mensaje = "Autenticación exitosa";
        respuesta.Datos = usuario;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[DEBUG] Excepción: {ex.Message}");
        Console.WriteLine($"[DEBUG] StackTrace: {ex.StackTrace}");
        respuesta.Exito = false;
        respuesta.Mensaje = "Error en la autenticación";
        respuesta.Errores = new List<string> { ex.Message };
    }

    return respuesta;
    }

    public string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:SecretKey"] ?? "ClaveSecretaSuperSegura12345678901234567890"));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "InventarioFerreteriaAPI",
            audience: _configuration["Jwt:Audience"] ?? "InventarioFerreteriaClients",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}