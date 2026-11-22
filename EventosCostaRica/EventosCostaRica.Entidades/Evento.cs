namespace EventosCostaRica.Entidades;

/// <summary>
/// Evento publicado por la empresa, con una distribución fija de asientos (10 filas de 10).
/// </summary>
public class Evento : EntidadBase
{
    public const int Filas = 10;

    public const int AsientosPorFila = 10;

    public const int Capacidad = Filas * AsientosPorFila;

    public const int MaximoAsientosPorCompra = 10;

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaEvento { get; set; }

    public string Lugar { get; set; } = string.Empty;

    public string BannerUrl { get; set; } = string.Empty;

    public List<Entrada> Entradas { get; set; } = new();

    // Propiedades calculadas (requieren que Entradas esté cargada).
    public int EntradasVendidas => Entradas.Count;

    public int AsientosDisponibles => Capacidad - EntradasVendidas;

    public bool Agotado => EntradasVendidas >= Capacidad;

    public bool Finalizado => FechaEvento < DateTime.Now;
}
