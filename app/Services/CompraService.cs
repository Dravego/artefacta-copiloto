using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.DTOs;
using Artefacta.Copiloto.Security;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class CompraService(
    ArtefactaDbContext db,
    UsuarioActual usuarioActual)
{
    public async Task<List<Models.Cliente>> BuscarClientesAsync(
        string? texto,
        int limite = 50)
    {
        var q = db.Clientes
            .AsNoTracking()
            .Where(x => x.Activo);

        // Un vendedor sólo ve clientes con los que ya tiene ventas.
        if (usuarioActual.EsVendedor &&
            !usuarioActual.VisionGlobal &&
            usuarioActual.VendedorId.HasValue)
        {
            var vendedorId = usuarioActual.VendedorId.Value;

            q = q.Where(c =>
                db.Ventas.Any(v =>
                    v.ClienteId == c.ClienteId &&
                    v.VendedorId == vendedorId));
        }

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.Trim().ToUpper();

            q = q.Where(x =>
                x.CodigoCliente.ToUpper().Contains(t) ||
                x.Nombre.ToUpper().Contains(t) ||
                (x.Rfc != null && x.Rfc.ToUpper().Contains(t)));
        }

        return await q
            .OrderBy(x => x.Nombre)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<CompraClienteDto?> ObtenerSugerenciasAsync(long clienteId)
    {
        if (!await PuedeVerClienteAsync(clienteId))
            return null;

        var cliente = await db.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClienteId == clienteId && x.Activo);

        if (cliente is null)
            return null;

        var resumen = await db.Ventas
            .AsNoTracking()
            .Where(x =>
                x.ClienteId == clienteId &&
                x.Estatus == "COMPLETADA")
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Numero = g.Count(),
                Total = g.Sum(x => x.Total),
                Ultima = g.Max(x => (DateTime?)x.FechaVenta)
            })
            .FirstOrDefaultAsync();

        var recompra = await ObtenerRecompraAsync(clienteId);
        var ventaCruzada = await ObtenerVentaCruzadaAsync(clienteId, recompra);
        var populares = await ObtenerPopularesAsync(clienteId, recompra, ventaCruzada);

        return new CompraClienteDto
        {
            ClienteId = cliente.ClienteId,
            CodigoCliente = cliente.CodigoCliente,
            Cliente = cliente.Nombre,
            UltimaCompra = resumen?.Ultima,
            TotalHistorico = resumen?.Total ?? 0m,
            NumeroVentas = resumen?.Numero ?? 0,
            Recompra = recompra,
            VentaCruzada = ventaCruzada,
            Populares = populares
        };
    }

    private async Task<List<ProductoCompraDto>> ObtenerRecompraAsync(long clienteId)
    {
        var hoy = DateTime.Today;

        var historial = await db.ClienteProductos
            .AsNoTracking()
            .Include(x => x.Producto)
                .ThenInclude(x => x.Categoria)
            .Where(x =>
                x.ClienteId == clienteId &&
                x.Producto.Activo &&
                x.NumCompras >= 2)
            .OrderByDescending(x => x.NumCompras)
            .ThenByDescending(x => x.ImporteTotal)
            .Take(100)
            .ToListAsync();

        var candidatos = historial
            .Select(x =>
            {
                var dias = x.UltimaCompra.HasValue
                    ? Math.Max(0, (hoy - x.UltimaCompra.Value.Date).Days)
                    : 0;

                var promedio = x.PromedioDiasCompra ?? 0m;
                var atraso = promedio > 0
                    ? Math.Max(0m, dias - promedio)
                    : 0m;

                var scoreFrecuencia = Math.Min(40m, x.NumCompras * 5m);
                var scoreAtraso = promedio > 0
                    ? Math.Min(45m, (atraso / promedio) * 45m)
                    : 0m;
                var scoreValor = Math.Min(15m, x.ImporteTotal / 50000m * 15m);

                var score = Math.Round(scoreFrecuencia + scoreAtraso + scoreValor, 2);

                return new ProductoCompraDto
                {
                    ProductoId = x.ProductoId,
                    CodigoProducto = x.Producto.CodigoProducto,
                    Producto = x.Producto.Nombre,
                    Categoria = x.Producto.Categoria?.Nombre,
                    Precio = x.Producto.Precio,
                    TipoSugerencia = "RECOMPRA",
                    Score = score,
                    Motivo = promedio > 0
                        ? $"Lo ha comprado {x.NumCompras} veces. Última compra hace {dias} días; frecuencia histórica aproximada: {promedio:N0} días."
                        : $"Lo ha comprado {x.NumCompras} veces y acumula {x.ImporteTotal:C2} en compras."
                };
            })
            .OrderByDescending(x => x.Score)
            .Take(8)
            .ToList();

        await AplicarPromocionesAsync(candidatos);
        return candidatos;
    }

    private async Task<List<ProductoCompraDto>> ObtenerVentaCruzadaAsync(
        long clienteId,
        List<ProductoCompraDto> recompra)
    {
        var productosComprados = await db.ClienteProductos
            .AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .Select(x => x.ProductoId)
            .ToListAsync();

        var categoriasCliente = await db.ClienteProductos
            .AsNoTracking()
            .Where(x =>
                x.ClienteId == clienteId &&
                x.Producto.CategoriaId != null)
            .Select(x => x.Producto.CategoriaId!.Value)
            .Distinct()
            .ToListAsync();

        if (categoriasCliente.Count == 0)
            return [];

        var desde = DateTime.Today.AddMonths(-12);

        var candidatos = await db.DetallesVenta
            .AsNoTracking()
            .Where(d =>
                d.Venta.Estatus == "COMPLETADA" &&
                d.Venta.FechaVenta >= desde &&
                d.Producto.Activo &&
                d.Producto.CategoriaId != null &&
                categoriasCliente.Contains(d.Producto.CategoriaId.Value) &&
                !productosComprados.Contains(d.ProductoId))
            .GroupBy(d => new
            {
                d.ProductoId,
                d.Producto.CodigoProducto,
                d.Producto.Nombre,
                Categoria = d.Producto.Categoria!.Nombre,
                d.Producto.Precio
            })
            .Select(g => new
            {
                g.Key.ProductoId,
                g.Key.CodigoProducto,
                g.Key.Nombre,
                g.Key.Categoria,
                g.Key.Precio,
                Cantidad = g.Sum(x => x.Cantidad),
                Importe = g.Sum(x => x.Importe),
                Ventas = g.Select(x => x.VentaId).Distinct().Count()
            })
            .OrderByDescending(x => x.Importe)
            .Take(8)
            .ToListAsync();

        var resultado = candidatos.Select(x => new ProductoCompraDto
        {
            ProductoId = x.ProductoId,
            CodigoProducto = x.CodigoProducto,
            Producto = x.Nombre,
            Categoria = x.Categoria,
            Precio = x.Precio,
            TipoSugerencia = "VENTA_CRUZADA",
            Score = Math.Round(
                Math.Min(100m,
                    Math.Min(60m, x.Ventas * 3m) +
                    Math.Min(40m, x.Importe / 100000m * 40m)), 2),
            Motivo = $"Producto popular dentro de categorías que este cliente ya compra. {x.Ventas:N0} ventas y {x.Importe:C2} vendidos en los últimos 12 meses."
        }).ToList();

        await AplicarPromocionesAsync(resultado);
        return resultado;
    }

    private async Task<List<ProductoCompraDto>> ObtenerPopularesAsync(
        long clienteId,
        List<ProductoCompraDto> recompra,
        List<ProductoCompraDto> cruzada)
    {
        var excluir = recompra.Select(x => x.ProductoId)
            .Concat(cruzada.Select(x => x.ProductoId))
            .Distinct()
            .ToList();

        var desde = DateTime.Today.AddMonths(-6);

        var top = await db.DetallesVenta
            .AsNoTracking()
            .Where(d =>
                d.Venta.Estatus == "COMPLETADA" &&
                d.Venta.FechaVenta >= desde &&
                d.Producto.Activo &&
                !excluir.Contains(d.ProductoId))
            .GroupBy(d => new
            {
                d.ProductoId,
                d.Producto.CodigoProducto,
                d.Producto.Nombre,
                Categoria = d.Producto.Categoria != null
                    ? d.Producto.Categoria.Nombre
                    : null,
                d.Producto.Precio
            })
            .Select(g => new
            {
                g.Key.ProductoId,
                g.Key.CodigoProducto,
                g.Key.Nombre,
                g.Key.Categoria,
                g.Key.Precio,
                Importe = g.Sum(x => x.Importe),
                Cantidad = g.Sum(x => x.Cantidad)
            })
            .OrderByDescending(x => x.Importe)
            .Take(6)
            .ToListAsync();

        var resultado = top.Select(x => new ProductoCompraDto
        {
            ProductoId = x.ProductoId,
            CodigoProducto = x.CodigoProducto,
            Producto = x.Nombre,
            Categoria = x.Categoria,
            Precio = x.Precio,
            TipoSugerencia = "POPULAR",
            Score = 50m,
            Motivo = $"Producto con alta demanda reciente: {x.Cantidad:N2} unidades y {x.Importe:C2} vendidos en los últimos 6 meses."
        }).ToList();

        await AplicarPromocionesAsync(resultado);
        return resultado;
    }

    private async Task AplicarPromocionesAsync(List<ProductoCompraDto> productos)
    {
        if (productos.Count == 0)
            return;

        var ids = productos.Select(x => x.ProductoId).Distinct().ToList();
        var hoy = DateTime.Today;

        var promociones = await db.PromocionProductos
            .AsNoTracking()
            .Where(x =>
                ids.Contains(x.ProductoId) &&
                x.Promocion.Activo &&
                x.Promocion.FechaInicio <= hoy &&
                x.Promocion.FechaFin >= hoy)
            .Select(x => new
            {
                x.ProductoId,
                x.Promocion.Nombre,
                x.Promocion.Valor
            })
            .ToListAsync();

        foreach (var p in productos)
        {
            var promo = promociones
                .Where(x => x.ProductoId == p.ProductoId)
                .OrderByDescending(x => x.Valor)
                .FirstOrDefault();

            if (promo is not null)
            {
                p.Promocion = promo.Nombre;
                p.ValorPromocion = promo.Valor;
            }
        }
    }

    private async Task<bool> PuedeVerClienteAsync(long clienteId)
    {
        if (!usuarioActual.EsVendedor ||
            usuarioActual.VisionGlobal ||
            !usuarioActual.VendedorId.HasValue)
        {
            return true;
        }

        var vendedorId = usuarioActual.VendedorId.Value;

        var cantidad = await db.Ventas.CountAsync(v =>
            v.ClienteId == clienteId &&
            v.VendedorId == vendedorId);

        return cantidad > 0;
    }
}
