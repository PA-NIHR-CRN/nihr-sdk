namespace NIHR.CRN.CPMS.Abstractions;

/// <summary>
/// Provides a seam to allow different ORMs (i.e. EFCore/EF6 to be used)
/// TODO: Move this interface to the SDK and implement in CPMS classic.
/// </summary>
public interface ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>
    where TUserProfile : IUserProfile<TRefPerson, TUserClaimMembership, TAcl>, new()
    where TRefPerson : IRefPerson<TAcl>, new()
    where TUserClaimMembership : IUserClaimMembership, new()
{
    Task<TUserProfile?> GetUserProfileByEmailAsync(string email);

    /// <summary>
    /// Gets a user profile by UUID
    /// </summary>
    /// <returns> A user profile including all active UserClaimMembership rows</returns>
    Task<TUserProfile?> GetUserProfileByUuidAsync(string uuid);

    void AddUserProfile(TUserProfile userProfile);

    Task SaveChangesAsync();

    /// <summary>
    /// Gets a user profile by email address
    /// </summary>
    /// <returns> A user profile including all active UserClaimMembership rows</returns>
    Task<TRefPerson?> GetRefPersonByEmailAsync(string email);
}