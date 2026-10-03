namespace Artefacta.Copiloto.Models;

public class PromocionProducto
{
    public long PromocionId { get; set; }
    public long ProductoId { get; set; }

    public Promocion Promocion { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
