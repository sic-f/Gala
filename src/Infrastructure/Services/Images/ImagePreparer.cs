using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Images;

public class ImagePreparer(IFormFile formFile)
{
    protected readonly IFormFile _formFile = formFile;

    public virtual async Task PrepareAsync(FileStream stream)
    {
        await _formFile.CopyToAsync(stream);

        stream.Position = 0;
    }
}