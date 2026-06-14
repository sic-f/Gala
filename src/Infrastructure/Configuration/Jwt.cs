namespace Infrastructure.Configuration;

public class Jwt
{
    public const string JwtOptions = "Jwt";

    public int TokenDays { get; set; }
    public int RefreshTokenDays { get; set; }
}
