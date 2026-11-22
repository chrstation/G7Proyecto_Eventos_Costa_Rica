using EventosCostaRica.Web.Infraestructura;

namespace EventosCostaRica.Web.Models;

/// <summary>
/// Entradas que un usuario compró para un mismo evento.
/// </summary>
public class BoletoViewModel
{
    public int EventoId { get; set; }

    public string EventoNombre { get; set; } = string.Empty;

    public DateTime FechaEvento { get; set; }

    public string Lugar { get; set; } = string.Empty;

    public List<string> Asientos { get; set; } = new();

    public DateTime FechaCompra { get; set; }

    public string FechaEventoTexto => FormatoFecha.Largo(FechaEvento);

    public string FechaCompraTexto => FormatoFecha.Corto(FechaCompra);

    public bool Finalizado => FechaEvento < DateTime.Now;
}
