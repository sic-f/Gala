using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Helpers;

namespace Utility.Factories;

public class PlaceFactory
{
    public static async Task<Place> CreateAsync(Place place, IServiceProvider services)
    {
        if (place.Picture is not null)
        {
            string testDirectory = CreateTestDirectory();
            CreatePictureFile(place.Picture!, testDirectory);
        }

        if (place.Pictures.Count != 0)
        {
            string testDirectory = CreateTestDirectory();
            CreatePicturesFiles(place.Pictures, testDirectory);
        }

        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        place = context.Place.Add(place).Entity;
        await context.SaveChangesAsync();

        return place;
    }

    public static async Task<Place> CreateCompleteDetails(Guid broId, IServiceProvider services)
    {
        Place place = new()
        {
            Name = "Test Name",
            Description = "Test Description",
            Information = "Test Information",
            Address = "Test Address",
            Mobile = "Test Mobile",
            Landline = "Test Landline",
            Email = "Test Email",
            Website = "https://www.example.net",
            Facebook = "https://www.facebook.com/test",
            Instagram = "@test",
            BroId = broId,
        };

        PlaceProfilePicture picture = new() { Normal = "Normal", Small = "Small" };
        IEnumerable<PlaceExtraPicture> pictures = [new PlaceExtraPicture(), new PlaceExtraPicture()];

        place.AddPicture(picture);
        place.AddPictures(pictures);

        string testDirectory = CreateTestDirectory();
        CreatePictureFile(picture, testDirectory);
        CreatePicturesFiles(pictures, testDirectory);

        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        place = context.Place.Add(place).Entity;
        await context.SaveChangesAsync();

        return place;
    }

    public static async Task<IEnumerable<Place>> CreateAsync(IEnumerable<Place> places, IServiceProvider services)
    {
        var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Place.AddRange(places);
        await context.SaveChangesAsync();

        return places;
    }

    private static string CreateTestDirectory()
    {
        var baseDirectory = Directory.GetCurrentDirectory();
        var testDirectory = baseDirectory.Replace(@"\tests\IntegrationTests\bin\Debug\net10.0",
                                                  @"\src\Ui\wwwroot\Images\Test");

        if (!Directory.Exists(testDirectory))
        {
            Directory.CreateDirectory(testDirectory);
        }

        return testDirectory;
    }

    private static void CreatePictureFile(PlaceProfilePicture picture, string testDirectory)
    {
        var normalFileName = Path.GetFileName(picture.Normal);
        var smallFileName = Path.GetFileName(picture.Small);
        var normalFilePath = Path.Combine(testDirectory, $"{normalFileName}");
        var smallFilePath = Path.Combine(testDirectory, $"{smallFileName}");

        using var sourceStream = File.OpenRead(TestImages.TestImageAPath);
        using var destinationStream = File.Create(normalFilePath);
        using var destinationStreamSmall = File.Create(smallFilePath);
        sourceStream.CopyTo(destinationStream);
        sourceStream.Seek(0, SeekOrigin.Begin);
        sourceStream.CopyTo(destinationStreamSmall);

        picture.NormalLocal = normalFilePath;
        picture.SmallLocal = smallFilePath;
    }

    private static void CreatePicturesFiles(IEnumerable<PlaceExtraPicture> pictures, string testDirectory)
    {
        foreach (PlaceExtraPicture picture in pictures)
        {
            var fileName = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

            var normalFilePath = Path.Combine(testDirectory, $"{fileName}.jpg");
            var smallFilePath = Path.Combine(testDirectory, $"{fileName}-small.jpg");

            using var sourceStream = File.OpenRead(TestImages.TestImageAPath);
            using var destinationStream = File.Create(normalFilePath);
            using var destinationStreamSmall = File.Create(smallFilePath);

            picture.NormalLocal = normalFilePath;
            picture.SmallLocal = smallFilePath;
            picture.Normal = "Normal";
            picture.Small = "Small";
        }
    }
}