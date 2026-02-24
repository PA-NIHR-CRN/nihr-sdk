namespace NIHR.CRN.CPMS.Abstractions;

public class CpmsDeploymentInfo
{
    public EnvironmentInfo Environment { get; set; } = new EnvironmentInfo();
    public DatabaseInfo Database { get; set; } = new DatabaseInfo();
    public ApplicationInfo Application { get; set; } = new ApplicationInfo();
}