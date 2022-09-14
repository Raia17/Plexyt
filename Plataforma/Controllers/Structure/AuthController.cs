using BstHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Auth;
using Plataforma.Extensions;
using Plataforma.Models.Identity;
using Plataforma.Services.Components;
using Plataforma.Services.Components.Repositories;
using Plataforma.Services.Contracts.FlashMessage;
using Plataforma.Services.Contracts.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Structure;

[AllowAnonymous]
public class AuthController : BaseController {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly EmailService _emailService;
    private readonly UsersRepository _usersRepository;
    private readonly IRazorViewToStringRenderer _razor;
    private readonly IFlashMessage _flashMessage;

    public AuthController(ApplicationDbContext dbContext, SignInManager<User> signInManager, IFlashMessage flashMessage, UserManager<User> userManager, EmailService emailService, IRazorViewToStringRenderer razor, UsersRepository usersRepository) : base(dbContext) {
        _signInManager = signInManager;
        _flashMessage = flashMessage;
        _userManager = userManager;
        _emailService = emailService;
        _razor = razor;
        _usersRepository = usersRepository;
    }
    
    [HttpGet]
    public IActionResult Index() {
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> Login(string redirect = "") {
        await _signInManager.SignOutAsync();
        ViewData["redirect"] = redirect;
        return View("../Structure/Auth/Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto loginDto, string redirect = "") {
        if (User.IsLogged()) {
            if (!string.IsNullOrEmpty(redirect)) return LocalRedirect(redirect);
            return RedirectToAction(nameof(HomeController.Index), typeof(HomeController).GetControllerName());
        }
        if (ModelState.IsValid) {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user != null) {
                if ((user.Active) || user.Master) {
                    var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, user.LockoutEnabled);
                    if (result.Succeeded) {
                        user.LastLogin = DateTime.Now;
                        await _dbContext.SaveChangesAsync();
                        if (!string.IsNullOrEmpty(redirect)) return LocalRedirect(redirect);
                        return RedirectToAction(nameof(HomeController.Index), typeof(HomeController).GetControllerName());
                    }
                }
            }
            _flashMessage.Danger("Utilizador ou Password errados");
        }

        return View("../Structure/Auth/Login", loginDto);
    }



    [HttpGet]
    public IActionResult ForgotPassword() {

        return View("../Structure/Auth/ForgotPassword");
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto) {
        if (!ModelState.IsValid) {
            return View("../Structure/Auth/ForgotPassword");
        }
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user != null) {
            if ((user.Active) || user.Master) {
                var token = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
                var title = "Recuperação de password";

                user.RecoverPasswordTime = DateTime.Now;
                user.RecoverPasswordToken = token;
                await _dbContext.SaveChangesAsync();

                var viewData = new Dictionary<string, object> {
                    {"Pretext", title},
                    {"Title", title},
                };
                var emailContent = await _razor.RenderEmailAsync("Auth/PasswordToken", token, viewData);
                _emailService.SendEmail(user.Email, title, emailContent);
            }
        }
        return RedirectToAction(nameof(RecoverPassword));
    }


    [HttpGet]
    public IActionResult RecoverPassword() {

        return View("../Structure/Auth/RecoverPassword");
    }

    [HttpPost]
    public async Task<IActionResult> RecoverPassword(RecoverPasswordDto dto) {
        if (!ModelState.IsValid) {
            return View("../Structure/Auth/RecoverPassword");
        }
        var users = _usersRepository.GetQueryableUsers(onlyActive: true);

        if (dto.Password != dto.PasswordConfirm) {
            _flashMessage.Danger("As passwords introduzidas não são iguais");
            return View("../Structure/Auth/RecoverPassword");
        }

        var user = await users.WithNoLockFirstOrDefaultAsync(u => u.RecoverPasswordToken == dto.Code && (u.RecoverPasswordTime != null && u.RecoverPasswordTime >= DateTime.Now.AddMinutes(-10)));
        if (user == null) {
            _flashMessage.Danger("O código introduzido não é válido");
            return View("../Structure/Auth/RecoverPassword");
        }
        if (!user.Active && !user.Master) {
            _flashMessage.Danger("O código introduzido não é válido");
            return View("../Structure/Auth/RecoverPassword");
        }
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, dto.Password);
        if (result.Succeeded) {
            _flashMessage.Success("A sua password foi reposta com sucesso!");
            return RedirectToAction(nameof(Login));
        } else {
            _flashMessage.Danger("Não foi possível alterar a sua password");
            ViewData["Error"] = result.Errors.FirstOrDefault()?.Description;
            return View("../Structure/Auth/RecoverPassword");
        }
    }


    [HttpGet]
    public async Task<IActionResult> InvalidLogin() {
        try {
            await _signInManager.SignOutAsync();
        } catch {
            // Ignored
        }
        ViewData["Title"] = "Login inválido";
        ViewData["Text"] = "Não foi possível efetuar o login";
        return View("../Structure/Auth/InvalidLogin");
    }

    public async Task<IActionResult> Logout() {
        try {
            await _signInManager.SignOutAsync();
        } catch {
            // Ignored
        }
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied() {
        ViewData["Title"] = "Acesso proibído";
        ViewData["Text"] = "Não tem permissões para o pedido efetuado";
        return View("../Structure/Auth/AccessDenied");
    }

    [HttpGet]
    public IActionResult LockedOut() {
        ViewData["Title"] = "Conta bloqueada";
        ViewData["Text"] = "A sua conta está bloqueada";
        return View("../Structure/Auth/LockedOut");
    }

}