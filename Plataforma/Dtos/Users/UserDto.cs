using Plataforma.Models.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Plataforma.Dtos.Users;

public class UserDto {
    public List<ClaimDto> UserClaims { get; set; }

    [Required(ErrorMessage = "Introduza o e-mail")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Introduza a password")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Introduza o nome")]
    public string Name { get; set; }

    public UserDto() {
        UserClaims = new List<ClaimDto>();
        foreach (var claimGroup in ClaimStore.ClaimGroups())
            foreach (var claim in claimGroup.Claims.Where(c => !string.IsNullOrEmpty(c.Type)).OrderBy(c => c.Name))
                UserClaims.Add(new ClaimDto {
                    Group = claimGroup.Name,
                    Type = claim.Type,
                    Name = claim.Name,
                    Enabled = false
                });
    }

    public void SetEnabledClaims(ICollection<IdentityUserClaim> roleClaims) {
        foreach (var claim in UserClaims.Where(claim => roleClaims.Any(rc => rc.ClaimType == claim.Type))) claim.Enabled = true;
    }
}