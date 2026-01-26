namespace NIHR.CRN.CPMS.Abstractions;

public interface IRefPerson<TAcl>
where TAcl : IAcl
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? OrcId { get; set; }
    bool Active { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public TAcl Acl { get; set; }
}

public interface IAcl
{
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdatedDate { get; set; }
}