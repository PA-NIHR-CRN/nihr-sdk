using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NIHR.Infrastructure.AspNetCore.TrustedUris;
using NIHR.Infrastructure.Paging;

namespace NIHR.Infrastructure.AspNetCore.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddPaging(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services.AddScoped<IPaginationService, RequestQueryParamPaginationService>();
        }
        
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
