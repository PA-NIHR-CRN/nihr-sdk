namespace NIHR.CRN.CPMS.Abstractions;

/// <summary>
/// Provides a seam to allow different ORMs (i.e. EFCore/EF6 to be used)
/// </summary>
public interface ICpmsUserStore<TUserProfile, TRefPerson, TUserClaimMembership, TAcl>
    where TUserProfile : IUserProfile<TRefPerson, TUserClaimMembership, TAcl>, new()
    where TRefPerson : IRefPerson<TAcl>, new()
    where TUserClaimMembership : IUserClaimMembership, new()
    where TAcl : IAcl
{
    Task<TUserProfile?> GetFirstOrDefaultUserProfileByEmailAsync(string email);

    /// <summary>
    /// Gets a user profile by UUID
    /// </summary>
    /// <returns> A user profile including all active UserClaimMembership rows</returns>
    Task<TUserProfile?> GetSingleOrDefaultUserProfileByUuidAsync(string uuid);

    void AddUserProfile(TUserProfile userProfile);

    Task SaveChangesAsync();

    /// <summary>
    /// Gets a user profile by email address
    /// </summary>
    /// <returns> A user profile including all active UserClaimMembership rows</returns>
    Task<TRefPerson?> GetSingleOrDefaultRefPersonByEmailAsync(string email);
}