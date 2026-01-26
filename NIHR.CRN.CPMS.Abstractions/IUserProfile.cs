namespace NIHR.CRN.CPMS.Abstractions;

public interface IUserProfile<TRefPerson, TUserClaimMembership, TAcl>
    where TRefPerson : IRefPerson<TAcl>
    where TUserClaimMembership : IUserClaimMembership
{
    string EmailId { get; set; }
    DateTime? LastLogin { get; set; }
    string? UserId { get; set; }
    bool Active { get; set; }
    
    TRefPerson Person { get; set; }
    
    ICollection<TUserClaimMembership> UserClaimMembership { get; }
}