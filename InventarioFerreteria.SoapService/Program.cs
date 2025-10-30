// InventarioFerreteria.SoapService/Program.cs
using Microsoft.EntityFrameworkCore;
using SoapCore;
using InventarioFerreteria.DataAccess;
using InventarioFerreteria.DataAccess.Interfaces;
using InventarioFerreteria.DataAccess.Repositories;
using InventarioFerreteria.Business.Interfaces;
using InventarioFerreteria.Business.Services;
using InventarioFerreteria.SoapService.Services;
using InventarioFerreteria.SoapService.Middleware;
using Serilog;
using Serilog.Events;

// ============================================
// CONFIGURACIÓN DE SERILOG (antes de builder)
// ============================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "InventarioFerreteria.SoapService")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10_485_760) // 10MB
    .WriteTo.File(
        path: "Logs/errors-.txt",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Error,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}",
        retainedFileCountLimit: 90)
    .CreateLogger();

try
{
    Log.Information("=== Iniciando Servicio SOAP de Inventario Ferretería ===");

    var builder = WebApplication.CreateBuilder(args);

    // ============================================
    // CONFIGURAR SERILOG COMO PROVEEDOR DE LOGGING
    // ============================================
    builder.Host.UseSerilog();

    // ============================================
    // FIJAR EL PUERTO - Escuchar en todas las interfaces
    // ============================================
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
    Log.Information("Servicio configurado en: http://0.0.0.0:5000");

    // ============================================
    // CONFIGURAR POSTGRESQL COMO TRANSIENT
    // ============================================
    builder.Services.AddTransient<ApplicationDbContext>(provider =>
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Log.Fatal("No se encontró la cadena de conexión 'DefaultConnection' en appsettings.json");
            throw new InvalidOperationException("Cadena de conexión no configurada");
        }

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
        
        return new ApplicationDbContext(optionsBuilder.Options);
    });

    // ============================================
    // REGISTRAR REPOSITORIOS
    // ============================================
    builder.Services.AddTransient<IArticuloRepository, ArticuloRepository>();
    builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
    builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
    builder.Services.AddTransient<IProveedorRepository, ProveedorRepository>();
    Log.Debug("Repositorios registrados correctamente");

    // ============================================
    // REGISTRAR SERVICIOS DE NEGOCIO
    // ============================================
    builder.Services.AddTransient<IArticuloService, ArticuloService>();
    builder.Services.AddTransient<IAutenticacionService, AutenticacionService>();
    builder.Services.AddTransient<ICategoriaService, CategoriaService>();
    builder.Services.AddTransient<IProveedorService, ProveedorService>();
    Log.Debug("Servicios de negocio registrados correctamente");

    // ============================================
    // REGISTRAR SERVICIO SOAP
    // ============================================
    builder.Services.AddTransient<IInventarioSoapService, InventarioSoapService>();
    builder.Services.AddSoapCore();
    Log.Debug("Servicio SOAP configurado correctamente");

    // ============================================
    // CONFIGURAR OPCIONES ADICIONALES
    // ============================================
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = false;
    });

    var app = builder.Build();

    // ============================================
    // MIDDLEWARE DE MANEJO DE ERRORES
    // ============================================
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // ============================================
    // LOGGING DE REQUESTS (solo en desarrollo)
    // ============================================
    if (app.Environment.IsDevelopment())
    {
        app.Use(async (context, next) =>
        {
            Log.Debug("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
            await next.Invoke();
            Log.Debug("Response: {StatusCode}", context.Response.StatusCode);
        });
    }

    // ============================================
    // VERIFICAR CONEXIÓN A BASE DE DATOS
    // ============================================
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.CanConnectAsync();
            Log.Information("✓ Conexión a base de datos PostgreSQL establecida correctamente");
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "✗ Error al conectar con la base de datos PostgreSQL");
        throw;
    }

    // ============================================
    // CONFIGURAR ENDPOINT SOAP
    // ============================================
    app.UseSoapEndpoint<IInventarioSoapService>("/InventarioService.asmx", new SoapEncoderOptions
    {
        WriteEncoding = System.Text.Encoding.UTF8
    });
    Log.Information("✓ Endpoint SOAP configurado en: http://0.0.0.0:5000/InventarioService.asmx");

    // ============================================
    // PÁGINA DE BIENVENIDA
    // ============================================
    app.MapGet("/", () =>
    {
        Log.Debug("Redirigiendo a WSDL desde ruta raíz");
        return Results.Redirect("/InventarioService.asmx");
    });

    // ============================================
    // ENDPOINT DE HEALTH CHECK
    // ============================================
    app.MapGet("/health", async (ApplicationDbContext dbContext) =>
    {
        try
        {
            var canConnect = await dbContext.Database.CanConnectAsync();
            if (canConnect)
            {
                return Results.Ok(new { status = "Healthy", database = "Connected", timestamp = DateTime.UtcNow });
            }
            return Results.Problem("No se puede conectar a la base de datos", statusCode: 503);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error en health check");
            return Results.Problem($"Error: {ex.Message}", statusCode: 503);
        }
    });

    Log.Information("=== Servicio SOAP listo para recibir peticiones ===");
    Log.Information("WSDL disponible en: http://0.0.0.0:5000/InventarioService.asmx?wsdl");

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "!!! El servicio SOAP terminó inesperadamente !!!");
    throw;
}
finally
{
    Log.Information("=== Servicio SOAP detenido ===");
    await Log.CloseAndFlushAsync();
}