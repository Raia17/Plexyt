using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Plataforma.TagHelpers;

[HtmlTargetElement("readonly-div", TagStructure = TagStructure.NormalOrSelfClosing)]
public class ReadonlyDiv : TagHelper {
    [HtmlAttributeName("asp-for")] public ModelExpression For { get; set; }
    public override void Process(TagHelperContext context, TagHelperOutput output) {
        if (For == null) return;
        output.SuppressOutput();
        output.Content.Clear();
        var value = For?.Model;
        var name = For?.Name;
        var presentationName = context.AllAttributes["name"]?.Value ?? "";
        var presentationCopy = (context.AllAttributes["copy"]?.Value ?? "").ToString() == "true";
        var presentationValue = context.AllAttributes["value"]?.Value?.ToString() ?? string.Empty;
        var onlyValue = (context.AllAttributes["onlyvalue"]?.Value ?? "").ToString() == "true";

        var textValue = (string.IsNullOrEmpty(value?.ToString()) || string.IsNullOrWhiteSpace(value?.ToString()) ? " &nbsp; " : value).ToString();
        if (!string.IsNullOrEmpty(presentationValue))
            textValue = presentationValue;

        var label = $"<label for=\"{name}\">{presentationName}</label>";
        var input = $"<div class=\"form-control readonly\" readonly=\"readonly\" id=\"{name}\">{textValue?.Replace("\n", "<br/>")}</div>";
        if (presentationCopy && !string.IsNullOrEmpty(textValue)) {
            input = $"<div class=\"input-group\">{input}<span class=\"input-group-text copy cursor-pointer\"><i class=\"fa-solid fa-copy\"></i></span></div>";
        }

        output.Content.AppendHtml(onlyValue ? $"{input}" : $"{label}{input}");
    }

}