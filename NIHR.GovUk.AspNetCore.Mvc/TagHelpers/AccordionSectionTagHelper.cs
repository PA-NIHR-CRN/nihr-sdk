using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

public class AccordionSectionTagHelper(IHtmlHelper htmlHelper) : PartialTagHelperBase(htmlHelper)
{
    public string Caption { get; set; }
    
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = null;
        var innerContent = await output.GetChildContentAsync();
        var content = await RenderPartialAsync("_AccordionSection", 
            new GovUkAccordionSectionModel(Caption, innerContent));
        output.Content.SetHtmlContent(content);
    }
}