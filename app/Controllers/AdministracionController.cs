using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Services;
using Artefacta.Copiloto.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN")]
public class AdministracionController(
    ArtefactaDbContext db,
    AdministracionUsuariosService usuariosService,
    AutenticacionService autenticacion) : Controller
{
    public async Task<IActionResult> Usuarios()
    {
        var usuarios = await db.Usuarios
            .AsNoTracking()
            .Include(x => x.Vendedor)
            .Include(x => x.Roles)
                .ThenInclude(x => x.Rol)
            .OrderBy(x => x.Username)
            .Select(x => new UsuarioListaViewModel
            {
                UsuarioId = x.UsuarioId,
                Username = x.Username,
                Nombre = x.Nombre,
                Email = x.Email,
                Vendedor = x.Vendedor != null ? x.Vendedor.Nombre : null,
                Activo = x.Activo,
                Roles = x.Roles.Select(r => r.Rol.Nombre).OrderBy(r => r).ToList()
            })
            .ToListAsync();

        return View(usuarios);
    }

    [HttpGet]
    public async Task<IActionResult> Nuevo()
        => View(await usuariosService.PrepararNuevoAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nuevo(UsuarioCrearViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await usuariosService.CargarCatalogosAsync(model);
            return View(model);
        }

        var resultado = await usuariosService.CrearAsync(model);

        if (!resultado.ok)
        {
            ModelState.AddModelError("", resultado.mensaje);
            await usuariosService.CargarCatalogosAsync(model);
            return View(model);
        }

        TempData["Mensaje"] = resultado.mensaje;
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpGet]
    public async Task<IActionResult> Editar(long id)
    {
        var model = await usuariosService.PrepararEdicionAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(UsuarioEditarViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await usuariosService.CargarCatalogosAsync(model);
            return View(model);
        }

        var resultado = await usuariosService.ActualizarAsync(model);

        if (!resultado.ok)
        {
            ModelState.AddModelError("", resultado.mensaje);
            await usuariosService.CargarCatalogosAsync(model);
            return View(model);
        }

        TempData["Mensaje"] = resultado.mensaje;
        return RedirectToAction(nameof(Usuarios));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(long id)
    {
        var ok = await autenticacion.ForzarCambioPasswordAsync(id);

        TempData["Mensaje"] = ok
            ? "Contraseña marcada para cambio obligatorio en el próximo acceso."
            : "No se encontró el usuario.";

        return RedirectToAction(nameof(Usuarios));
    }
}
