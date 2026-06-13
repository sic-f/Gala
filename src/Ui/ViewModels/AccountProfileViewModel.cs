using System.ComponentModel.DataAnnotations;

namespace Ui.ViewModels;

public class AccountProfileViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}