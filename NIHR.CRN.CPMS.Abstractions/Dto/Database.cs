namespace NIHR.CRN.CPMS.Abstractions;

public class Database
{
    public int LatencyMs { get; set; }
    public string LatestMigration { get; set; } = string.Empty;
}