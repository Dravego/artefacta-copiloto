using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize]
public class HomeController(CopilotoService copiloto) : Controller
{
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await copiloto.ObtenerDashboardAsync());
        }
        catch (Exception ex)
        {
            ViewBag.ErrorConexion = ex.Message;
            return View(new DTOs.DashboardDto());
        }
    }

    public IActionResult Error() => View();
}
