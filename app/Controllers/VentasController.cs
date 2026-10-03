using Artefacta.Copiloto.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Artefacta.Copiloto.Controllers;

[Authorize(Roles = "ADMIN,GERENTE,SUPERVISOR,VENDEDOR,CONSULTA")]
public class VentasController(VentaService ventas) : Controller
{
    public async Task<IActionResult> Index() => View(await ventas.UltimasAsync());

    public async Task<IActionResult> Detalle(long id)
    {
        var venta = await ventas.ObtenerDetalleAsync(id);
        return venta is null ? NotFound() : View(venta);
    }
}
