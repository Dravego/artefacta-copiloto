using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class CopilotoController(CopilotoService copiloto, ClienteService clientes) : Controller
{
    public async Task<IActionResult> Index(string? q)
    {
        ViewBag.Q = q;
        ViewBag.Clientes = await clientes.BuscarAsync(q, 25);
        return View();
    }

    public async Task<IActionResult> Cliente(long id)
    {
        var analisis = await copiloto.AnalizarClienteAsync(id);
        return analisis is null ? NotFound() : View(analisis);
    }

    [HttpGet]
    public async Task<IActionResult> PruebaConexion()
    {
        try
        {
            return Content(await copiloto.PruebaConexionAsync(), "text/plain; charset=utf-8");
        }
        catch (Exception ex)
        {
            Response.StatusCode = 500;
            return Content($"Error de conexión: {ex.Message}", "text/plain; charset=utf-8");
        }
    }
}
