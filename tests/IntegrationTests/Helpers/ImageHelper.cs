using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace IntegrationTests.Helpers;

public class ImageHelper
{
    private const string TestImagePathA = "Utility\\Support\\TestImages\\TestImageA.jpg";
    private const string TestImagePathB = "Utility\\Support\\TestImages\\TestImageB.jpg";
    private const string TestImagePathC = "Utility\\Support\\TestImages\\TestImageC.jpg";

    public static string TestImageAPath =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(OldPath, TestImagePathA);

    public static string TestImageBPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(OldPath, TestImagePathB);

    public static string TestImageCPath =>
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
            .Replace(OldPath, TestImagePathC);

    private static string CurrentAssembly { get; set; } = "IntegrationTests";
    private static string OldPath => $"{CurrentAssembly}\\bin\\Debug\\net10.0";

    public static IFormFile MockImageFormFile(string fileName = "TestImage.jpg",
                                              string? contentType = null,
                                              string? assembly = null)
    {
        if (assembly is not null) CurrentAssembly = assembly;

        using var fileStream = File.OpenRead(TestImageAPath);
        var stream = new MemoryStream();
        fileStream.CopyTo(stream);
        FormFile formFile = new(stream, 0, stream.Length, "TestImage", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType ?? "image/jpeg"
        };

        return formFile;
    }

    public static IEnumerable<string> PlacePicturesPaths() => [TestImageBPath, TestImageCPath];
}
