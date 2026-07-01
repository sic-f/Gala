using Core.Interfaces;

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
    public ICollection<PlaceProfilePicture> ProfilePictures { get; } = Enumerable.Empty<PlaceProfilePicture>().ToList();
    public List<PlaceExtraPicture> Pictures { get; } = new List<PlaceExtraPicture>();
    public Guid BroId { get; set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset LastModified { get; set; }

    public void UpdateDetails(IPlaceDetailer details)
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
        LastModified = DateTimeOffset.UtcNow;
    }

    public void AddProfilePicture(PlaceProfilePicture profilePicture)
    {
        ProfilePictures.Add(profilePicture);
    }
}