using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("section-with-heading")]
    public class SectionWithHeadingTagHelper : TagHelper
    {
        public string heading { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            output.AppendClass("section");

            output.PreContent.AppendHtml($"""<h2>{heading}</h2>""");
        }
    }
}
