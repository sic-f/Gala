using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlacePictureService
{
    PlacePicture Fetch(Guid id);
    void ModifyCaption();
    void DeletePicture();
}
