using Microsoft.EntityFrameworkCore;
using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class TestUserStore (TestDbContext dbContext) : ICpmsUserStore<UserProfile, RefPerson, UserClaimMembership, Acl>
{
    public Task<UserProfile?> GetUserProfileByEmailAsync(string email) =>
        UserProfilesWithRoles.SingleOrDefaultAsync(i => i.EmailId == email);

    public Task<RefPerson?> GetRefPersonByEmailAsync(string email) =>
        dbContext.RefPerson.SingleOrDefaultAsync(i => i.Email == email);

    public Task<UserProfile?> GetUserProfileByUuidAsync(string uuid)  =>
        UserProfilesWithRoles.SingleOrDefaultAsync(i => i.UserId == uuid);

    private IQueryable<UserProfile> UserProfilesWithRoles =>
        dbContext.UserProfile
            .Include(i => i.Person)
            .Include(i => i.UserClaimMembership);

    public void AddUserProfile(UserProfile userProfile) => dbContext.UserProfile.Add(userProfile);

    public Task SaveChangesAsync() => dbContext.SaveChangesAsync();
}