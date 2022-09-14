using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Helpers;
using Plataforma.Models.Structure;
using Plataforma.Services.Components.Repositories;
using Plataforma.Services.Contracts.FlashMessage;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Admin;

[Authorize("Settings")]
public class SettingsController : BaseController {
    private readonly IFlashMessage _flashMessage;
    private readonly SettingsRepository _settingsRepository;

    public SettingsController(ApplicationDbContext dbContext, IFlashMessage flashMessage, SettingsRepository settingsRepository) : base(dbContext) {
        _flashMessage = flashMessage;
        _settingsRepository = settingsRepository;
    }
    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Definições";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }

    [HttpGet]
    public async Task<IActionResult> Index() {
        var settings = await _settingsRepository.GetSettingsAsync();
        return View("../Admin/Settings/Index", settings);
    }


    [HttpPost]
    public async Task<IActionResult> Index(Settings settings) {
        var currentSettings = await _settingsRepository.GetSettingsAsync();
        await _settingsRepository.SaveSettingsAsync(settings);
        _flashMessage.Success("Definições atualizadas com sucesso!");
        return Redirect(nameof(Index));
    }
}