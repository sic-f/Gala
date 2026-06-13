using System.ComponentModel.DataAnnotations;

namespace Ui.ViewModels;

public class ChangePasswordViewModel
{
    public string OldPassword { get; set; } = null!;
    public string Password { get; set; } = null!;
    [Compare("Password", ErrorMessage = "'Password Confirmation' and 'Password' do not match.")]
    public string? PasswordConfirmation { get; set; }
}