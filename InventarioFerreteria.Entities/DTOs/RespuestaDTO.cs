// InventarioFerreteria.Entities/DTOs/RespuestaDTO.cs
using System.Runtime.Serialization;

namespace InventarioFerreteria.Entities.DTOs;

[DataContract]
public class RespuestaDTO<T>
{
    [DataMember]
    public bool Exito { get; set; }
    
    [DataMember]
    public string Mensaje { get; set; } = string.Empty;
    
    [DataMember]
    public T? Datos { get; set; }
    
    [DataMember]
    public List<string>? Errores { get; set; }
}