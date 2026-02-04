namespace NIHR.CRN.CPMS.Abstractions;

public class CpmsDeploymentInfo
{
    public EnvironmentInfo EnvironmentInfo { get; set; } = new EnvironmentInfo();
    public DatabaseInfo DatabaseInfo { get; set; } = new DatabaseInfo();
    public ApplicationInfo Application { get; set; } = new ApplicationInfo();
}