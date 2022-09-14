using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Data;
using Plataforma.Extensions;
using Plataforma.Helpers;
using Plataforma.Models.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Abstract;

[Authorize]
[Route("{controller=Home}/{action=Index}")]
public abstract class BaseController : Controller {
    protected readonly ApplicationDbContext _dbContext;

    protected BaseController(ApplicationDbContext dbContext) {
        _dbContext = dbContext;
        CultureHelper.SetCulture();
    }

    protected async Task<User> GetAuthenticatedUserAsync() {
        var idClaim = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        return await _dbContext.Users.WithNoLockFirstOrDefaultAsync(u => u.Id == idClaim);
    }

    protected int GetAuthenticatedUserId() {
        try {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty);
        } catch {
            return 0;
        }
    }
}