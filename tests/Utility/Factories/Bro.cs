using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utility.Helpers;
using static Core.Dto.Bro;

namespace Utility.Factories;

public class BroFactory
{
    public static async Task<Bro> CreateBroAsync(string email, string password, ApplicationFactory<Program> factory)
    {
        Bro bro = new() { UserName = email, Email = email };

        await CreateBroAsync(bro, password, factory);

        return await RepositoryHelper.GetUserBy(email, factory);
    }

    public static async Task<Bro> CreateBroAsync(BroWithFullName broDetails, ApplicationFactory<Program> factory)
    {
        Bro bro = new() { UserName = broDetails.Email, Email = broDetails.Email, FirstName = broDetails.FirstName, LastName = broDetails.LastName };

        await CreateBroAsync(bro, broDetails.Password, factory);

        return await RepositoryHelper.GetUserBy(broDetails.Email, factory);
    }

    public static async Task<Bro> CreateBroAsync(BroWithFullName broDetails, ApplicationDbContext context)
    {
        Bro bro = new() { UserName = broDetails.Email, Email = broDetails.Email, FirstName = broDetails.FirstName, LastName = broDetails.LastName };

        await CreateBroAsync(bro, context);

        return await RepositoryHelper.GetUserBy(broDetails.Email, context);
    }

    private static async Task CreateBroAsync(Bro bro, string password, ApplicationFactory<Program> factory)
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var userManager = scopedServices.GetRequiredService<UserManager<Bro>>();

        var result = await userManager.CreateAsync(bro, password);

        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.First().Description);
        }
    }

    private static async Task CreateBroAsync(Bro bro, ApplicationDbContext context)
    {
        await context.Users.AddAsync(bro);
        await context.SaveChangesAsync();
    }
}