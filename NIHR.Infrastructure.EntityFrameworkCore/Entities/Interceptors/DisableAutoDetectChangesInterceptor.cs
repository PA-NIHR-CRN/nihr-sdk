using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NIHR.Infrastructure.EntityFrameworkCore;

public class DisableAutoDetectChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            eventData.Context.ChangeTracker.DetectChanges();
        }

        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            eventData.Context.ChangeTracker.AutoDetectChangesEnabled = false;
            eventData.Context.ChangeTracker.DetectChanges();
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        if (eventData.Context != null)
        {
            eventData.Context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return result;
    }

    public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context != null)
        {
            eventData.Context.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        return new ValueTask<int>(result);
    }
}
