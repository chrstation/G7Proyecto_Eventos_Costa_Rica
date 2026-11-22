using System.ComponentModel.DataAnnotations;

namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos del formulario de inicio de sesión.
/// </summary>
public class LoginViewModel
{
    [Display(Name = "Correo electrónico")]
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    [Display(Name = "Contraseña")]
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
