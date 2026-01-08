using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common;

public static class CpmsUserProfileStoreExtensions
{
    public static async Task<TUserProfile> SynchronizeUserProfileAsync<TUserProfile, TRefPerson, TUserClaimMembership>(
        this ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership> userStore, string? uuid,
        string email, ExtendedUserAttributes? extendedUserAttributes = null)
        where TUserProfile : class, IUserProfile<TRefPerson, TUserClaimMembership>,  new()
        where TRefPerson : class, IRefPerson, new()
        where TUserClaimMembership : class, IUserClaimMembership, new()
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException(nameof(email));
        }

        var userProfile = string.IsNullOrWhiteSpace(uuid) ? null
            : await userStore.GetUserProfileByUuidAsync(uuid);

        if (userProfile == null)
        {
            userProfile = await userStore.GetUserProfileByEmailAsync(email);

            if (userProfile == null)
            {
                userProfile = new TUserProfile
                {
                    EmailId = email,
                    LastLogin = DateTime.Now,
                    UserId = uuid
                };
                userStore.AddUserProfile(userProfile);
            }
            else if (!string.IsNullOrWhiteSpace(uuid))
            {
                userProfile.UserId = uuid;
            }
        }
        else
        {
            userProfile.EmailId = email;
        }

        // If the user has no claims then add PublicUser
        if (userProfile.UserClaimMembership.Count == 0)
        {
            userProfile.UserClaimMembership.Add(new()
            {
                ClaimTypeId = (long)ClaimTypes.PublicUser
            });
        }


        if (userProfile.Person == null)
        {
            var person = await userStore.GetRefPersonByEmailAsync(userProfile.EmailId);
            userProfile.Person = person ?? new();
        }

        userProfile.Person.Email = email;

        if (extendedUserAttributes != null)
        {
            userProfile.Person.FirstName = extendedUserAttributes.FirstName ?? string.Empty;
            userProfile.Person.LastName = extendedUserAttributes.LastName ?? string.Empty;
            userProfile.Person.OrcId = extendedUserAttributes.OrcId ?? string.Empty;
        }

        userProfile.LastLogin = DateTime.Now;

        await userStore.SaveChangesAsync();

        return userProfile;
    }
}