using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Plataforma.Dtos.Media;
using Plataforma.Extensions;
using Plataforma.Models.Media;

namespace Plataforma.TagHelpers;

[HtmlTargetElement("file-input", TagStructure = TagStructure.NormalOrSelfClosing)]
public class FileInputTagHelper : TagHelper {
    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; }

    private File _file;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileInputTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor) {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        if (For == null) return;
        var uploadedFile = (UploadedFile)For?.Model;
        _file = uploadedFile?.FileModel;
        output.SuppressOutput();
        output.Content.Clear();
        output.Content.AppendHtml(CreateInputTags().GetHtml());
        output.Content.AppendHtml(CreateFileTags()?.GetHtml());
    }

    #region "Input Upload"

    private TagBuilder CreateInputTags() {
        var tb = new TagBuilder("div");

        tb.AddCssClass("custom-file");
        tb.AddCssClass(string.IsNullOrEmpty(_file?.Binary) ? "" : "d-none");
        tb.InnerHtml.AppendHtml(CreateInputTag().GetHtml());
        tb.InnerHtml.AppendHtml(CreateEmptyLabelTag().GetHtml());
        return tb;
    }
    private TagBuilder CreateInputTag() {
        var tb = new TagBuilder("input") { TagRenderMode = TagRenderMode.SelfClosing };
        var accepts = "";//FileHelper.AcceptedExtensions.Aggregate("", (current, extension) => current + ((current != "" ? "," : "") + "." + extension));
        tb.Attributes.Add("name", For.Name + ".File");
        tb.Attributes.Add("type", "file");
        tb.Attributes.Add("accept", accepts);
        tb.AddCssClass("custom-file-input");
        return tb;
    }
    private TagBuilder CreateEmptyLabelTag() {
        var tb = new TagBuilder("label");
        tb.AddCssClass("custom-file-label");
        return tb;
    }
    #endregion

    #region "File"
    private TagBuilder CreateFileTags() {
        if (string.IsNullOrEmpty(_file?.Binary)) {
            return null;
        }
        var tb = new TagBuilder("div");
        tb.InnerHtml.AppendHtml(CreateDeleteInputTag().GetHtml());
        tb.InnerHtml.AppendHtml(CreateFileLinkTag().GetHtml());
        tb.InnerHtml.AppendHtml(CreateDeleteButtonTag().GetHtml());
        return tb;
    }
    private TagBuilder CreateFileLinkTag() {
        var tb = new TagBuilder("a") { TagRenderMode = TagRenderMode.Normal };
        tb.AddCssClass("btn btn-primary mr-1 open-file");
        tb.Attributes.Add("href", _linkGenerator.GetUriByAction(_httpContextAccessor.HttpContext, "FileRead", "Files", new { _file.Id }));
        tb.Attributes.Add("target", "_blank");
        tb.InnerHtml.AppendHtml("<i class=\"fa-solid fa-eye mr-1\"></i>" + _file?.Filename ?? "(Ficheiro sem nome)");
        return tb;
    }

    private TagBuilder CreateDeleteButtonTag() {
        var tb = new TagBuilder("button");
        tb.AddCssClass("btn btn-danger file-input-delete pb-2");
        tb.Attributes.Add("type", "button");
        tb.InnerHtml.Append("Apagar ficheiro");
        return tb;
    }
    private TagBuilder CreateDeleteInputTag() {
        var tb = new TagBuilder("input") { TagRenderMode = TagRenderMode.SelfClosing };
        tb.Attributes.Add("name", For.Name + ".Delete");
        tb.Attributes.Add("type", "hidden");
        tb.Attributes.Add("value", "false");
        return tb;
    }
    #endregion

}