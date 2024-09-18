using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NIHR.GovUk.AspNetCore.Mvc
{
    public class PartialTagHelperBase(IHtmlHelper htmlHelper) : TagHelper
    {

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        protected async Task<IHtmlContent> RenderPartialAsync(string partialViewName, object model)
        {
            (htmlHelper as IViewContextAware)?.Contextualize(ViewContext);
            return await htmlHelper.PartialAsync(partialViewName, model);
        }
    }
}
