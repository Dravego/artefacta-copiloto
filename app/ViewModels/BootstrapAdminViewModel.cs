using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class BootstrapAdminViewModel
{
    [Required, StringLength(100, MinimumLength = 4)]
    [Display(Name = "Usuario administrador")]
    public string Username { get; set; } = "admin";

    [Required, StringLength(150)]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = "Administrador ARTEFACTA";

    [EmailAddress, StringLength(150)]
    [Display(Name = "Correo")]
    public string? Email { get; set; }

    [Required, DataType(DataType.Password), MinLength(10)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password))]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = "";
}
