using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Helpers;
using Plataforma.Dtos.Users;
using Plataforma.Models.Identity;
using Plataforma.Services.Components.Repositories;
using Plataforma.Services.Contracts.FlashMessage;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Admin;

[Authorize("Users")]
public class UsersController : BaseController {
    private readonly IFlashMessage _flashMessage;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly UsersRepository _usersRepository;

    public UsersController(ApplicationDbContext dbContext, IFlashMessage flashMessage, IMapper mapper, UsersRepository usersRepository, UserManager<User> userManager) : base(dbContext) {
        _flashMessage = flashMessage;
        _mapper = mapper;
        _usersRepository = usersRepository;
        _userManager = userManager;
    }
    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Utilizadores";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }

    [HttpGet]
    public IActionResult Index() {
        return View("../Admin/Users/Index", _usersRepository.GetQueryableUsers(onlyActive: false));
    }


    [HttpGet]
    public IActionResult Create() {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo utilizador", null)
        );
        return View("../Admin/Users/Form", new UserDto());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserDto dto) {
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto("Novo utilizador", null)
        );

        if (!ModelState.IsValid) return View("../Admin/Users/Form", dto);

        var user = _mapper.Map(dto, new User());

        user.UserName = user.Email;
        user.Active = true;
        user.EmailConfirmed = true;
        user.Claims.AddRange(dto.UserClaims.Where(uc => uc.Enabled).Select(uc => new IdentityUserClaim(user, uc.Type, uc.Name)));

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded) {
            _flashMessage.Success("Registo criado com sucesso");
            return RedirectToAction(nameof(Index));
        }
        ViewData["Error"] = result.Errors.FirstOrDefault()?.Description;

        return View("../Admin/Users/Form", dto);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id) {
        var model = await _usersRepository.GetUserByIdAsync(id, onlyActive: false);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto(model.Name, null)
        );

        var dto = _mapper.Map(model, new UserDto());
        dto.SetEnabledClaims(model.Claims);
        ViewData["Model"] = model;

        return View("../Admin/Users/Form", dto);
    }
    [HttpPost("{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserDto dto) {
        var model = await _usersRepository.GetUserByIdAsync(id, onlyActive: false);
        if (model == null) return NotFound();

        ViewData["BreadCrumbs"] = new BreadCrumbsDto(
            new BreadCrumbDto(ViewData["Title"]?.ToString(), Url.Action(nameof(Index))),
            new BreadCrumbDto(model.Name, null)
        );

        ViewData["Model"] = model;

        ModelState.Remove("email");
        ModelState.Remove("password");
        ModelState.Remove("name");
        if (!ModelState.IsValid) return View("../Admin/Users/Form", dto);
        dto.Email = model.Email;
        _mapper.Map(dto, model);

        model.Claims.Clear();
        model.Claims.AddRange(dto.UserClaims.Where(uc => uc.Enabled).Select(uc => new IdentityUserClaim(model, uc.Type, uc.Name)));

        await _dbContext.SaveChangesAsync();
        _flashMessage.Success("Registo atualizado com sucesso");
        return RedirectToAction(nameof(Edit), new { id });
    }

}