using Core.Dto;
using System.Net.Http.Json;

namespace Utility.Scopes;

public class PlacePictureScope : ApplicationFactory<Program>
{
    protected override string DefaultStartUrl => "api/Place-Pictures";

    public async Task<PlacePictureDto> GetPlaceProfilePictureAsync(Guid id)
    {
        return (await Client.GetFromJsonAsync<PlacePictureDto>($"{DefaultStartUrl}/{id}"))!;
    }

    public async Task UpdatePictureCaptionAsync(Guid id, string caption)
    {
        var picture = new { Caption = caption };
        Response = await Client.PutAsJsonAsync($"{DefaultStartUrl}/{id}", picture);
    }

    public async Task DeletePictureAsync(Guid id)
    {
        Response = await Client.DeleteAsync($"{DefaultStartUrl}/{id}");
    }
}
