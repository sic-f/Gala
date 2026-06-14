using Core.Entities;
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>().Property(rf => rf.CreatedAt).HasDefaultValueSql<DateTimeOffset>("GETUTCDATE()");
        builder.Entity<RefreshToken>().Property(rf => rf.Token).HasMaxLength(128);
        builder.Entity<RefreshToken>().Property(rf => rf.ReplacedByToken).HasMaxLength(128);
        builder.Entity<RefreshToken>().HasIndex(rf => rf.Token).IsUnique();
        builder.Entity<RefreshToken>().HasIndex(rf => rf.ReplacedByToken).IsUnique();
        builder.Entity<RefreshToken>().HasIndex(rf => new { rf.Expires, rf.Revoked });
        builder.Entity<RefreshToken>().HasIndex(rt => rt.BroId).IsUnique(false);
        builder.Entity<Bro>().HasMany<RefreshToken>().WithOne().HasForeignKey(rt => rt.BroId).OnDelete(DeleteBehavior.Cascade);
    }
}