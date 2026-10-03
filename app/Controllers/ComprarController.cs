using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class ComprarController(CompraService compraService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? q)
    {
        ViewBag.Q = q;
        return View(await compraService.BuscarClientesAsync(q));
    }

    [HttpGet]
    public async Task<IActionResult> Cliente(long id)
    {
        var model = await compraService.ObtenerSugerenciasAsync(id);

        return model is null
            ? NotFound()
            : View(model);
    }
}
