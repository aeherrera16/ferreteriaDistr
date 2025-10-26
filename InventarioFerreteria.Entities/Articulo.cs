// InventarioFerreteria.Entities/Articulo.cs
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace InventarioFerreteria.Entities;

[DataContract]
public class Articulo
{
    [DataMember]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    [DataMember]
    public string Codigo { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    [DataMember]
    public string Nombre { get; set; } = string.Empty;
    
    [DataMember]
    public string? Descripcion { get; set; }
    
    [DataMember]
    public int? CategoriaId { get; set; }
    
    [Range(0.01, double.MaxValue)]
    [DataMember]
    public decimal PrecioCompra { get; set; }
    
    [Range(0.01, double.MaxValue)]
    [DataMember]
    public decimal PrecioVenta { get; set; }
    
    [Range(0, int.MaxValue)]
    [DataMember]
    public int Stock { get; set; }
    
    [DataMember]
    public int StockMinimo { get; set; } = 5;
    
    [DataMember]
    public int? ProveedorId { get; set; }
    
    [DataMember]
    public DateTime FechaCreacion { get; set; }
    
    [DataMember]
    public DateTime FechaActualizacion { get; set; }
    
    [DataMember]
    public bool Activo { get; set; } = true;
    
    // Propiedades de navegación - NO serializar
    public Categoria? Categoria { get; set; }
    public Proveedor? Proveedor { get; set; }
    
    // Propiedad calculada
    [IgnoreDataMember]
    public bool RequiereReposicion => Stock < StockMinimo;
}