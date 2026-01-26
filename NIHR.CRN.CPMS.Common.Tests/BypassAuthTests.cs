using FluentAssertions;
using Microsoft.Extensions.Logging;
using NIHR.CRN.CPMS.Common.Tests.Database;

namespace NIHR.CRN.CPMS.Common.Tests;

public class BypassAuthTests
{
    const string bypassEmail = "bypass-user@test.com";
    const string bypassUuid = "8FC095FE-6427-447E-8AE5-1F24A287D57B";
    
    const string email = "alice@test.com";
    const string firstName = "Alice";
    const string lastName = "Allison";
    const string orcId = "123456";
    const string uuid = "FE5329D2-2A95-484C-BF7E-3B8CA8F8444E";
    
    [Test]
    public async Task BypassOnProduction()
    {
        using var fixture = new AuthenticatorFixture(new(){Bypass = true, BypassEmail = bypassEmail}, AuthenticatorFixture.EnvProduction);
        
        var result = await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        fixture.Log.GetSnapshot().Count(i => i.Level == LogLevel.Warning).Should().Be(1);

        result.EmailId.Should().Be(email);
        result.UserId.Should().Be(uuid);
    }
    
    [Test]
    public async Task BypassOnDevForNewEmail()
    {
        using var fixture = new AuthenticatorFixture(new(){Bypass = true, BypassEmail = bypassEmail}, AuthenticatorFixture.EnvDevelopment);

        const string email = "alice@test.com";
        const string firstName = "Alice";
        const string lastName = "Allison";
        const string orcId = "123456";
        const string uuid = "FE5329D2-2A95-484C-BF7E-3B8CA8F8444E";
        
        var result = await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        fixture.Log.GetSnapshot().Count(i => i.Level == LogLevel.Warning).Should().Be(0);

        result.EmailId.Should().Be(bypassEmail);
        result.UserId.Should().BeNull();
    }
    
    [Test]
    public async Task BypassOnDevForExistingEmail()
    {
        using var fixture = new AuthenticatorFixture(new(){Bypass = true, BypassEmail = bypassEmail}, AuthenticatorFixture.EnvDevelopment);
   
        const string email = "alice@test.com";
        const string firstName = "Alice";
        const string lastName = "Allison";
        const string orcId = "123456";
        const string uuid = "FE5329D2-2A95-484C-BF7E-3B8CA8F8444E";
        
        fixture.Context.UserProfile.Add(new()
        {
            EmailId = bypassEmail,
            UserId =  bypassUuid,
            Person = new RefPerson()
            {
                FirstName = "",
                LastName = "",
                OrcId = "",
                Email = bypassEmail,
                Acl = new()
            }
        });

        fixture.Context.SaveChanges();
        
        var result = await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        fixture.Log.GetSnapshot().Count(i => i.Level == LogLevel.Warning).Should().Be(0);
        result.EmailId.Should().Be(bypassEmail);
        result.UserId.Should().Be(bypassUuid);
    }
}