using Core.Interfaces;
using Infrastructure.Configuration.Storage.Options;
using Infrastructure.Services.Pictures;
using Infrastructure.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

public static class InfrastructureOptions
{
    public static void AddInfrastructureOptions(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddOptions<Jwt>().BindConfiguration(Jwt.JwtOptions);

        services.Configure<AvatarOptions>(AvatarOptions.Avatar, configuration.GetSection("Images:Avatar"));
        services.Configure<PlaceImageOptions>(PlaceImageOptions.Place, configuration.GetSection("Images:Place"));
        services.AddScoped<IPicturesAttacher, AttachPictures>();

        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IPlacePictureRepository, PlacePictureRepository>();
        services.AddScoped<ISavedPlaceRepository, SavedPlaceRepository>();
    }
}
