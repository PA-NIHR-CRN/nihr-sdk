namespace NIHR.CRN.CPMS.Abstractions;

public interface IRefPerson
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? OrcId { get; set; }
    bool Active { get; set; }
}