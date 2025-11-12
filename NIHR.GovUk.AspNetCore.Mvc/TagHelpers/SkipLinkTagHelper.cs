using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.Extension.Jdr.TagHelpers
{
    [HtmlTargetElement("skip-link")]
    public class SkipLinkTagHelper : TagHelper
    {
        private const string DefaultHref = "#main-content";
        private const string DefaultText = "Skip to main content";
        private const string GovUkClass = "govuk-skip-link";
        private const string GovUkDataModule = "govuk-skip-link";

        public string Href { get; set; } = DefaultHref;

        public string Text { get; set; } = DefaultText;

        public string? Class { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            var existingClasses = new HashSet<string>(
                (output.Attributes["class"]?.Value?.ToString() ?? "")
                    .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                StringComparer.OrdinalIgnoreCase
            );

            if (!existingClasses.Contains(GovUkClass))
                existingClasses.Add(GovUkClass);

            if (!string.IsNullOrWhiteSpace(Class))
            {
                foreach (var extra in Class.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    existingClasses.Add(extra);
            }

            var classString = string.Join(" ", existingClasses);
            output.Attributes.SetAttribute("class", classString);

            if (!output.Attributes.ContainsName("href"))
            {
                output.Attributes.SetAttribute("href", string.IsNullOrWhiteSpace(Href) ? DefaultHref : Href);
            }

            if (!output.Attributes.ContainsName("data-module"))
            {
                output.Attributes.SetAttribute("data-module", GovUkDataModule);
            }

            if (!output.Content.IsModified)
            {
                output.Content.SetContent(string.IsNullOrWhiteSpace(Text) ? DefaultText : Text);
            }
        }
    }
}
