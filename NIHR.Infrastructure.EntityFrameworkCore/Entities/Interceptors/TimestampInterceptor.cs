using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace NIHR.Infrastructure.EntityFrameworkCore;

public class TimestampInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        return new ValueTask<InterceptionResult<int>>(HandleTimestamps(eventData, result));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        return HandleTimestamps(eventData, result);
    }

    protected static InterceptionResult<int> HandleTimestamps(DbContextEventData eventData, InterceptionResult<int> result)
    {
       if (eventData.Context is null)
        {
            return result;
        }

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is { State: EntityState.Added, Entity: ITimestamped inserted })
            {
                if (inserted.CreatedAt == default)
                {
                    inserted.CreatedAt = DateTime.UtcNow;
                }

                if (inserted.UpdatedAt == default)
                {
                    inserted.UpdatedAt = inserted.CreatedAt;
                }
            }

            if (entry is { State: EntityState.Modified, Entity: ITimestamped updated })
            {
                updated.UpdatedAt = DateTime.UtcNow;
            }

            if (entry is { State: EntityState.Deleted, Entity: ITimestamped deleted })
            {
                deleted.UpdatedAt = DateTime.UtcNow;
            }
        }
        return result;
    }
}
