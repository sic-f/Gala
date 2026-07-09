using Core.Values;

namespace Infrastructure.Services.Images;

public class UpdateImage()
{
    public const string Small = "small";
    private string _fileName = string.Empty;
    private GetImagesPath _getImagesPath = default!;
    private UrlSegments _urlSegments = default!;
    private ImagePreparer _preparer = default!;

    public async Task<UpdateImage> ComputeAsync(string fileName,
                                                 GetImagesPath getImagesPath,
                                                 UrlSegments urlSegments,
                                                 ImagePreparer preparer,
                                                 string? imageVariant = null)
    {
        SetFields(fileName, getImagesPath, urlSegments, preparer, imageVariant);

        await PersistAvatarAsync();

        return this;
    }

    public string GetUri()
    {
        return $"{_urlSegments.Scheme}://{_urlSegments.Host}/{_getImagesPath.GetImageKindSegment()}/{_fileName}";
    }

    private async Task PersistAvatarAsync()
    {
        using var stream = new FileStream(_getImagesPath.FileNamePath(_fileName), FileMode.Create);
        await _preparer.PrepareAsync(stream);
    }

    private void SetFields(string fileName,
                           GetImagesPath getImagesPath,
                           UrlSegments urlSegments,
                           ImagePreparer preparer,
                           string? imageVariant)
    {
        _fileName = imageVariant is null ? $"{fileName}.jpg" : $"{fileName}-{imageVariant}.jpg";
        _getImagesPath = getImagesPath;
        _urlSegments = urlSegments;
        _preparer = preparer;
    }
}