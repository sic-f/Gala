using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class PlacePictureService(ApplicationDbContext context) : IPlacePictureService
{
    public PlacePicture Fetch(Guid id)
    {
        return context.PlacePicture.First(pp => pp.Id == id);
    }

    public PlacePicture NoTrackingFetch(Guid id)
    {
        return context.PlacePicture.AsNoTracking().First(pp => pp.Id == id);
    }

    public void ModifyCaption()
    {
        context.SaveChanges();
    }

    public void DeletePicture()
    {
        context.SaveChanges();
    }
}
