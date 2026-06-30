using Core.Entities;
using Infrastructure.Data;

namespace Infrastructure.Services.Images;

public class DeleteAvatar(GetImagesPath getImagesPath, ApplicationDbContext context)
{
    public async Task ComputeAsync(AccountAvatar avatar)
    {
        string normalFileName = new Uri(avatar.Normal).Segments.Last();
        string smallFileName = new Uri(avatar.Small).Segments.Last();

        File.Delete(getImagesPath.Avatar(normalFileName));
        File.Delete(getImagesPath.Avatar(smallFileName));

        context.AccountAvatar.Remove(avatar);
        await context.SaveChangesAsync();
    }
}
