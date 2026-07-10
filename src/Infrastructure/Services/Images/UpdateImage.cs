namespace Infrastructure.Services.Images;

public class UpdateImage()
{
    private ImagePreparer _preparer = default!;
    private string _fileName = default!;

    public async Task<UpdateImage> ComputeAsync(string fileName, ImagePreparer preparer)
    {
        SetFields(fileName, preparer);

        await PersistAvatarAsync();

        return this;
    }

    private async Task PersistAvatarAsync()
    {
        if (!Directory.Exists(_fileName))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_fileName)!);
        }

        using var stream = new FileStream(_fileName, FileMode.Create);
        await _preparer.PrepareAsync(stream);
    }

    private void SetFields(string fileName, ImagePreparer preparer)
    {
        _fileName = fileName;
        _preparer = preparer;
    }
}