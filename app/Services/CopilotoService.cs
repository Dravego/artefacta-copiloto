using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class CopilotoService(ArtefactaDbContext db)
{
    public async Task<DashboardDto> ObtenerDashboardAsync()
    {
        var inicioMes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var ventasMesQuery = db.Ventas.AsNoTracking()
            .Where(x => x.Estatus == "COMPLETADA" && x.FechaVenta >= inicioMes);

        var ventasMes = await ventasMesQuery.CountAsync();
        var totalMes = await ventasMesQuery.SumAsync(x => (decimal?)x.Total) ?? 0m;

        var topVendedores = await ventasMesQuery
            .Where(x => x.VendedorId != null)
            .GroupBy(x => new { x.VendedorId, x.Vendedor!.Nombre })
            .Select(g => new TopVendedorDto
            {
                VendedorId = g.Key.VendedorId!.Value,
                Vendedor = g.Key.Nombre,
                Ventas = g.Count(),
                Total = g.Sum(x => x.Total)
            })
            .OrderByDescending(x => x.Total)
            .Take(5)
            .ToListAsync();

        var topProductos = await db.DetallesVenta.AsNoTracking()
            .Where(x =>
                x.Venta.Estatus == "COMPLETADA" &&
                x.Venta.FechaVenta >= inicioMes)
            .GroupBy(x => new { x.ProductoId, x.Producto.Nombre })
            .Select(g => new TopProductoDto
            {
                ProductoId = g.Key.ProductoId,
                Producto = g.Key.Nombre,
                Cantidad = g.Sum(x => x.Cantidad),
                Importe = g.Sum(x => x.Importe)
            })
            .OrderByDescending(x => x.Importe)
            .Take(5)
            .ToListAsync();

        return new DashboardDto
        {
            ClientesActivos = await db.Clientes.AsNoTracking().CountAsync(x => x.Activo),
            ProductosActivos = await db.Productos.AsNoTracking().CountAsync(x => x.Activo),
            VentasMes = ventasMes,
            TotalVentasMes = totalMes,
            TicketPromedioMes = ventasMes == 0 ? 0m : totalMes / ventasMes,
            RecomendacionesPendientes = await db.Recomendaciones.AsNoTracking()
                .CountAsync(x => x.Estatus == "PENDIENTE"),
            TopVendedores = topVendedores,
            TopProductos = topProductos
        };
    }

    public async Task<CopilotoClienteDto?> AnalizarClienteAsync(long clienteId)
    {
        var cliente = await db.Clientes.AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .Select(x => new { x.ClienteId, x.CodigoCliente, x.Nombre })
            .FirstOrDefaultAsync();

        if (cliente is null) return null;

        var resumenVentas = await db.Ventas.AsNoTracking()
            .Where(x => x.ClienteId == clienteId && x.Estatus == "COMPLETADA")
            .GroupBy(_ => 1)
            .Select(g => new
            {
                UltimaCompra = g.Max(x => (DateTime?)x.FechaVenta),
                Total = g.Sum(x => x.Total),
                Numero = g.Count()
            })
            .FirstOrDefaultAsync();

        var frecuentes = await db.ClienteProductos.AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.NumCompras)
            .ThenByDescending(x => x.ImporteTotal)
            .Take(10)
            .Select(x => new ProductoFrecuenteDto
            {
                ProductoId = x.ProductoId,
                Producto = x.Producto.Nombre,
                NumCompras = x.NumCompras,
                ImporteTotal = x.ImporteTotal,
                UltimaCompra = x.UltimaCompra
            })
            .ToListAsync();

        var recomendaciones = await db.Recomendaciones.AsNoTracking()
            .Where(x => x.ClienteId == clienteId &&
                (x.Estatus == "PENDIENTE" || x.Estatus == "CONTACTADO"))
            .OrderByDescending(x => x.Score)
            .Take(10)
            .Select(x => new RecomendacionDto
            {
                RecomendacionId = x.RecomendacionId,
                Tipo = x.Tipo,
                Prioridad = x.Prioridad,
                Score = x.Score,
                Motivo = x.Motivo,
                Producto = x.Producto != null ? x.Producto.Nombre : null,
                Estatus = x.Estatus
            })
            .ToListAsync();

        var recientes = await db.Ventas.AsNoTracking()
            .Where(x => x.ClienteId == clienteId)
            .OrderByDescending(x => x.FechaVenta)
            .Take(10)
            .Select(x => new VentaRecienteDto
            {
                VentaId = x.VentaId,
                Folio = x.Folio,
                FechaVenta = x.FechaVenta,
                Total = x.Total,
                Estatus = x.Estatus
            })
            .ToListAsync();

        var ultima = resumenVentas?.UltimaCompra;
        var numero = resumenVentas?.Numero ?? 0;
        var total = resumenVentas?.Total ?? 0m;

        return new CopilotoClienteDto
        {
            ClienteId = cliente.ClienteId,
            CodigoCliente = cliente.CodigoCliente,
            Nombre = cliente.Nombre,
            UltimaCompra = ultima,
            TotalHistorico = total,
            NumeroVentas = numero,
            TicketPromedio = numero == 0 ? 0m : total / numero,
            DiasSinComprar = ultima.HasValue
                ? Math.Max(0, (DateTime.Today - ultima.Value.Date).Days)
                : 0,
            ProductosFrecuentes = frecuentes,
            Recomendaciones = recomendaciones,
            VentasRecientes = recientes
        };
    }

    public async Task<string> PruebaConexionAsync()
    {
        await db.Database.OpenConnectionAsync();

        try
        {
            var clientes = await db.Clientes.AsNoTracking().CountAsync();
            var productos = await db.Productos.AsNoTracking().CountAsync();
            var ventas = await db.Ventas.AsNoTracking().CountAsync();

            return $"Conexión correcta. CLIENTES={clientes:N0}, PRODUCTOS={productos:N0}, VENTAS={ventas:N0}.";
        }
        finally
        {
            await db.Database.CloseConnectionAsync();
        }
    }
}
