using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Data;

namespace Plataforma.Controllers.Abstract;

[AllowAnonymous]
[Route("/api/{controller=Home}/{action=Index}")]
public abstract class ApiController : BaseController {
    protected ApiController(ApplicationDbContext dbContext) : base(dbContext) { }
}