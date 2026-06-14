using Core.Entities;
using Infrastructure.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Jwt;

public class Jwt(Bro bro)
{
    private const string Issuer = "Gala";
    private const string Audience = "Lakwatsa";

    public static string GenerateToken(Bro For)
    {
        return new Jwt(For).Token();
    }

    //public static string GenerateRefreshToken()
    //{
    //    string refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    //}

    private static SigningCredentials SigningCredentials()
    {
        return new SigningCredentials(SecretKey(), SecurityAlgorithms.HmacSha256);
    }

    private static SymmetricSecurityKey SecretKey()
    {
        return new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                "1f354b830d17f2cb7a01dedf8933b58a")
        );
    }

    private string Token()
    {
        return new JsonWebTokenHandler().CreateToken(TokenDescriptor());
    }

    private SecurityTokenDescriptor TokenDescriptor()
    {
        return new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Claims = Claims(),
            Expires = DateTime.Now.AddSeconds(20),
            SigningCredentials = SigningCredentials()
        };
    }

    private Dictionary<string, object> Claims()
    {
        return new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = bro.Id,
            [JwtRegisteredClaimNames.Email] = bro.Email!,
            [JwtRegisteredClaimNames.GivenName] = FirstName(),
            [JwtRegisteredClaimNames.FamilyName] = LastName(),
            [JwtRegisteredClaimNames.Iat] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [JwtRegisteredClaimNames.Typ] = "access",
        };
    }

    private string FirstName() => bro.FirstName ?? string.Empty;
    private string LastName() => bro.LastName ?? string.Empty;
}