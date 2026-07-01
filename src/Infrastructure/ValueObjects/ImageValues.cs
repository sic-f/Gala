using Microsoft.AspNetCore.Http;

namespace Infrastructure.ValueObjects;

public class ImageValues
{
    public record Images(IFormFile? Picture, IFormFileCollection? Pictures);
}
