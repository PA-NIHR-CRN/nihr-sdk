using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions
{
    public static class TagHelperClassExtensions
    {
        public static bool HasClass(this TagHelperContext context, string className)
        {
            return context.AllAttributes.HasClass(className);
        }

        public static bool HasClass(this TagHelperOutput output, string className)
        {
            return output.Attributes.HasClass(className);
        }

        public static bool HasClass(this IList<TagHelperAttribute> attributes, string className)
        {
            var existingClasses = attributes
                .Where(attr => attr.Name.Equals("class", StringComparison.OrdinalIgnoreCase))
                .SelectMany(attr => attr.Value?.ToString()?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>());

            return existingClasses.Any(cls => cls.Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        public static void AppendClass(this TagHelperOutput output, string className)
        {
            if (!output.HasClass(className))
            {
                output.AddClass(className, HtmlEncoder.Default);
            }
        }

        public static void PrependClass(this TagHelperOutput output, string className, HtmlEncoder? htmlEncoder = null)
        {
            htmlEncoder ??= HtmlEncoder.Default;

            if (!output.Attributes.TryGetAttribute("class", out var classAttr))
            {
                output.Attributes.Add("class", className);
                return;
            }

            var classList = classAttr.Value?.ToString()?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            if (!classList.Contains(className, StringComparer.OrdinalIgnoreCase))
            {
                classList.Insert(0, className);
                output.Attributes.SetAttribute("class", string.Join(" ", classList));
            }
        }

        public static void RemoveClass(this TagHelperOutput output, string className, HtmlEncoder? htmlEncoder = null)
        {
            htmlEncoder ??= HtmlEncoder.Default;

            if (!output.Attributes.TryGetAttribute("class", out var classAttr)) return;

            var classList = classAttr.Value?.ToString()?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

            classList.RemoveAll(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));
            output.Attributes.SetAttribute("class", string.Join(" ", classList));
        }
    }
}
