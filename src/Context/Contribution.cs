using Core.Dto;
using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Values;

namespace Context;

public class Contribution(IPicturesAttacher attachPictures,
                          IPlaceService repository,
                          ISavedPlaceService savedPlaceRepository,
                          IDeletePicture deleteImage)
{
    private Place Place { get; set; } = default!;

    public async Task<IEnumerable<PlaceDetailsHomePage>> ShowPlaceInHomePageAsync(DateTimeOffset offset, int limit, Guid broId)
    {
        IEnumerable<PlaceDetailsHomePage> result = [.. repository.FetchPlacePaginatedWithPicture(offset, limit).Select(p =>
        {
            return new PlaceDetailsHomePage()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Information = p.Information,
                Address = p.Address,
                Mobile = p.Mobile,
                Landline = p.Landline,
                Email = p.Email,
                Website = p.Website,
                Facebook = p.Facebook,
                Instagram = p.Instagram,
                BroId = p.BroId,
                Picture = GetPicture(p),
                CreatedAt = p.CreatedAt
            };
        })];

        foreach (PlaceDetailsHomePage details in result)
        {
            details.BookmarkId = await GetIsSavedIdAsync(details.Id, broId);
        }

        return result;
    }

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

        Place.SetDetails(details);

        await repository.ModifyPlaceAsync(Place);

        return Place;
    }

    public async Task<PlaceDetails> DisplayPlaceAsync(Guid id, Guid broId)
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
                                BookmarkId: await GetIsSavedIdAsync(id, broId),
                                Picture: GetPicture(),
                                Pictures: GetPictures());
    }

    public async Task DeleteAsync(Guid id, Guid broId)
    {
        Place = await repository.FetchPlaceAsync(id);

        if (broId != Place.BroId)
        {
            throw new UnauthorizedContributionAccessException("❌👦🏣 Bro not allowed to delete other people's places.");
        }

        IEnumerable<Core.PlaceAggregate.PlacePicture> placePictures = repository.FetchPlacePictureAndPictures(id);

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

    private static PlacePictureLean? GetPicture(Place place)
    {
        if (place.Picture is null) return null;

        return new(Id: place.Picture.Id, Small: place.Picture.Small, PlaceId: place.Picture.PlaceId);
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

    private async Task<Guid?> GetIsSavedIdAsync(Guid placeId, Guid broId)
    {
        return await savedPlaceRepository.GetSavedPlaceId(placeId, broId);
    }
}
