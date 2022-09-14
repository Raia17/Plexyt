using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Account;
using Plataforma.Dtos.Helpers;
using Plataforma.Services.Contracts.FlashMessage;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Account;

public class PersonalSettingsController : BaseController {
    private readonly IFlashMessage _flashMessage;
    private readonly IMapper _mapper;



    public PersonalSettingsController(ApplicationDbContext dbContext, IFlashMessage flashMessage, IMapper mapper) : base(dbContext) {
        _flashMessage = flashMessage;
        _mapper = mapper;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        var user = GetAuthenticatedUserAsync().Result;
        ViewData["Title"] = "Definições";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null), new BreadCrumbDto(user.Name, null));
    }

    [HttpGet]
    public async Task<IActionResult> Index() {
        var user = await GetAuthenticatedUserAsync();
        var dto = new PersonalSettingsDto();
        _mapper.Map(user, dto);
        return View("../Account/PersonalSettings", dto);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Index(PersonalSettingsDto dto) {
        if (!ModelState.IsValid) return View("../Account/PersonalSettings", dto);
        var user = await GetAuthenticatedUserAsync();
        _mapper.Map(dto, user);
        await _dbContext.SaveChangesAsync();
        _flashMessage.Success("Os seus dados foram atualizados com sucesso");
        return View("../Account/PersonalSettings", dto);
    }
}