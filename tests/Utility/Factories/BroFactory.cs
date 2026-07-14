using Core.Dto;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Utility.Helpers;

namespace Utility.Factories;

public class BroFactory
{
    /// <summary>
    /// Creates Bro quickly with pre-defined field values, just supply a service provider.
    /// </summary>
    /// <param name="services">The service provider (IServiceProvider) to create a scope for the database context.</param>
    /// <returns>The created Bro.</returns>
    public static async Task<Bro> CreateAsync(IServiceProvider services)
    {
        var email = "test@example.me";
        Bro bro = new() { UserName = email, Email = email };

        await CreateBroAsync(bro, password: "Hey@123", services);

        return await RepositoryHelper.GetUserBy(email, services);
    }

    /// <summary>
    /// Creates Bro with email and password as arguments, and a service provider.
    /// </summary>
    /// <param name="email">Email of Bro to be created.</param>
    /// <param name="password">Password for the Bro account to be created.</param>
    /// <param name="services">The service provider (IServiceProvider) to create a scope for the database context.</param>
    /// <returns>The created Bro.</returns>
    public static async Task<Bro> CreateBroAsync(string email, string password, IServiceProvider services)
    {
        Bro bro = new() { UserName = email, Email = email };

        await CreateBroAsync(bro, password, services);

        return await RepositoryHelper.GetUserBy(email, services);
    }

    /// <summary>
    /// Creates Bro with BroWithFullName dto as argument, and a service provider.
    /// </summary>
    /// <param name="broDetails">BroWithFullName dto to supply details for creating Bro.</param>
    /// <param name="services">The service provider (IServiceProvider) to create a scope for the database context.</param>
    /// <returns>The created Bro.</returns>
    public static async Task<Bro> CreateBroAsync(BroWithFullName broDetails, IServiceProvider services)
    {
        Bro bro = new() { UserName = broDetails.Email, Email = broDetails.Email, FirstName = broDetails.FirstName, LastName = broDetails.LastName };

        await CreateBroAsync(bro, broDetails.Password, services);

        return await RepositoryHelper.GetUserBy(broDetails.Email, services);
    }

    /// <summary>
    /// Creates Bro with BroWithFullName dto as argument, and a database context.
    /// </summary>
    /// <param name="broDetails">BroWithFullName dto to supply details for creating Bro.</param>
    /// <param name="context">The database context (ApplicationDbContext) to use for persisting to the database.</param>
    /// <returns>The created Bro.</returns>
    public static async Task<Bro> CreateBroAsync(BroWithFullName broDetails, ApplicationDbContext context)
    {
        Bro bro = new() { UserName = broDetails.Email, Email = broDetails.Email, FirstName = broDetails.FirstName, LastName = broDetails.LastName };

        await CreateBroAsync(bro, context);

        return await RepositoryHelper.GetUserBy(broDetails.Email, context);
    }

    /// <summary>
    /// Creates Bro with a Bro unpersisted instance and a password as arguments, and a service provider.
    /// </summary>
    /// <param name="bro">A newed-up Bro.</param>
    /// <param name="password">Password for the Bro account to be created.</param>
    /// <param name="services">The service provider (IServiceProvider) to create a scope for the database context.</param>
    /// <returns>void</returns>
    /// <exception cref="Exception"></exception>
    private static async Task CreateBroAsync(Bro bro, string password, IServiceProvider services)
    {
        using var scope = services.CreateScope();
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