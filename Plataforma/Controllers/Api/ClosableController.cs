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
public class ClosableController : ApiController {
    public ClosableController(ApplicationDbContext dbContext) : base(dbContext) { }

    [HttpPost]
    public async Task<IActionResult> Index(string modelName, int key, int open = 0) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");

        if (modelName == null) return BadRequest("The ModelName is a required parameter.");
        var modelType = Type.GetType(modelName);

        if (modelType == null) return BadRequest("Model type not found");

        var dbType = _dbContext.GetType();
        var dbSetProp = dbType.GetProperties()
            .FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Any(t => t == modelType));
        if (dbSetProp == null) return BadRequest("DbSet not found");


        if (!typeof(IClosable).IsAssignableFrom(modelType)) return BadRequest($"The model must implement the {nameof(IClosable)} interface");

        var dbSet = (dynamic)dbSetProp.GetValue(_dbContext);
        if (dbSet == null) return BadRequest("DbSet not found in DbContext");
        var model = (IClosable)dbSet.Find(key);
        if (model == null) return NotFound($"{modelType.Name} with primary key {key} was not found");

        if (open == 1) {
            model.Closed = false;
            model.ClosedUser = "";
            model.ClosedUserId = null;
            model.ClosedTime = null;
        } else {
            var user = await GetAuthenticatedUserAsync();
            model.Closed = true;
            model.ClosedUser = user.UserName;
            model.ClosedUserId = user.Id;
            model.ClosedTime = DateTime.Now;
        }


        await _dbContext.SaveChangesAsync();
        return Ok(model.Closed);
    }
}