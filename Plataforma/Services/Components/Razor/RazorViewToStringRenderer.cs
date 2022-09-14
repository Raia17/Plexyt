using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Plataforma.Services.Contracts.Razor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Plataforma.Services.Components.Razor;
public class RazorViewToStringRenderer : IRazorViewToStringRenderer {
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewToStringRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider) {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderEmailAsync<TModel>(string viewName, TModel model, IDictionary<string, object> viewData = null) {
        return await RenderViewToStringAsync("/Views/Emails/" + viewName.TrimStart('/').Replace(".cshtml", "") + ".cshtml", model, viewData);
    }

    public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, IDictionary<string, object> viewData = null) {
        var actionContext = GetActionContext();
        var view = FindView(actionContext, viewName);

        var viewDataDictionary = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary()) {
            Model = model
        };
        if (viewData != null) {
            foreach (var pair in viewData) {
                viewDataDictionary[pair.Key] = pair.Value;
            }
        }

        await using var output = new StringWriter();
        var viewContext = new ViewContext(actionContext, view, viewDataDictionary,
            new TempDataDictionary(
                actionContext.HttpContext,
                _tempDataProvider),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext);

        return output.ToString();
    }

    private IView FindView(ActionContext actionContext, string viewName) {
        var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success) {
            return getViewResult.View;
        }

        var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success) {
            return findViewResult.View;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(
                searchedLocations));

        throw new InvalidOperationException(errorMessage);
    }

    private ActionContext GetActionContext() {
        var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
        return new ActionContext(httpContextAccessor.HttpContext, new RouteData(), new ActionDescriptor());
    }

}
