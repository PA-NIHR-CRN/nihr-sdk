using Microsoft.AspNetCore.Builder;
using NIHR.Application;

namespace NIHR.Infrastructure.AspNetCore;

public static class NihrWebHostBuilderFactoryExtensions
{
    public static WebApplicationBuilder WebApplicationBuilder(this NihrHostBuilderFactory factory, string[] args, string? appName = null)
    {
        var result = WebApplication.CreateBuilder(args);
        factory.Configure(result, appName);
        return result;
    }
}