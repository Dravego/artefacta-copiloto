using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class OportunidadesController(
    OportunidadService oportunidades,
    RecomendacionService recomendaciones) : Controller
{
    public async Task<IActionResult> Index(int limite = 50)
    {
        limite = Math.Clamp(limite, 1, 200);
        ViewBag.Limite = limite;
        return View(await oportunidades.DetectarAsync(limite));
    }

    [Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(int limite = 50)
    {
        limite = Math.Clamp(limite, 1, 200);
        var resultado = await recomendaciones.GenerarDesdeOportunidadesAsync(limite);

        TempData["Mensaje"] =
            $"Analizadas: {resultado.OportunidadesAnalizadas}. " +
            $"Creadas: {resultado.RecomendacionesCreadas}. " +
            $"Omitidas por duplicado: {resultado.RecomendacionesOmitidasPorDuplicado}.";

        return RedirectToAction(nameof(Index), new { limite });
    }
}
