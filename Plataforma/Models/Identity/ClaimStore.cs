using Plataforma.Controllers.Account;
using Plataforma.Controllers.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BstHelpers;

namespace Plataforma.Models.Identity;

public static class ClaimStore {

    public static readonly Claim MasterClaim = new("Master", "Master");

    private static readonly List<ApplicationClaimGroup> Claims = new() {
        new ApplicationClaimGroup("Utilizadores", new[] {new ApplicationClaim("Users", "", true, typeof(UsersController).GetControllerName(), "fas fa-user") }, true ),
        new ApplicationClaimGroup("Definições", new[] {new ApplicationClaim("Settings", "", true, typeof(SettingsController).GetControllerName(), "fas fa-gear") }, true ),
        new ApplicationClaimGroup("Alterar Password", new[] {new ApplicationClaim("", "", true, typeof(ChangePasswordController).GetControllerName(), "fas fa-key") }, true ),
        new ApplicationClaimGroup("", new[] {new ApplicationClaim("", "", true, "" )}, true, whiteSpace: true ),
        new ApplicationClaimGroup("Folhas de Obra", new[] {new ApplicationClaim("WorkSheets", "", true, typeof(WorkSheetsController).GetControllerName(), "fas fa-file-lines") }, true ),
        new ApplicationClaimGroup("Veículos", new[] {new ApplicationClaim("Vehicles", "", true, typeof(VehiclesController).GetControllerName(), "fas fa-car") }, true ),
        new ApplicationClaimGroup("Funcionários", new[] {new ApplicationClaim("Employees", "", true, typeof(EmployeesController).GetControllerName(), "fas fa-helmet-safety") }, true ),
        new ApplicationClaimGroup("Clientes", new[] {new ApplicationClaim("Clients", "", true, typeof(ClientsController).GetControllerName(), "fas fa-users") }, true ),
        new ApplicationClaimGroup("Materiais", new[] {new ApplicationClaim("Materials", "", true, typeof(MaterialsController).GetControllerName(), "fas fa-pen-ruler") }, true ),



        //new ApplicationClaimGroup("Administração",
        //    new[] {
        //        new ApplicationClaim("Users", "Utilizadores", true,typeof(UsersController).GetControllerName(), "fas fa-user"),
        //        new ApplicationClaim("Settings", "Definições", true,typeof(SettingsController).GetControllerName()),
        //    }
        //),
        //new ApplicationClaimGroup("Trabalho",
        //    new[] {
        //        new ApplicationClaim("WorkSheets", "Folhas de Obra", true,typeof(WorkSheetsController).GetControllerName()),
        //        new ApplicationClaim("Vehicles", "Veículos", true,typeof(VehiclesController).GetControllerName()),
        //        new ApplicationClaim("Employees", "Funcionários", true,typeof(EmployeesController).GetControllerName()),
        //        new ApplicationClaim("Clients", "Clientes", true,typeof(ClientsController).GetControllerName()),
        //        new ApplicationClaim("Materials", "Materiais", true,typeof(MaterialsController).GetControllerName()),
        //    }
        //),
        //new ApplicationClaimGroup("Alterar Password", new[] {new ApplicationClaim("", "", true, typeof(ChangePasswordController).GetControllerName()) }, true),
    };


    public static List<ApplicationClaimGroup> ClaimGroups(bool menus = false) {
        var claims = Claims;
        if (menus) claims = claims.Where(c => c.Claims.Any(cl => cl.Menu)).ToList();
        return claims;
    }

    public static List<ApplicationClaim> ClaimList(bool menus = false) {
        var claimsList = new List<ApplicationClaim>();
        foreach (var claimGroup in Claims) {
            var claims = claimGroup.Claims.ToList();
            if (menus) claims = claims.Where(c => c.Menu).ToList();
            claimsList.AddRange(claims.Select(claim => new ApplicationClaim(claim.Type, claim.Name, claim.Menu, claim.ControllerName)));
        }
        return claimsList;
    }
}

public class ApplicationClaimGroup {
    public string Name { get; set; }
    public bool OnlyMenu { get; set; }
    public bool WhiteSpace { get; set; }
    private readonly Func<IServiceProvider, string, Task<string>> _nameFuncAsync;
    private readonly Func<IServiceProvider, string, string> _nameFunc;
    public ApplicationClaim[] Claims { get; set; }

    public ApplicationClaimGroup(string name, ApplicationClaim[] claims,
        bool onlyMenu = false,
        Func<IServiceProvider, string, Task<string>> nameFuncAsync = null,
        Func<IServiceProvider, string, string> nameFunc = null,
        bool whiteSpace = false) {
        Name = name;
        OnlyMenu = onlyMenu;
        Claims = claims;
        _nameFunc = nameFunc;
        _nameFuncAsync = nameFuncAsync;
        WhiteSpace = whiteSpace;
    }

    public async Task<string> GetComputedName(IServiceProvider serviceProvider) {
        if (_nameFuncAsync != null) return await _nameFuncAsync(serviceProvider, Name) ?? Name;
        if (_nameFunc != null) return _nameFunc(serviceProvider, Name) ?? Name;

        return Name;
    }
}

public class ApplicationClaim : Claim {
    public string Name { get; }
    public string MenuName { get; }
    public bool Menu { get; }
    public string Icon { get; }
    public string CustomType { get; }
    private readonly Func<IServiceProvider, string, Task<string>> _nameFuncAsync;
    private readonly Func<IServiceProvider, string, string> _nameFunc;
    public string ControllerName { get; }

    public ApplicationClaim(string type, string name, bool menu, string controllerName, string icon = "", string customType = "", string menuName = "",
        Func<IServiceProvider, string, Task<string>> nameFuncAsync = null,
        Func<IServiceProvider, string, string> nameFunc = null) : base(type, type) {
        Menu = menu;
        Name = name;
        MenuName = menuName;
        Icon = icon;
        _nameFunc = nameFunc;
        _nameFuncAsync = nameFuncAsync;
        ControllerName = controllerName;
        CustomType = customType;
    }

    public async Task<string> GetComputedName(IServiceProvider serviceProvider) {
        if (_nameFuncAsync != null) return await _nameFuncAsync(serviceProvider, Name) ?? Name;
        if (_nameFunc != null) return _nameFunc(serviceProvider, Name) ?? Name;
        return Name;
    }
}