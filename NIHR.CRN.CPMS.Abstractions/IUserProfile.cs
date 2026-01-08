namespace NIHR.CRN.CPMS.Abstractions;

public interface IUserProfile<TRefPerson, TUserClaimMembership>
    where TRefPerson : IRefPerson
    where TUserClaimMembership : IUserClaimMembership
{
    string EmailId { get; set; }
    DateTime? LastLogin { get; set; }
    string? UserId { get; set; }
    
    TRefPerson Person { get; set; }
    
    ICollection<TUserClaimMembership> UserClaimMembership { get; set; }
}