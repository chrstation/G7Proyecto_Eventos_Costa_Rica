namespace EventosCostaRica.Entidades;

/// <summary>
/// Posición de un asiento dentro de la sala (fila y número, ambos desde 1).
/// </summary>
public sealed record Asiento(int Fila, int Numero)
{
    public string Etiqueta => CrearEtiqueta(Fila, Numero);

    /// <summary>
    /// Convierte fila y número en una etiqueta legible. Ejemplo: fila 3, número 5 es "C5".
    /// </summary>
    public static string CrearEtiqueta(int fila, int numero)
    {
        return $"{(char)('A' + fila - 1)}{numero}";
    }
}
