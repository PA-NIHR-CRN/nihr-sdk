using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    public class NavigationTagHelper : PartialTagHelperBase
    {
        public NavigationTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.ViewData.HasNavigation())
            {
                var content = await RenderPartialAsync("_Navigation", ViewContext.ViewData.Navigation().Items);
                output.TagName = null;
                output.TagMode = TagMode.StartTagAndEndTag;

                output.Content.SetHtmlContent(content);
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
