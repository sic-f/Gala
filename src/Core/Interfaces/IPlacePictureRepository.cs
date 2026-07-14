using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlacePictureRepository
{
    PlacePicture Fetch(Guid id);
    PlacePicture NoTrackingFetch(Guid id);
    void ModifyCaption();
    void DeletePicture(PlacePicture picture);
}
