using Microsoft.AspNetCore.Http;
using Plataforma.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Plataforma.Services.Components;

public class RedirectService {
    private readonly IHttpContextAccessor _httpContextAccessor;
    public RedirectService(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetGoBackUrl(string defaultRedirect = "") {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return "";
        var session = context.Session;

        var goBackUrl = "";
        if (!string.IsNullOrEmpty(context.Request.Query["b"])) {
            goBackUrl = WebUtility.UrlDecode(context.Request.Query["b"]);
            if (!goBackUrl.StartsWith("/") && !goBackUrl.StartsWith("http")) goBackUrl = $"/{goBackUrl}";
            return goBackUrl;
        }
        if (goBackUrl.Length == 0) {
            try {
                var currentController = context.Request.RouteValues["controller"]?.ToString() ?? "";
                var stateDict = session.GetObject<IDictionary<string, string>>("IndexState");
                if (stateDict != null) {
                    var target = session.GetObject<IDictionary<string, string>>("IndexState").FirstOrDefault(i => i.Key == currentController).Value;
                    goBackUrl = !string.IsNullOrEmpty(target) ? target : $"/{currentController}";
                } else {
                    goBackUrl = $"/{currentController}";
                }
            } catch {
                // Ignored
            }
        }

        if (goBackUrl.Length == 0)
            goBackUrl = !string.IsNullOrEmpty(defaultRedirect) ? defaultRedirect : "/";

        return goBackUrl.StartsWith(string.Concat(context.Request.PathBase, "/")) ? goBackUrl : string.Concat(context.Request.PathBase, goBackUrl);
    }


    public string SetGoBackUrl(string url = "") {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return "";
        var session = context.Session;
        if (string.IsNullOrEmpty(url)) return url;
        var currentController = context.Request.RouteValues["controller"]?.ToString() ?? "";
        if (currentController == "") return url;
        var state = session.GetObject<IDictionary<string, string>>("IndexState") ?? new Dictionary<string, string>();
        state[currentController] = url;
        session.SetObject("IndexState", state);
        return url;
    }

}