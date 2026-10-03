namespace Artefacta.Copiloto.Models;

public class HistorialRecomendacion
{
    public long HistorialId { get; set; }
    public long RecomendacionId { get; set; }
    public DateTime Fecha { get; set; }
    public string Accion { get; set; } = null!;
    public long? UsuarioId { get; set; }
    public string? Resultado { get; set; }

    public Recomendacion Recomendacion { get; set; } = null!;
    public Usuario? Usuario { get; set; }
}
