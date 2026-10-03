namespace Artefacta.Copiloto.DTOs;

public class DashboardDto
{
    public int ClientesActivos { get; set; }
    public int ProductosActivos { get; set; }
    public int VentasMes { get; set; }
    public decimal TotalVentasMes { get; set; }
    public decimal TicketPromedioMes { get; set; }
    public int RecomendacionesPendientes { get; set; }
    public List<TopVendedorDto> TopVendedores { get; set; } = [];
    public List<TopProductoDto> TopProductos { get; set; } = [];
}

public class TopVendedorDto
{
    public long VendedorId { get; set; }
    public string Vendedor { get; set; } = "";
    public int Ventas { get; set; }
    public decimal Total { get; set; }
}

public class TopProductoDto
{
    public long ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public decimal Cantidad { get; set; }
    public decimal Importe { get; set; }
}
