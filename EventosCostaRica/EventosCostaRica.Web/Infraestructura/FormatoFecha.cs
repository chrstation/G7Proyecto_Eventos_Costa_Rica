using System.Globalization;

namespace EventosCostaRica.Web.Infraestructura;

/// <summary>
/// Formatos de fecha en español para mostrar en las vistas.
/// </summary>
public static class FormatoFecha
{
    private static readonly CultureInfo Cultura = new("es-CR");

    public static string Largo(DateTime fecha)
    {
        return fecha.ToString("dddd d 'de' MMMM yyyy, HH:mm", Cultura);
    }

    public static string Corto(DateTime fecha)
    {
        return fecha.ToString("dd/MM/yyyy HH:mm", Cultura);
    }
}
