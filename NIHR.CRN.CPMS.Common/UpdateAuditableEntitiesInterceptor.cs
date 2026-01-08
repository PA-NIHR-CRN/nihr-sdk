using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NIHR.CRN.CPMS.Abstractions;
using NIHR.Infrastructure;

namespace NIHR.CRN.CPMS.Common;

public sealed class UpdateAuditableEntitiesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserIdProvider<long> _currentUserIdReader;
    private readonly TimeProvider _timeProvider;

    public UpdateAuditableEntitiesInterceptor(ICurrentUserIdProvider<long> currentUserIdReader,
        TimeProvider timeProvider)
    {
        _currentUserIdReader = currentUserIdReader;
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        const long systemUserId = 1;

        DateTime currentTime = _timeProvider.GetLocalNow().DateTime;
        var userProfileId = _currentUserIdReader.UserId ?? systemUserId;

        foreach (var entry in context.ChangeTracker.Entries<ICreationAuditable>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    SetCreatedInfo(entry.Entity, userProfileId, currentTime);
                    break;
                case EntityState.Deleted:
                case EntityState.Modified:
                    SetModifiedInfo(entry.Entity as IModificationAuditable, userProfileId, currentTime);
                    break;
            }
        }
    }

    private static void SetCreatedInfo(ICreationAuditable entity, long userProfileId, DateTime currentTime)
    {
        entity.CreatedBy = userProfileId;
        entity.CreatedDate = currentTime;
        SetModifiedInfo(entity as IModificationAuditable, userProfileId, currentTime);
    }

    private static void SetModifiedInfo(IModificationAuditable? modifiableEntity, long userProfileId, DateTime currentTime)
    {
        if (modifiableEntity != null)
        {
            modifiableEntity.ModifiedBy = userProfileId;
            modifiableEntity.ModifiedDate = currentTime;
        }
    }
}
