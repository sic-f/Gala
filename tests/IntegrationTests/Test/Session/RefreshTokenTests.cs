using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Identity;
using IntegrationTests.Scopes;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Security.Cryptography;
using Utility.Factories;
using static Core.Dto.Session;

namespace IntegrationTests.Test.Session;

[Collection("Application Factory Collection")]
public class RefreshTokenTests(SessionScope app) : BaseTest(app, isAuthenticated: false), IClassFixture<SessionScope>
{
    public override Task DisposeAsync() => Task.Run(() => app.ClearToken());

    [Fact]
    public async Task RefreshTokenTestAsync()
    {
        Bro bro = await BroFactory.CreateAsync("token@example.net", "Hey@123", app.Services);
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        RefreshToken refreshToken = new()
        {
            Token = token,
            Expires = DateTimeOffset.UtcNow.AddSeconds(60),
            BroId = bro.Id,
        };

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        context.RefreshToken.Add(refreshToken);
        await context.SaveChangesAsync();

        await app.RefreshTokenAsync(token);
        RefreshTokenResponse? response = await app.RefreshTokenResponseAsync();

        Assert.Equal(HttpStatusCode.OK, app.ResponseStatusCode);
        Assert.NotNull(response?.RefreshToken);
        Assert.NotNull(response.Token);
    }

    [Fact]
    public async Task RevokeExistingRefreshTokensWhenCurrentIsStolenTestAsync()
    {
        Bro bro = await BroFactory.CreateAsync("token@example.net", "Hey@123", app.Services);
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        RefreshToken refreshToken = new()
        {
            Token = token,
            Expires = DateTimeOffset.UtcNow.AddDays(-3),
            BroId = bro.Id,
            Revoked = DateTimeOffset.UtcNow.AddDays(-3)
        };

        IEnumerable<RefreshToken> multipleRefreshTokens = BuildMultipleRefreshTokens(bro.Id);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        context.RefreshToken.Add(refreshToken);
        context.RefreshToken.AddRange(multipleRefreshTokens);
        await context.SaveChangesAsync();

        await app.RefreshTokenAsync(token);
        RefreshTokenResponse? response = await app.RefreshTokenResponseAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, app.ResponseStatusCode);
        Assert.Null(response?.RefreshToken);
        Assert.Null(response?.Token);
    }

    private static IEnumerable<RefreshToken> BuildMultipleRefreshTokens(Guid broId)
    {
        return
        [
            new ()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTimeOffset.UtcNow.AddDays(-5),
                BroId = broId
            },
            new ()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTimeOffset.UtcNow.AddDays(-5),
                BroId = broId
            },
            new ()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTimeOffset.UtcNow.AddDays(-5),
                BroId = broId
            },
            new ()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTimeOffset.UtcNow.AddDays(-5),
                BroId = broId
            }
        ];
    }
}
