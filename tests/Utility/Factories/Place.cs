using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;

namespace Utility.Factories;

public class PlaceFactory
{
    public static Place CreatePlace(PlaceAttributes attributes, ApplicationFactory<Program> app)
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
            BroId = attributes.BroId,
        };

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        place = context.Place.Add(place).Entity;
        context.SaveChanges();

        return place;
    }
}