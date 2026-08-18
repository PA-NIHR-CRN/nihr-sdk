using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

[HtmlTargetElement("govuk-tab", ParentTag = "govuk-tabs")]
public class TabTagHelper(IHtmlHelper htmlHelper)
    : PartialTagHelperBase(htmlHelper)
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public bool Selected { get; set; }

    public override async Task ProcessAsync(
        TagHelperContext context,
        TagHelperOutput output)
    {
        if (!context.Items.TryGetValue(
                TabsTagHelper.ContextVariableName,
                out var contextValue) ||
            contextValue is not TabsContext tabsContext)
        {
            throw new InvalidOperationException(
                "govuk-tab must be nested inside govuk-tabs");
        }

        var childContent = await output.GetChildContentAsync();

        tabsContext.Tabs.Add(
            new GovUkTabModel
            {
                Id = Id,
                Title = Title,
                Selected = Selected,
                Content = new HtmlString(childContent.GetContent())
            });

        output.TagName = null;
    }
}