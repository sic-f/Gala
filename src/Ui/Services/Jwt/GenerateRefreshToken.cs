using Core.Entities;
using Infrastructure.Data;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
namespace Ui.Services.Jwt;

public class GenerateRefreshToken(ApplicationDbContext context, IOptions<Infrastructure.Configuration.Jwt> options)
{
    public string Compute(Guid broId)
    {
        RefreshToken refreshToken = new()
        {
            Token = CreateRefreshToken(),
            Expires = DateTimeOffset.UtcNow.AddSeconds(options.Value.RefreshTokenDays),
            BroId = broId
        };

        context.RefreshToken.Add(refreshToken);
        context.SaveChanges();

        return refreshToken.Token;
    }

    private static string CreateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
