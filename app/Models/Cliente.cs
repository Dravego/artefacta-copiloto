namespace Artefacta.Copiloto.Models;

public class Cliente
{
    public long ClienteId { get; set; }
    public string CodigoCliente { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? RazonSocial { get; set; }
    public string? Rfc { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaAlta { get; set; }
    public bool Activo { get; set; }

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
    public ICollection<PromocionCliente> Promociones { get; set; } = new List<PromocionCliente>();
    public ICollection<ClienteProducto> Productos { get; set; } = new List<ClienteProducto>();
    public ICollection<Recomendacion> Recomendaciones { get; set; } = new List<Recomendacion>();
}
