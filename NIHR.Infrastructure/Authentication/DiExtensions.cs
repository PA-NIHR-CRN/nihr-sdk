using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIHR.Infrastructure.Settings;

namespace NIHR.Infrastructure.Authentication
{
    public static class DiExtensions
    {
        public static void AddAuthBypassSettings(this IHostApplicationBuilder builder, string sectionName = "AuthenticationBypass")
        {
            builder.Services.Configure<AuthenticationBypassSettings>(builder.Configuration.GetSection(sectionName));
        }
    }
}