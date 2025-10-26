// InventarioFerreteria.Entities/Categoria.cs
using System.Runtime.Serialization;

namespace InventarioFerreteria.Entities;

[DataContract]
public class Categoria
{
    [DataMember]
    public int Id { get; set; }
    
    [DataMember]
    public string Nombre { get; set; } = string.Empty;
    
    [DataMember]
    public string? Descripcion { get; set; }
}