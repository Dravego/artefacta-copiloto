namespace Artefacta.Copiloto.Models;

public class ClienteProducto
{
    public long ClienteId { get; set; }
    public long ProductoId { get; set; }
    public DateTime? PrimeraCompra { get; set; }
    public DateTime? UltimaCompra { get; set; }
    public long NumCompras { get; set; }
    public decimal CantidadTotal { get; set; }
    public decimal ImporteTotal { get; set; }
    public decimal? PromedioDiasCompra { get; set; }
    public decimal? TicketPromedio { get; set; }
    public DateTime FechaActualizacion { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
