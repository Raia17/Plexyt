using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plataforma.Controllers.Abstract;
using Plataforma.Data;
using Plataforma.Dtos.Helpers;

namespace Plataforma.Controllers.Structure;


public class HomeController : BaseController {
    public HomeController(ApplicationDbContext dbContext) : base(dbContext) {
    }
    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Dashboard";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }

    public IActionResult Index() {
        return View("../Structure/Home/Index");
    }


}