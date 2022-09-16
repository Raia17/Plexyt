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

[Authorize("Vehicles")]
public class VehiclesController : BaseController
{
    private readonly IFlashMessage _flashMessage;
    private readonly IMapper _mapper;

    public VehiclesController(ApplicationDbContext dbContext, IFlashMessage flashMessage, IMapper mapper) : base(dbContext)
    {
        _flashMessage = flashMessage;
        _mapper = mapper;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Veículos";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }



    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View("../Work/Vehicles/Index", await _dbContext.Vehicles.ToListAsync());
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo veículo", null)
        );

        ViewBag.Clients = new SelectList(await _dbContext.Clients.ToListAsync(), "Id", "Name");

        return View("../Work/Vehicles/Form", new VehicleDto());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleDto dto)
    {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo veículo", null)
        );

        if (!ModelState.IsValid) return View("../Work/Vehicles/Form", dto);

        var model = _mapper.Map(dto, new Vehicle());
        var result = await _dbContext.Vehicles.AddAsync(model);

        if (result != null)
        {
            _flashMessage.Success("Registo criado com sucesso");
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["Error"] = "Ocurreu um erro";

        return View("../Work/Vehicles/Form", dto);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _dbContext.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar veículo - " + model.LicensePlate, null)
        );

        var dto = _mapper.Map(model, new VehicleDto());

        ViewBag.Clients = new SelectList(await _dbContext.Clients.ToListAsync(), "Id", "Name");

        return View("../Work/Vehicles/Form", dto);
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VehicleDto dto)
    {
        var model = await _dbContext.Vehicles.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar veículo - " + model.LicensePlate, null)
        );

        if (!ModelState.IsValid) return View("../Work/Vehicles/Form", dto);

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
        var model = await _dbContext.Vehicles.FirstOrDefaultAsync(m => m.Id == id);

        if (model == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Vehicles.Remove(model);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}