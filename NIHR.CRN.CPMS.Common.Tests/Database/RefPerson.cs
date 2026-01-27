using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class RefPerson : IRefPerson<Acl>
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? OrcId { get; set; }
    public bool Active { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public long AclId { get; set; }

    public ICollection<UserProfile> UserProfiles { get; } = new List<UserProfile>();

    public Acl Acl { get; set; }
    public long? CreatedBy { get; set; }
    public long? ModifiedBy { get; set; }
}