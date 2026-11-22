using System.Linq.Expressions;
using EventosCostaRica.Entidades;

namespace EventosCostaRica.Datos;

/// <summary>
/// Repositorio genérico: define las operaciones CRUD una sola vez para todas las entidades.
/// </summary>
/// <typeparam name="T">Tipo de entidad.</typeparam>
public interface IRepositorio<T>
    where T : EntidadBase
{
    Task<List<T>> ObtenerTodosAsync(params Expression<Func<T, object>>[] incluir);

    Task<List<T>> BuscarAsync(Expression<Func<T, bool>> condicion, params Expression<Func<T, object>>[] incluir);

    Task<T?> ObtenerPorIdAsync(int id, params Expression<Func<T, object>>[] incluir);

    Task<bool> ExisteAsync(Expression<Func<T, bool>> condicion);

    Task<int> ContarAsync(Expression<Func<T, bool>> condicion);

    Task AgregarAsync(T entidad);

    Task AgregarVariosAsync(IEnumerable<T> entidades);

    Task<bool> ActualizarAsync(T entidad);

    Task<bool> EliminarAsync(int id);
}
