using BstHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Plataforma.Controllers.Abstract;
using Plataforma.Controllers.Admin;
using Plataforma.Data;
using Plataforma.Dtos.Helpers;
using Plataforma.Models.Work;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Plataforma.Controllers.Structure;


public class HomeController : BaseController {
    public HomeController(ApplicationDbContext dbContext) : base(dbContext) {
    }
    public override void OnActionExecuting(ActionExecutingContext context) {
        base.OnActionExecuting(context);
        ViewData["Title"] = "Dashboard";
        ViewData["BreadCrumbs"] = new BreadCrumbsDto(new BreadCrumbDto(ViewData["Title"].ToString(), null));
    }

    public async Task<IActionResult> Index(int mm = -1, int? yy = null) {
        int yyyy;
        if(!yy.HasValue) { yyyy = DateTime.Now.Year; } else { yyyy = (int)yy; }
        
        if (mm == -1) { mm = DateTime.Now.Month; }

        var date = new DateTime(yyyy, mm, 1);
        ViewBag.Date = date;
        ViewBag.Worksheets = await _dbContext.WorkSheets.Where(m => m.Date.Month == mm && m.Date.Year == yyyy).ToListAsync();

        

        return View("../Structure/Home/Index", LoadCards());
    }


    public List<DashboardCardDto> LoadCards()
    {
        var list = new List<DashboardCardDto>();
        list.Add(new DashboardCardDto() { Name = "Folhas de Obra", Icon = "fas fa-file-text", Count = _dbContext.WorkSheets.Count(), Controller = typeof(WorkSheetsController).GetControllerName(), Action = "Create" });
        list.Add(new DashboardCardDto() { Name = "Veículos", Icon = "fas fa-car", Count = _dbContext.Vehicles.Count(), Controller = typeof(VehiclesController).GetControllerName(), Action = "Create" });
        list.Add(new DashboardCardDto() { Name = "Materiais", Icon = "fas fa-pencil-ruler", Count = _dbContext.Materials.Count(), Controller = typeof(MaterialsController).GetControllerName(), Action = "Create" });
        list.Add(new DashboardCardDto() { Name = "Clientes", Icon = "fas fa-users", Count = _dbContext.Clients.Count(), Controller = typeof(ClientsController).GetControllerName(), Action = "Create" });
        list.Add(new DashboardCardDto() { Name = "Funcionários", Icon = "fas fa-helmet-safety", Count = _dbContext.Employees.Count(), Controller = typeof(EmployeesController).GetControllerName(), Action = "Create" });
        return list;
    }

}

