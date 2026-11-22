namespace EventosCostaRica.Web.Models;

/// <summary>
/// Datos que se muestran en la página de error.
/// </summary>
public class ErrorViewModel
{
    public int Codigo { get; set; } = 500;

    public string Titulo { get; set; } = string.Empty;

    public string Mensaje { get; set; } = string.Empty;

    public string IdSolicitud { get; set; } = string.Empty;
}
