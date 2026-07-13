using Core.Entities;
using Core.PlaceAggregate;
using Core.SavedAggregate;
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
    public DbSet<PlacePicture> PlacePicture => Set<PlacePicture>();
    public DbSet<PlaceProfilePicture> PlaceProfilePicture => Set<PlaceProfilePicture>();
    public DbSet<PlaceExtraPicture> PlaceExtraPicture => Set<PlaceExtraPicture>();
    public DbSet<Saved> Saved => Set<Saved>();
    public DbSet<SavedPlace> SavedPlace => Set<SavedPlace>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}