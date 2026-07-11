using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPlaceService
{
    Task<Place> CreatePlaceAsync(Place place);
    Task<Place> FetchPlaceWithPictureAsync(Guid id);
    Task<Place> FetchPlaceAsync(Guid id);
    IEnumerable<PlacePicture> FetchPlacePictureAndPictures(Guid id);
    Task ModifyPlaceAsync(Place place);
    Task DeletePlaceAsync(Place place);
}
