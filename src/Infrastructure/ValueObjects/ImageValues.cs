using Microsoft.AspNetCore.Http;

namespace Infrastructure.ValueObjects;

public class ImageValues
{
    public record Images(IFormFile? ProfilePicture, IFormFileCollection? Pictures);
}
