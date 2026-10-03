namespace Artefacta.Copiloto.DTOs;

public class CopilotoClienteDto
{
    public long ClienteId { get; set; }
    public string CodigoCliente { get; set; } = "";
    public string Nombre { get; set; } = "";
    public DateTime? UltimaCompra { get; set; }
    public decimal TotalHistorico { get; set; }
    public int NumeroVentas { get; set; }
    public decimal TicketPromedio { get; set; }
    public int DiasSinComprar { get; set; }
    public List<ProductoFrecuenteDto> ProductosFrecuentes { get; set; } = [];
    public List<RecomendacionDto> Recomendaciones { get; set; } = [];
    public List<VentaRecienteDto> VentasRecientes { get; set; } = [];
}

public class ProductoFrecuenteDto
{
    public long ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public long NumCompras { get; set; }
    public decimal ImporteTotal { get; set; }
    public DateTime? UltimaCompra { get; set; }
}

public class RecomendacionDto
{
    public long RecomendacionId { get; set; }
    public string Tipo { get; set; } = "";
    public string Prioridad { get; set; } = "";
    public decimal Score { get; set; }
    public string Motivo { get; set; } = "";
    public string? Producto { get; set; }
    public string Estatus { get; set; } = "";
}

public class VentaRecienteDto
{
    public long VentaId { get; set; }
    public string Folio { get; set; } = "";
    public DateTime FechaVenta { get; set; }
    public decimal Total { get; set; }
    public string Estatus { get; set; } = "";
}
