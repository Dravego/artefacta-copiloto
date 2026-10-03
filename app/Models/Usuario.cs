namespace Artefacta.Copiloto.Models;

public class Usuario
{
    public long UsuarioId { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string? Email { get; set; }
    public long? VendedorId { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? UltimoAcceso { get; set; }

    public Vendedor? Vendedor { get; set; }
    public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
    public ICollection<HistorialRecomendacion> HistorialRecomendaciones { get; set; } = new List<HistorialRecomendacion>();
}
