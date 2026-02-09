namespace NIHR.CRN.CPMS.Abstractions;

public interface ICpmsUserProfileManager<TUserProfile>
{
    Task<TUserProfile> FetchAndUpdateUserProfileAsync(string email, string uuid,
        ExtendedUserAttributes? extendedUserAttributes);
}