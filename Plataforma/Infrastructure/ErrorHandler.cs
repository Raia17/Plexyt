using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Data;
using Plataforma.Services.Components;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Plataforma.Infrastructure;

public class ErrorHandler {
    private readonly RequestDelegate _next;
    private readonly ApplicationDbContext _dbContext;

    public ErrorHandler(RequestDelegate next, ApplicationDbContext dbContext) {
        _next = next;
        _dbContext = dbContext;
    }

    public async Task Invoke(HttpContext context, EmailService emailService, ConfigurationsService configurationsService) {
        var redirect = false;
        var errorStatus = 404;
        try {
            await _next(context);
            errorStatus = context.Response.StatusCode;
            // Handle error status codes
            if (errorStatus is >= 400 and < 600 && !context.Request.Path.ToUriComponent().Contains("/404"))
                redirect = true;
        } catch (Exception ex) {
            //Handle uncaught global exceptions (treat as 500 error)
            Handle500(context, emailService, configurationsService, ex);
        } finally {
            if (redirect && !context.Request.Path.ToUriComponent().ToLower().Contains("api", StringComparison.InvariantCultureIgnoreCase)) {
                var linkGenerator = context.RequestServices.GetService<LinkGenerator>();
                if (linkGenerator != null) context.Response.Redirect(linkGenerator.GetPathByAction(context, "Index", "Error", new { errorCode = errorStatus }) ?? "/");
            }
        }
    }

    //500
    private void Handle500(HttpContext context, EmailService emailService, ConfigurationsService configurationsService, Exception ex) {
        var lastErrorTime = DateTime.MinValue;
        var errorFile = Path.Combine(Environment.CurrentDirectory, "error.log");
        try {
            if (File.Exists(errorFile)) {
                if (File.ReadAllText(errorFile) == ex.Message) {
                    lastErrorTime = File.GetLastWriteTime(errorFile);
                }
            }
        } catch {
            // Ignored
        }

        if (DateTime.Now > lastErrorTime.AddHours(2)) {
            var userText = "";
            try {
                var value = context?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (value != null) {
                    var userId = int.Parse(value);
                    var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                        userText = string.Concat("<br/><br/><b>Utilizador</b>:", user.Email, " (<b>", user.Id, "</b>)");
                }
            } catch {
                //Ignored
            }

            emailService.SendEmail(
                new[] { configurationsService.ErrorEmail },
                string.Concat("Erro na plataforma ", configurationsService.Title),
                string.Concat(
                    "<b>Data: </b>",
                    $"{DateTime.Now:dd-MM-yyyy} às {DateTime.Now:HH:mm:ss}",
                    userText,
                    "<br/><br/><b>Mensagem</b>:<br/>",
                    ex.Message,
                    "<br/><br/><b>Erro</b>:<br/>",
                    ex.ToString()
                )
            );
        }

        if (context == null) return;
        if (context.Request.Path.ToUriComponent().ToLower().Contains("api", StringComparison.InvariantCultureIgnoreCase)) return;
        var linkGenerator = context.RequestServices.GetService<LinkGenerator>();
        if (linkGenerator != null)
            context.Response.Redirect(linkGenerator.GetPathByAction(context, "Index", "Error", new { errorCode = 500 }) ?? "/");

    }
}