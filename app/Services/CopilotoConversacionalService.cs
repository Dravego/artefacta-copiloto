using System.Globalization;
using System.Text;
using Artefacta.Copiloto.Data;
using Artefacta.Copiloto.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Artefacta.Copiloto.Services;

public class CopilotoConversacionalService(
    ArtefactaDbContext db,
    OportunidadService oportunidadService)
{
    public async Task<CopilotoChatDto> ResponderAsync(string? pregunta)
    {
        var original = (pregunta ?? "").Trim();

        if (string.IsNullOrWhiteSpace(original))
            return Bienvenida();

        var texto = Normalizar(original);

        if (ContieneAlguno(texto,
            "clientes contactar", "contactar hoy", "oportunidades", "mejores oportunidades",
            "clientes debo contactar", "a quien contactar"))
        {
            return await ResponderOportunidadesAsync(original);
        }

        if (ContieneAlguno(texto,
            "ventas del mes", "cuanto hemos vendido", "cuanto vendimos", "ventas este mes",
            "resumen de ventas", "ventas mes"))
        {
            return await ResponderVentasMesAsync(original);
        }

        if (ContieneAlguno(texto,
            "mejores vendedores", "top vendedores", "quien vende mas", "vendedores del mes"))
        {
            return await ResponderTopVendedoresAsync(original);
        }

        if (ContieneAlguno(texto,
            "productos mas vendidos", "top productos", "que productos se venden mas",
            "productos del mes"))
        {
            return await ResponderTopProductosAsync(original);
        }

        if (ContieneAlguno(texto,
            "recomendaciones pendientes", "recomendaciones activas", "pendientes de recomendar"))
        {
            return await ResponderRecomendacionesAsync(original);
        }

        // Búsqueda de cliente por texto: frases como:
        // "qué le puedo ofrecer a Sofia", "analiza cliente CLI000078", "historial de Luis"
        if (ContieneAlguno(texto,
            "ofrecer", "recomendar", "analiza", "analizar cliente", "historial", "cliente"))
        {
            var resultadoCliente = await ResponderClienteAsync(original, texto);
            if (resultadoCliente is not null)
                return resultadoCliente;
        }

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "NO_RECONOCIDA",
            Respuesta = "Todavía no reconozco esa consulta. En esta entrega el Copiloto trabaja con intenciones comerciales controladas para que cada respuesta provenga de Oracle.",
            Sugerencias =
            [
                "¿Qué clientes debo contactar hoy?",
                "¿Cuánto hemos vendido este mes?",
                "¿Quiénes son los mejores vendedores?",
                "¿Cuáles son los productos más vendidos?",
                "Muéstrame las recomendaciones pendientes."
            ]
        };
    }

    private CopilotoChatDto Bienvenida() => new()
    {
        Intencion = "BIENVENIDA",
        Respuesta = "Puedo consultar Oracle y ayudarte a priorizar trabajo comercial. Prueba una de estas preguntas.",
        Sugerencias =
        [
            "¿Qué clientes debo contactar hoy?",
            "¿Cuánto hemos vendido este mes?",
            "¿Quiénes son los mejores vendedores?",
            "¿Cuáles son los productos más vendidos?",
            "Muéstrame las recomendaciones pendientes."
        ]
    };

    private async Task<CopilotoChatDto> ResponderOportunidadesAsync(string original)
    {
        var oportunidades = await oportunidadService.DetectarAsync(10);

        var datos = oportunidades.Select(x => new CopilotoDatoDto
        {
            Titulo = $"{x.Cliente} · {x.Prioridad}",
            Subtitulo = $"{x.Producto} · Score {x.Score:N2}",
            Detalle = $"Atraso estimado: {x.DiasAtraso} días. Última compra: {x.UltimaCompra:dd/MM/yyyy}. Importe histórico del producto: {x.ImporteTotal:C2}.",
            Url = $"/Copiloto/Cliente/{x.ClienteId}"
        }).ToList();

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "OPORTUNIDADES",
            Respuesta = oportunidades.Count == 0
                ? "No encontré oportunidades de recompra vencidas con la regla actual."
                : $"Encontré {oportunidades.Count} oportunidades prioritarias. Las ordené por score y valor histórico.",
            Datos = datos,
            Sugerencias =
            [
                "Muéstrame las recomendaciones pendientes.",
                "¿Cuánto hemos vendido este mes?"
            ]
        };
    }

    private async Task<CopilotoChatDto> ResponderVentasMesAsync(string original)
    {
        var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var ventas = await db.Ventas.AsNoTracking()
            .Where(x => x.Estatus == "COMPLETADA" && x.FechaVenta >= inicio)
            .Select(x => new { x.Total, x.FechaVenta })
            .ToListAsync();

        var total = ventas.Sum(x => x.Total);
        var cantidad = ventas.Count;
        var ticket = cantidad == 0 ? 0m : total / cantidad;

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "VENTAS_MES",
            Respuesta = cantidad == 0
                ? "No hay ventas completadas registradas en el mes actual."
                : $"En el mes actual hay {cantidad:N0} ventas completadas por {total:C2}. El ticket promedio es {ticket:C2}.",
            Datos =
            [
                new CopilotoDatoDto
                {
                    Titulo = "Ventas del mes",
                    Subtitulo = $"{cantidad:N0} operaciones",
                    Detalle = $"Total: {total:C2} · Ticket promedio: {ticket:C2}",
                    Url = "/Ventas"
                }
            ],
            Sugerencias =
            [
                "¿Quiénes son los mejores vendedores?",
                "¿Cuáles son los productos más vendidos?"
            ]
        };
    }

    private async Task<CopilotoChatDto> ResponderTopVendedoresAsync(string original)
    {
        var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var top = await db.Ventas.AsNoTracking()
            .Where(x =>
                x.Estatus == "COMPLETADA" &&
                x.FechaVenta >= inicio &&
                x.VendedorId != null)
            .GroupBy(x => new { x.VendedorId, x.Vendedor!.Nombre })
            .Select(g => new
            {
                VendedorId = g.Key.VendedorId!.Value,
                Vendedor = g.Key.Nombre,
                Ventas = g.Count(),
                Total = g.Sum(x => x.Total)
            })
            .OrderByDescending(x => x.Total)
            .Take(10)
            .ToListAsync();

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "TOP_VENDEDORES",
            Respuesta = top.Count == 0
                ? "No encontré ventas completadas para vendedores durante el mes actual."
                : $"Estos son los {top.Count} vendedores con mayor venta acumulada del mes.",
            Datos = top.Select(x => new CopilotoDatoDto
            {
                Titulo = x.Vendedor,
                Subtitulo = $"{x.Ventas:N0} ventas",
                Detalle = $"Venta acumulada: {x.Total:C2}"
            }).ToList(),
            Sugerencias =
            [
                "¿Cuánto hemos vendido este mes?",
                "¿Qué clientes debo contactar hoy?"
            ]
        };
    }

    private async Task<CopilotoChatDto> ResponderTopProductosAsync(string original)
    {
        var inicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        var top = await db.DetallesVenta.AsNoTracking()
            .Where(x =>
                x.Venta.Estatus == "COMPLETADA" &&
                x.Venta.FechaVenta >= inicio)
            .GroupBy(x => new { x.ProductoId, x.Producto.Nombre, x.Producto.CodigoProducto })
            .Select(g => new
            {
                g.Key.ProductoId,
                g.Key.Nombre,
                g.Key.CodigoProducto,
                Cantidad = g.Sum(x => x.Cantidad),
                Importe = g.Sum(x => x.Importe)
            })
            .OrderByDescending(x => x.Importe)
            .Take(10)
            .ToListAsync();

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "TOP_PRODUCTOS",
            Respuesta = top.Count == 0
                ? "No encontré productos vendidos durante el mes actual."
                : $"Estos son los {top.Count} productos con mayor importe vendido del mes.",
            Datos = top.Select(x => new CopilotoDatoDto
            {
                Titulo = x.Nombre,
                Subtitulo = $"{x.CodigoProducto} · {x.Cantidad:N2} unidades",
                Detalle = $"Importe vendido: {x.Importe:C2}",
                Url = $"/Productos?q={Uri.EscapeDataString(x.CodigoProducto)}"
            }).ToList(),
            Sugerencias =
            [
                "¿Cuánto hemos vendido este mes?",
                "¿Quiénes son los mejores vendedores?"
            ]
        };
    }

    private async Task<CopilotoChatDto> ResponderRecomendacionesAsync(string original)
    {
        var recomendaciones = await db.Recomendaciones.AsNoTracking()
            .Include(x => x.Cliente)
            .Include(x => x.Producto)
            .Where(x => x.Estatus == "PENDIENTE")
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.FechaGeneracion)
            .Take(10)
            .ToListAsync();

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "RECOMENDACIONES",
            Respuesta = recomendaciones.Count == 0
                ? "No hay recomendaciones pendientes."
                : $"Hay recomendaciones pendientes. Te muestro las {recomendaciones.Count} de mayor score.",
            Datos = recomendaciones.Select(x => new CopilotoDatoDto
            {
                Titulo = $"{x.Cliente.Nombre} · {x.Prioridad}",
                Subtitulo = $"{x.Producto?.Nombre ?? "Sin producto"} · Score {x.Score:N2}",
                Detalle = x.Motivo,
                Url = $"/Copiloto/Cliente/{x.ClienteId}"
            }).ToList(),
            Sugerencias =
            [
                "¿Qué clientes debo contactar hoy?",
                "¿Cuánto hemos vendido este mes?"
            ]
        };
    }

    private async Task<CopilotoChatDto?> ResponderClienteAsync(string original, string textoNormalizado)
    {
        var clientes = await db.Clientes.AsNoTracking()
            .Where(x => x.Activo)
            .Select(x => new
            {
                x.ClienteId,
                x.CodigoCliente,
                x.Nombre,
                x.Rfc
            })
            .ToListAsync();

        var coincidencias = clientes
            .Select(x => new
            {
                Cliente = x,
                Puntos =
                    (textoNormalizado.Contains(Normalizar(x.CodigoCliente)) ? 100 : 0) +
                    (!string.IsNullOrWhiteSpace(x.Rfc) && textoNormalizado.Contains(Normalizar(x.Rfc)) ? 90 : 0) +
                    PuntuacionNombre(textoNormalizado, x.Nombre)
            })
            .Where(x => x.Puntos > 0)
            .OrderByDescending(x => x.Puntos)
            .ThenBy(x => x.Cliente.Nombre)
            .Take(5)
            .ToList();

        if (coincidencias.Count == 0)
            return null;

        var mejor = coincidencias[0];

        // Si hay varias coincidencias similares, no adivinamos:
        if (coincidencias.Count > 1 && coincidencias[1].Puntos >= mejor.Puntos - 5)
        {
            return new CopilotoChatDto
            {
                Pregunta = original,
                Intencion = "CLIENTE_AMBIGUO",
                Respuesta = "Encontré varios clientes que podrían coincidir. Selecciona el correcto.",
                Datos = coincidencias.Select(x => new CopilotoDatoDto
                {
                    Titulo = x.Cliente.Nombre,
                    Subtitulo = x.Cliente.CodigoCliente,
                    Detalle = x.Cliente.Rfc ?? "",
                    Url = $"/Copiloto/Cliente/{x.Cliente.ClienteId}"
                }).ToList()
            };
        }

        var c = mejor.Cliente;

        var resumen = await db.Ventas.AsNoTracking()
            .Where(x => x.ClienteId == c.ClienteId && x.Estatus == "COMPLETADA")
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Ventas = g.Count(),
                Total = g.Sum(x => x.Total),
                Ultima = g.Max(x => (DateTime?)x.FechaVenta)
            })
            .FirstOrDefaultAsync();

        var productos = await db.ClienteProductos.AsNoTracking()
            .Where(x => x.ClienteId == c.ClienteId)
            .OrderByDescending(x => x.NumCompras)
            .ThenByDescending(x => x.ImporteTotal)
            .Take(5)
            .Select(x => new
            {
                x.Producto.Nombre,
                x.NumCompras,
                x.ImporteTotal,
                x.UltimaCompra
            })
            .ToListAsync();

        var recs = await db.Recomendaciones.AsNoTracking()
            .Include(x => x.Producto)
            .Where(x => x.ClienteId == c.ClienteId &&
                (x.Estatus == "PENDIENTE" || x.Estatus == "CONTACTADO"))
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToListAsync();

        var total = resumen?.Total ?? 0m;
        var ventas = resumen?.Ventas ?? 0;
        var ticket = ventas == 0 ? 0m : total / ventas;

        var datos = new List<CopilotoDatoDto>
        {
            new()
            {
                Titulo = c.Nombre,
                Subtitulo = $"{c.CodigoCliente} · {ventas:N0} ventas",
                Detalle = $"Venta histórica: {total:C2} · Ticket promedio: {ticket:C2} · Última compra: {(resumen?.Ultima?.ToString("dd/MM/yyyy") ?? "Sin compras")}",
                Url = $"/Copiloto/Cliente/{c.ClienteId}"
            }
        };

        datos.AddRange(recs.Select(r => new CopilotoDatoDto
        {
            Titulo = $"Recomendación · {r.Prioridad}",
            Subtitulo = $"{r.Producto?.Nombre ?? "Sin producto"} · Score {r.Score:N2}",
            Detalle = r.Motivo,
            Url = $"/Copiloto/Cliente/{c.ClienteId}"
        }));

        if (recs.Count == 0)
        {
            datos.AddRange(productos.Select(p => new CopilotoDatoDto
            {
                Titulo = p.Nombre,
                Subtitulo = $"{p.NumCompras} compras históricas",
                Detalle = $"Importe acumulado: {p.ImporteTotal:C2} · Última compra: {(p.UltimaCompra?.ToString("dd/MM/yyyy") ?? "N/D")}",
                Url = $"/Copiloto/Cliente/{c.ClienteId}"
            }));
        }

        return new CopilotoChatDto
        {
            Pregunta = original,
            Intencion = "CLIENTE",
            Respuesta = recs.Count > 0
                ? $"Encontré a {c.Nombre}. Tiene recomendaciones activas; te muestro primero las de mayor score."
                : $"Encontré a {c.Nombre}. No tiene recomendaciones activas, así que te muestro sus productos de compra más frecuente como contexto comercial.",
            Datos = datos,
            Sugerencias =
            [
                "¿Qué clientes debo contactar hoy?",
                "Muéstrame las recomendaciones pendientes."
            ]
        };
    }

    private static int PuntuacionNombre(string texto, string nombre)
    {
        var palabras = Normalizar(nombre)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x.Length >= 3)
            .Distinct()
            .ToArray();

        if (palabras.Length == 0)
            return 0;

        var coincidencias = palabras.Count(texto.Contains);
        if (coincidencias == 0)
            return 0;

        return 10 + coincidencias * 15;
    }

    private static bool ContieneAlguno(string texto, params string[] opciones)
        => opciones.Any(x => texto.Contains(Normalizar(x)));

    private static string Normalizar(string texto)
    {
        var s = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var ch in s)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (categoria != UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }

        return sb.ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace("¿", "")
            .Replace("?", "")
            .Replace("¡", "")
            .Replace("!", "")
            .Trim();
    }
}
