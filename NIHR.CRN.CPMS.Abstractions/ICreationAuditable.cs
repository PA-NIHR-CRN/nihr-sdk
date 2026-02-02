namespace NIHR.CRN.CPMS.Abstractions;

public interface ICreationAuditable
{
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}