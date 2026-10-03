using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class UsuarioEditarViewModel
{
    public long UsuarioId { get; set; }

    [Required, StringLength(100, MinimumLength = 4)]
    [Display(Name = "Usuario")]
    public string Username { get; set; } = "";

    [Required, StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = "";

    [EmailAddress, StringLength(150)]
    [Display(Name = "Correo")]
    public string? Email { get; set; }

    [Display(Name = "Vendedor asociado")]
    public long? VendedorId { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; }

    public List<long> RolesSeleccionados { get; set; } = [];
    public List<RolOpcionViewModel> Roles { get; set; } = [];
    public List<VendedorOpcionViewModel> Vendedores { get; set; } = [];
}
