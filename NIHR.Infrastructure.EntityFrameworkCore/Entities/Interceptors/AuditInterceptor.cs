using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NIHR.Infrastructure.EntityFrameworkCore.Internal;

namespace NIHR.Infrastructure.EntityFrameworkCore;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        return new ValueTask<InterceptionResult<int>>(HandleAuditColumns(eventData, result));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return HandleAuditColumns(eventData, result);
    }

    protected InterceptionResult<int> HandleAuditColumns(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is null)
        {
            return result;
        }

        var _ = eventData.Context.TryGetService(out ILogger<AuditInterceptor>? logger);

        if (!eventData.Context.TryGetService(out ICurrentUserIdProvider<int>? currentUserIdProvider) || (currentUserIdProvider?.SuppressUnknownUserIdWarning == false && currentUserIdProvider?.UserId is null))
        {
            logger?.LogWarning("Unable to determine user id. Ensure that an instance of {providerName} is configured and returns the id of the current user.", nameof(ICurrentUserIdProvider<int>));

            return result;
        }


        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Added, Entity: IAudit inserted })
            {
                if (inserted.CreatedById == default)
                {
                    inserted.CreatedById = currentUserIdProvider.UserId.Value;
                }

                if (inserted.UpdatedAt == default)
                {
                    inserted.UpdatedById = currentUserIdProvider.UserId.Value;
                }
            }

            if (entry is { State: EntityState.Modified, Entity: IAudit updated })
            {
                updated.UpdatedById = currentUserIdProvider.UserId.Value;
            }

            if (entry is { State: EntityState.Deleted, Entity: IAudit deleted })
            {
                deleted.UpdatedById = currentUserIdProvider.UserId.Value;
            }
        }

        return result;
    }
}
