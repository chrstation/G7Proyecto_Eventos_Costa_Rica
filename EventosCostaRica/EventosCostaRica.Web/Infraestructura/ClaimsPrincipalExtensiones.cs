using System.Security.Claims;

namespace EventosCostaRica.Web.Infraestructura;

/// <summary>
/// Métodos de ayuda para leer datos del usuario autenticado.
/// </summary>
public static class ClaimsPrincipalExtensiones
{
    public static int ObtenerIdUsuario(this ClaimsPrincipal usuario)
    {
        var valor = usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return int.TryParse(valor, out var id) ? id : 0;
    }
}
