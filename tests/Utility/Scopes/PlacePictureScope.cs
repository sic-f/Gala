using Core.Dto;
using System.Net.Http.Json;

namespace Utility.Scopes;

public class PlacePictureScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Place-Pictures";

    public async Task<PlacePicture> GetPlaceProfilePictureAsync(Guid id)
    {
        return (await Client.GetFromJsonAsync<PlacePicture>($"{DefaultStartUrl}/{id}"))!;
    }

    public async Task UpdatePictureCaptionAsync(Guid id, string caption)
    {
        var picture = new { Caption = caption };
        Response = await Client.PutAsJsonAsync($"{DefaultStartUrl}/{id}", picture);
    }
}
