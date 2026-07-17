using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Repositories;

public class PlacePictureRepository(ApplicationDbContext context) : IPlacePictureRepository
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

    public void DeletePicture(PlacePicture picture)
    {
        context.PlacePicture.Remove(picture);
        context.SaveChanges();
    }
}
