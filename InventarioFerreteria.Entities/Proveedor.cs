// InventarioFerreteria.Entities/Proveedor.cs
using System.Runtime.Serialization;

namespace InventarioFerreteria.Entities;

[DataContract]
public class Proveedor
{
    [DataMember]
    public int Id { get; set; }
    
    [DataMember]
    public string Nombre { get; set; } = string.Empty;
    
    [DataMember]
    public string? Telefono { get; set; }
    
    [DataMember]
    public string? Email { get; set; }
    
    [DataMember]
    public string? Direccion { get; set; }
}