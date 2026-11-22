using EventosCostaRica.Entidades;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Contrato de la lógica de negocio para la compra de entradas.
/// </summary>
public interface IEntradaServicio
{
    Task<List<Entrada>> ListarPorEventoAsync(int eventoId);

    Task<List<Entrada>> ListarPorUsuarioAsync(int usuarioId);

    Task ComprarAsync(int eventoId, int usuarioId, IReadOnlyCollection<Asiento> asientos);
}
