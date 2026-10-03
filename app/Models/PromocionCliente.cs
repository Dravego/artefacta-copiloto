namespace Artefacta.Copiloto.Models;

public class PromocionCliente
{
    public long PromocionId { get; set; }
    public long ClienteId { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime? FechaExpiracion { get; set; }
    public string Estatus { get; set; } = null!;

    public Promocion Promocion { get; set; } = null!;
    public Cliente Cliente { get; set; } = null!;
}
