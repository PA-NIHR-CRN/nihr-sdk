using Contentful.AspNetCore;
using Contentful.Core.Models;
using Microsoft.Extensions.Configuration;
using NIHR.GovUk.AspNetCore.Mvc;
using NIHR.GovUk.AspNetCore.Mvc.ContentManagement;
using NIHR.Infrastructure.Interfaces;
using NIHR.Infrastructure.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddContentManagement(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddContentful(configuration);
            services.AddScoped<IContentProvider, ContentfulService>();

            services.AddTransient((c) =>
            {
                var renderer = new HtmlRenderer();
                renderer.AddRenderer(new GovUkHeadingRenderer(renderer.Renderers) { Order = 10 });
                renderer.AddRenderer(new GovUkParagraphRenderer(renderer.Renderers) { Order = 10 });
                return renderer;
            });

            return services;
        }

        public static IServiceCollection AddGovUk(this IServiceCollection services, Action<GovUkOptions> configureOptions)
        {

            services.AddTransient(_ =>
            {
                var options = new GovUkOptions();
                configureOptions(options);
                return options;
            });

            return services;
        }
    }
}
