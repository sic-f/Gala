using Core.Dto;
using Core.PlaceAggregate;

namespace Core.Interfaces;

public interface IPicturesAttacher
{
    Task ComputeAsync(Brodiebles brodybles, PictureUploads pictureUploads);
    Task ComputeAsync(Place place, PictureUploads pictureUploads);
}
