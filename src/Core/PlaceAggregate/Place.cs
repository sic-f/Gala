using Core.Dto;

namespace Core.PlaceAggregate;

public class Place
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Information { get; set; }
    public string Address { get; set; } = null!;
    public string? Mobile { get; set; }
    public string? Landline { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public Guid BroId { get; set; }
    public ICollection<PlaceProfilePicture> ProfilePictures { get; } = [];
    public List<PlaceExtraPicture> Pictures { get; } = [];
    public ICollection<Review> Reviews { get; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastModified { get; set; }
    public PlaceProfilePicture? Picture => ProfilePictures.FirstOrDefault();

    public void SetDetails(PlaceDetails details)
    {
        Name = details.Name;
        Description = details.Description;
        Information = details.Information;
        Address = details.Address;
        Mobile = details.Mobile;
        Landline = details.Landline;
        Email = details.Email;
        Website = details.Website;
        Facebook = details.Facebook;
        Instagram = details.Instagram;
    }

    public void AddPicture(PlaceProfilePicture profilePicture)
    {
        ProfilePictures.Add(profilePicture);
    }

    public void AddPictures(IEnumerable<PlaceExtraPicture> pictures)
    {
        Pictures.AddRange(pictures);
    }
}