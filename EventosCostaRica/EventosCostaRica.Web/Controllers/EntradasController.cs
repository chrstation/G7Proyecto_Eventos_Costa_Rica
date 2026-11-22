using EventosCostaRica.Entidades;
using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Infraestructura;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Compra de boletos y consulta de "Mis boletos". Disponible para administradores y clientes.
/// </summary>
public class EntradasController : Controller
{
    private readonly IEntradaServicio _entradaServicio;
    private readonly IEventoServicio _eventoServicio;

    public EntradasController(IEntradaServicio entradaServicio, IEventoServicio eventoServicio)
    {
        _entradaServicio = entradaServicio;
        _eventoServicio = eventoServicio;
    }

    // GET: /Entradas/Comprar/5   (el id es el del evento)
    [HttpGet]
    public async Task<IActionResult> Comprar(int id)
    {
        var modelo = await ConstruirCompraAsync(id);

        if (modelo is null)
        {
            return NotFound();
        }

        if (!modelo.Evento.PuedeComprar)
        {
            TempData["Error"] = modelo.Evento.Agotado
                ? "Este evento está agotado."
                : "Este evento ya finalizó.";

            return RedirectToAction("Detalle", "Eventos", new { id });
        }

        return View(modelo);
    }

    // POST: /Entradas/Comprar/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comprar(int id, List<string>? asientos)
    {
        try
        {
            var seleccion = ConvertirAsientos(asientos)
                ?? throw new ValidacionException("La selección de asientos no es válida.");

            await _entradaServicio.ComprarAsync(id, User.ObtenerIdUsuario(), seleccion);

            var etiquetas = string.Join(", ", seleccion.Select(a => a.Etiqueta));
            TempData["Mensaje"] = $"¡Compra realizada! Sus asientos: {etiquetas}.";

            return RedirectToAction(nameof(MisBoletos));
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(nameof(Comprar), new { id });
        }
    }

    // GET: /Entradas/MisBoletos
    [HttpGet]
    public async Task<IActionResult> MisBoletos()
    {
        var entradas = await _entradaServicio.ListarPorUsuarioAsync(User.ObtenerIdUsuario());

        var boletos = entradas
            .GroupBy(e => e.EventoId)
            .Select(grupo => new BoletoViewModel
            {
                EventoId = grupo.Key,
                EventoNombre = grupo.First().Evento!.Nombre,
                FechaEvento = grupo.First().Evento!.FechaEvento,
                Lugar = grupo.First().Evento!.Lugar,
                FechaCompra = grupo.Max(e => e.FechaCompra),
                Asientos = grupo
                    .OrderBy(e => e.Fila)
                    .ThenBy(e => e.Numero)
                    .Select(e => e.Etiqueta)
                    .ToList()
            })
            .OrderBy(b => b.FechaEvento)
            .ToList();

        return View(boletos);
    }

    private async Task<CompraViewModel?> ConstruirCompraAsync(int eventoId)
    {
        try
        {
            var evento = await _eventoServicio.ObtenerPorIdAsync(eventoId);
            var vendidas = await _entradaServicio.ListarPorEventoAsync(eventoId);

            return new CompraViewModel
            {
                Evento = EventoViewModel.DesdeEntidad(evento),
                Ocupados = vendidas.Select(e => $"{e.Fila}-{e.Numero}").ToHashSet()
            };
        }
        catch (ValidacionException)
        {
            return null;
        }
    }

    // Convierte los valores "fila-numero" que envía el formulario en asientos.
    private static List<Asiento>? ConvertirAsientos(IEnumerable<string>? valores)
    {
        var asientos = new List<Asiento>();

        foreach (var valor in valores ?? Enumerable.Empty<string>())
        {
            var partes = valor.Split('-');

            if (partes.Length != 2 || !int.TryParse(partes[0], out var fila) || !int.TryParse(partes[1], out var numero))
            {
                return null;
            }

            asientos.Add(new Asiento(fila, numero));
        }

        return asientos;
    }
}
