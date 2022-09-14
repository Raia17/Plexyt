using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NonFactors.Mvc.Grid;
using Plataforma.Models.Contracts;
using System;

namespace Plataforma.Extensions;


public static class GridExtensions {
    public static IHtmlGrid<T> ShowTotal<T>(this IHtmlGrid<T> html, bool top = true, bool bottom = false) {
        html.Grid.Attributes["totalTop"] = top;
        html.Grid.Attributes["totalBottom"] = bottom;
        return html;
    }
}

public static class GridColumnsExtensions {
    public static void AddId<T>(this IGridColumnsOf<T> gridColumn) {
        var isMaster = gridColumn.Grid.ViewContext?.HttpContext?.User?.IsMaster() ?? false;
        if (isMaster)
            gridColumn.Add()
                .Titled("Id")
                .RenderedAs(m => m.GetType().GetProperty("Id")?.GetValue(m))
                .CssClasses = "w-1 p-2 d-none";
    }
    public static void AddActivating<T>(this IGridColumnsOf<T> gridColumn, Func<T, bool> when = null) where T : IActivable {
        gridColumn.Add()
            .RenderedAs(m => {
                var type = (m.GetType().Namespace ?? "").StartsWith("Castle.Proxies")
                    ? m.GetType().BaseType
                    : m.GetType();

                if (when != null && when(m) == false) return "";

                return string.Format(
                    "<a data-key=\"" + m.Id + "\" data-model=\"" + type?.FullName +
                    "\" class=\"activable {0}\" href=\"\"><i class=\"fa {1}\"></i></a>",
                    m.Active ? "activated" : "deactivated", m.Active ? "fa-toggle-on" : "fa-toggle-off");
            })
            .Encoded(false).CssClasses = "w-1 p-2";
    }

    public static void AddSorting<T>(this IGridColumnsOf<T> gridColumn) where T : ISortable {

        gridColumn.Add(m => m.Order)
            .RenderedAs(m => {
                var type = (m.GetType().Namespace ?? "").StartsWith("Castle.Proxies")
                    ? m.GetType().BaseType
                    : m.GetType();
                return "<button data-key=\"" + m.Id + "\" data-model=\"" + type?.FullName +
                       "\" class=\"btn-link p-0 m-0 bg-transparent border-0 sort-handle\" type=\"button\"><i class=\"fas fa-sort\"></i></button>";
            })
            .Titled("").Encoded(false).CssClasses = "p-2";

    }

    public static void AddActions<T>(this IGridColumnsOf<T> gridColumn, Action<ActionBuilder<T>> action, string title = "") {
        var viewContext = gridColumn.Grid.ViewContext;
        if (viewContext == null) return;
        var httpContext = viewContext.HttpContext;
        gridColumn.Add().RenderedAs(m => {
            var builder = new ActionBuilder<T>(m, httpContext, httpContext.RequestServices.GetService<LinkGenerator>(), viewContext.RouteData.Values["controller"]?.ToString(), gridColumn);
            action(builder);
            return builder.Html;
        }).Titled(title).Encoded(false).CssClasses = "p-2 w-1 white-space-nowrap align-middle";
    }
}

#region Actions
public class ActionBuilder<TEntity> {
    public string Html { get; set; }
    public LinkGenerator LinkGenerator { get; set; }
    public HttpContext HttpContext { get; set; }
    public TEntity Model { get; set; }
    public string Controller { get; set; }
    public IGridColumnsOf<TEntity> GridColumn { get; set; }

    public ActionBuilder(TEntity model, HttpContext context, LinkGenerator linkGenerator, string controller, IGridColumnsOf<TEntity> gridColumn) {
        Model = model;
        HttpContext = context;
        LinkGenerator = linkGenerator;
        Controller = controller;
        Html = "";
        GridColumn = gridColumn;
    }


    public ActionBuilder<TEntity> AddEdit(string action = null, string controller = null,
        object routeValues = null, string text = "Editar", string title = "Editar registo", bool showText = false, Func<TEntity, bool> when = null) {

        if (when != null && when(Model) == false) return this;

        var id = Model.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        action ??= "Edit";
        controller ??= Controller;
        routeValues ??= new { id };
        var link = LinkGenerator.GetPathByAction(HttpContext, action, controller, routeValues);
        var span = string.IsNullOrEmpty(text)
            ? ""
            : $@"<span class=""d-none d-md-inline-block ms-md-2 align-middle"">{text}</span>";
        var titleobj = string.IsNullOrEmpty(title) ? "" : $@"title=""{title}""";
        if (!showText)
            span = "";
        Html += $"<a href=\"{link}\" class=\"btn btn-primary btn-xs d-inline-block mx-1\" {titleobj}><i class=\"fas fa-pencil-alt\"></i>{span}</a>";
        return this;
    }

    public ActionBuilder<TEntity> AddShow(string action = null, string controller = null,
        object routeValues = null, string text = "Ver", string icon = null, string title = "Mostrar registo", bool showText = false, Func<TEntity, bool> when = null) {

        if (when != null && when(Model) == false) return this;

        var id = Model.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        action ??= "Show";
        controller ??= Controller;
        routeValues ??= new { id };

        var link = LinkGenerator.GetPathByAction(HttpContext, action, controller, routeValues);
        var span = string.IsNullOrEmpty(text)
            ? ""
            : $@"<span class=""d-none d-md-inline-block ms-md-2 align-middle"">{text}</span>";
        var titleobj = string.IsNullOrEmpty(title) ? "" : $@"title=""{title}""";
        icon = string.IsNullOrEmpty(icon) ? "fa-eye" : icon;
        if (!showText)
            span = "";
        Html += $"<a href=\"{link}\" class=\"btn btn-primary btn-xs d-inline-block mx-1\" {titleobj}><i class=\"fa {icon}\"></i>{span}</a>";
        return this;
    }

    public ActionBuilder<TEntity> AddDelete(string action = default, string controller = default,
        object routeValues = null, string text = "Eliminar", bool goBack = true, string title = "Eliminar registo", bool showText = false, Func<TEntity, bool> when = null) {

        if (when != null && when(Model) == false) return this;

        var id = Model.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        action ??= "Delete";
        controller ??= Controller;
        routeValues ??= new { id };
        var name = goBack ? "GoBack" : "";

        var link = LinkGenerator.GetPathByAction(HttpContext, action, controller, routeValues);
        var span = string.IsNullOrEmpty(text)
            ? ""
            : $@"<span class=""d-none d-md-inline-block ms-md-2 align-middle"">{text}</span>";
        var titleobj = string.IsNullOrEmpty(title) ? "" : $@"title=""{title}""";
        if (!showText)
            span = "";
        Html +=
            $@"<form method=""post"" class=""form-confirm d-inline-block mx-1"" data-message=""Tem a certeza que deseja eliminar este registo?"" action=""{link}"">
                        <button type=""submit"" name=""{name}"" class=""btn btn-danger btn-xs"" {titleobj}><i class=""fas fa-trash""></i>{span}</button>
                   </form>";

        return this;
    }

    public ActionBuilder<TEntity> AddClose(string model = null) {
        if (Model is not IClosable closable) return this;

        var id = closable.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        var closed = closable.Closed;
        if (model == null) {
            var type = (Model.GetType().Namespace ?? "").StartsWith("Castle.Proxies")
                ? Model.GetType().BaseType
                : Model.GetType();
            model = type?.FullName;
        }

        Html += $"<span data-key=\"{id}\"  data-model=\"{model}" +
            $"\" class=\"d-inline-block mx-1 closable btn btn-outline-primary btn-xs\" data-open=\"{(closed ? "1" : "0")}\">" +
            $"<i class=\"fas {(closed ? "fa-undo" : "fa-check")}\"></i></span>";

        return this;
    }

    public ActionBuilder<TEntity> AddCustom(string action = null, string controller = null,
        object routeValues = null, string text = "", string title = "", string btnClass = "btn-primary",
        string icon = "fas fa-pencil-alt", Func<TEntity, bool> when = null) {
        if (when != null && when(Model) == false) return this;

        var id = Model.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        action ??= "Edit";
        controller ??= Controller;
        routeValues ??= new { id };
        var link = LinkGenerator.GetPathByAction(HttpContext, action, controller, routeValues);
        var span = string.IsNullOrEmpty(text)
            ? ""
            : $@"<span class=""d-none d-md-inline-block ms-md-2 align-middle"">{text}</span>";

        if (string.IsNullOrEmpty(title))
            title = text;
        var titleObj = string.IsNullOrEmpty(title) ? "" : $@"title=""{title}""";
        if (title == null)
            titleObj = "";
        var iconObj = $"<i class=\"{icon}\"></i>";
        if (string.IsNullOrEmpty(icon))
            iconObj = "";
        Html += $"<a href=\"{link}\" class=\"btn {btnClass} btn-xs d-inline-block mx-1\" {titleObj}>{iconObj}{span}</a>";
        return this;
    }


    public ActionBuilder<TEntity> AddModal(string action = null, string controller = null,
        object routeValues = null, string text = "", string title = "", string btnClass = "btn-primary",
        string icon = "fas fa-pencil-alt", Func<TEntity, bool> when = null) {
        if (when != null && when(Model) == false) return this;

        var id = Model.GetType().GetProperty("Id")?.GetValue(Model) ?? "";
        action ??= "Edit";
        controller ??= Controller;
        routeValues ??= new { id };
        var link = LinkGenerator.GetPathByAction(HttpContext, action, controller, routeValues);
        var span = string.IsNullOrEmpty(text)
            ? ""
            : $@"<span class=""d-none d-md-inline-block ms-md-2 align-middle"">{text}</span>";

        if (string.IsNullOrEmpty(title))
            title = text;
        var titleObj = string.IsNullOrEmpty(title) ? "" : $@"title=""{title}""";
        if (title == null)
            titleObj = "";
        var iconObj = $"<i class=\"{icon}\"></i>";
        if (string.IsNullOrEmpty(icon))
            iconObj = "";
        Html += $"<button type=\"button\" data-url=\"{link}\" class=\"btn {btnClass} btn-xs d-inline-block mx-1 btn-modal\" {titleObj}>{iconObj}{span}</button>";
        return this;
    }


    public ActionBuilder<TEntity> AddCustomHtml(Func<TEntity, string> action, Func<TEntity, bool> when = null) {
        if (when != null && when(Model) == false) return this;

        Html += action(Model);
        return this;
    }

}
#endregion