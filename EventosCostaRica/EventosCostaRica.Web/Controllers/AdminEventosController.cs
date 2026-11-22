using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Infraestructura;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Administración de eventos (crear, leer, actualizar y borrar). Solo administradores.
/// </summary>
[Authorize(Roles = NombresRol.Administrador)]
public class AdminEventosController : Controller
{
    private readonly IEventoServicio _servicio;

    public AdminEventosController(IEventoServicio servicio)
    {
        _servicio = servicio;
    }

    // GET: /AdminEventos
    public async Task<IActionResult> Index()
    {
        var eventos = await _servicio.ListarAsync();

        return View(eventos.Select(EventoViewModel.DesdeEntidad).ToList());
    }

    // GET: /AdminEventos/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new EventoViewModel { FechaEvento = DateTime.Today.AddDays(7).AddHours(19) });
    }

    // POST: /AdminEventos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventoViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _servicio.RegistrarAsync(modelo.ComoEntidad());
            TempData["Mensaje"] = "Evento registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }
    }

    // GET: /AdminEventos/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var evento = await _servicio.ObtenerPorIdAsync(id);

        return View(EventoViewModel.DesdeEntidad(evento));
    }

    // POST: /AdminEventos/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventoViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _servicio.ModificarAsync(modelo.ComoEntidad());
            TempData["Mensaje"] = "Evento modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }
    }

    // GET: /AdminEventos/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var evento = await _servicio.ObtenerPorIdAsync(id);

        return View(EventoViewModel.DesdeEntidad(evento));
    }

    // POST: /AdminEventos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _servicio.BorrarAsync(id);
            TempData["Mensaje"] = "Evento borrado correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
