using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Linq.Expressions;

namespace NIHR.Infrastructure.EntityFrameworkCore
{
    public class SoftDeleteConvention : IModelFinalizingConvention
    {
        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            var softDeleteDataTypes = modelBuilder.Metadata.GetEntityTypes().Select(x => x).Where(t => typeof(ISoftDelete).IsAssignableFrom(t.ClrType) && t.ClrType.IsClass && !t.ClrType.IsAbstract);

            foreach (var type in softDeleteDataTypes)
            {
                // TODO: Support addition to existing query filter(s).
                type.SetQueryFilter(GenerateSoftDeleteLambdaExpressionForType(type.ClrType));
            }
        }

        private static LambdaExpression? GenerateSoftDeleteLambdaExpressionForType(Type type)
        {
            // Generates expression equivalent to
            // x => x.IsDeleted == false;

            // TODO: Consider filtering out entities that are not marked as soft-delete but have a required relationship with a soft-delete entity.

            var parameter = Expression.Parameter(type, "x");
            var falseConstantValue = Expression.Constant(false);
            var propertyAccess = Expression.PropertyOrField(parameter, nameof(ISoftDelete.IsDeleted));
            var equalExpression = Expression.Equal(propertyAccess, falseConstantValue);
            var lambda = Expression.Lambda(equalExpression, parameter);

            return lambda;
        }
    }
}