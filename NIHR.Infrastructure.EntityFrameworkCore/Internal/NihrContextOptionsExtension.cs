using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace NIHR.Infrastructure.EntityFrameworkCore.Internal
{
    internal class NihrContextOptionsExtension : IDbContextOptionsExtension
    {
        private NihrConventionOptions _conventionOptions;

        public NihrContextOptionsExtension(NihrConventionOptions conventionOptions)
        {
            _conventionOptions = conventionOptions;
        }

        public DbContextOptionsExtensionInfo Info => new ExtensionInfo(this);

        public void ApplyServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNihrConventions(_conventionOptions);
        }

        public void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension) : base(extension) { }

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    return "NIHR Convention";
                }
            }

            public override int GetServiceProviderHashCode()
            {
                return LogFragment.GetHashCode();
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return other.GetServiceProviderHashCode() == GetServiceProviderHashCode();
            }

            public override void PopulateDebugInfo([NotNull] IDictionary<string, string> debugInfo)
            {
            }
        }
    }
}
