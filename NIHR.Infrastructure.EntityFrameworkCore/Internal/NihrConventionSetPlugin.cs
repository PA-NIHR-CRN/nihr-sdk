using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace NIHR.Infrastructure.EntityFrameworkCore.Internal
{
    internal class NihrConventionSetPlugin : IConventionSetPlugin
    {
        private readonly NihrConventionOptions _conventionOptions;

        public NihrConventionSetPlugin(NihrConventionOptions conventionOptions)
        {
            _conventionOptions = conventionOptions;
        }

        public ConventionSet ModifyConventions(ConventionSet conventionSet)
        {
            if (!_conventionOptions.UseTableNameFromDbSet)
            {
                conventionSet.Remove(typeof(TableNameFromDbSetConvention));
            }

            conventionSet.Add(new ReferenceDataConvention());
            conventionSet.Add(new SoftDeleteConvention());

            return conventionSet;
        }
    }
}