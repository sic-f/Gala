using Infrastructure.Configuration.Storage;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Infrastructure.Services.Images;

public class GetImagesPath(IOptionsSnapshot<ImagesOptions> imagesOptions)
{
    private readonly ImagesOptions _avatarPaths = imagesOptions.Get(ImagesOptions.Avatar);
    private string? _imagesFolder;

    /// <summary>
    /// Returns the folder path for account avatars.
    /// </summary>
    public string ImagesFolder()
    {
        _imagesFolder ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(_avatarPaths.IncorrectPath, _avatarPaths.CorrectPath);

        return _imagesFolder;
    }

    /// <summary>
    /// Returns the absolute path for the account avatar.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    public string Avatar(string fileName)
    {
        return Path.Combine(ImagesFolder(), fileName);
    }
}
