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
    public DbSet<AccountAvatar> AccountAvatar => Set<AccountAvatar>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}