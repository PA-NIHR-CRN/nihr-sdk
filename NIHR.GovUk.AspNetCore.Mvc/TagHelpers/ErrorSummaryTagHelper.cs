using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

public class ErrorSummaryTagHelper(IHtmlHelper htmlHelper, ILogger<ErrorSummaryTagHelper> logger) : PartialTagHelperBase(htmlHelper)
{
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;

        if (ViewContext.ViewData.ModelState.IsValid)
        {
            output.SuppressOutput();
        }
        else
        {
            var content = await RenderPartialAsync("_ErrorSummary", ViewContext.Errors());
            output.Content.SetHtmlContent(content);
        }
    }
}