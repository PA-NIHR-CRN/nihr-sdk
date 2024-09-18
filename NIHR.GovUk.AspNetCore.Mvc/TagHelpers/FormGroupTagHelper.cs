using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    public class FormGroupTagHelper : TagHelper
    {
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public ModelExpression? For { get; set; }

        public bool IncludeChildErrors { get; set; }

        public string? Label { get; set; } = null;
        public string LabelLevel { get; set; } = "h3";

        private readonly IHtmlGenerator _generator;

        public FormGroupTagHelper(IHtmlGenerator generator)
        {
            _generator = generator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For == null)
            {
                return;
            }

            if (ViewContext == null)
            {
                return;
            }

            output.TagName = "div";
            output.AddClass("govuk-form-group", HtmlEncoder.Default);

            var label = _generator.GenerateLabel(ViewContext, For.ModelExplorer, For.Name, Label, new { @class = "govuk-label govuk-label--l" });


            var modelName = label.Attributes["for"]?.Replace('_', '.') ?? string.Empty;

            List<string> modelNames = [modelName];

            if (IncludeChildErrors)
            {
                var childPrefix = modelName + ".";

                modelNames.AddRange(ViewContext.ViewData.ModelState.Where(x => x.Key.StartsWith(childPrefix)).Select(x => x.Key));
            }

            var modelState = ViewContext.ViewData.ModelState.Where(x => modelNames.Contains(x.Key));

            if (modelState?.Where(x => x.Value?.Errors.Count > 0).Any() ?? false)
            {
                output.AddClass("govuk-form-group--error", HtmlEncoder.Default);
            }

            var labelWrapper = new TagBuilder(LabelLevel);
            labelWrapper.AddCssClass("govuk-label-wrapper");
            labelWrapper.InnerHtml.SetHtmlContent(label);

            output.PreContent.AppendHtml(labelWrapper);

            if (!string.IsNullOrEmpty(For.Metadata.Description))
            {
                var hintBuilder = new TagBuilder("div");
                hintBuilder.AddCssClass("govuk-hint");
                hintBuilder.GenerateId($"{For.Name}-hint", "-");

                hintBuilder.InnerHtml.Append(For.Metadata.Description);

                output.PreContent.AppendHtml(hintBuilder);
            }

            if (!ViewContext.ViewData.ModelState.IsValid)
            {
                foreach (var error in ViewContext.Errors().Where(x => modelNames.Contains(x.Key)))
                {
                    var validationMessage = _generator.GenerateValidationMessage(ViewContext, For.ModelExplorer, error.Key, null, null, null);

                    output.PreContent.AppendHtml($"""
                    <span class="govuk-error-message">
                        <span class="govuk-visually-hidden">Error:</span>
                """);

                    output.PreContent.AppendHtml(validationMessage);
                    output.PreContent.AppendHtml("</span>");
                }
            }
        }
    }
}
