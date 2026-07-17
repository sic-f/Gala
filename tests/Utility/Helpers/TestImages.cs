using System.Reflection;

namespace Utility.Helpers;

public class TestImages
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
}
