using EventosCostaRica.Entidades;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Contrato de la lógica de negocio para los eventos.
/// </summary>
public interface IEventoServicio
{
    Task<List<Evento>> ListarAsync();

    Task<List<Evento>> ListarProximosAsync();

    Task<Evento> ObtenerPorIdAsync(int id);

    Task RegistrarAsync(Evento evento);

    Task ModificarAsync(Evento evento);

    Task BorrarAsync(int id);
}
