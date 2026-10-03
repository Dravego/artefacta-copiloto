namespace Artefacta.Copiloto.Models;

public class Rol
{
    public long RolId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<UsuarioRol> Usuarios { get; set; } = new List<UsuarioRol>();
}
