namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos de la pantalla de compra: el evento y los asientos que ya están vendidos.
/// </summary>
public class CompraViewModel
{
    public EventoViewModel Evento { get; set; } = new();

    /// <summary>
    /// Asientos ocupados con el formato "fila-numero", por ejemplo "3-5".
    /// </summary>
    public HashSet<string> Ocupados { get; set; } = new();

    public int Filas => EventosCostaRica.Entidades.Evento.Filas;

    public int AsientosPorFila => EventosCostaRica.Entidades.Evento.AsientosPorFila;

    public int MaximoAsientos => EventosCostaRica.Entidades.Evento.MaximoAsientosPorCompra;
}
