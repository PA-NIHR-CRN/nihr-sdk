using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using NIHR.Application.Options;

namespace NIHR.Application;

public class AwsSecretConfigurationProvider : ConfigurationProvider
{
    private const string AwsVersionStageCurrent = "AWSCURRENT";
    private readonly AwsSsoOptions _ssoOptions;
    private readonly string _region;
    private readonly string _secretName;

    public AwsSecretConfigurationProvider(AwsSsoOptions ssoOptions, string region, string secretName)
    {
        _ssoOptions = ssoOptions;
        _region = region;
        _secretName = secretName;
    }

    public override void Load()
    {
        var secret = GetSecret();

        Data = JsonSerializer.Deserialize<Dictionary<string, string>>(secret);
    }

    private string GetSecret()
    {
        var request = new GetSecretValueRequest
        {
            SecretId = _secretName,
            VersionStage = AwsVersionStageCurrent
        };

        using (var client = new AmazonSecretsManagerClient(
                   new SSOAWSCredentials(_ssoOptions.AccountId, _ssoOptions.Region, _ssoOptions.RoleName, _ssoOptions.StartUrl),
                   RegionEndpoint.GetBySystemName(_region)))
        {
            var response = client.GetSecretValueAsync(request).Result;

            string secretString;
            if (response.SecretString != null)
            {
                secretString = response.SecretString;
            }
            else
            {
                var memoryStream = response.SecretBinary;
                var reader = new StreamReader(memoryStream);
                secretString =
                    System.Text.Encoding.UTF8
                        .GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            return secretString;
        }
    }
}
