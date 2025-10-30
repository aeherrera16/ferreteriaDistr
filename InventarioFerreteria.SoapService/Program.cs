// InventarioFerreteria.SoapService/Program.cs
using Microsoft.EntityFrameworkCore;
using SoapCore;
using InventarioFerreteria.DataAccess;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.DataAccess.Repositories;
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.Business.Services;
using InventarioFerreteria.SoapService.Services;

var builder = WebApplication.CreateBuilder(args);

// Fijar el puerto
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Configurar PostgreSQL como TRANSIENT
builder.Services.AddTransient<ApplicationDbContext>(provider =>
{
    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    return new ApplicationDbContext(optionsBuilder.Options);
});

// Registrar repositorios como Transient
builder.Services.AddTransient<IArticuloRepository, ArticuloRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IProveedorRepository, ProveedorRepository>();

// Registrar servicios de negocio como Transient
builder.Services.AddTransient<IArticuloService, ArticuloService>();
builder.Services.AddTransient<IAutenticacionService, AutenticacionService>();
builder.Services.AddTransient<ICategoriaService, CategoriaService>();
builder.Services.AddTransient<IProveedorService, ProveedorService>();

// Registrar servicio SOAP como Transient
builder.Services.AddTransient<IInventarioSoapService, InventarioSoapService>();

// Configurar SoapCore
builder.Services.AddSoapCore();

var app = builder.Build();

// Configurar endpoint SOAP
app.UseSoapEndpoint<IInventarioSoapService>("/InventarioService.asmx", new SoapEncoderOptions());

// Página de bienvenida
app.MapGet("/", () => Results.Redirect("/InventarioService.asmx"));

app.Run();
