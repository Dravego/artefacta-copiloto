namespace Artefacta.Copiloto.Models;

public class Promocion
{
    public long PromocionId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public string Tipo { get; set; } = null!;
    public decimal Valor { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal MinimoCompra { get; set; }
    public bool Activo { get; set; }

    public ICollection<PromocionProducto> Productos { get; set; } = new List<PromocionProducto>();
    public ICollection<PromocionCliente> Clientes { get; set; } = new List<PromocionCliente>();
}
