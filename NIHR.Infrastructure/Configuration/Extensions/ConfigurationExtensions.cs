using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Amazon;
using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NIHR.Infrastructure.Configuration;
using NIHR.Infrastructure.Settings;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConfigurationExtensions
    {

        //TODO chat with chris about this global exception handling
        public static IServiceCollection ConfigureNihrLogging(this IServiceCollection services,
    IConfiguration configuration)
        {
            var loggerOptions = new LambdaLoggerOptions
            {
                IncludeCategory = true,
                IncludeLogLevel = true,
                IncludeNewline = true,
                IncludeEventId = true,
                IncludeException = true,
                IncludeScopes = true,
            };

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));

                if (IsRunningInLambda())
                {
                    loggingBuilder.AddLambdaLogger(loggerOptions);
                }

                if (Debugger.IsAttached || Environment.UserInteractive)
                {
                    loggingBuilder.AddConsole().AddDebug();
                }
            });

            return services;
        }

        public static IHostApplicationBuilder ConfigureNihrLogging(this IHostApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var services = builder.Services;

            services.ConfigureNihrLogging(configuration);

            return builder;
        }

        private static bool IsRunningInLambda()
        {
            var executionEnv = Environment.GetEnvironmentVariable("AWS_EXECUTION_ENV");
            return !string.IsNullOrEmpty(executionEnv) && executionEnv.StartsWith("AWS_Lambda_");
        }
        public static IConfigurationBuilder AddNihrConfiguration(this IConfigurationBuilder configuration,
IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment.IsDevelopment())
            {
                configuration.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.user.json", optional: true, reloadOnChange: true);
            }

            return configuration;
        }

        public static IConfigurationManager AddNihrConfiguration(this IConfigurationManager configuration, IServiceCollection services,
IHostEnvironment hostEnvironment)
        {

            AddNihrConfiguration(configuration, hostEnvironment);

            var secretsManagerSettings = services.GetSectionAndValidate<AwsSecretsManagerSettings>(configuration).Value;
            if (secretsManagerSettings.Enabled)
            {
                configuration.AddAwsSecretsManager(secretsManagerSettings.SecretName,
                    () => new AmazonSecretsManagerClient(
                        RegionEndpoint.GetBySystemName(secretsManagerSettings.Region)));
            }

            return configuration;
        }

        public static IHostApplicationBuilder AddNihrConfiguration(this IHostApplicationBuilder builder)
        {
            var hostEnvironment = builder.Environment;
            var configuration = builder.Configuration;
            var services = builder.Services;

            configuration.AddNihrConfiguration(services, hostEnvironment);

            return builder;
        }

        private static void AddAwsSecretsManager(this IConfigurationBuilder configurationBuilder, string secretName,
            Func<IAmazonSecretsManager> secretsManagerClientFactory)
        {
            var configurationSource = new AwsSecretsManagerConfigurationSource(secretName, secretsManagerClientFactory);
            configurationBuilder.Add(configurationSource);
        }

        public static IOptions<T> GetSectionAndValidate<T>(this IHostApplicationBuilder builder
            ) where T : class, new() => builder.Services.GetSectionAndValidate<T>(builder.Configuration);

        public static IOptions<T> GetSectionAndValidate<T>(this IServiceCollection services,
            IConfiguration configuration) where T : class, new()
        {
            var sectionName = typeof(T).Name;
            var sectionNameProperty = typeof(T).GetProperty("SectionName");
            if (sectionNameProperty != null)
            {
                var instance = Activator.CreateInstance<T>();
                sectionName = sectionNameProperty.GetValue(instance) as string;
                if (string.IsNullOrWhiteSpace(sectionName))
                {
                    throw new InvalidOperationException(
                        $"The 'SectionName' property in {typeof(T).Name} cannot be null or whitespace.");
                }
            }

            var settings = configuration.GetSection(sectionName).Get<T>();

            if (settings == null)
            {
                settings = BindFlatConfigurationKeys<T>(configuration, sectionName);
            }

            if (settings is IValidatableObject validatable)
            {
                var validationContext = new ValidationContext(settings);
                var validationResult = validatable.Validate(validationContext);

                if (validationResult.Any())
                {
                    throw new OptionsValidationException(string.Empty, typeof(T), validationResult.Select(x => x.ErrorMessage));
                }
            }

            services.AddOptions<T>()
                .BindConfiguration(sectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();


            return Options.Options.Create(settings);
        }

        private static T BindFlatConfigurationKeys<T>(IConfiguration configuration, string sectionName) where T : class, new()
        {
            var instance = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var key = $"{sectionName}__{property.Name}";
                var value = configuration[key];
                if (value != null)
                {
                    property.SetValue(instance, Convert.ChangeType(value, property.PropertyType));
                }
            }

            return instance;
        }
    }
}
