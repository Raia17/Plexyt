using System.ComponentModel.DataAnnotations;

namespace Plataforma.Dtos.Account;

public class PersonalSettingsDto {

    [Required(ErrorMessage = "Escolha a posição das mensagens")]
    public string MessagesPosition { get; set; }

}