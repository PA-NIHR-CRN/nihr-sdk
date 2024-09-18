using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    public class PhaseBannerTagHelper : PartialTagHelperBase
    {
        public PhaseBannerTagHelper(IHtmlHelper htmlHelper) : base(htmlHelper)
        {
        }

        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = await RenderPartialAsync("_PhaseBanner", new { });
            output.TagName = null;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.SetHtmlContent(content);
        }
    }
}
