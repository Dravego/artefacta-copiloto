using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class UsuarioEditarRolesViewModel
{
    public long UsuarioId { get; set; }
    public string Username { get; set; } = "";
    public string Nombre { get; set; } = "";

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public List<RolOpcionViewModel> Roles { get; set; } = [];
}

public class RolOpcionViewModel
{
    public long RolId { get; set; }
    public string Nombre { get; set; } = "";
    public bool Seleccionado { get; set; }
}
