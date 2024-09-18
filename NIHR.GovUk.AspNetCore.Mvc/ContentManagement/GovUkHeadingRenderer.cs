using Contentful.Core.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace NIHR.GovUk.AspNetCore.Mvc.ContentManagement
{
    public class GovUkHeadingRenderer : GovUkBaseRenderer
    {
        public GovUkHeadingRenderer(ContentRendererCollection rendererCollection) : base(rendererCollection)
        {
        }

        public override async Task<string> RenderAsync(IContent content)
        {
            var level = int.Parse(content.GetType().Name.Replace("Heading", string.Empty));
            var size = level switch
            {
                1 => "l",
                2 => "m",
                _ => "s",
            };

            var tagBuilder = new TagBuilder($"h{level}");
            tagBuilder.AddCssClass($"govuk-heading-{size}");

            IHeading heading = content as IHeading;

            tagBuilder.InnerHtml.SetHtmlContent(await GetInnerHtmlAsync(heading.Content));

            return RenderTagBuilder(tagBuilder);
        }

        public override bool SupportsContent(IContent content)
        {
            return content is IHeading;
        }
    }
}