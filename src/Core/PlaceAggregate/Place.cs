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
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset LastModified { get; set; }
    public PlaceProfilePicture? Picture => ProfilePictures.FirstOrDefault();

    public void AddPicture(PlaceProfilePicture profilePicture)
    {
        ProfilePictures.Add(profilePicture);
    }

    public void AddPictures(IEnumerable<PlaceExtraPicture> pictures)
    {
        Pictures.AddRange(pictures);
    }
}