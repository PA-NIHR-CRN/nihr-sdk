using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers
{
    public class PaginationTagHelper : PartialTagHelper
    {
        public string? AnchorId { get; set; }

        public PaginationTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope) : base(viewEngine, viewBufferScope)
        {
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = "_Pagination";
            }

            if (!string.IsNullOrWhiteSpace(AnchorId))
            {
                ViewData = new ViewDataDictionary(ViewContext.ViewData) { { "AnchorId", AnchorId } };
            }

            return base.ProcessAsync(context, output);
        }
    }
}
