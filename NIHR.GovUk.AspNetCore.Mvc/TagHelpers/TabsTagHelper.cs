using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

[HtmlTargetElement("govuk-tabs")]
public class TabsTagHelper(IHtmlHelper htmlHelper)
    : PartialTagHelperBase(htmlHelper)
{
    internal const string ContextVariableName = "__GovUkTabsContext";

    public string Title { get; set; } = "Contents";

    public override async Task ProcessAsync(
        TagHelperContext context,
        TagHelperOutput output)
    {
        var tabsContext = new TabsContext();

        context.Items[ContextVariableName] = tabsContext;

        await output.GetChildContentAsync();

        output.TagName = null;

        var model = new GovUkTabsModel(
            Title,
            tabsContext.Tabs);

        var content = await RenderPartialAsync(
            "_Tabs",
            model);

        output.Content.SetHtmlContent(content);
    }
}