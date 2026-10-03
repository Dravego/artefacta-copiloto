namespace Artefacta.Copiloto.Models;

public class UsuarioRol
{
    public long UsuarioId { get; set; }
    public long RolId { get; set; }

    public Usuario Usuario { get; set; } = null!;
    public Rol Rol { get; set; } = null!;
}
