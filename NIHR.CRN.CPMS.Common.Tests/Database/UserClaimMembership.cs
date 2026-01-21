using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class UserClaimMembership : IUserClaimMembership
{
    public long Id { get; set; }
    public long ClaimTypeId { get; set; }
    public DateTime? CreatedDate { get; set; }

    public long UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
}