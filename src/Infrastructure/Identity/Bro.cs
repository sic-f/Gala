using Core.Entities;
using Core.SavedAggregate;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Identity;

public class Bro : IdentityUser<Guid>
{
    [StringLength(60)]
    public string? FirstName { get; set; }
    [StringLength(60)]
    public string? LastName { get; set; }
    public AccountAvatar? Avatar { get; set; }
    public ICollection<SavedPlace> SavedPlaces { get; } = Enumerable.Empty<SavedPlace>().ToList();

    public string Name()
    {
        if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
        {
            return $"{FirstName} {LastName}";
        }

        if (!string.IsNullOrEmpty(FirstName))
        {
            return FirstName;
        }

        return Email!;
    }
}