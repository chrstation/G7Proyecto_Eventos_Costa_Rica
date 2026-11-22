using System.ComponentModel.DataAnnotations;
using EventosCostaRica.Entidades;

namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos de un usuario para las vistas de administración.
/// </summary>
public class UsuarioViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un rol.")]
    public RolUsuario? Rol { get; set; }

    [Display(Name = "Contraseña")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string? Contrasena { get; set; }

    public static UsuarioViewModel DesdeEntidad(Usuario usuario)
    {
        return new UsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Rol = usuario.Rol
        };
    }

    public Usuario ComoEntidad()
    {
        return new Usuario
        {
            Id = Id,
            Nombre = Nombre.Trim(),
            Correo = Correo.Trim(),
            Rol = Rol.GetValueOrDefault(RolUsuario.Cliente)
        };
    }
}
