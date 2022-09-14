using Microsoft.AspNetCore.Http;
using Plataforma.Extensions;
using Plataforma.Services.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Infrastructure;

public sealed class RedirectHandler {
    private readonly RequestDelegate _next;
    private readonly RedirectService _redirectService;

    public RedirectHandler(RequestDelegate next, RedirectService redirectService) {
        _next = next;
        _redirectService = redirectService;
    }

    public async Task Invoke(HttpContext context) {
        var session = context.Session;

        var currController = context.Request.RouteValues["controller"]?.ToString() ?? "";
        if (currController != "") {
            var currAction = context.Request.RouteValues["action"]?.ToString();
            if (currAction == "Index") {
                var state = session.GetObject<IDictionary<string, string>>("IndexState") ?? new Dictionary<string, string>();
                state[currController] = context.Request.Path + context.Request.QueryString.ToString();
                session.SetObject("IndexState", state);
            }
        }

        await _next(context);

        try {
            if (context.Request.Method == HttpMethods.Post && context.Request.Form.ContainsKey("GoBack") && !context.Response.HasStarted) {
                var goBackUrl = _redirectService.GetGoBackUrl();

                if (goBackUrl != "") {
                    context.Response.Redirect(goBackUrl);
                } else {
                    var stateDict = session.GetObject<IDictionary<string, string>>("IndexState");
                    if (stateDict != null) {
                        var target = session.GetObject<IDictionary<string, string>>("IndexState").FirstOrDefault(i => i.Key == currController).Value;
                        if (!string.IsNullOrEmpty(target))
                            context.Response.Redirect(target);
                        else
                            context.Response.Redirect("/" + currController);
                    } else {
                        context.Response.Redirect("/" + currController);
                    }
                }
            }
        } catch {
            // ignored
        }
    }

}