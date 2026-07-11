using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Values;

namespace Context;

public class Contribution(IPicturesAttacher attachImages, IPlaceService repository)
{
    private Place Place { get; set; } = default!;

    public async Task<Place> SubmitAsync(IPlaceUpdater details, PictureUploads pictureUploads, Guid broId)
    {
        BuildPlace(details, broId);

        await attachImages.ComputeAsync(Place, pictureUploads);

        return await repository.SavePlaceAsync(Place);
    }

    private void BuildPlace(IPlaceUpdater details, Guid broId)
    {
        Place = new()
        {
            Name = details.Name,
            Description = details.Description,
            Information = details.Information,
            Address = details.Address,
            Mobile = details.Mobile,
            Landline = details.Landline,
            Email = details.Email,
            Website = details.Website,
            Facebook = details.Facebook,
            Instagram = details.Instagram,
            BroId = broId,
        };
    }
}
