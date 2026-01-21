using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class UserProfile : IUserProfile<RefPerson, UserClaimMembership>
{
    public long Id { get; set; }
    public string EmailId { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? UserId { get; set; }
    public RefPerson Person { get; set; }
    public bool Active { get; set; }
    public ICollection<UserClaimMembership> UserClaimMembership { get; } = new List<UserClaimMembership>();
    
    public long RefPersonId { get; set; }
}