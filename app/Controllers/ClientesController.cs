using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class ClientesController(ClienteService clientes) : Controller
{
    public async Task<IActionResult> Index(string? q)
    {
        ViewBag.Q = q;
        return View(await clientes.BuscarAsync(q));
    }
}
