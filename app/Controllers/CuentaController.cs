using System.Security.Claims;
using Artefacta.Copiloto.Services;
using Artefacta.Copiloto.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artefacta.Copiloto.Controllers;

[AllowAnonymous]
public class CuentaController(AutenticacionService autenticacion) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.PuedeInicializarAdmin = !await autenticacion.ExisteAdminActivoAsync();

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewBag.PuedeInicializarAdmin = !await autenticacion.ExisteAdminActivoAsync();

        // IMPORTANTE:
        // El estado "password pendiente" se evalúa ANTES de validar la contraseña.
        // Así, incluso si el campo contraseña está vacío, un usuario cuyo
        // PASSWORD_HASH sea el valor centinela será enviado al cambio obligatorio.
        if (!string.IsNullOrWhiteSpace(model.Username) &&
            await autenticacion.RequiereCambioPasswordAsync(model.Username))
        {
            return RedirectToAction(
                nameof(CambiarPasswordInicial),
                new
                {
                    username = model.Username.Trim(),
                    returnUrl = model.ReturnUrl
                });
        }

        if (!ModelState.IsValid)
            return View(model);

        var usuario = await autenticacion.ValidarAsync(
            model.Username,
            model.Password);

        if (usuario is null)
        {
            ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            return View(model);
        }

        await IniciarSesionAsync(usuario, model.Recordarme);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
            Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> CambiarPasswordInicial(
        string username,
        string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            return RedirectToAction(nameof(Login));

        // La pantalla sólo existe para usuarios activos cuyo hash siga
        // siendo exactamente el valor centinela.
        if (!await autenticacion.RequiereCambioPasswordAsync(username))
        {
            TempData["Mensaje"] =
                "El usuario ya tiene una contraseña definida o no está activo.";
            return RedirectToAction(nameof(Login));
        }

        ViewBag.ReturnUrl = returnUrl;

        return View(new CambioPasswordInicialViewModel
        {
            Username = username
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPasswordInicial(
        CambioPasswordInicialViewModel model,
        string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var actualizado = await autenticacion.EstablecerPasswordPendienteAsync(
            model.Username,
            model.NuevaPassword);

        if (!actualizado)
        {
            ModelState.AddModelError(
                "",
                "No fue posible cambiar la contraseña. El usuario ya no está pendiente de cambio o está inactivo.");
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        TempData["Mensaje"] =
            "Contraseña establecida correctamente. Inicia sesión con la nueva contraseña.";

        return RedirectToAction(
            nameof(Login),
            new { returnUrl });
    }


    [HttpGet]
    [Authorize]
    public IActionResult CambiarMiPassword()
        => View(new CambiarMiPasswordViewModel());

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarMiPassword(
        CambiarMiPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(idClaim, out var usuarioId))
            return RedirectToAction(nameof(Login));

        var ok = await autenticacion.CambiarPasswordAutenticadoAsync(
            usuarioId,
            model.PasswordActual,
            model.NuevaPassword);

        if (!ok)
        {
            ModelState.AddModelError(
                "",
                "La contraseña actual no es correcta.");
            return View(model);
        }

        TempData["Mensaje"] =
            "Contraseña actualizada correctamente.";

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> InicializarAdministrador()
    {
        if (await autenticacion.ExisteAdminActivoAsync())
            return RedirectToAction(nameof(Login));

        return View(new BootstrapAdminViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> InicializarAdministrador(
        BootstrapAdminViewModel model)
    {
        if (await autenticacion.ExisteAdminActivoAsync())
        {
            ModelState.AddModelError("", "Ya existe un administrador activo.");
            return View(model);
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await autenticacion.CrearPrimerAdminAsync(
                model.Username,
                model.Nombre,
                model.Email,
                model.Password);

            TempData["Mensaje"] = "Administrador creado. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }


    [HttpGet]
    public async Task<IActionResult> DiagnosticoPassword(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return Content("Indica ?username=admin", "text/plain; charset=utf-8");

        var pendiente = await autenticacion.RequiereCambioPasswordAsync(username);

        return Content(
            $"Usuario={username.Trim()} | CambioPasswordPendiente={(pendiente ? "SI" : "NO")}",
            "text/plain; charset=utf-8");
    }

    [HttpGet]
    public IActionResult AccesoDenegado() => View();

    private async Task IniciarSesionAsync(
        Models.Usuario usuario,
        bool recordar)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new("nombre", usuario.Nombre)
        };

        if (usuario.VendedorId.HasValue)
        {
            claims.Add(new Claim(
                "vendedor_id",
                usuario.VendedorId.Value.ToString()));
        }

        foreach (var rol in usuario.Roles
                     .Select(x => x.Rol.Nombre)
                     .Distinct())
        {
            claims.Add(new Claim(ClaimTypes.Role, rol));
        }

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties
            {
                IsPersistent = recordar,
                AllowRefresh = true,
                ExpiresUtc = recordar
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddHours(8)
            });
    }
}
