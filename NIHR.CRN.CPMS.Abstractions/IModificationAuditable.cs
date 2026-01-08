namespace NIHR.CRN.CPMS.Abstractions;

public interface IModificationAuditable : ICreationAuditable
{
    public long? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
}