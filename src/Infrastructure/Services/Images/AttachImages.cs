using Core.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Processing;
using static Core.ValueObjects.General;
using static Core.ValueObjects.Images;

namespace Infrastructure.Services.Images;

public class AttachImages(UpdateAvatar updateAvatar, DeleteAvatar deleteAvatar, ValidateImageType validateImageType)
{
    public async Task ComputeAsync(Bro bro, IFormFile newAvatar, UrlSegments imageUrlSegments)
    {
        validateImageType.Compute(newAvatar);

        await RemoveOldProfilePictureAsync(bro.Avatar);

        await BuildProfilePictureAsync(bro, newAvatar, imageUrlSegments);
    }

    protected async Task RemoveOldProfilePictureAsync(AccountAvatar? oldAvatar)
    {
        if (oldAvatar is null) return;

        await deleteAvatar.ComputeAsync(oldAvatar);
    }

    private async Task BuildProfilePictureAsync(Bro bro, IFormFile newAvatar, UrlSegments imageUrlSegments)
    {
        var fileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        string normalUrl = (await updateAvatar.ComputeAsync(
            fileName,
            imageUrlSegments,
            preparer: new ResizedImagePreparer(
                newAvatar,
                imageDimensions: new ImageDimensions(Width: 640, Height: 360),
                resizeMode: ResizeMode.Max))).GetUri();

        string smallUrl = (await updateAvatar.ComputeAsync(
                fileName,
                imageUrlSegments,
                preparer: new ResizedImagePreparer(
                    newAvatar,
                    imageDimensions: new ImageDimensions(Width: 64, Height: 64),
                    resizeMode: ResizeMode.Max),
                imageVariant: UpdateAvatar.Small)).GetUri();

        bro.Avatar = new AccountAvatar
        {
            Normal = normalUrl,
            Small = smallUrl
        };
    }
}