using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// TODO test this with json format from aws secrets manager
namespace NIHR.Infrastructure.Configuration
{
    public class AwsSecretsManagerConfigurationProvider : ConfigurationProvider
    {
        private readonly IAmazonSecretsManager _client;
        private readonly string _secretName;

        public AwsSecretsManagerConfigurationProvider(IAmazonSecretsManager client, string secretName)
        {
            _client = client;
            _secretName = secretName;
        }

        public override async void Load()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            // TODO had to make this sync to work with ecs, ask CON if there is a better way
            var response = Task
                .Run(async () =>
                    await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = _secretName }))
                .GetAwaiter().GetResult();

            var secretString = response.SecretString;
            if (string.IsNullOrEmpty(secretString))
            {
                throw new InvalidOperationException($"Secret {_secretName} is empty or not found.");
            }

            ParseSecret(secretString);
        }

        private void ParseSecret(string secretString)
        {
            try
            {
                var jToken = JToken.Parse(secretString);
                var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                ExtractValues(jToken, prefix: string.Empty, data);
                Data = FlattenDictionary(data);
            }
            catch (JsonReaderException ex)
            {
                throw new FormatException(
                    $"The secret value for {_secretName} is not in a recognized format. Expected JSON.", ex);
            }
        }

        private void ExtractValues(JToken token, string prefix, IDictionary<string, object> data)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (var child in token.Children<JProperty>())
                    {
                        var childKey = JoinKey(prefix, child.Name.Replace("__", ":"));
                        ExtractValues(child.Value, childKey, data);
                    }

                    break;
                case JTokenType.Array:
                    int index = 0;
                    foreach (var child in token.Children())
                    {
                        ExtractValues(child, JoinKey(prefix, index.ToString()), data);
                        index++;
                    }

                    break;
                default:
                    InsertIntoNestedDictionary(data, prefix, token.ToString());
                    break;
            }
        }

        private void InsertIntoNestedDictionary(IDictionary<string, object> data, string key, string value)
        {
            var keys = key.Split(':');
            var currentData = data;
            for (int i = 0; i < keys.Length - 1; i++)
            {
                if (!currentData.ContainsKey(keys[i]))
                {
                    currentData[keys[i]] = new Dictionary<string, object>();
                }

                currentData = (Dictionary<string, object>)currentData[keys[i]];
            }

            currentData[keys[^1]] = value;
        }

        private Dictionary<string, string> FlattenDictionary(Dictionary<string, object> data)
        {
            var flat = new Dictionary<string, string>();
            foreach (var kvp in data)
            {
                FlattenDictionaryHelper(kvp.Key, kvp.Value, flat);
            }

            return flat;
        }

        private void FlattenDictionaryHelper(string prefix, object value, Dictionary<string, string> flat)
        {
            if (value is Dictionary<string, object> dict)
            {
                foreach (var kvp in dict)
                {
                    FlattenDictionaryHelper(prefix + ":" + kvp.Key, kvp.Value, flat);
                }
            }
            else
            {
                flat[prefix] = value.ToString();
            }
        }

        private static string JoinKey(string prefix, string key)
        {
            return string.IsNullOrEmpty(prefix) ? key : $"{prefix}:{key}";
        }

        public async Task ForceReloadAsync()
        {
            await LoadAsync();
            OnReload();
        }
    }
}
