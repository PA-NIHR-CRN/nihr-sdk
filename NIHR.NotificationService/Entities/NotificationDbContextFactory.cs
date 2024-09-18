using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NIHR.NotificationService.Settings;

namespace NIHR.NotificationService.Context;

public class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
{
    public NotificationDbContext CreateDbContext(string[] args)
    {
        // TODO: make this more consistent. Base factory in NIHR.Infrastructure.EntityFrameworkCore.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.user.json", optional: true)
            .Build();

        var dbSettings = configuration.GetSection(DbSettings.SectionName).Get<DbSettings>();
        var connectionString = DbSettings.BuildConnectionString(dbSettings) ??
                               Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

        if (connectionString is null)
        {
            throw new ArgumentNullException(nameof(connectionString), "Database connection string not configured.");
        }

        var options = new DbContextOptionsBuilder<NotificationDbContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), x =>
            {
                x.CommandTimeout(300);
            })
            .Options;


        return new NotificationDbContext(options);
    }
}
