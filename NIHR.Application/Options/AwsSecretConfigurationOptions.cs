namespace NIHR.Application.Options;

public class AwsSecretConfigurationOptions
{
    public bool Enabled { get; set; }
    public string Region { get; set; }
    public string SecretNamePrefix { get; set; }
}