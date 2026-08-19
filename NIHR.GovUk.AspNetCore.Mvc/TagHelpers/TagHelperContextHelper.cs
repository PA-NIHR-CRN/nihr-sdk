using Microsoft.AspNetCore.Razor.TagHelpers;

namespace NIHR.GovUk.AspNetCore.Mvc.TagHelpers;

public static class TagHelperContextHelper
{
    public static T GetRequired<T>(this TagHelperContext context, object key, string? notFoundErrorText = null)
    {
        if (context.Items.TryGetValue(key, out object? value))
        {
            if (value is T typedResult)
            {
                return typedResult;
            }

            throw new InvalidOperationException(
                $"An item with the specified key was found but was not of type {typeof(T)}");
        }
        
        throw new InvalidOperationException(
            notFoundErrorText ?? "An item with the specified key was not found");
    }
}