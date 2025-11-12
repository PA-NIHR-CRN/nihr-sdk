using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions;
using System;
using System.Text.Encodings.Web;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("section-with-heading")]
    public class SectionWithHeadingTagHelper : TagHelper
    {
        private const int DefaultHeadingLevel = 2;
        private const string DefaultSectionClass = "section";

        [HtmlAttributeName("heading")]
        public string Heading { get; set; } = string.Empty;

        [HtmlAttributeName("level")]
        public int Level { get; set; } = DefaultHeadingLevel;

        [HtmlAttributeName("class")]
        public string? Class { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Heading))
            {
                throw new InvalidOperationException(
                    $"The <section-with-heading> tag requires a non-empty '{nameof(Heading)}' attribute.");
            }

            var headingLevel = Math.Clamp(Level, 1, 6);

            output.TagName = "section";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.AppendClass(DefaultSectionClass);
            if (!string.IsNullOrWhiteSpace(Class))
                output.AppendClass(Class);

            var encodedHeading = HtmlEncoder.Default.Encode(Heading);
            var headingTag = $"<h{headingLevel}>{encodedHeading}</h{headingLevel}>";

            output.PreContent.AppendHtml(headingTag);
        }
    }
}
