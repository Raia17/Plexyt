using System.ComponentModel.DataAnnotations;

namespace Plataforma.Dtos.Helpers;

public class NameDto {
    public string Name { get; set; }
}
public class EmailDto {
    [EmailAddress(ErrorMessage = "Endereço de e-mail inválido")]
    public string Email { get; set; }
}