// InventarioFerreteria.SoapService/Middleware/ErrorHandlingMiddleware.cs
using System.Net;
using System.Text;

namespace InventarioFerreteria.SoapService.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en la petición: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Ha ocurrido un error interno en el servidor";

        // ✅ CORREGIDO: Usar type pattern matching
        switch (exception)
        {
            case ArgumentNullException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Parámetro requerido no proporcionado";
                break;
            case ArgumentException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = "No autorizado";
                break;
            case KeyNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;
            case InvalidOperationException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
            default:
                // Para errores inesperados, no exponer detalles
                message = "Error interno del servidor. Contacte al administrador.";
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "text/xml; charset=utf-8";

        var soapFault = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <soap:Fault>
            <faultcode>soap:Server</faultcode>
            <faultstring>{System.Security.SecurityElement.Escape(message)}</faultstring>
            <detail>
                <error>
                    <timestamp>{DateTime.UtcNow:o}</timestamp>
                    <statusCode>{(int)statusCode}</statusCode>
                </error>
            </detail>
        </soap:Fault>
    </soap:Body>
</soap:Envelope>";

        await context.Response.WriteAsync(soapFault, Encoding.UTF8);
    }
}