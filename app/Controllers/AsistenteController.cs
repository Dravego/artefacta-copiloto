using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class AsistenteController(CopilotoConversacionalService copiloto) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? pregunta)
    {
        var modelo = await copiloto.ResponderAsync(pregunta);
        return View(modelo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Preguntar(string? pregunta)
    {
        return RedirectToAction(nameof(Index), new { pregunta });
    }
}
