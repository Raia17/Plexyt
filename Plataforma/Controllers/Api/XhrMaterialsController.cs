using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using System.Linq;

namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class XhrMaterialsController : ApiController
{
    public XhrMaterialsController(ApplicationDbContext dbContext) : base(dbContext) {}

    [HttpGet]
    public IActionResult GetMaterials()
    {
        var materials = _dbContext.Materials.ToList();

        return Json(materials.Select(m => new { m.Id, Text = m.Name, Brand = m.Brand }));

    }
}