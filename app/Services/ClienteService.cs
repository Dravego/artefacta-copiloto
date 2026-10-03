using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.Models;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class ClienteService(ArtefactaDbContext db)
{
    public Task<List<Cliente>> BuscarAsync(string? texto, int limite = 100)
    {
        var q = db.Clientes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.Trim().ToUpper();
            q = q.Where(x =>
                x.CodigoCliente.ToUpper().Contains(t) ||
                x.Nombre.ToUpper().Contains(t) ||
                (x.Rfc != null && x.Rfc.ToUpper().Contains(t)));
        }

        return q.OrderBy(x => x.Nombre).Take(limite).ToListAsync();
    }

    public Task<Cliente?> ObtenerAsync(long id) =>
        db.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.ClienteId == id);
}
