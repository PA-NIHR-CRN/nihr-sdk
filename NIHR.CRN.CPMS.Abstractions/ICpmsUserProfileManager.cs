namespace NIHR.CRN.CPMS.Abstractions;

public interface ICpmsUserProfileManager<TUserProfile>
{
    Task<Result<TUserProfile>> FetchAndUpdateUserProfileAsync(string? email, string? uuid, string? firstName,
        string? lastName, string? orcId);
}