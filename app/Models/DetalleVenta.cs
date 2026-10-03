namespace Artefacta.Copiloto.Models;

public class DetalleVenta
{
    public long DetalleVentaId { get; set; }
    public long VentaId { get; set; }
    public long ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Importe { get; set; }

    public Venta Venta { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
