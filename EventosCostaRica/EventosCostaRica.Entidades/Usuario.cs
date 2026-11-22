namespace EventosCostaRica.Entidades;

/// <summary>
/// Usuario del sistema (administrador o cliente).
/// </summary>
public class Usuario : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string ContrasenaHash { get; set; } = string.Empty;

    public RolUsuario Rol { get; set; } = RolUsuario.Cliente;

    public List<Entrada> Entradas { get; set; } = new();
}
