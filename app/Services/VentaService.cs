using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Models;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class VentaService(ArtefactaDbContext db)
{
    public Task<List<Venta>> UltimasAsync(int limite = 100) =>
        db.Ventas.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .OrderByDescending(x => x.FechaVenta)
            .Take(limite)
            .ToListAsync();

    public Task<Venta?> ObtenerDetalleAsync(long id) =>
        db.Ventas.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Vendedor)
            .Include(x => x.Detalles)
                .ThenInclude(x => x.Producto)
            .FirstOrDefaultAsync(x => x.VentaId == id);
}
