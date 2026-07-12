namespace Infrastructure.Services.Images;

public class ImagePreparer(Stream pictureStream)
{
    protected readonly Stream _pictureStream = pictureStream;

    public virtual async Task PrepareAsync(FileStream stream)
    {
        await _pictureStream.CopyToAsync(stream);
        _pictureStream.Seek(0, SeekOrigin.Begin);
    }
}