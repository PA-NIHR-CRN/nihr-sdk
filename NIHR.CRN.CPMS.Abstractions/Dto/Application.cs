namespace NIHR.CRN.CPMS.Abstractions;

public class Application
{
    public string CpmsVersion { get; set; } = string.Empty;
    public string CommitHash { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public string FullHash { get; set; } = string.Empty;
    public int Build { get; set; }
}