namespace NIHR.CRN.CPMS.Abstractions;

public class ExtendedUserAttributes (string firstName, string lastName, string? orcid)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string? Orcid { get; } = orcid;
}