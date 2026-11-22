using System.Diagnostics;
using EventosCostaRica.Web.Infraestructura;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Página de error a la que llega el manejador global de excepciones y los códigos 404, 400, etc.
/// </summary>
[AllowAnonymous]
public class ErrorController : Controller
{
    // /Error  (excepciones)  y  /Error/404  (códigos de estado)
    [Route("Error/{codigo:int?}")]
    [AcceptVerbs("GET", "POST")]
    public IActionResult Index(int? codigo)
    {
        var mensajeGlobal = HttpContext.Items[ManejadorGlobalExcepciones.ClaveMensaje] as string;

        var modelo = new ErrorViewModel
        {
            Codigo = codigo ?? 500,
            IdSolicitud = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        };

        modelo.Titulo = modelo.Codigo switch
        {
            404 => "No encontrado",
            400 => "Solicitud incorrecta",
            _ => "Ocurrió un error"
        };

        modelo.Mensaje = mensajeGlobal ?? modelo.Codigo switch
        {
            404 => "La página o el registro que busca no existe.",
            400 => "La solicitud no es válida.",
            _ => "Ocurrió un error inesperado. Intente nuevamente."
        };

        return View(modelo);
    }

    // Ruta de prueba para demostrar el manejador global (se puede eliminar).
    [Route("Error/Probar")]
    public IActionResult Probar()
    {
        throw new InvalidOperationException("Excepción de prueba para el manejador global.");
    }
}
