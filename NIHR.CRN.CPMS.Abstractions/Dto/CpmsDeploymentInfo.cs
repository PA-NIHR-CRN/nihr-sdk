namespace NIHR.CRN.CPMS.Abstractions;

public class CpmsDeploymentInfo
{
    public Environment Environment { get; set; } = new Environment();
    public Database Database { get; set; } = new Database();
    public Application Application { get; set; } = new Application();
}