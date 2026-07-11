namespace Core.Interfaces;

public interface IDeletePicture
{
    Task ComputeAsync(IImageDeletable picture);
    Task DeleteAllAsync(IEnumerable<IImageDeletable> pictures);
}
