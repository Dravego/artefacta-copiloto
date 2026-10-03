using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Usuario")]
    public string Username { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}
