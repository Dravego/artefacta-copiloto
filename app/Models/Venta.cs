namespace Artefacta.Copiloto.Models;

public class Venta
{
    public long VentaId { get; set; }
    public string Folio { get; set; } = null!;
    public long ClienteId { get; set; }
    public long? VendedorId { get; set; }
    public DateTime FechaVenta { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Total { get; set; }
    public string Estatus { get; set; } = null!;

    public Cliente Cliente { get; set; } = null!;
    public Vendedor? Vendedor { get; set; }
    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
