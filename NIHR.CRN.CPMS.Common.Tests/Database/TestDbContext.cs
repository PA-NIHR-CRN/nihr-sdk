using Microsoft.EntityFrameworkCore;

namespace NIHR.CRN.CPMS.Common.Tests.Database;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfile { get; set; }
    public DbSet<UserClaimMembership> UserClaimMembership { get; set; }
    public DbSet<RefPerson> RefPerson { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>()
            .HasMany(e => e.UserClaimMembership)
            .WithOne(e => e.UserProfile)
            .HasForeignKey(e => e.UserProfileId)
            .HasPrincipalKey(e => e.Id);
        
        modelBuilder.Entity<RefPerson>()
            .HasMany(e => e.UserProfiles)
            .WithOne(e => e.Person)
            .HasForeignKey(e => e.RefPersonId)
            .HasPrincipalKey(e => e.Id);

        modelBuilder.Entity<RefPerson>()
            .HasOne(e => e.Acl)
            .WithMany(e => e.RefPeople)
            .HasForeignKey(e => e.AclId)
            .HasPrincipalKey(e => e.Id);
    }
}