using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Página de inicio (landing). Es pública: se puede ver sin iniciar sesión.
/// </summary>
[AllowAnonymous]
public class HomeController : Controller
{
    private const int CantidadDestacados = 3;

    private readonly IEventoServicio _eventoServicio;

    public HomeController(IEventoServicio eventoServicio)
    {
        _eventoServicio = eventoServicio;
    }

    // GET: /
    public async Task<IActionResult> Index()
    {
        var eventos = await _eventoServicio.ListarProximosAsync();

        var modelo = new InicioViewModel
        {
            Destacados = eventos
                .Take(CantidadDestacados)
                .Select(EventoViewModel.DesdeEntidad)
                .ToList(),
            TotalEventos = eventos.Count,
            AsientosDisponibles = eventos.Sum(e => e.AsientosDisponibles)
        };

        return View(modelo);
    }
}
