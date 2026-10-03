using System.ComponentModel.DataAnnotations;

namespace Artefacta.Copiloto.ViewModels;

public class CambioPasswordInicialViewModel
{
    [Required]
    public string Username { get; set; } = "";

    [Required, DataType(DataType.Password), MinLength(10)]
    [Display(Name = "Nueva contraseña")]
    public string NuevaPassword { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(NuevaPassword))]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = "";
}
