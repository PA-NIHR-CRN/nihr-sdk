using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris;

internal class TrustedUriModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(Uri) ? new TrustedUriModelBinder() : null;
    }
}