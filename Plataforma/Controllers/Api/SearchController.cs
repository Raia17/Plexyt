using Microsoft.AspNetCore.Authorization;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;

namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class SearchController : ApiController {

    public SearchController(ApplicationDbContext dbContext) : base(dbContext) {
    }

}