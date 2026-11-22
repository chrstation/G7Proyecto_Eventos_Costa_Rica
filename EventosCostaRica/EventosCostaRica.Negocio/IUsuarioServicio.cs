using EventosCostaRica.Entidades;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Contrato de la lógica de negocio para los usuarios.
/// </summary>
public interface IUsuarioServicio
{
    Task<List<Usuario>> ListarAsync();

    Task<Usuario> ObtenerPorIdAsync(int id);

    Task<Usuario> AutenticarAsync(string correo, string contrasena);

    Task RegistrarAsync(Usuario usuario, string contrasena);

    Task ModificarAsync(Usuario usuario, string? nuevaContrasena);

    Task BorrarAsync(int id, int idUsuarioActual);
}
