using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Helpers;
using Plataforma.Dtos.Work;
using Plataforma.Models.Identity;
using Plataforma.Models.Work;
using Plataforma.Services.Components.Repositories;
using Plataforma.Services.Contracts.FlashMessage;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Admin;

[Authorize("Materials")]
public class MaterialsController : BaseController
{
    private readonly IFlashMessage _flashMessage;
    private readonly IMapper _mapper;

    public MaterialsController(ApplicationDbContext dbContext, IFlashMessage flashMessage, IMapper mapper) : base(dbContext)
    {
        _flashMessage = flashMessage;
        _mapper = mapper;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Materiais";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }



    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View("../Work/Materials/Index", await _dbContext.Materials.ToListAsync());
    }


    [HttpGet]
    public IActionResult Create()
    {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo material", null)
        );

        return View("../Work/Materials/Form", new MaterialDto());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaterialDto dto)
    {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo material", null)
        );

        if (!ModelState.IsValid) return View("../Work/Materials/Form", dto);

        var model = _mapper.Map(dto, new Material());
        var result = await _dbContext.Materials.AddAsync(model);

        if (result != null)
        {
            _flashMessage.Success("Registo criado com sucesso");
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["Error"] = "Ocurreu um erro";

        return View("../Work/Materials/Form", dto);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _dbContext.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar material - " + model.Name, null)
        );

        var dto = _mapper.Map(model, new MaterialDto());

        return View("../Work/Materials/Form", dto);
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MaterialDto dto)
    {
        var model = await _dbContext.Materials.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar material - " + model.Name, null)
        );

        if (!ModelState.IsValid) return View("../Work/Materials/Form", dto);

        var result = _mapper.Map(dto, model);

        if (result != null)
        {
            await _dbContext.SaveChangesAsync();
            _flashMessage.Success("Registo atualizado com sucesso");
            return RedirectToAction(nameof(Index));
        }

        ViewData["Error"] = "Ocurreu um erro";
        return RedirectToAction(nameof(Edit), id);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await _dbContext.Materials.FirstOrDefaultAsync(m => m.Id == id);

        if (model == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Materials.Remove(model);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}