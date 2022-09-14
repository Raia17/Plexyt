using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class SortableController : ApiController {
    public SortableController(ApplicationDbContext dbContext) : base(dbContext) { }

    [HttpPost]
    public async Task<IActionResult> Index(string modelName, List<int> items) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");

        if (modelName == null) return BadRequest("The ModelName is a required parameter.");
        var modelType = Type.GetType(modelName);

        if (modelType == null) return BadRequest("Model type not found");

        var dbType = _dbContext.GetType();
        var dbSetProp = dbType.GetProperties()
            .FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Any(t => t == modelType));
        if (dbSetProp == null) return BadRequest("DbSet not found");


        if (!typeof(ISortable).IsAssignableFrom(modelType)) return BadRequest($"The model must implement the {nameof(ISortable)} interface");


        var dbSet = (dynamic)dbSetProp.GetValue(_dbContext);
        if (dbSet == null) return BadRequest("DbSet not found in DbContext");
        foreach (ISortable entity in dbSet)
            if (items.Contains(entity.Id)) entity.Order = items.Count - items.IndexOf(entity.Id);

        await _dbContext.SaveChangesAsync();
        return Ok(true);
    }
}