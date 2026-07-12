using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Values;

namespace Context;

public class Contribution(IPicturesAttacher attachPictures, IPlaceService repository, IDeletePicture deleteImage)
{
    private Place Place { get; set; } = default!;

    public async Task<Place> SubmitAsync(IPlaceUpdater details, PictureUploads pictureUploads, Guid broId)
    {
        BuildPlace(details, broId);

        await attachPictures.ComputeAsync(Place, pictureUploads);

        return await repository.CreatePlaceAsync(Place);
    }

    public PlaceDetails DisplayPlace(Guid id)
    {
        Place = repository.FetchPlaceComplete(id);

        return new PlaceDetails(Id: Place.Id,
                                Name: Place.Name,
                                Description: Place.Description,
                                Information: Place.Information,
                                Address: Place.Address,
                                Mobile: Place.Mobile,
                                Landline: Place.Landline,
                                Email: Place.Email,
                                Website: Place.Website,
                                Facebook: Place.Facebook,
                                Instagram: Place.Instagram,
                                BroId: Place.BroId,
                                Picture: GetPicture(),
                                Pictures: GetPictures());
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

    public async Task DeleteAsync(Guid id, Guid broId)
    {
        Place = await repository.FetchPlaceAsync(id);

        if (broId != Place.BroId)
        {
            throw new UnauthorizedContributionAccessException("❌👦🏣 Bro not allowed to delete other people's places.");
        }

        IEnumerable<PlacePicture> placePictures = repository.FetchPlacePictureAndPictures(id);

        await deleteImage.DeleteAllAsync(placePictures);

        await repository.DeletePlaceAsync(Place);
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

    private PlacePictureLean? GetPicture()
    {
        if (Place.Picture is null) return null;

        return new(Id: Place.Picture.Id, Small: Place.Picture.Small, PlaceId: Place.Picture.PlaceId);
    }

    private IEnumerable<PlacePictureLean> GetPictures()
    {
        if (Place.Pictures.Count != 0)
        {
            return Place.Pictures.Select(p =>
                new PlacePictureLean(Id: p.Id, Small: p.Small, PlaceId: p.PlaceId));
        }

        return [];
    }
}
