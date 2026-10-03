using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.DTOs;
using Artefacta.Copiloto.Models;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class RecomendacionService(
    ArtefactaDbContext db,
    OportunidadService oportunidades)
{
    public Task<List<Recomendacion>> PendientesAsync(long? clienteId = null, int limite = 100)
    {
        var q = db.Recomendaciones.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Producto)
            .Where(x => x.Estatus == "PENDIENTE");

        if (clienteId.HasValue)
            q = q.Where(x => x.ClienteId == clienteId.Value);

        return q.OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.FechaGeneracion)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<GeneracionRecomendacionesDto> GenerarDesdeOportunidadesAsync(int limite = 50)
    {
        var detectadas = await oportunidades.DetectarAsync(limite);

        var resultado = new GeneracionRecomendacionesDto
        {
            OportunidadesAnalizadas = detectadas.Count
        };

        foreach (var o in detectadas)
        {
            var duplicadas = await db.Recomendaciones.CountAsync(x =>
                x.ClienteId == o.ClienteId &&
                x.ProductoId == o.ProductoId &&
                x.Tipo == "RECOMPRA" &&
                (x.Estatus == "PENDIENTE" || x.Estatus == "CONTACTADO"));

            if (duplicadas > 0)
            {
                resultado.RecomendacionesOmitidasPorDuplicado++;
                continue;
            }

            db.Recomendaciones.Add(new Recomendacion
            {
                ClienteId = o.ClienteId,
                ProductoId = o.ProductoId,
                Tipo = "RECOMPRA",
                Score = o.Score,
                Prioridad = o.Prioridad,
                Motivo = o.Motivo,
                FechaGeneracion = DateTime.Now,
                FechaExpiracion = DateTime.Now.AddDays(30),
                Estatus = "PENDIENTE"
            });

            resultado.RecomendacionesCreadas++;
        }

        if (resultado.RecomendacionesCreadas > 0)
            await db.SaveChangesAsync();

        return resultado;
    }

    public async Task<bool> CambiarEstatusAsync(long id, string estatus)
    {
        var permitidos = new[] { "PENDIENTE", "CONTACTADO", "ACEPTADA", "RECHAZADA", "EXPIRADA" };
        if (!permitidos.Contains(estatus))
            return false;

        var rec = await db.Recomendaciones.FirstOrDefaultAsync(x => x.RecomendacionId == id);
        if (rec is null)
            return false;

        rec.Estatus = estatus;

        db.HistorialRecomendaciones.Add(new HistorialRecomendacion
        {
            RecomendacionId = rec.RecomendacionId,
            Fecha = DateTime.Now,
            Accion = "CAMBIO_ESTATUS",
            Resultado = $"Estatus actualizado a {estatus}."
        });

        await db.SaveChangesAsync();
        return true;
    }
}
