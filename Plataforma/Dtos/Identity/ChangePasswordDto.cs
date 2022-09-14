using System.ComponentModel.DataAnnotations;

namespace Plataforma.Dtos.Identity.User;

public class ChangePasswordDto {

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Introduza a password atual")]
    public string CurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Introduza a nova password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Introduza a confirmação de password")]
    [Compare(nameof(NewPassword), ErrorMessage = "As passwords não são iguais")]
    public string ConfirmPassword { get; set; }
}