using Core.Interfaces;
using Core.Services;
using Infrastructure.Configuration.Storage.Options;
using Infrastructure.Services.Images;
using Infrastructure.Services.Jwt;
using Infrastructure.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class InfrastructureOptions
{
    public static void AddInfrastructureOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOptions<Jwt>().BindConfiguration(Jwt.JwtOptions);
        services.AddScoped<GenerateToken>();
        services.AddScoped<GenerateRefreshToken>();
        services.AddScoped<RevokeAllActiveRefreshTokens>();

        services.Configure<AvatarOptions>(AvatarOptions.Avatar, configuration.GetSection("Images:Avatar"));
        services.Configure<PlaceImageOptions>(PlaceImageOptions.Place, configuration.GetSection("Images:Place"));
        services.AddScoped<IPicturesAttacher, AttachPictures>();
        services.AddScoped<ValidateImageType>();
        services.AddScoped<UpdateImage>();
        services.AddScoped<DeleteImage>();
        services.AddScoped<ParseToPictureUploads>();
        services.AddScoped<IGetImagePaths, GetImagePaths>();

        services.AddScoped<IPlaceService, PlaceService>();
    }
}
