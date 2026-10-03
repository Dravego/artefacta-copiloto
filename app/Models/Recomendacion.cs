namespace Artefacta.Copiloto.Models;

public class Recomendacion
{
    public long RecomendacionId { get; set; }
    public long ClienteId { get; set; }
    public long? ProductoId { get; set; }
    public string Tipo { get; set; } = null!;
    public decimal Score { get; set; }
    public string Prioridad { get; set; } = null!;
    public string Motivo { get; set; } = null!;
    public DateTime FechaGeneracion { get; set; }
    public DateTime? FechaExpiracion { get; set; }
    public string Estatus { get; set; } = null!;

    public Cliente Cliente { get; set; } = null!;
    public Producto? Producto { get; set; }
    public ICollection<HistorialRecomendacion> Historial { get; set; } = new List<HistorialRecomendacion>();
}
