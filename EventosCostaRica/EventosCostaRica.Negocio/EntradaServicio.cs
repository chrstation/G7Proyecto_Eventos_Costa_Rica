using EventosCostaRica.Datos;
using EventosCostaRica.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Reglas de negocio de la compra de entradas (no incluye el pago).
/// </summary>
public class EntradaServicio : IEntradaServicio
{
    private readonly IRepositorio<Entrada> _entradas;
    private readonly IRepositorio<Evento> _eventos;
    private readonly IRepositorio<Usuario> _usuarios;

    public EntradaServicio(
        IRepositorio<Entrada> entradas,
        IRepositorio<Evento> eventos,
        IRepositorio<Usuario> usuarios)
    {
        _entradas = entradas;
        _eventos = eventos;
        _usuarios = usuarios;
    }

    public Task<List<Entrada>> ListarPorEventoAsync(int eventoId)
    {
        return _entradas.BuscarAsync(e => e.EventoId == eventoId);
    }

    public Task<List<Entrada>> ListarPorUsuarioAsync(int usuarioId)
    {
        return _entradas.BuscarAsync(e => e.UsuarioId == usuarioId, e => e.Evento!);
    }

    public async Task ComprarAsync(int eventoId, int usuarioId, IReadOnlyCollection<Asiento> asientos)
    {
        var evento = await _eventos.ObtenerPorIdAsync(eventoId)
            ?? throw new ValidacionException("El evento no existe.");

        if (evento.FechaEvento <= DateTime.Now)
        {
            throw new ValidacionException("El evento ya finalizó, no se pueden comprar entradas.");
        }

        if (!await _usuarios.ExisteAsync(u => u.Id == usuarioId))
        {
            throw new ValidacionException("El usuario no existe.");
        }

        ValidarSeleccion(asientos);

        var vendidas = await _entradas.BuscarAsync(e => e.EventoId == eventoId);
        var ocupados = vendidas.Select(e => new Asiento(e.Fila, e.Numero)).ToHashSet();
        var repetidos = asientos.Where(ocupados.Contains).ToList();

        if (repetidos.Count > 0)
        {
            var etiquetas = string.Join(", ", repetidos.Select(a => a.Etiqueta));
            throw new ValidacionException($"Los asientos {etiquetas} ya fueron vendidos. Elija otros.");
        }

        var ahora = DateTime.Now;
        var nuevas = asientos
            .Select(a => new Entrada
            {
                EventoId = eventoId,
                UsuarioId = usuarioId,
                Fila = a.Fila,
                Numero = a.Numero,
                FechaCompra = ahora
            })
            .ToList();

        try
        {
            await _entradas.AgregarVariosAsync(nuevas);
        }
        catch (DbUpdateException)
        {
            // El índice único de la base de datos impidió vender un asiento dos veces.
            throw new ValidacionException(
                "Alguno de los asientos acaba de ser comprado por otra persona. Actualice la pantalla y elija otros.");
        }
    }

    private static void ValidarSeleccion(IReadOnlyCollection<Asiento> asientos)
    {
        if (asientos.Count == 0)
        {
            throw new ValidacionException("Debe seleccionar al menos un asiento.");
        }

        if (asientos.Count > Evento.MaximoAsientosPorCompra)
        {
            throw new ValidacionException($"Solo puede comprar hasta {Evento.MaximoAsientosPorCompra} asientos por compra.");
        }

        if (asientos.Distinct().Count() != asientos.Count)
        {
            throw new ValidacionException("Hay asientos repetidos en la selección.");
        }

        var existenTodos = asientos.All(a =>
            a.Fila >= 1 && a.Fila <= Evento.Filas &&
            a.Numero >= 1 && a.Numero <= Evento.AsientosPorFila);

        if (!existenTodos)
        {
            throw new ValidacionException("Alguno de los asientos seleccionados no existe.");
        }
    }
}
