namespace Core.Services.Pictures;

public class PersistPicture
{
    public async Task ComputeAsync(string fileName, Stream sourceStream)
    {
        InitializeDirectory(fileName);

        using var fileStream = new FileStream(fileName, FileMode.Create);
        await sourceStream.CopyToAsync(fileStream);
        sourceStream.Seek(0, SeekOrigin.Begin);
    }

    private static void InitializeDirectory(string fileName)
    {
        if (!Directory.Exists(fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
        }
    }
}