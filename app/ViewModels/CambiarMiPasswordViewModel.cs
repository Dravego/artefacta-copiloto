using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class CambiarMiPasswordViewModel
{
    [Required, DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string PasswordActual { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(10)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(NuevaPassword))]
    [Display(Name = "Confirmar nueva contraseña")]
    public string ConfirmarPassword { get; set; } = "";
}
