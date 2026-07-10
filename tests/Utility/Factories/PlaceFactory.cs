using Core.PlaceAggregate;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Utility.Dto;
using Utility.Helpers;

namespace Utility.Factories;

public class PlaceFactory
{
    public static async Task<Place> CreatePlaceAsync(PlaceAttributes attributes, ApplicationFactory<Program> app)
    {
        Place place = new()
        {
            Name = attributes.Name,
            Description = attributes.Description,
            Information = attributes.Information,
            Address = attributes.Address,
            Mobile = attributes.Mobile,
            Landline = attributes.Landline,
            Email = attributes.Email,
            Website = attributes.Website,
            Facebook = attributes.Facebook,
            Instagram = attributes.Instagram,
            BroId = attributes.BroId
        };

        if (attributes.Picture is not null)
        {
            place.AddPicture(attributes.Picture!);
            string testDirectory = CreateTestDirectory();
            CreatePictureFile(place.Picture!, testDirectory);
        }

        if (attributes.Pictures.Any())
        {
            place.AddPictures(attributes.Pictures);
            string testDirectory = CreateTestDirectory();
            CreatePicturesFiles(place.Pictures, testDirectory);
        }


        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        place = context.Place.Add(place).Entity;
        await context.SaveChangesAsync();

        return place;
    }

    public static async Task<Place> CreatePlaceCompleteDetails(Guid broId, ApplicationFactory<Program> app)
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

        using var scope = app.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var context = scopedServices.GetRequiredService<ApplicationDbContext>();

        place = context.Place.Add(place).Entity;
        await context.SaveChangesAsync();

        return place;
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
        var normalFilePath = Path.Combine(testDirectory, $"{picture.Normal}.jpg");
        var smallFilePath = Path.Combine(testDirectory, $"{picture.Small}.jpg");

        using var sourceStream = File.OpenRead(ImageHelper.TestImageAPath);
        using var destinationStream = File.Create(normalFilePath);
        using var destinationStreamSmall = File.Create(smallFilePath);

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

            using var sourceStream = File.OpenRead(ImageHelper.TestImageAPath);
            using var destinationStream = File.Create(normalFilePath);
            using var destinationStreamSmall = File.Create(smallFilePath);

            picture.NormalLocal = normalFilePath;
            picture.SmallLocal = smallFilePath;
            picture.Normal = "Normal";
            picture.Small = "Small";
        }
    }
}