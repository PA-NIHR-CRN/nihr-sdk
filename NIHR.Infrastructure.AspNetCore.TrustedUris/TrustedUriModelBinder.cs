using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

internal class TrustedUriModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var redirectValidator =
            context.ActionContext.HttpContext.RequestServices.GetRequiredService<ITrustedUriValidator>();
        var value = context.ValueProvider.GetValue(context.ModelName).FirstValue;
        
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }
        
        if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
        {
            if (context.ModelMetadata is DefaultModelMetadata defaultModelMetadata && defaultModelMetadata.Attributes
                    .Attributes.OfType<AllowUntrustedUrisAttribute>().Any())
            {
                context.Result = ModelBindingResult.Success(uri);
            }
            
            var validationResult = redirectValidator.Validate(uri);
            if (validationResult == ValidationResult.Success)
            {
                context.Result = ModelBindingResult.Success(uri);
            }
            else
            {
                throw new UntrustedUriException(validationResult!.ErrorMessage!);
            }
        }
        else
        {
            context.Result = ModelBindingResult.Failed();
            context.ModelState.AddModelError(context.ModelName, "URL is not well-formed");
        }
        
        return Task.CompletedTask;
    }
}