using ManagementPlatform.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Identity.User;
using Plataforma.Models.Identity;
using Plataforma.Services.Contracts.FlashMessage;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Account;

[Authorize]
public class ChangePasswordController : BaseController {
    private readonly IFlashMessage _flashMessage;
    private readonly UserManager<User> _userManager;

    public ChangePasswordController(ApplicationDbContext dbContext, IFlashMessage flashMessage, UserManager<User> userManager) : base(dbContext) {
        _flashMessage = flashMessage;
        _userManager = userManager;
    }

    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Menu"] = "ChangePassword";
        ViewData["Title"] = "Alterar Password";
    }

    [HttpGet]
    public async Task<IActionResult> Index() {
        var user = await GetAuthenticatedUserAsync();

        ViewData["PasswordValidation"] = await PasswordValidation.GetList(_userManager, user);

        return View("../Account/ChangePassword", new ChangePasswordDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ChangePasswordDto dto) {
        var user = await GetAuthenticatedUserAsync();
        ViewData["PasswordValidation"] = await PasswordValidation.GetList(_userManager, user);
        if (!ModelState.IsValid) return View("../Account/ChangePassword");
        if (!await _userManager.CheckPasswordAsync(user, dto.CurrentPassword)) {
            _flashMessage.Danger("A password atual está incorreta");
            return View("../Account/ChangePassword", dto);
        }
        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (result.Succeeded) {
            _flashMessage.Success("A sua password foi alterada com sucesso");
            return RedirectToAction(nameof(Index));
        } else {
            _flashMessage.Danger("Não foi possível alterar a sua password");
            ViewData["Error"] = result.Errors.FirstOrDefault()?.Description;
        }
        return View("../Account/ChangePassword");
    }
}