using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace NIHR.Infrastructure.EntityFrameworkCore.Internal
{
    internal static class ServiceProviderExtensions
    {
        public static bool TryGetService<TService>(this IInfrastructure<IServiceProvider> accessor, out TService? service) where TService : class?
        {
            // See Microsoft.EntityFrameworkCore.Infrastructure.Internal.InfrastructureExtensions.GetService()

            var serviceType = typeof(TService);

            IServiceProvider instance = accessor.Instance;

            service = (instance.GetService(serviceType) ?? instance.GetService<IDbContextOptions>()?.Extensions.OfType<CoreOptionsExtension>().FirstOrDefault()?.ApplicationServiceProvider?.GetService(serviceType)) as TService;

            return service != null;
        }

        public static IServiceCollection AddEntityFrameworkNihrConventions(this IServiceCollection serviceCollection, NihrConventionOptions? conventionOptions = null)
        {
            new EntityFrameworkServicesBuilder(serviceCollection)
                .TryAdd<IConventionSetPlugin, NihrConventionSetPlugin>();

            if (conventionOptions is not null)
            {
                serviceCollection.AddTransient(p => conventionOptions);
            }

            return serviceCollection;
        }
    }
}
