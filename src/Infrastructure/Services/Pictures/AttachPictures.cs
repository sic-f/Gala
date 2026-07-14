using Core.Entities;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Services.Pictures;
using Core.Values;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Services.Pictures;

public class AttachPictures(PersistPicture persistPicture, IDeletePicture deleteImage) : IPicturesAttacher
{
    public async Task ComputeAsync(Brodiebles brodybles, PictureUploads pictureUploads)
    {
        if (IsNotUpdatingPicture(pictureUploads.Picture)) return;

        await RemoveOldProfilePictureAsync(brodybles.Avatar);

        await BuildProfilePictureAsync(brodybles, pictureUploads.Picture);
    }

    public async Task ComputeAsync(Place place, PictureUploads pictureUploads)
    {
        if (HasNoPicturesToUpdate(pictureUploads.Picture, pictureUploads.Pictures)) return;

        if (IsUpdatingPicture(pictureUploads.Picture))
        {
            await RemoveOldProfilePictureAsync(place.Picture);
        }

        await BuildPlacePictureAsync(place, pictureUploads.Picture);

        await BuildPicturesAsync(place, pictureUploads.Pictures);
    }

    private static bool HasNoPicturesToUpdate([NotNullWhen(false)] PictureUpload? picture,
                                              [NotNullWhen(false)] IEnumerable<PictureUpload>? pictures)
    {
        return IsNotUpdatingPicture(picture) && IsNotAddingPictures(pictures);
    }

    private static bool IsNotUpdatingPicture([NotNullWhen(false)] PictureUpload? picture)
    {
        return picture is null;
    }

    private static bool IsNotAddingPictures([NotNullWhen(false)] IEnumerable<PictureUpload>? pictures)
    {
        return pictures is null;
    }

    private static bool IsUpdatingPicture([NotNullWhen(false)] PictureUpload? picture)
    {
        return picture is not null;
    }

    private async Task RemoveOldProfilePictureAsync(IImageDeletable? oldPicture)
    {
        if (oldPicture is null) return;

        await deleteImage.ComputeAsync(oldPicture);
    }

    private static async Task BuildProfilePictureAsync(Brodiebles brodiebles, PictureUpload picture)
    {
        if (picture is null) return;

        await new ResizePersistPicture(
            fileName: picture.Paths.NormalLocal,
            sourceStream: picture.Stream,
            imageDimensions: new ImageDimensions(Width: 640, Height: 360),
            resizeMode: ResizeMode.Max).ComputeAsync();

        await new ResizePersistPicture(
            fileName: picture.Paths.SmallLocal,
            sourceStream: picture.Stream,
            imageDimensions: new ImageDimensions(Width: 64, Height: 64),
            resizeMode: ResizeMode.Max).ComputeAsync();

        brodiebles.Avatar = new AccountAvatar
        {
            Normal = picture.Paths.Normal,
            Small = picture.Paths.Small,
            NormalLocal = picture.Paths.NormalLocal,
            SmallLocal = picture.Paths.SmallLocal,
        };
    }

    private async Task BuildPlacePictureAsync(Place place, PictureUpload picture)
    {
        if (picture is null) return;

        await persistPicture.ComputeAsync(
            fileName: picture.Paths.NormalLocal,
            sourceStream: picture.Stream);

        await new ResizePersistPicture(
            picture.Paths.SmallLocal,
            sourceStream: picture.Stream,
            imageDimensions: new ImageDimensions(Width: 384, Height: 288),
            resizeMode: ResizeMode.Pad).ComputeAsync();

        place.AddPicture(new PlaceProfilePicture
        {
            Normal = picture.Paths.Normal,
            Small = picture.Paths.Small,
            NormalLocal = picture.Paths.NormalLocal,
            SmallLocal = picture.Paths.SmallLocal,
            UploaderId = place.BroId,
        });
    }

    private async Task BuildPicturesAsync(Place place, IEnumerable<PictureUpload> pictures)
    {
        if (pictures is null) return;

        ICollection<PlaceExtraPicture> placeExtraPictures = [];

        foreach (var picture in pictures)
        {
            await persistPicture.ComputeAsync(
                fileName: picture.Paths.NormalLocal,
                sourceStream: picture.Stream);

            await new ResizePersistPicture(
                picture.Paths.SmallLocal,
                sourceStream: picture.Stream,
                imageDimensions: new ImageDimensions(Width: 800, Height: 740),
                resizeMode: ResizeMode.Crop).ComputeAsync();

            placeExtraPictures.Add(new PlaceExtraPicture
            {
                Normal = picture.Paths.Normal,
                Small = picture.Paths.Small,
                NormalLocal = picture.Paths.NormalLocal,
                SmallLocal = picture.Paths.SmallLocal,
                UploaderId = place.BroId,
            });
        }

        place.AddPictures(placeExtraPictures);
    }
}