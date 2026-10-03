using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class RecomendacionesController(RecomendacionService recomendaciones) : Controller
{
    public async Task<IActionResult> Index()
        => View(await recomendaciones.PendientesAsync());

    [Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Estatus(long id, string estatus, long? clienteId = null)
    {
        var ok = await recomendaciones.CambiarEstatusAsync(id, estatus);
        TempData["Mensaje"] = ok
            ? $"Recomendación actualizada a {estatus}."
            : "No fue posible actualizar la recomendación.";

        if (clienteId.HasValue)
            return RedirectToAction("Cliente", "Copiloto", new { id = clienteId.Value });

        return RedirectToAction(nameof(Index));
    }
}
