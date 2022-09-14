using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.Services.Contracts.FlashMessage;
using System;

namespace Plataforma.Controllers.Structure;

[Route("error")]
[AllowAnonymous]
public class ErrorController : Controller {

    private readonly IFlashMessage _flashMessage;

    public ErrorController(IFlashMessage flashMessage) {
        _flashMessage = flashMessage;
    }

    [Route("{errorCode}")]
    public IActionResult Index(string errorCode) {
        switch (errorCode) {
            case "404":
                Response.StatusCode = 404;
                ViewData["Title"] = "404";
                ViewData["Text"] = "A página solicitada não foi encontrada";
                break;
            case "400":
                Response.StatusCode = 400;
                ViewData["Title"] = "400";
                ViewData["Text"] = "A informação submetida não é válida";
                break;
            case "403":
            case "405":
                ViewData["Title"] = "403";
                ViewData["Text"] = "Não tem permissões para o pedido efetuado";
                break;
            case "500":
                if (Request.Query["ms"] == "1") {
                    try {
                        _flashMessage.Clear();
                    } catch {
                        // Ignored
                    }
                    return Redirect("/");
                }
                ViewData["Title"] = "500";
                ViewData["Text"] = "Ocorreu um erro no pedido efetuado";
                break;
            default:
                throw new Exception(string.Concat("Status code não processado: ", errorCode));
        }
        return View("../Structure/Error/Index");
    }

}