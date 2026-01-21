using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class RefPerson : IRefPerson
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? OrcId { get; set; }
    public bool Active { get; set; }

    public ICollection<UserProfile> UserProfiles { get; } = new List<UserProfile>();

}