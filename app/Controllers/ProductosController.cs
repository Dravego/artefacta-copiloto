using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class ProductosController(ProductoService productos) : Controller
{
    public async Task<IActionResult> Index(string? q)
    {
        ViewBag.Q = q;
        return View(await productos.BuscarAsync(q));
    }
}
