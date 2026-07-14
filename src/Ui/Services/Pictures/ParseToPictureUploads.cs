using Core.Exceptions;
using Core.Interfaces;
using Core.PlaceAggregate;
using Core.Values;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ui.Services.Pictures;

public class ParseToPictureUploads(IGetPicturePaths getPicturePaths, ValidateImageType validateImageType)
{
    private UrlSegments UrlSegments { get; set; } = default!;
    private ModelStateDictionary? ModelState { get; set; }

    public async Task<PictureUploads> FromFormFilesAsync<T>(UrlSegments urlSegments,
                                                            IFormFile? picture = null,
                                                            IFormFileCollection? pictures = null,
                                                            ModelStateDictionary? modelState = null) where T : IImageTypeSegmentable
    {
        UrlSegments = urlSegments;
        ModelState = modelState;

        return new PictureUploads(
            Picture: await ParseProfilePictureAsync<T>(picture),
            Pictures: await ParsePicturesAsync<T>(pictures));
    }

    public async Task<PictureUpload?> ParseProfilePictureAsync<T>(IFormFile? picture) where T : IImageTypeSegmentable
    {
        if (picture is null) return null;

        try
        {
            validateImageType.Validate(picture);
        }
        catch (ImageTypeException ex)
        {
            ModelState?.AddModelError(nameof(Place.Picture), ex.Message);
            throw;
        }

        PicturePaths picturePaths = new();

        getPicturePaths.GetURIs<T>(UrlSegments, picturePaths);
        getPicturePaths.GetImagePaths<T>(picturePaths);
        getPicturePaths.ReInitializeFileName();

        MemoryStream stream = new();
        await picture.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);

        return new PictureUpload(Stream: stream,
                                 Paths: picturePaths,
                                 ContentType: picture.ContentType,
                                 FileName: picture.FileName);
    }

    private async Task<IEnumerable<PictureUpload>?> ParsePicturesAsync<T>(IFormFileCollection? pictures) where T : IImageTypeSegmentable
    {
        if (pictures is null) return null;

        var pictureUploads = new List<PictureUpload>();

        foreach (IFormFile picture in pictures)
        {
            if (picture.Length > 0)
            {
                try
                {
                    validateImageType.Validate(picture);
                }
                catch (ImageTypeException ex)
                {
                    ModelState?.AddModelError(nameof(Place.Pictures), ex.Message);
                    throw;
                }

                PicturePaths picturePaths = new();

                getPicturePaths.GetURIs<T>(UrlSegments, picturePaths);
                getPicturePaths.GetImagePaths<T>(picturePaths);
                getPicturePaths.ReInitializeFileName();

                MemoryStream stream = new();
                await picture.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);

                pictureUploads.Add(new PictureUpload(Stream: stream,
                                                     Paths: picturePaths,
                                                     ContentType: picture.ContentType,
                                                     FileName: picture.FileName));
            }
        }

        return pictureUploads;
    }
}
