using System.ComponentModel.DataAnnotations;

namespace Plataforma.Dtos.Auth;

public class LoginDto {
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string Email { get; set; }

    [Required(ErrorMessage = "A password é obrigatória")]
    public string Password { get; set; }
}
public class ForgotPasswordDto {
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    public string Email { get; set; }

}
public class RecoverPasswordDto {
    [Required(ErrorMessage = "O código é obrigatório")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Introduza a nova password")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Introduza a confirmação de password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "As passwords não são iguais")]
    public string PasswordConfirm { get; set; }

}