namespace NIHR.CRN.CPMS.Abstractions;

public interface IUserClaimMembership
{
    long ClaimTypeId { get; set; }
    DateTime CreatedDate { get; set; }
}