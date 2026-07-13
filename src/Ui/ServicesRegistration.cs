using Context;

namespace Ui;

public static class ServicesRegistration
{
    public static void AddContextServices(this IServiceCollection services)
    {
        services.AddScoped<Contribution>();
        services.AddScoped<PlacePicture>();
        services.AddScoped<Saved>();
    }
}
