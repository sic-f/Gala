using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;

namespace Context;

public class PlacePicture(IPlacePictureRepository repository, IDeletePicture deletePicture)
{
    private Core.PlaceAggregate.PlacePicture Picture { get; set; } = default!;

    public PlacePictureDto DisplayPicture(Guid id, int slideIndex)
    {
        Picture = repository.NoTrackingFetch(id);

        return new PlacePictureDto(Id: Picture.Id,
                                   Normal: Picture.Normal,
                                   PlaceId: Picture.PlaceId,
                                   UploaderId: Picture.UploaderId,
                                   Index: slideIndex,
                                   Caption: Picture?.Caption);
    }

    public Core.PlaceAggregate.PlacePicture ModifyCaption(Guid id, string? caption)
    {
        Picture = repository.Fetch(id);

        if (string.IsNullOrEmpty(Picture.Caption) && string.IsNullOrEmpty(caption))
        {
            throw new BlankPictureCaptionException("Content is blank. Either input content or cancel.");
        }

        Picture.Caption = caption;
        Picture.LastModified = DateTimeOffset.UtcNow;

        repository.ModifyCaption();

        return Picture;
    }

    public void DeletePicture(Guid id)
    {
        Picture = repository.NoTrackingFetch(id);

        deletePicture.ComputeAsync(Picture);

        repository.DeletePicture(Picture);
    }
}
