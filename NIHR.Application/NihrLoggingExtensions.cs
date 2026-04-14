using System;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NIHR.Application.Options;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.AwsCloudWatch;

namespace NIHR.Application;

public static class NihrLoggingExtensions
{
    public static void AddNihrLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        Serilog.Debugging.SelfLog.Enable(Console.Error);

        var loggingConfiguration = new LoggerConfiguration();
        loggingConfiguration.ReadFrom.Configuration(configuration);
        loggingConfiguration.WriteTo.NihrCloudWatch(configuration);
        builder.AddSerilog(loggingConfiguration.CreateLogger());
    }

    public static void NihrCloudWatch(this LoggerSinkConfiguration loggingConfig, IConfiguration config)
    {
        var awsCloudWatchOptions = config.GetSection("AwsCloudWatch").Get<AwsCloudWatchOptions>();
        var awsSsoOptions = config.GetSection("AwsSso").Get<AwsSsoOptions>();

        if (string.IsNullOrWhiteSpace(awsCloudWatchOptions.LogGroupName))
        {
            Console.Error.WriteLine(
                "Warning: AWS cloudwatch logging disabled. Configure 'CloudWatch:LogGroupName' to enable.");
        }
        else
        {
            AmazonCloudWatchLogsClient client;

            if (!string.IsNullOrWhiteSpace(awsSsoOptions.AccountId))
            {
                var regionEndpoint = Amazon.RegionEndpoint.EUWest1;
                if (!string.IsNullOrWhiteSpace(awsCloudWatchOptions.Region))
                {
                    regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsCloudWatchOptions.Region);
                }

                client = new AmazonCloudWatchLogsClient(new SSOAWSCredentials(awsSsoOptions.AccountId,
                    awsSsoOptions.Region, awsSsoOptions.RoleName, awsSsoOptions.StartUrl), regionEndpoint);
            }
            else
            {
                client = new AmazonCloudWatchLogsClient();
            }

            loggingConfig.AmazonCloudWatch(awsCloudWatchOptions.LogGroupName,
                logStreamPrefix: DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"),
                restrictedToMinimumLevel: awsCloudWatchOptions.MinimumLogEventLevel,
                cloudWatchClient: client);
        }
    }
}
