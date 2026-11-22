namespace EventosCostaRica.Entidades;

/// <summary>
/// Clase base de todas las entidades. Permite que el repositorio genérico
/// trabaje con cualquier entidad que tenga un identificador.
/// </summary>
public abstract class EntidadBase
{
    public int Id { get; set; }
}
