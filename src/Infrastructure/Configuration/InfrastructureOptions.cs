using Infrastructure.Services.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class InfrastructureOptions
{
    public static void AddInfrastructureOptions(this IServiceCollection services)
    {
        services.AddOptions<Jwt>().BindConfiguration(Jwt.JwtOptions);
        services.AddScoped<GenerateRefreshToken>();
        services.AddScoped<RevokeAllActiveRefreshTokens>();
    }
}
