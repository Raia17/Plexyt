using System.ComponentModel.DataAnnotations;

namespace Plataforma.Dtos.Users;

public class ClaimDto {
    [Required] public string Type { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Group { get; set; }
    [Required] public bool Enabled { get; set; }
}