using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.DTOs;
using Microsoft.EntityFrameworkCore;
using Artefacta.Copiloto.Security;

namespace Artefacta.Copiloto.Services;

public class OportunidadService(ArtefactaDbContext db, UsuarioActual usuarioActual)
{
    public async Task<List<OportunidadDto>> DetectarAsync(int limite = 50)
    {
        // Se toman relaciones cliente-producto con historial suficiente.
        // El cálculo de vencimiento se realiza en memoria para evitar depender
        // de traducciones específicas de funciones de fecha del proveedor Oracle.
        var query = db.ClienteProductos
            .AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Producto)
            .Where(x =>
                x.NumCompras >= 2 &&
                x.UltimaCompra != null &&
                x.PromedioDiasCompra != null &&
                x.Cliente.Activo &&
                x.Producto.Activo);

        // Un usuario con rol VENDEDOR y VENDEDOR_ID asociado ve oportunidades
        // únicamente de clientes a los que ya ha vendido. Roles globales ven todo.
        if (usuarioActual.EsVendedor &&
            !usuarioActual.VisionGlobal &&
            usuarioActual.VendedorId.HasValue)
        {
            var vendedorId = usuarioActual.VendedorId.Value;

            query = query.Where(cp =>
                db.Ventas.Any(v =>
                    v.ClienteId == cp.ClienteId &&
                    v.VendedorId == vendedorId));
        }

        var candidatos = await query
            .OrderByDescending(x => x.NumCompras)
            .ThenByDescending(x => x.ImporteTotal)
            .Take(5000)
            .ToListAsync();

        var hoy = DateTime.Today;

        var oportunidades = candidatos
            .Select(x =>
            {
                var ultima = x.UltimaCompra!.Value.Date;
                var promedio = Math.Max(1m, x.PromedioDiasCompra!.Value);
                var diasDesde = Math.Max(0, (hoy - ultima).Days);
                var diasAtraso = Math.Max(0, diasDesde - (int)Math.Round(promedio));

                if (diasAtraso <= 0)
                    return null;

                // Score determinístico 0..100:
                // 60% atraso relativo, 25% recurrencia, 15% valor histórico.
                var atrasoRelativo = Math.Min(1m, diasAtraso / promedio);
                var recurrencia = Math.Min(1m, x.NumCompras / 10m);
                var valor = Math.Min(1m, x.ImporteTotal / 50000m);

                var score = Math.Round(
                    (atrasoRelativo * 60m) +
                    (recurrencia * 25m) +
                    (valor * 15m), 2);

                var prioridad =
                    score >= 80m ? "CRITICA" :
                    score >= 65m ? "ALTA" :
                    score >= 45m ? "MEDIA" : "BAJA";

                return new OportunidadDto
                {
                    ClienteId = x.ClienteId,
                    CodigoCliente = x.Cliente.CodigoCliente,
                    Cliente = x.Cliente.Nombre,
                    ProductoId = x.ProductoId,
                    Producto = x.Producto.Nombre,
                    UltimaCompra = ultima,
                    PromedioDiasCompra = promedio,
                    DiasDesdeUltimaCompra = diasDesde,
                    DiasAtraso = diasAtraso,
                    NumCompras = x.NumCompras,
                    ImporteTotal = x.ImporteTotal,
                    Score = score,
                    Prioridad = prioridad,
                    Motivo = $"Cliente con patrón de recompra de aproximadamente {promedio:N0} días. " +
                             $"Han transcurrido {diasDesde} días desde la última compra; atraso estimado: {diasAtraso} días."
                };
            })
            .Where(x => x is not null)
            .Cast<OportunidadDto>()
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.ImporteTotal)
            .Take(limite)
            .ToList();

        return oportunidades;
    }
}
