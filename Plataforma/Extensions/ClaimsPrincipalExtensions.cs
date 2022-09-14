using Plataforma.Models.Identity;
using System;
using System.Security.Claims;

namespace Plataforma.Extensions;

public static class ClaimsPrincipalExtensions {
    public static int GetUserId(this ClaimsPrincipal principal) {
        if (principal == null) throw new ArgumentNullException(nameof(principal));
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(claim?.Value ?? "0");
    }

    public static bool IsLogged(this ClaimsPrincipal principal) {
        return principal.GetUserId() != 0;
    }
    public static bool IsMaster(this ClaimsPrincipal principal) {
        return principal.HasClaim(c => c.Type == ClaimStore.MasterClaim.Type);
    }

}