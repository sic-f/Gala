using Infrastructure.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ui.Services.Jwt;

public class GenerateToken(IOptions<Configuration.Jwt> jwtOptions)
{
    private const string Issuer = "Gala";
    private const string Audience = "Lakwatsa";
    private Bro _bro = null!;

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

    public string Compute(Bro bro)
    {
        _bro = bro;
        return new JsonWebTokenHandler().CreateToken(TokenDescriptor());
    }

    private SecurityTokenDescriptor TokenDescriptor()
    {
        return new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Claims = Claims(),
            Expires = DateTime.Now.AddSeconds(jwtOptions.Value.TokenDays),
            SigningCredentials = SigningCredentials()
        };
    }

    private Dictionary<string, object> Claims()
    {
        return new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = _bro.Id,
            [JwtRegisteredClaimNames.Email] = _bro.Email!,
            [JwtRegisteredClaimNames.GivenName] = FirstName(),
            [JwtRegisteredClaimNames.FamilyName] = LastName(),
            [JwtRegisteredClaimNames.Picture] = Avatar(),
            [JwtRegisteredClaimNames.Iat] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString(),
            [JwtRegisteredClaimNames.Typ] = "access",
        };
    }

    private string FirstName() => _bro.FirstName ?? string.Empty;
    private string LastName() => _bro.LastName ?? string.Empty;
    private string Avatar() => _bro.Avatar?.Normal ?? string.Empty;
}