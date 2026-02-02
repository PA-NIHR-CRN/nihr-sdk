using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NIHR.CRN.CPMS.Common.Tests.Database;

namespace NIHR.CRN.CPMS.Common.Tests;

public class ProductionAuthTests()
{
    [Test]
    public async Task NewEmailAndUuid()
    {
        using var fixture =  new AuthenticatorFixture(new(), AuthenticatorFixture.EnvProduction);
        
        const string email = "alice@test.com";
        const string firstName = "Alice";
        const string lastName = "Allison";
        const string orcId = "123456";
        const string uuid = "FE5329D2-2A95-484C-BF7E-3B8CA8F8444E";
        
        await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        Assert.That(fixture.Context.RefPerson.Count(i => i.Email == email), Is.EqualTo(1));

        var userProfile = AssertHasExactlyOneUserProfileAndRefPerson(fixture.Context, uuid, email);
        AssertHasOnlyPublicUser(userProfile);
        Assert.That(userProfile.Person.FirstName, Is.EqualTo(firstName));
        Assert.That(userProfile.Person.LastName, Is.EqualTo(lastName));
        Assert.That(userProfile.Person.Email, Is.EqualTo(email));
        Assert.That(userProfile.Person.OrcId, Is.EqualTo(orcId));
    }
    
    [Test]
    public async Task PublicUserAddedForEmailWithNoClaims()
    {
        using var fixture =  new AuthenticatorFixture(new(), AuthenticatorFixture.EnvProduction);

        const string email = "Belinda@test.com";
        const string uuid = "52AC495C-948F-4207-A0DB-33D96E1B9F8D";

        fixture.Context.UserProfile.Add(new()
        {
            EmailId = email,
            Person = new RefPerson()
            {
                Email = email,
                Acl = new()
            }
        });
        
        await fixture.Context.SaveChangesAsync();

        await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(
            email, uuid, "Belinda", "Bosworth", "123456");
        
        Assert.That(fixture.Context.RefPerson.Count(i => i.Email == email), Is.EqualTo(1));

        var userProfile = AssertHasExactlyOneUserProfileAndRefPerson(fixture.Context, uuid, email);
        AssertHasOnlyPublicUser(userProfile);
    }
    
    [Test]
    public async Task PublicUserNotAddedForEmailWithClaims()
    {
        using var fixture =  new AuthenticatorFixture(new(), AuthenticatorFixture.EnvProduction);

        const string email = "Callie@test.com";
        const string uuid = "EE43AEAF-2023-4305-87B3-B4D796E73566";

        fixture.Context.UserProfile.Add(new()
        {
            EmailId = email,
            Person = new RefPerson()
            {
                Email = email,
                Acl = new()
            },
            UserClaimMembership =
            {
                new UserClaimMembership()
                {
                    ClaimTypeId = (long)ClaimTypes.TriageOfficer
                }
            }
        });
        
        await fixture.Context.SaveChangesAsync();

        await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, "Callie", "Cortez", "345231");
        
        Assert.That(fixture.Context.RefPerson.Count(i => i.Email == email), Is.EqualTo(1));

        var userProfile = AssertHasExactlyOneUserProfileAndRefPerson(fixture.Context, uuid, email);
        // Assert that the user does NOT have public user
        Assert.That(userProfile.UserClaimMembership.All(i => i.ClaimTypeId != (long)ClaimTypes.PublicUser));
    }
    
    [Test]
    public async Task DetailsAreUpdatedForExistingEmail()
    {
        using var fixture =  new AuthenticatorFixture(new(), AuthenticatorFixture.EnvProduction);

        const string email = "daria@test.com";
        const string firstName = "Daria";
        const string lastName = "Davis";
        const string orcId = "78901";
        const string uuid = "47061399-8B55-4950-88AC-58697E438759";

        fixture.Context.UserProfile.Add(new()
        {
            EmailId = email,
            Person = new RefPerson()
            {
                FirstName = "oldValue",
                LastName = "oldValue",
                OrcId = "oldValue",
                Email = email,
                Acl = new()
            },
            UserClaimMembership =
            {
                new UserClaimMembership()
                {
                    ClaimTypeId = (long)ClaimTypes.TriageOfficer
                }
            }
        });
        
        await fixture.Context.SaveChangesAsync();
        
        await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        var userProfile = AssertHasExactlyOneUserProfileAndRefPerson(fixture.Context, uuid, email);
        
        Assert.That(userProfile.UserId, Is.EqualTo(uuid));
        Assert.That(userProfile.Person.FirstName, Is.EqualTo(firstName));
        Assert.That(userProfile.Person.LastName, Is.EqualTo(lastName));
        Assert.That(userProfile.Person.Email, Is.EqualTo(email));
        Assert.That(userProfile.Person.OrcId, Is.EqualTo(orcId));
    }
    
    [Test]
    public async Task DetailsAreUpdatedForExistingUuid()
    {
        using var fixture =  new AuthenticatorFixture(new(), AuthenticatorFixture.EnvProduction);

        const string oldEmail = "erin-old-email@test.com";
        const string email = "erin@test.com";
        const string firstName = "Erin";
        const string lastName = "Evis";
        const string orcId = "69375";
        const string uuid = "C5CBA0F4-D4E5-4DF6-A255-1159EB93FF42";

        fixture.Context.UserProfile.Add(new()
        {
            EmailId = email,
            UserId =  uuid,
            Person = new RefPerson()
            {
                FirstName = "oldFirstName",
                LastName = "oldLastName",
                OrcId = "oldOrcId",
                Email = oldEmail,
                Acl = new()
            },
            UserClaimMembership =
            {
                new UserClaimMembership()
                {
                    ClaimTypeId = (long)ClaimTypes.TriageOfficer
                }
            }
        });
        
        await fixture.Context.SaveChangesAsync();
        
        var result = await fixture.UserProfileManager.FetchAndUpdateUserProfileAsync(email, uuid, firstName, lastName, orcId);
        
        AssertEmailDoesNotExist(fixture.Context, oldEmail);
        var userProfile = AssertHasExactlyOneUserProfileAndRefPerson(fixture.Context, uuid, email);
        
        Assert.That(userProfile.Person.FirstName, Is.EqualTo(firstName));
        Assert.That(userProfile.Person.LastName, Is.EqualTo(lastName));
        Assert.That(userProfile.Person.Email, Is.EqualTo(email));
        Assert.That(userProfile.Person.OrcId, Is.EqualTo(orcId));
        
        result.Should().BeEquivalentTo(userProfile);
    }
    
    //TODO: Test caching
    
    private void AssertEmailDoesNotExist(TestDbContext context, string email)
    {
        context.UserProfile.Where(i => i.EmailId == email).Should().BeEmpty();
        context.RefPerson.Where(i => i.Email == email).Should().BeEmpty();
    }
    
    private UserProfile AssertHasExactlyOneUserProfileAndRefPerson(TestDbContext context, string uuid, string email)
    {
        context.UserProfile.Where(i => i.UserId == uuid).Should().ContainSingle();
        context.UserProfile.Where(i => i.EmailId == email).Should().ContainSingle();
        context.RefPerson.Where(i => i.Email == email).Should().ContainSingle();
        
        var userProfile = context.UserProfile
            .Include(i => i.UserClaimMembership)
            .Include(i => i.Person)
            .Single(i => i.UserId == uuid);
        
        userProfile.EmailId.Should().Be(email);
        userProfile.Person.Email.Should().Be(email);
        
        return userProfile;
    }
    
    static void AssertHasPublicUser(UserProfile userProfile)
    {
        Assert.That(userProfile.UserClaimMembership.Any(i => i.ClaimTypeId == (long)ClaimTypes.PublicUser));
    }
    
    static void AssertHasOnlyPublicUser(UserProfile userProfile)
    {
        AssertHasPublicUser(userProfile);
        Assert.That(userProfile.UserClaimMembership.Count(), Is.EqualTo(1));
    }
}