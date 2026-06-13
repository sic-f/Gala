using Infrastructure.Identity;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ui.ViewModels;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(UserManager<Bro> userManager) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(LoginViewModel model)
    {
        var user = await ValidateAccount(model);

        if (user is null)
        {
            return BadRequest(new { Message = "Login failed." });
        }

        return Ok(new { Token = Jwt.GenerateToken(For: user) });
    }

    private async Task<Bro?> ValidateAccount(LoginViewModel model)
    {
        var identityUser = await userManager.FindByNameAsync(model.Email);
        if (identityUser is not null)
        {
            var result = userManager.PasswordHasher
                                    .VerifyHashedPassword(identityUser,
                                                          identityUser.PasswordHash!,
                                                          model.Password);

            return result == PasswordVerificationResult.Failed ? null : identityUser;
        }

        return null;
    }
}