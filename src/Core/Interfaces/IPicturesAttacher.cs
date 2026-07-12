using Core.PlaceAggregate;
using Core.Values;

namespace Core.Interfaces;

public interface IPicturesAttacher
{
    Task ComputeAsync(Brodiebles brodybles, PictureUploads pictureUploads);
    Task ComputeAsync(Place place, PictureUploads pictureUploads);
}
