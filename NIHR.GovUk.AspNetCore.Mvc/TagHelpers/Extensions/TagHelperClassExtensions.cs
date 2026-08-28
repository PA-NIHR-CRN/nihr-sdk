using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers.Extensions
{
    public static class TagHelperClassExtensions
    {
        private static List<string> ParseClasses(string? classValue)
            => classValue?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList() ?? new List<string>();

        public static bool HasClass(this TagHelperOutput output, string className)
        {
            var classAttr = output.Attributes["class"]?.Value?.ToString();
            return ParseClasses(classAttr).Any(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        public static void AppendClass(this TagHelperOutput output, string className)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrWhiteSpace(className))
                return;

            var classAttr = output.Attributes["class"]?.Value?.ToString();
            var classes = ParseClasses(classAttr);

            classes.RemoveAll(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));

            classes.Add(className);

            output.Attributes.SetAttribute("class", string.Join(" ", classes));
        }

        public static void PrependClass(this TagHelperOutput output, string className)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            if (string.IsNullOrWhiteSpace(className))
                return;

            var classAttr = output.Attributes["class"]?.Value?.ToString();
            var classes = ParseClasses(classAttr);

            classes.RemoveAll(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));

            classes.Insert(0, className);

            output.Attributes.SetAttribute("class", string.Join(" ", classes));
        }

        public static void AddClass(this TagHelperOutput output, string className)
        {
            var classAttr = output.Attributes["class"]?.Value?.ToString();
            var classes = ParseClasses(classAttr);

            if (!classes.Contains(className, StringComparer.OrdinalIgnoreCase))
            {
                classes.Add(className);
                output.Attributes.SetAttribute("class", string.Join(" ", classes));
            }
        }

        public static void RemoveClass(this TagHelperOutput output, string className)
        {
            var classAttr = output.Attributes["class"]?.Value?.ToString();
            var classes = ParseClasses(classAttr);

            classes.RemoveAll(c => c.Equals(className, StringComparison.OrdinalIgnoreCase));
            output.Attributes.SetAttribute("class", string.Join(" ", classes));
        }
    }
}
