using System.ComponentModel.DataAnnotations;

namespace Ui.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}