namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class Acl
{
    public long Id { get; set; }
    
    public virtual ICollection<RefPerson> RefPeople { get; set; }
}