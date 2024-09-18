using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NIHR.Infrastructure.EntityFrameworkCore.Internal;

namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public static class NihrDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseNihrConventions(this DbContextOptionsBuilder options, Action<NihrConventionOptions>? conventionOptionsAction = null)
        {
            var conventionOptions = GetConventionOptions(conventionOptionsAction);

            if (conventionOptions.DisableAutodetectChangesInInterceptors)
            {
                options.AddInterceptors(new DisableAutoDetectChangesInterceptor());
            }

            options.AddInterceptors(
                new SoftDeleteInterceptor(),
                new TimestampInterceptor(),
                new AuditInterceptor()
                );

            var extension = new NihrContextOptionsExtension(conventionOptions);

            ((IDbContextOptionsBuilderInfrastructure)options).AddOrUpdateExtension(extension);
            return options;
        }

        private static NihrConventionOptions GetConventionOptions(Action<NihrConventionOptions>? conventionOptionsAction)
        {
            var conventionOptions = new NihrConventionOptions();

            if (conventionOptionsAction is not null)
            {
                conventionOptionsAction(conventionOptions);
            }

            return conventionOptions;
        }
    }
}
