namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos de la página de inicio: eventos destacados y algunas cifras.
/// </summary>
public class InicioViewModel
{
    public List<EventoViewModel> Destacados { get; set; } = new();

    public int TotalEventos { get; set; }

    public int AsientosDisponibles { get; set; }

    public int AsientosPorEvento => EventosCostaRica.Entidades.Evento.Capacidad;
}
