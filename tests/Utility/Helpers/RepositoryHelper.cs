using Core.PlaceAggregate;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Utility.Helpers;

public class RepositoryHelper
{
    public static T GetFirstRecord<T>(ApplicationFactory<Program> factory) where T : class
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        return context.Set<T>().First();
    }

    public static async Task<Bro> GetUserBy(string email, ApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var userManager = scopedServices.GetRequiredService<UserManager<Bro>>();

        return (await userManager.FindByEmailAsync(email))!;
    }

    public static async Task<Bro> GetUserBy(string email, ApplicationDbContext context)
    {
        return context.Users.FirstOrDefault<Bro>(bro => bro.Email == email)!;
    }

    public static async Task<Place> GetPlaceByIdAsync(Guid Id, ApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        return await context.Place.FirstAsync(p => p.Id == Id);
    }

    public static async Task<Place> GetPlaceByIdAsync(Guid Id, ApplicationDbContext context)
    {
        return await context.Place.FirstAsync(p => p.Id == Id);
    }

    public static async Task<PlaceProfilePicture> GetPlacePictureByIdAsync(Guid Id, ApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        return await context.PlaceProfilePicture.FirstAsync(p => p.Id == Id);
    }
}