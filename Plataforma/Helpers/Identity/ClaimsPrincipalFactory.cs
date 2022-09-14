using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Plataforma.Models.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Plataforma.Helpers.Identity;

public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<User> {
    public ClaimsPrincipalFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user) {
        var identity = await base.GenerateClaimsAsync(user);
        if (user.Master) {
            foreach (var claim in ClaimStore.ClaimList().Where(c=>c.Type != "")) identity.AddClaim(new Claim(claim.Type, claim.Name));
            identity.AddClaim(ClaimStore.MasterClaim);
        } else
            foreach (var claim in user.Claims.Where(c=>c.ClaimType != "")) identity.AddClaim(new Claim(claim.ClaimType, claim.ClaimName));
        return identity;
    }
}