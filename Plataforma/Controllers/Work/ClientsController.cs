using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

[Authorize("Clients")]
public class ClientsController : BaseController {
    private readonly IFlashMessage _flashMessage;
    private readonly IMapper _mapper;

    public ClientsController(ApplicationDbContext dbContext, IFlashMessage flashMessage, IMapper mapper) : base(dbContext) {
        _flashMessage = flashMessage;
        _mapper = mapper;
    }
    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Clientes";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }



    [HttpGet]
    public async Task<IActionResult> Index() {
        return View("../Work/Clients/Index", await _dbContext.Clients.ToListAsync());
    }


    [HttpGet]
    public IActionResult Create() {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo cliente", null)
        );
        return View("../Work/Clients/Form", new ClientDto());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientDto dto) {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo cliente", null)
        );

        if (!ModelState.IsValid) return View("../Work/Clients/Form", dto);

        var model = _mapper.Map(dto, new Client());
        var result = await _dbContext.Clients.AddAsync(model);

        if (result != null) {
            _flashMessage.Success("Registo criado com sucesso");
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["Error"] = "Ocurreu um erro";

        return View("../Work/Clients/Form", dto);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id) {
        var model = await _dbContext.Clients.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar cliente - " + model.Name, null)
        );

        var dto = _mapper.Map(model, new ClientDto());

        return View("../Work/Clients/Form", dto);
    }

    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClientDto dto) {
        var model = await _dbContext.Clients.FirstOrDefaultAsync(m => m.Id == id);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Editar cliente - " + model.Name, null)
        );

        if (!ModelState.IsValid) return View("../Work/Clients/Form", dto);

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
        var model = await _dbContext.Clients.FirstOrDefaultAsync(m => m.Id == id);

        if (model == null)
        {
            return RedirectToAction(nameof(Index));
        }

        _dbContext.Clients.Remove(model);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}