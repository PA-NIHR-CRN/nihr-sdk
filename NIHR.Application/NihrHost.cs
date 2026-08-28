using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NIHR.Application;

public static class NihrHost
{
    public static NihrHostBuilderFactory Create => new();
}

public class NihrHostBuilderFactory
{
    public HostApplicationBuilder ApplicationBuilder(string[]? args = null, string appName = null)
    {
        var result = Host.CreateApplicationBuilder(args);
        Configure(result, appName);
        return result;
    }

    public void Configure(IHostApplicationBuilder result, string appName)
    {
        // The Web Application builder relies on initial configuration values to configure Kestrel - so 
        // we cannot clear the existing configuration, just append to it.
        result.Configuration.AddNihrConfiguration(appName);
        result.Logging.ClearProviders();
        result.Logging.AddNihrLogging(result.Configuration);
    }
}