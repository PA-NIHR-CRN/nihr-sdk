using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace NIHR.Application;

public static class NihrConfigurationExtensions
{
    private const string AppSettingsJsonFilename = "appsettings.json";
    private const string NihrAppNameName = nameof(Layer0.NihrAppName);
    private const string NihrEnvironmentName = nameof(Layer0.NihrEnvironment);
    private const string NihrEnvironmentDevelopment = "development";

    public static bool ValueExists(this IConfiguration configuration, string key) => 
        configuration.AsEnumerable().Any(x => x.Key == key);

    public static void AddNihrConfiguration(this IConfigurationBuilder configurationBuilder, string? appName = null)
    {
        ConfigureLayer0(configurationBuilder, appName); // basic environmental configuration
        ConfigureLayer1(configurationBuilder); // AWS configuration
        ConfigureLayer2(configurationBuilder); // AWS Secret
        ConfigureLayer3(configurationBuilder); // Local JSON files
    }

    private static void ConfigureLayer0(IConfigurationBuilder builder, string appName)
    {
        List<KeyValuePair<string, string>> inMemoryValues =
        [
            new(NihrAppNameName, appName ?? GetAppNameByConvention())
        ];

        if (Debugger.IsAttached && !builder.Build().ValueExists(NihrEnvironmentName))
        {
            // Developer convenience so that the environment defaults to 'development' when a debugger is attached.
            inMemoryValues.Add(new(NihrEnvironmentName, NihrEnvironmentDevelopment));
        }

        builder.AddInMemoryCollection(inMemoryValues);
    }

    private static void ConfigureLayer1(IConfigurationBuilder builder)
    {
        Layer0 layer0 = builder.Build().Get<Layer0>();
        layer0.ValidateAndThrow();

        const string awsSecretsJsonFilename = "awsSecrets.json";
        string awsSecretsEnvJsonFilename = $"awsSecrets.{layer0.NihrEnvironment}.json";

        if (layer0.HasConfigurationRoot)
        {
            // Machine scope
            builder.AddJsonFile(Path.Combine(layer0.NihrConfigurationRoot!, awsSecretsJsonFilename), true);
            builder.AddJsonFile(Path.Combine(layer0.NihrConfigurationRoot, awsSecretsEnvJsonFilename), true);

            // Application scope
            builder.AddJsonFile(Path.Combine(layer0.AppConfigRoot!, awsSecretsJsonFilename), true);
            builder.AddJsonFile(Path.Combine(layer0.AppConfigRoot, awsSecretsEnvJsonFilename),
                true);
        }

        // Instance scope
        builder.AddJsonFile(awsSecretsJsonFilename, true);
        builder.AddJsonFile(awsSecretsEnvJsonFilename, true);
    }

    private static void ConfigureLayer2(IConfigurationBuilder builder)
    {
        var layer1 = builder.Build().Get<Layer1>();
        layer1.ValidateAndThrow();


        if (layer1.AwsSecretConfiguration?.Enabled == true)
        {
            string[] secretNames =
            [
                $"{layer1.NihrAppName}",
                $"{layer1.NihrAppName}.{layer1.NihrEnvironment}"
            ];

            foreach (string secretName in secretNames)
            {
                var fullSecretName = $"{layer1.AwsSecretConfiguration.SecretNamePrefix}{secretName}";
                builder.Add(new AwsSecretConfigurationSource(layer1.AwsSso,
                    layer1.AwsSecretConfiguration.Region, fullSecretName));
            }
        }
    }

    private static void ConfigureLayer3(IConfigurationBuilder builder)
    {
        Layer2 layer2 = builder.Build().Get<Layer2>();
        layer2.ValidateAndThrow();

        string appSettingsEnvJsonFilename = $"appsettings.{layer2.NihrEnvironment}.json";

        if (layer2.HasConfigurationRoot)
        {
            // Machine scope
            builder.AddJsonFile(Path.Combine(layer2.NihrConfigurationRoot!, AppSettingsJsonFilename),
                true);
            builder.AddJsonFile(
                Path.Combine(layer2.NihrConfigurationRoot, appSettingsEnvJsonFilename), true);

            // Application scope
            builder.AddJsonFile(Path.Combine(layer2.AppConfigRoot!, AppSettingsJsonFilename), true);
            builder.AddJsonFile(Path.Combine(layer2.AppConfigRoot, appSettingsEnvJsonFilename), true);
        }

        // Instance scope
        builder.AddJsonFile(appSettingsEnvJsonFilename,
            true); // Allows non-secret env-specific settings to be persisted to source control.
        builder.AddJsonFile("appsettings.user.json",
            true); // Included for compatability with existing dev environments.
    }

    private static string GetAppNameByConvention()
    {
        var callingAssemblyName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
        return callingAssemblyName;
    }
}
