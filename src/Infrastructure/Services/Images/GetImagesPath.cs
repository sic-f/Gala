using Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Infrastructure.Services.Images;

public class GetImagesPath
{
    private string _imageKindDirectory;

    public GetImagesPath(IPathSwapElements paths)
    {
        InitializeImageDirectory(paths);
    }

    /// <summary>
    /// Returns the absolute path for the image file. 🛣️🚶‍
    /// </summary>
    /// <param name="fileName">The name of the image file. 🗒️</param>
    public string FileNamePath(string fileName)
    {
        return Path.Combine(_imageKindDirectory, fileName);
    }

    /// <summary>
    /// Returns the folder path for the kind of image, e.g. A place's picture(s), an account's avatar, etc. 📁📂
    /// </summary>
    /// <param name="paths">A value object that contains incorrect and correct paths as properties. 
    /// The correct path will replace the incorrect path where the value object will be used. ☑️</param>
    [MemberNotNull(nameof(_imageKindDirectory))]
    private void InitializeImageDirectory(IPathSwapElements paths)
    {
        _imageKindDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(paths.IncorrectPath, paths.CorrectPath);
    }
}
