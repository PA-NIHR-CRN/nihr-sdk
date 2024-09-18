using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NIHR.Infrastructure.EntityFrameworkCore.Internal;

namespace NIHR.Infrastructure.EntityFrameworkCore;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public const string HardDeleteAnnotation = "HardDelete";

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        return new ValueTask<InterceptionResult<int>>(HandleSoftDelete(eventData, result));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return HandleSoftDelete(eventData, result);
    }

    protected static InterceptionResult<int> HandleSoftDelete(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (TryHardDelete(eventData, entry))
            {
                // Entry has been marked for hard delete, bypass soft delete.
                continue;
            }

            if (entry is { State: EntityState.Deleted, Entity: ISoftDelete delete })
            {
                if (entry is { Entity: IPersonalInformation pii })
                {
                    pii.Anonymise();
                }

                entry.State = EntityState.Modified;
                delete.IsDeleted = true;
            }
        }

        return result;
    }

    public static object? GetKey<T>(DbContext context, T entity)
    {
        var keyName = context.Model.FindEntityType(entity.GetType()).FindPrimaryKey().Properties
            .Select(x => x.Name).Single();

        return entity.GetType().GetProperty(keyName).GetValue(entity, null);
    }

    protected static bool TryHardDelete(DbContextEventData eventData, EntityEntry? entry)
    {
        if (eventData.Context is null)
        {
            return false;
        }

        if (entry is null)
        {
            return false;
        }

        _ = eventData.Context.TryGetService(out ILogger<SoftDeleteInterceptor>? logger);

        _ = eventData.Context.TryGetService(out IHardDeleteAuthorizationProvider? authorisationProvider);

        if (entry.Metadata.GetRuntimeAnnotations().Any(x => x.Name == HardDeleteAnnotation && x.Value is true))
        {
            if (authorisationProvider?.CanHardDelete() ?? false)
            {
                logger?.LogWarning("{Entry} '{Id}' hard deleted.", entry.Metadata.ContainingEntityType, GetKey(eventData.Context, entry.Entity));

                return true;
            }

            throw new UnauthorizedHardDeleteException($"Unauthorized hard delete on {entry.Metadata.ContainingEntityType} '{GetKey(eventData.Context, entry.Entity)}'.");
        }

        return false;
    }
}
