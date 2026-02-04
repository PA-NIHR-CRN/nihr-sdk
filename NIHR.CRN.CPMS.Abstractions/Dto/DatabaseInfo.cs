namespace NIHR.CRN.CPMS.Abstractions;

public class DatabaseInfo
{
    public int LatencyMs { get; set; }
    public string LatestMigration { get; set; } = string.Empty;
}