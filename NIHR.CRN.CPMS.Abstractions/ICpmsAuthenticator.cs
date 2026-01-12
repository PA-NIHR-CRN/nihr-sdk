namespace NIHR.CRN.CPMS.Abstractions;

public interface ICpmsAuthenticator<TUserProfile>
{
    Task<AuthResult<TUserProfile>> SynchronizeUserProfileAsync(bool isDevelopmentEnvironment,
        string? email, string? uuid, string? firstName, string? lastName, string? orcId);
}