using Core.Entities;
using Core.Interfaces;
using Core.PlaceAggregate;
using Infrastructure.Identity;
using Infrastructure.ValueObjects;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics.CodeAnalysis;
using static Core.ValueObjects.General;
using static Core.ValueObjects.ImageValues;

namespace Infrastructure.Services.Images;

public class AttachImages(UpdateAvatar updateAvatar, ValidateImageType validateImageType)
{
    public async Task ComputeAsync(Bro bro,
                                   IFormFile? newAvatar,
                                   GetImagesPath getImagesPath,
                                   UrlSegments imageUrlSegments)
    {
        if (IsNotUpdatingPicture(newAvatar)) return;

        validateImageType.Validate(newAvatar);

        await RemoveOldProfilePictureAsync(bro.Avatar, getImagesPath);

        await BuildProfilePictureAsync(bro, newAvatar, getImagesPath, imageUrlSegments);
    }

    public async Task ComputeAsync(Place place,
                                   ImageValues.Images images,
                                   GetImagesPath getImagesPath,
                                   UrlSegments urlSegments)
    {
        if (HasNoPicturesToUpdate(images.Picture, images.Pictures)) return;

        validateImageType.Validate(images);

        await RemoveOldProfilePictureAsync(place.Picture, getImagesPath);

        await BuildPlacePictureAsync(place, images.Picture, getImagesPath, urlSegments);

        await BuildPicturesAsync(place, images.Pictures, getImagesPath, urlSegments);
    }

    private static bool HasNoPicturesToUpdate([NotNullWhen(false)] IFormFile? picture,
                                              [NotNullWhen(false)] IFormFileCollection? pictures)
    {
        return IsNotUpdatingPicture(picture) && IsNotAddingPictures(pictures);
    }

    private static bool IsNotUpdatingPicture([NotNullWhen(false)] IFormFile? picture)
    {
        return picture is null;
    }

    private static bool IsNotAddingPictures([NotNullWhen(false)] IFormFileCollection? pictures)
    {
        return pictures is null;
    }

    private static async Task RemoveOldProfilePictureAsync(IImageDeletable? oldPicture,
                                                           GetImagesPath getImagesPath)
    {
        if (oldPicture is null) return;

        await DeleteAvatar.ComputeAsync(oldPicture, getImagesPath);
    }

    private async Task BuildProfilePictureAsync(Bro bro,
                                                IFormFile newAvatar,
                                                GetImagesPath getImagesPath,
                                                UrlSegments imageUrlSegments)
    {
        var fileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        string normalUrl = (await updateAvatar.ComputeAsync(
            fileName,
            getImagesPath,
            imageUrlSegments,
            preparer: new ResizedImagePreparer(
                newAvatar,
                imageDimensions: new ImageDimensions(Width: 640, Height: 360),
                resizeMode: ResizeMode.Max))).GetUri();

        string smallUrl = (await updateAvatar.ComputeAsync(
                fileName,
                getImagesPath,
                imageUrlSegments,
                preparer: new ResizedImagePreparer(
                    newAvatar,
                    imageDimensions: new ImageDimensions(Width: 64, Height: 64),
                    resizeMode: ResizeMode.Max),
                imageVariant: UpdateAvatar.Small)).GetUri();

        bro.Avatar = new AccountAvatar
        {
            Normal = normalUrl,
            Small = smallUrl,
        };
    }

    private async Task BuildPlacePictureAsync(Place place,
                                              IFormFile picture,
                                              GetImagesPath getImagesPath,
                                              UrlSegments urlSegments)
    {
        if (picture is null) return;

        var fileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        string normalUrl = (await updateAvatar.ComputeAsync(
            fileName,
            getImagesPath,
            urlSegments,
            preparer: new ImagePreparer(picture))).GetUri();

        var smallUrl = (await updateAvatar.ComputeAsync(
            fileName,
            getImagesPath,
            urlSegments,
            preparer: new ResizedImagePreparer(
                picture,
                imageDimensions: new ImageDimensions(Width: 854, Height: 480),
                resizeMode: ResizeMode.Max),
            imageVariant: UpdateAvatar.Small)).GetUri();

        place.AddPicture(new PlaceProfilePicture
        {
            Normal = normalUrl,
            Small = smallUrl,
            UploaderId = place.BroId,
        });
    }

    private async Task BuildPicturesAsync(Place place,
                                          IFormFileCollection pictures,
                                          GetImagesPath getImagesPath,
                                          UrlSegments urlSegments)
    {
        if (pictures is null) return;

        var placePicturesTasks = pictures.Select(async picture =>
        {
            var fileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

            var normalUrl = (await updateAvatar.ComputeAsync(
                fileName,
                getImagesPath,
                urlSegments,
                preparer: new ImagePreparer(picture))).GetUri();

            var smallUrl = (await updateAvatar.ComputeAsync(
                fileName,
                getImagesPath,
                urlSegments,
                preparer: new ResizedImagePreparer(
                    picture,
                    imageDimensions: new ImageDimensions(Width: 300, Height: 300),
                    resizeMode: ResizeMode.Crop),
                imageVariant: UpdateAvatar.Small)).GetUri();

            return new PlaceExtraPicture
            {
                Normal = normalUrl,
                Small = smallUrl,
                UploaderId = place.BroId,
            };
        }).ToList();

        place.AddPictures((await Task.WhenAll(placePicturesTasks)).ToList());
    }
}