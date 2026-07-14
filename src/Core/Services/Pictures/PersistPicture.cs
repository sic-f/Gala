namespace Core.Services.Pictures;

public class PersistPicture
{
    public async Task ComputeAsync(string fileName, Stream sourceStream)
    {
        using var fileStream = new FileStream(fileName, FileMode.Create);
        await sourceStream.CopyToAsync(fileStream);
        sourceStream.Seek(0, SeekOrigin.Begin);
    }
}