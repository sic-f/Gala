using System.ComponentModel.DataAnnotations;

namespace Ui.ViewModels;

public class AccountViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    [Compare("Password")]
    public string PasswordConfirmation { get; set; } = null!;
}