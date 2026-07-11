using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Values;

namespace Context;

public class Contribution(IPicturesAttacher attachPictures, IPlaceService repository)
{
    private Place Place { get; set; } = default!;

    public async Task<Place> SubmitAsync(IPlaceUpdater details, PictureUploads pictureUploads, Guid broId)
    {
        BuildPlace(details, broId);

        await attachPictures.ComputeAsync(Place, pictureUploads);

        return await repository.CreatePlaceAsync(Place);
    }

    public async Task<Place> ModifyAsync(IPlaceUpdater details, PictureUploads pictureUploads, Guid id, Guid broId)
    {
        Place = await repository.FetchPlaceWithPictureAsync(id);

        if (Place.BroId != broId)
        {
            throw new UnauthorizedContributionAccessException(
                "❌👦🏣 You did not submit this place and is not the owner, you are not allowed to modify it.");
        }

        await attachPictures.ComputeAsync(Place, pictureUploads);

        Place.UpdateDetails(details);

        await repository.ModifyPlaceAsync(Place);

        return Place;
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
