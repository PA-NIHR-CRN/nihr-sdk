using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NIHR.Infrastructure.AspNetCore.TrustedUris
{
    public static class Extensions
    {
        public static IServiceCollection AddTrustedUriValidation(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITrustedUriValidator, TrustedUriValidator>();
            services.AddOptions<TrustedUriSettings>().BindConfiguration("TrustedUris")
                .ValidateDataAnnotations().ValidateOnStart();
            return services;
        }

        public static MvcOptions AddTrustedUriValidation(this MvcOptions options)
        {
            options.Filters.Add<UntrustedUriExceptionFilter>();
            options.ModelBinderProviders.Insert(0, new TrustedUriModelBinderProvider());
            return options;
        }
    }
}
