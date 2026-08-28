using System;
using Microsoft.Extensions.Configuration;
using NIHR.Application.Options;

namespace NIHR.Application;

public class AwsSecretConfigurationSource : IConfigurationSource
{
    private readonly AwsSsoOptions _awsSsoOptions;
    private readonly string _region;
    private readonly string _secretName;

    public AwsSecretConfigurationSource(AwsSsoOptions awsSsoOptions, string region, string secretName)
    {
        _awsSsoOptions = awsSsoOptions ?? throw new ArgumentNullException(nameof(awsSsoOptions));
        _region = region;
        _secretName = secretName;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AwsSecretConfigurationProvider(_awsSsoOptions, _region, _secretName);
    }
}
