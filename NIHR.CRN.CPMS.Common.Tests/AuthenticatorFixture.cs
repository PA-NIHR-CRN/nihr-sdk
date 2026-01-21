using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NIHR.CRN.CPMS.Abstractions;
using NIHR.CRN.CPMS.Common.Tests.Database;

namespace NIHR.CRN.CPMS.Common.Tests;

public class AuthenticatorFixture : IDisposable
{
    public const string EnvDevelopment = "Development";
    public const string EnvProduction = "Production";
    
    public TestDbContext Context { get; private set; }

    public FakeLogCollector Log { get; } = new FakeLogCollector();
    
    private TestUserStore _userStore;
    private MemoryCache _memoryCache;
    public CpmsUserProfileManager<UserProfile, RefPerson, UserClaimMembership> UserProfileManager { get; private set; }
    private SqliteConnection _connection;
    

    public AuthenticatorFixture(AuthenticationBypassSettings bypassSettings, string envName)
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        FakeLogger<CpmsUserProfileManager<UserProfile, RefPerson, UserClaimMembership>> logger = new(Log); 
        
        var contextOptions = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        
        Context = new TestDbContext(contextOptions);
        
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
        
        _userStore = new TestUserStore(Context);
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        
        UserProfileManager =
            new CpmsUserProfileManager<UserProfile, RefPerson, UserClaimMembership>(
                _userStore,
                Options.Create(bypassSettings), _memoryCache, logger,
                new TestHostEnvironment(envName, "CPMS", string.Empty, null),
                new FakeTimeProvider(new DateTimeOffset(2024, 15, 03, 10, 19, 45, TimeSpan.Zero)));
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
        _connection.Dispose();
        Context.Dispose();
    }
}