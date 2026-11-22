using System.Linq.Expressions;
using EventosCostaRica.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EventosCostaRica.Datos;

/// <summary>
/// Implementación genérica del repositorio con Entity Framework Core.
/// </summary>
/// <typeparam name="T">Tipo de entidad.</typeparam>
public class Repositorio<T> : IRepositorio<T>
    where T : EntidadBase
{
    private readonly AppDbContext _contexto;
    private readonly DbSet<T> _conjunto;

    public Repositorio(AppDbContext contexto)
    {
        _contexto = contexto;
        _conjunto = contexto.Set<T>();
    }

    public async Task<List<T>> ObtenerTodosAsync(params Expression<Func<T, object>>[] incluir)
    {
        return await ConstruirConsulta(incluir).ToListAsync();
    }

    public async Task<List<T>> BuscarAsync(Expression<Func<T, bool>> condicion, params Expression<Func<T, object>>[] incluir)
    {
        return await ConstruirConsulta(incluir).Where(condicion).ToListAsync();
    }

    public async Task<T?> ObtenerPorIdAsync(int id, params Expression<Func<T, object>>[] incluir)
    {
        return await ConstruirConsulta(incluir).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<bool> ExisteAsync(Expression<Func<T, bool>> condicion)
    {
        return await _conjunto.AnyAsync(condicion);
    }

    public async Task<int> ContarAsync(Expression<Func<T, bool>> condicion)
    {
        return await _conjunto.CountAsync(condicion);
    }

    public async Task AgregarAsync(T entidad)
    {
        _conjunto.Add(entidad);
        await _contexto.SaveChangesAsync();
    }

    public async Task AgregarVariosAsync(IEnumerable<T> entidades)
    {
        // Un solo SaveChanges: todas las entidades se guardan juntas o ninguna.
        _conjunto.AddRange(entidades);
        await _contexto.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAsync(T entidad)
    {
        if (!await _conjunto.AnyAsync(e => e.Id == entidad.Id))
        {
            return false;
        }

        _conjunto.Update(entidad);
        await _contexto.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var existente = await _conjunto.FindAsync(id);

        if (existente is null)
        {
            return false;
        }

        _conjunto.Remove(existente);
        await _contexto.SaveChangesAsync();
        return true;
    }

    private IQueryable<T> ConstruirConsulta(Expression<Func<T, object>>[] incluir)
    {
        IQueryable<T> consulta = _conjunto.AsNoTracking();

        foreach (var propiedad in incluir)
        {
            consulta = consulta.Include(propiedad);
        }

        return consulta;
    }
}
