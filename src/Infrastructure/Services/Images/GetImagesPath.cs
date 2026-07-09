using Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Infrastructure.Services.Images;

public class GetImagesPath
{
    private readonly IPathSwapElements _paths;
    private string _imageKindDirectory;

    public GetImagesPath(IPathSwapElements paths)
    {
        _paths = paths;
        InitializeImageDirectory();
        CreateDirectory();
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
    /// Supplies correct image kind directory segment. 🛣️🚶‍
    /// </summary>
    /// <returns>The correct image kind directory segment after wwwroot and excluding the file name. ☑️
    /// e.g. "image/place"</returns>
    public string GetImageKindSegment()
    {
        StringBuilder sb = new();
        int index = 0;
        do
        {
            index = _imageKindDirectory.IndexOf(_paths.ImageKindSegmentWithWindowsDirectorySeparator, index);
            if (index != -1)
            {
                sb.Append(_paths.ImageKindSegmentWitGeneralDirectorySeparator);
                index++;
            }
        } while (index != -1);

        return sb.ToString();
    }

    /// <summary>
    /// Returns the folder path for the kind of image, e.g. A place's picture(s), an account's avatar, etc. 📁📂
    /// </summary>
    /// <param name="paths">A value object that contains incorrect and correct paths as properties. 
    /// The correct path will replace the incorrect path where the value object will be used. ☑️</param>
    [MemberNotNull(nameof(_imageKindDirectory))]
    private void InitializeImageDirectory()
    {
        _imageKindDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(_paths.IncorrectPath, _paths.CorrectPath);
    }

    private void CreateDirectory()
    {
        if (!Directory.Exists(_imageKindDirectory))
        {
            Directory.CreateDirectory(_imageKindDirectory);
        }
    }
}
