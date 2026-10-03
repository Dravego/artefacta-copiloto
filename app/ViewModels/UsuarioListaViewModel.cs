namespace Artefacta.Copiloto.ViewModels;

public class UsuarioListaViewModel
{
    public long UsuarioId { get; set; }
    public string Username { get; set; } = "";
    public string Nombre { get; set; } = "";
    public string? Email { get; set; }
    public string? Vendedor { get; set; }
    public bool Activo { get; set; }
    public List<string> Roles { get; set; } = [];
}
