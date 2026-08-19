using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NIHR.GovUk.AspNetCore.Mvc.Models;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

public class RadioSet(IHtmlHelper htmlHelper) : PartialTagHelperBase(htmlHelper)
{
    public const string ContextVariableName = "__govuk_radios_context";
    public ModelExpression? For { get; set; }
    
    public string? Name { get; set; }
    
    public object? Value { get; set; }
    
    public GovUkRadioSize Size { get; set; } = GovUkRadioSize.Default;
    
    public GovUkRadioLayout Layout { get; set; } = GovUkRadioLayout.Default;    

    public override void Init(TagHelperContext context)
    {
        context.Items.Add(ContextVariableName, new RadiosContext(
            Name ?? For?.Name,
            Name == null ? For?.ModelExplorer.Model : Value));
        base.Init(context);
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {        
        output.TagName = null;
        var innerContent = await output.GetChildContentAsync();
        var content = await RenderPartialAsync("_Radios", 
            new GovUkRadiosModel(Size, Layout, innerContent));
        output.Content.SetHtmlContent(content);
    }
}