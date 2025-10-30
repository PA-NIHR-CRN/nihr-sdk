using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "class")]
    public class SkipLinkTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output.HasClass("skip-link"))
            {
                output.RemoveClass("skip-link");
                output.PrependClass("govuk-skip-link");
            }
        }
    }
}
