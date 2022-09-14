using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plataforma.Services.Contracts.Razor;

public interface IRazorViewToStringRenderer {
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, IDictionary<string, object> viewData = null);
    Task<string> RenderEmailAsync<TModel>(string viewName, TModel model, IDictionary<string, object> viewData = null);
}