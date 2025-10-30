using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("accordion")]
    public class AccordionTagHelper : TagHelper
    {
        public string? Id { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "accordion");

            if (!string.IsNullOrWhiteSpace(Id))
            {
                output.Attributes.SetAttribute("id", Id);
            }
        }
    }
}
