using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public class ReferenceDataConvention : IModelFinalizingConvention
    {
        private const string _referenceDataTableNamePrefix = "SysRef";

        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            var refDataTypes = modelBuilder.Metadata.GetEntityTypes().Select(x => x).Where(t => typeof(IReferenceData).IsAssignableFrom(t.ClrType) && t.ClrType.IsClass && !t.ClrType.IsAbstract);

            foreach (var type in refDataTypes)
            {
                // TODO: use TableName as base rather than type name
                // Make this configurable?
                // Use ClrType.Name for backwards compatibility.

                // var tableName = type.GetTableName();
                var tableName = type.ClrType.Name;

                if (!tableName?.StartsWith(_referenceDataTableNamePrefix) ?? false)
                {
                    type.SetTableName(_referenceDataTableNamePrefix + tableName);
                }
            }
        }
    }
}