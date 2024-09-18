using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NIHR.GovUk.AspNetCore.Mvc.ContentManagement
{
    public class GovUkParagraphRenderer : GovUkBaseRenderer
    {

        public GovUkParagraphRenderer(ContentRendererCollection rendererCollection) : base(rendererCollection)
        {
        }

        public override async Task<string> RenderAsync(IContent content)
        {
            Paragraph paragraph = content as Paragraph;
            var tagBuilder = new TagBuilder("p");
            tagBuilder.AddCssClass("govuk-body");

            tagBuilder.InnerHtml.SetHtmlContent(await GetInnerHtmlAsync(paragraph.Content));

            return RenderTagBuilder(tagBuilder);
        }

        public override bool SupportsContent(IContent content)
        {
            return content is Paragraph;
        }
    }
}