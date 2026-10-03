namespace Artefacta.Copiloto.Models;

public class Producto
{
    public long ProductoId { get; set; }
    public string CodigoProducto { get; set; } = null!;
    public string? Sku { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public long? CategoriaId { get; set; }
    public decimal Precio { get; set; }
    public decimal? Costo { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }

    public Categoria? Categoria { get; set; }
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    public ICollection<PromocionProducto> Promociones { get; set; } = new List<PromocionProducto>();
    public ICollection<ClienteProducto> Clientes { get; set; } = new List<ClienteProducto>();
    public ICollection<Recomendacion> Recomendaciones { get; set; } = new List<Recomendacion>();
}
