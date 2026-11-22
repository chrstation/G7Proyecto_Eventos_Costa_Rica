using EventosCostaRica.Negocio;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace EventosCostaRica.Web.Infraestructura;

/// <summary>
/// Manejador global de excepciones. Recibe cualquier excepción no controlada de la aplicación,
/// la registra en el log y prepara un mensaje amigable para la página de error.
/// </summary>
public class ManejadorGlobalExcepciones : IExceptionHandler
{
    public const string ClaveMensaje = "MensajeError";

    private readonly ILogger<ManejadorGlobalExcepciones> _logger;

    public ManejadorGlobalExcepciones(ILogger<ManejadorGlobalExcepciones> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception excepcion,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            excepcion,
            "Error no controlado en {Metodo} {Ruta}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        httpContext.Items[ClaveMensaje] = excepcion switch
        {
            ValidacionException => excepcion.Message,
            DbUpdateException => "No se pudo guardar la información en la base de datos. Intente de nuevo.",
            _ => "Ocurrió un error inesperado. Si el problema continúa, contacte al administrador."
        };

        // Se devuelve false para que el middleware muestre la página /Error.
        return ValueTask.FromResult(false);
    }
}
