using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    public class BreadcrumbsTagHelper : PartialTagHelper
    {
        public BreadcrumbsTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope) : base(viewEngine, viewBufferScope)
        {
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = "_Breadcrumbs";
            }

            return base.ProcessAsync(context, output);
        }
    }
}
