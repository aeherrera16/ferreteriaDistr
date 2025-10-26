// InventarioFerreteria.Entities/Usuario.cs
using System.Runtime.Serialization;

namespace InventarioFerreteria.Entities;

[DataContract]
public class Usuario
{
    [DataMember]
    public int Id { get; set; }
    
    [DataMember]
    public string NombreUsuario { get; set; } = string.Empty;
    
    [DataMember]
    public string PasswordHash { get; set; } = string.Empty;
    
    [DataMember]
    public string Rol { get; set; } = "Usuario";
    
    [DataMember]
    public DateTime FechaCreacion { get; set; }
    
    [DataMember]
    public bool Activo { get; set; } = true;
}