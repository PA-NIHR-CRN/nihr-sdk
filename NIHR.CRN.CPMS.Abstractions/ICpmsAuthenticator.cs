namespace NIHR.CRN.CPMS.Abstractions;

public interface ICpmsAuthenticator<TUserProfile>
{
    Task<Result<TUserProfile>> SynchronizeUserProfileAsync(string? email, string? uuid, string? firstName,
        string? lastName, string? orcId);
}