using Microsoft.EntityFrameworkCore;
using InventarioFerreteria.Entities;

namespace InventarioFerreteria.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Articulo> Articulos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Articulo
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.ToTable("articulos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Codigo).HasColumnName("codigo");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.CategoriaId).HasColumnName("categoriaid");
            entity.Property(e => e.PrecioCompra).HasColumnName("preciocompra");
            entity.Property(e => e.PrecioVenta).HasColumnName("precioventa");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.StockMinimo).HasColumnName("stockminimo");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedorid");
            entity.Property(e => e.FechaCreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.FechaActualizacion).HasColumnName("fechaactualizacion");
            entity.Property(e => e.Activo).HasColumnName("activo");

            entity.HasOne(e => e.Categoria)
                .WithMany()
                .HasForeignKey(e => e.CategoriaId);

            entity.HasOne(e => e.Proveedor)
                .WithMany()
                .HasForeignKey(e => e.ProveedorId);
        });

        // Configuración de Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        // Configuración de Proveedor
        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("proveedores");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
        });

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NombreUsuario).HasColumnName("nombreusuario");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordhash");
            entity.Property(e => e.Rol).HasColumnName("rol");
            entity.Property(e => e.FechaCreacion).HasColumnName("fechacreacion");
            entity.Property(e => e.Activo).HasColumnName("activo");
        });
    }
}