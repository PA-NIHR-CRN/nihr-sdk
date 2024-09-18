using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using NIHR.Infrastructure.Interfaces;
using System.Text.Encodings.Web;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

[HtmlTargetElement(Attributes = "with-content")]
public class WithContentTagHelper(IContentProvider contentProvider, ILogger<WithContentTagHelper> logger) : TagHelper
{
    public string? WithContent { get; set; }

    public ContentMode ContentMode { get; set; } = ContentMode.Replace;

    public bool UseFallbackContentOnError { get; set; } = true;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrWhiteSpace(WithContent))
        {
            return;
        }

        var content = ContentMode switch
        {
            ContentMode.Replace => output.Content,
            ContentMode.PreContent => output.PreContent,
            ContentMode.PostContent => output.PostContent,
            _ => output.Content
        };

        try
        {
            var source = await contentProvider.GetContentAsync<RmsPage>(WithContent);

            output.AddClass("has-cms-content", HtmlEncoder.Default);

            var span = new TagBuilder("span");
            span.AddCssClass("is-cms-content");
            span.InnerHtml.SetHtmlContent(source.Content);

            content.SetHtmlContent(span);
        }
        catch (Exception ex)
        {
            if (UseFallbackContentOnError)
            {
                output.AddClass("has-fallback-content", HtmlEncoder.Default);
                logger.LogError(ex.Message, ex);
            }
            else
            {
                throw;
            }
        }
    }
}

public enum ContentMode
{
    PreContent,
    Replace,
    PostContent
}
