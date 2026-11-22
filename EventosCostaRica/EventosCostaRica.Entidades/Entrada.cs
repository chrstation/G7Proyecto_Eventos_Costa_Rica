namespace EventosCostaRica.Entidades;

/// <summary>
/// Entrada comprada: un asiento de un evento a nombre de un usuario.
/// </summary>
public class Entrada : EntidadBase
{
    public int EventoId { get; set; }

    public Evento? Evento { get; set; }

    public int UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }

    public int Fila { get; set; }

    public int Numero { get; set; }

    public DateTime FechaCompra { get; set; }

    public string Etiqueta => Asiento.CrearEtiqueta(Fila, Numero);
}
