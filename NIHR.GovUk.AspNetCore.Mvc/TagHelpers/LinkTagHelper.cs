using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    [HtmlTargetElement("a")]
    public class LinkTagHelper : TagHelper
    {

        [HtmlAttributeName("no-govuk-link")]
        public bool NoGovUkLink { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            return; // TODO: Review all links without govuk-link class. Class may change styles unexpectedly. Consider adding no-govuk-link.
            if (NoGovUkLink)
                return;

            output.PrependClass("govuk-link");
        }
    }
}
