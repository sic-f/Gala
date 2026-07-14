using Context;
using Ui.Services.Jwt;
using Ui.Services.Pictures;

namespace Ui;

public static class ServicesRegistration
{
    public static void AddContextServices(this IServiceCollection services)
    {
        services.AddScoped<GenerateToken>();
        services.AddScoped<GenerateRefreshToken>();
        services.AddScoped<RevokeAllActiveRefreshTokens>();

        services.AddScoped<ValidateImageType>();
        services.AddScoped<ParseToPictureUploads>();

        services.AddScoped<Contribution>();
        services.AddScoped<PlacePicture>();
        services.AddScoped<Saved>();
    }
}
