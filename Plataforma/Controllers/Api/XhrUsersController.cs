using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Models.Identity;
using Plataforma.Services.Components.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Api;

[AllowAnonymous]
public class XhrUsersController : ApiController {
    private readonly UsersRepository _usersRepository;
    private readonly UserManager<User> _userManager;

    public XhrUsersController(ApplicationDbContext dbContext, UsersRepository usersRepository, UserManager<User> userManager) : base(dbContext) {
        _usersRepository = usersRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index() {
        return View("../Admin/Users/Index", _usersRepository.GetQueryableUsers(onlyActive: false));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(int id, string password) {
        if (!User.Identity?.IsAuthenticated ?? false)
            return BadRequest("Not authenticated");
        var currentUser = await GetAuthenticatedUserAsync();

        if (string.IsNullOrEmpty(password)) return BadRequest("Password inválida");

        var user = await _usersRepository.GetUserByIdAsync(id);
        if (user == null) return BadRequest("Utilizador inválido");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, password);

        if (result.Succeeded) return Ok("Password alterada com sucesso");
        return BadRequest(result.Errors.FirstOrDefault()?.Description);
    }
}