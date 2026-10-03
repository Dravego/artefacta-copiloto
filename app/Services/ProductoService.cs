using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Models;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class ProductoService(ArtefactaDbContext db)
{
    public Task<List<Producto>> BuscarAsync(string? texto, int limite = 100)
    {
        var q = db.Productos.AsNoTracking().Include(x => x.Categoria).AsQueryable();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.Trim().ToUpper();
            q = q.Where(x =>
                x.CodigoProducto.ToUpper().Contains(t) ||
                x.Nombre.ToUpper().Contains(t) ||
                (x.Sku != null && x.Sku.ToUpper().Contains(t)));
        }

        return q.OrderBy(x => x.Nombre).Take(limite).ToListAsync();
    }
}
