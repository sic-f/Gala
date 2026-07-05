using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;

namespace Utility.Factories;

public class PlaceFactory
{
    public static async Task<Place> CreatePlaceAsync(PlaceAttributes attributes, ApplicationFactory<Program> app)
    {
        Place place = new()
        {
            Name = attributes.Name,
            Description = attributes.Description,
            Information = attributes.Information,
            Address = attributes.Address,
            Mobile = attributes.Mobile,
            Landline = attributes.Landline,
            Email = attributes.Email,
            Website = attributes.Website,
            Facebook = attributes.Facebook,
            Instagram = attributes.Instagram,
            BroId = attributes.BroId
        };

        if (attributes.Picture is not null) place.AddPicture(attributes.Picture!);
        place.AddPictures(attributes.Pictures);

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        place = (await context.Place.AddAsync(place)).Entity;
        await context.SaveChangesAsync();

        return place;
    }
}