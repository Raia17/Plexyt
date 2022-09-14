using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Models.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class ActivableController : ApiController {
    public ActivableController(ApplicationDbContext dbContext) : base(dbContext) { }

    [HttpPost]
    public async Task<IActionResult> Index(string modelName, int key) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");
        var modelType = Type.GetType(modelName);

        if (modelType == null) return BadRequest("Model type not found");

        var dbType = _dbContext.GetType();
        var dbSetProp = dbType.GetProperties()
            .FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Any(t => t == modelType));
        if (dbSetProp == null) return BadRequest("DbSet not found");

        if (!typeof(IActivable).IsAssignableFrom(modelType)) return BadRequest($"The model must implement the {nameof(IActivable)} interface");


        var dbSet = (dynamic)dbSetProp.GetValue(_dbContext);
        if (dbSet == null) return BadRequest("DbSet not found in DbContext");
        var model = (IActivable)dbSet.Find(key);
        if (model == null) return NotFound($"{modelType.Name} with primary key {key} was not found");

        model.Active = !model.Active;
        await _dbContext.SaveChangesAsync();
        return Ok(model.Active);
    }
}