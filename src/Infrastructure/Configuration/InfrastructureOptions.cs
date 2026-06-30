using Infrastructure.Configuration.Storage;
using Infrastructure.Services.Images;
using Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class InfrastructureOptions
{
    public static void AddInfrastructureOptions(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddOptions<Jwt>().BindConfiguration(Jwt.JwtOptions);
        services.AddScoped<GenerateToken>();
        services.AddScoped<GenerateRefreshToken>();
        services.AddScoped<RevokeAllActiveRefreshTokens>();

        services.Configure<ImagesOptions>(ImagesOptions.Avatar, builder.Configuration.GetSection("Images:Avatar"));
        services.AddScoped<GetImagesPath>();
        services.AddScoped<AttachImages>();
        services.AddScoped<ValidateImageType>();
        services.AddScoped<UpdateAvatar>();
        services.AddScoped<DeleteAvatar>();
    }
}
