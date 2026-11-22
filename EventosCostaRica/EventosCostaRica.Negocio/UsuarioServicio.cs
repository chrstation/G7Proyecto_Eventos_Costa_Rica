using System.Net.Mail;
using EventosCostaRica.Datos;
using EventosCostaRica.Entidades;

namespace EventosCostaRica.Negocio;

/// <summary>
/// Reglas de negocio de los usuarios: autenticación, correo único y protección del último administrador.
/// </summary>
public class UsuarioServicio : IUsuarioServicio
{
    private const int LongitudMinimaContrasena = 6;

    private readonly IRepositorio<Usuario> _usuarios;
    private readonly IRepositorio<Entrada> _entradas;
    private readonly IHasheadorContrasena _hasheador;

    public UsuarioServicio(
        IRepositorio<Usuario> usuarios,
        IRepositorio<Entrada> entradas,
        IHasheadorContrasena hasheador)
    {
        _usuarios = usuarios;
        _entradas = entradas;
        _hasheador = hasheador;
    }

    public async Task<List<Usuario>> ListarAsync()
    {
        var usuarios = await _usuarios.ObtenerTodosAsync();

        return usuarios.OrderBy(u => u.Nombre).ToList();
    }

    public async Task<Usuario> ObtenerPorIdAsync(int id)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id);

        return usuario ?? throw new ValidacionException($"No existe un usuario con el Id {id}.");
    }

    public async Task<Usuario> AutenticarAsync(string correo, string contrasena)
    {
        var correoNormalizado = NormalizarCorreo(correo);
        var coincidencias = await _usuarios.BuscarAsync(u => u.Correo == correoNormalizado);
        var usuario = coincidencias.FirstOrDefault();

        // El mismo mensaje para correo o contraseña incorrectos, para no revelar qué usuarios existen.
        if (usuario is null || !_hasheador.Verificar(contrasena, usuario.ContrasenaHash))
        {
            throw new ValidacionException("Correo o contraseña incorrectos.");
        }

        return usuario;
    }

    public async Task RegistrarAsync(Usuario usuario, string contrasena)
    {
        ValidarDatos(usuario);
        ValidarContrasena(contrasena);

        usuario.Correo = NormalizarCorreo(usuario.Correo);

        if (await _usuarios.ExisteAsync(u => u.Correo == usuario.Correo))
        {
            throw new ValidacionException("Ya existe un usuario con ese correo.");
        }

        // El Id lo genera la base de datos, por lo que nunca se repite.
        usuario.Id = 0;
        usuario.ContrasenaHash = _hasheador.Hashear(contrasena);
        await _usuarios.AgregarAsync(usuario);
    }

    public async Task ModificarAsync(Usuario usuario, string? nuevaContrasena)
    {
        ValidarDatos(usuario);

        var existente = await _usuarios.ObtenerPorIdAsync(usuario.Id)
            ?? throw new ValidacionException($"No existe un usuario con el Id {usuario.Id}.");

        var correo = NormalizarCorreo(usuario.Correo);

        if (await _usuarios.ExisteAsync(u => u.Correo == correo && u.Id != usuario.Id))
        {
            throw new ValidacionException("Otro usuario ya utiliza ese correo.");
        }

        if (existente.Rol == RolUsuario.Administrador && usuario.Rol != RolUsuario.Administrador)
        {
            await ValidarQueNoSeaElUltimoAdministradorAsync();
        }

        existente.Nombre = usuario.Nombre;
        existente.Correo = correo;
        existente.Rol = usuario.Rol;

        // Si no se escribe contraseña nueva, se conserva la actual.
        if (!string.IsNullOrWhiteSpace(nuevaContrasena))
        {
            ValidarContrasena(nuevaContrasena);
            existente.ContrasenaHash = _hasheador.Hashear(nuevaContrasena);
        }

        await _usuarios.ActualizarAsync(existente);
    }

    public async Task BorrarAsync(int id, int idUsuarioActual)
    {
        if (id == idUsuarioActual)
        {
            throw new ValidacionException("No puede borrar su propio usuario.");
        }

        var existente = await _usuarios.ObtenerPorIdAsync(id)
            ?? throw new ValidacionException($"No existe un usuario con el Id {id}.");

        if (await _entradas.ExisteAsync(e => e.UsuarioId == id))
        {
            throw new ValidacionException("No se puede borrar el usuario porque tiene entradas compradas.");
        }

        if (existente.Rol == RolUsuario.Administrador)
        {
            await ValidarQueNoSeaElUltimoAdministradorAsync();
        }

        await _usuarios.EliminarAsync(id);
    }

    private async Task ValidarQueNoSeaElUltimoAdministradorAsync()
    {
        var administradores = await _usuarios.ContarAsync(u => u.Rol == RolUsuario.Administrador);

        if (administradores <= 1)
        {
            throw new ValidacionException("Debe existir al menos un usuario administrador.");
        }
    }

    private static string NormalizarCorreo(string correo)
    {
        return correo.Trim().ToLowerInvariant();
    }

    private static void ValidarDatos(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            throw new ValidacionException("El nombre es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(usuario.Correo) || !MailAddress.TryCreate(usuario.Correo.Trim(), out _))
        {
            throw new ValidacionException("El correo electrónico no tiene un formato válido.");
        }

        if (!Enum.IsDefined(usuario.Rol))
        {
            throw new ValidacionException("El rol seleccionado no es válido.");
        }
    }

    private static void ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena) || contrasena.Length < LongitudMinimaContrasena)
        {
            throw new ValidacionException($"La contraseña debe tener al menos {LongitudMinimaContrasena} caracteres.");
        }
    }
}
