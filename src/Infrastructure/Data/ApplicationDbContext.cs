using Core.Entities;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<Bro, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
    { }

    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();
    public DbSet<Place> Place => Set<Place>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>().Property(rt => rt.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Entity<RefreshToken>().Property(rt => rt.Token).HasMaxLength(128);
        builder.Entity<RefreshToken>().Property(rt => rt.ReplacedByToken).HasMaxLength(128);
        builder.Entity<RefreshToken>().HasIndex(rt => rt.Token).IsUnique();
        builder.Entity<RefreshToken>().HasIndex(rt => rt.ReplacedByToken).IsUnique();
        builder.Entity<RefreshToken>().HasIndex(rt => new { rt.Expires, rt.Revoked });
        builder.Entity<RefreshToken>().HasIndex(rt => rt.BroId).IsUnique(false);
        builder.Entity<Bro>().HasMany<RefreshToken>().WithOne().HasForeignKey(rt => rt.BroId).OnDelete(DeleteBehavior.Cascade);
    }
}