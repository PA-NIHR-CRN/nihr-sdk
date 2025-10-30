using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("details", ParentTag = "accordion")]
    public class AccordionItemTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "details";
            output.Attributes.SetAttribute("class", "accordion-item");

            var childContent = output.GetChildContentAsync().Result.GetContent();

            var summaryEndIndex = childContent.IndexOf("</summary>", StringComparison.OrdinalIgnoreCase);
            if (summaryEndIndex >= 0)
            {
                summaryEndIndex += "</summary>".Length;
                var summaryPart = childContent.Substring(0, summaryEndIndex);
                var contentPart = childContent.Substring(summaryEndIndex);

                var wrappedContent = $@"{summaryPart}<div class=""accordion-content"">{contentPart}</div>";
                output.Content.SetHtmlContent(wrappedContent);
            }
            else
            {
                output.Content.SetHtmlContent($@"<div class=""accordion-content"">{childContent}</div>");
            }
        }
    }
}
