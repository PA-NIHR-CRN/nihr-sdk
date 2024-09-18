using System;
using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace NIHR.Infrastructure.Configuration
{
    public class AwsSecretsManagerConfigurationSource : JsonStreamConfigurationSource
    {
        private readonly string _secretName;
        private readonly Func<IAmazonSecretsManager> _secretsManagerClientFactory;

        public AwsSecretsManagerConfigurationSource(string secretName,
            Func<IAmazonSecretsManager> secretsManagerClientFactory)
        {
            _secretName = secretName;
            _secretsManagerClientFactory = secretsManagerClientFactory;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AwsSecretsManagerConfigurationProvider(_secretsManagerClientFactory(), _secretName);
        }
    }
}
