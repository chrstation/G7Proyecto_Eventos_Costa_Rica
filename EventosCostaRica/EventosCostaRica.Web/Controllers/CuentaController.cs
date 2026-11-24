using System.Security.Claims;
using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Inicio y cierre de sesión.
/// </summary>
[AllowAnonymous]
public class CuentaController : Controller
{
    private readonly IUsuarioServicio _usuarioServicio;

    public CuentaController(IUsuarioServicio usuarioServicio)
    {
        _usuarioServicio = usuarioServicio;
    }

    // GET: /Cuenta/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Eventos");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Cuenta/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            var usuario = await _usuarioServicio.AutenticarAsync(modelo.Correo, modelo.Contrasena);

            var reclamos = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Correo),
                new(ClaimTypes.Role, usuario.Rol.ToString())
            };

            var identidad = new ClaimsIdentity(reclamos, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad));

            if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
            {
                return LocalRedirect(modelo.ReturnUrl);
            }

            return RedirectToAction("Index", "Eventos");
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }
    }

    // POST: /Cuenta/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }

    // GET: /Cuenta/AccesoDenegado
    [HttpGet]
    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
