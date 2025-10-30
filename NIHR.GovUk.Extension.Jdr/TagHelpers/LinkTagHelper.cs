using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "class")]
    public class LinkTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!output.HasClass("button"))
                return;

            output.RemoveClass("button");
            output.PrependClass("govuk-button");
        }
    }
}
