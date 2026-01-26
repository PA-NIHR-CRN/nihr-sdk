using NIHR.CRN.CPMS.Abstractions;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class Acl : IAcl
{
    public long Id { get; set; }
    
    public virtual ICollection<RefPerson> RefPeople { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}