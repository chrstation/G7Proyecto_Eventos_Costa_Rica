using EventosCostaRica.Datos;
using EventosCostaRica.Entidades;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Reglas de negocio de los eventos.
/// </summary>
public class EventoServicio : IEventoServicio
{
    private readonly IRepositorio<Evento> _eventos;
    private readonly IRepositorio<Entrada> _entradas;

    public EventoServicio(IRepositorio<Evento> eventos, IRepositorio<Entrada> entradas)
    {
        _eventos = eventos;
        _entradas = entradas;
    }

    public async Task<List<Evento>> ListarAsync()
    {
        var eventos = await _eventos.ObtenerTodosAsync(e => e.Entradas);

        return eventos.OrderBy(e => e.FechaEvento).ToList();
    }

    public async Task<List<Evento>> ListarProximosAsync()
    {
        var ahora = DateTime.Now;
        var eventos = await _eventos.BuscarAsync(e => e.FechaEvento >= ahora, e => e.Entradas);

        return eventos.OrderBy(e => e.FechaEvento).ToList();
    }

    public async Task<Evento> ObtenerPorIdAsync(int id)
    {
        var evento = await _eventos.ObtenerPorIdAsync(id, e => e.Entradas);

        return evento ?? throw new ValidacionException($"No existe un evento con el Id {id}.");
    }

    public async Task RegistrarAsync(Evento evento)
    {
        Validar(evento);

        if (evento.FechaEvento <= DateTime.Now)
        {
            throw new ValidacionException("La fecha del evento debe ser futura.");
        }

        // El Id lo genera la base de datos, por lo que nunca se repite.
        evento.Id = 0;
        await _eventos.AgregarAsync(evento);
    }

    public async Task ModificarAsync(Evento evento)
    {
        Validar(evento);

        if (!await _eventos.ActualizarAsync(evento))
        {
            throw new ValidacionException($"No existe un evento con el Id {evento.Id}.");
        }
    }

    public async Task BorrarAsync(int id)
    {
        if (!await _eventos.ExisteAsync(e => e.Id == id))
        {
            throw new ValidacionException($"No existe un evento con el Id {id}.");
        }

        if (await _entradas.ExisteAsync(e => e.EventoId == id))
        {
            throw new ValidacionException("No se puede borrar el evento porque ya tiene entradas vendidas.");
        }

        await _eventos.EliminarAsync(id);
    }

    private static void Validar(Evento evento)
    {
        if (string.IsNullOrWhiteSpace(evento.Nombre))
        {
            throw new ValidacionException("El nombre del evento es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(evento.Lugar))
        {
            throw new ValidacionException("El lugar del evento es obligatorio.");
        }

        if (evento.FechaEvento == default)
        {
            throw new ValidacionException("La fecha del evento es obligatoria.");
        }

        if (!string.IsNullOrWhiteSpace(evento.BannerUrl) && !EsUrlWebValida(evento.BannerUrl))
        {
            throw new ValidacionException("El banner debe ser una dirección web (http o https) válida.");
        }
    }

    private static bool EsUrlWebValida(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}
