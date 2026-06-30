using static Core.ValueObjects.General;

namespace Infrastructure.Services.Images;

public class UpdateAvatar(GetImagesPath getImagesPath)
{
    public const string Small = "small";
    private string _fileName = string.Empty;
    private UrlSegments _imageUrlSegments = default!;
    private ImagePreparer _preparer = default!;

    public async Task<UpdateAvatar> ComputeAsync(string fileName, UrlSegments imageUrlSegments, ImagePreparer preparer, string? imageVariant = null)
    {
        SetFields(fileName, imageUrlSegments, preparer, imageVariant);

        await PersistAvatarAsync();

        return this;
    }

    public string GetUri()
    {
        return $"{_imageUrlSegments.Scheme}://{_imageUrlSegments.Host}/images/avatar/{_fileName}";
    }

    private async Task PersistAvatarAsync()
    {
        if (!Directory.Exists(getImagesPath.ImagesFolder()))
        {
            Directory.CreateDirectory(getImagesPath.ImagesFolder());
        }

        using var stream = new FileStream(getImagesPath.Avatar(_fileName), FileMode.Create);

        await _preparer.PrepareAsync(stream);
    }

    private void SetFields(string fileName, UrlSegments imageUrlSegments, ImagePreparer preparer, string? imageVariant)
    {
        _fileName = imageVariant is null ? $"{fileName}.jpg" : $"{fileName}-{imageVariant}.jpg";
        _imageUrlSegments = imageUrlSegments;
        _preparer = preparer;
    }
}