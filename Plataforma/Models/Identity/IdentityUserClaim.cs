using Microsoft.AspNetCore.Identity;
// ReSharper disable VirtualMemberCallInConstructor

namespace Plataforma.Models.Identity;

public class IdentityUserClaim : IdentityUserClaim<int> {

    public string ClaimName { get; set; }
    public virtual User User { get; set; }

    public IdentityUserClaim() { }

    public IdentityUserClaim(User user, string type, string name) {
        User = user;
        ClaimType = type;
        ClaimValue = type;
        ClaimName = name;
    }
}