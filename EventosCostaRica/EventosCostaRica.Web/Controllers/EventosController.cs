using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Página de inicio (próximos eventos) y detalle de un evento. Disponible para administradores y clientes.
/// </summary>
public class EventosController : Controller
{
    private readonly IEventoServicio _eventoServicio;

    public EventosController(IEventoServicio eventoServicio)
    {
        _eventoServicio = eventoServicio;
    }

    // GET: /Eventos
    public async Task<IActionResult> Index()
    {
        var eventos = await _eventoServicio.ListarProximosAsync();

        return View(eventos.Select(EventoViewModel.DesdeEntidad).ToList());
    }

    // GET: /Eventos/Detalle/5
    public async Task<IActionResult> Detalle(int id)
    {
        try
        {
            var evento = await _eventoServicio.ObtenerPorIdAsync(id);

            return View(EventoViewModel.DesdeEntidad(evento));
        }
        catch (ValidacionException)
        {
            return NotFound();
        }
    }
}
