using EventosCostaRica.Negocio;
using EventosCostaRica.Web.Infraestructura;
using EventosCostaRica.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventosCostaRica.Web.Controllers;

/// <summary>
/// Administración de usuarios (crear, leer, actualizar y borrar). Solo administradores.
/// </summary>
[Authorize(Roles = NombresRol.Administrador)]
public class AdminUsuariosController : Controller
{
    private readonly IUsuarioServicio _servicio;

    public AdminUsuariosController(IUsuarioServicio servicio)
    {
        _servicio = servicio;
    }

    // GET: /AdminUsuarios
    public async Task<IActionResult> Index()
    {
        var usuarios = await _servicio.ListarAsync();

        return View(usuarios.Select(UsuarioViewModel.DesdeEntidad).ToList());
    }

    // GET: /AdminUsuarios/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new UsuarioViewModel());
    }

    // POST: /AdminUsuarios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioViewModel modelo)
    {
        if (string.IsNullOrWhiteSpace(modelo.Contrasena))
        {
            ModelState.AddModelError(nameof(modelo.Contrasena), "La contraseña es obligatoria.");
        }

        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _servicio.RegistrarAsync(modelo.ComoEntidad(), modelo.Contrasena!);
            TempData["Mensaje"] = "Usuario registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }
    }

    // GET: /AdminUsuarios/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var usuario = await _servicio.ObtenerPorIdAsync(id);

        return View(UsuarioViewModel.DesdeEntidad(usuario));
    }

    // POST: /AdminUsuarios/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioViewModel modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            await _servicio.ModificarAsync(modelo.ComoEntidad(), modelo.Contrasena);
            TempData["Mensaje"] = "Usuario modificado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidacionException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            return View(modelo);
        }
    }

    // GET: /AdminUsuarios/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _servicio.ObtenerPorIdAsync(id);

        return View(UsuarioViewModel.DesdeEntidad(usuario));
    }

    // POST: /AdminUsuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _servicio.BorrarAsync(id, User.ObtenerIdUsuario());
            TempData["Mensaje"] = "Usuario borrado correctamente.";
        }
        catch (ValidacionException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
