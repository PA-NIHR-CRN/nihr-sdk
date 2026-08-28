using NIHR.Application.Options;

namespace NIHR.Application;

internal class Layer1 : Layer0
{
    public AwsSsoOptions AwsSso { get; set; }
    public AwsSecretConfigurationOptions AwsSecretConfiguration { get; set; }
}