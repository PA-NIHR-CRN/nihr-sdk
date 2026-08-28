using Serilog.Events;

namespace NIHR.Application.Options;

internal class AwsCloudWatchOptions
{
    public string Region { get; set; }
    public string LogGroupName { get; set; }

    public LogEventLevel MinimumLogEventLevel { get; set; } = LogEventLevel.Warning;
}
