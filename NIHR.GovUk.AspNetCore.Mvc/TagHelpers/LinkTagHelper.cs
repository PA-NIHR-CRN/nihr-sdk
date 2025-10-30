using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    [HtmlTargetElement("a")]
    public class LinkTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.PrependClass("govuk-link");
        }
    }
}
