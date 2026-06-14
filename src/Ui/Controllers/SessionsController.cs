using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ui.ViewModels;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(UserManager<Bro> userManager,
                                ApplicationDbContext context) : ControllerBase
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

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] string token,
                                             GenerateRefreshToken generateRefreshToken,
                                             RevokeAllActiveRefreshTokens revokeAllActiveRefreshTokens)
    {
        RefreshToken? existingRefreshToken = context.RefreshToken.FirstOrDefault(rt => rt.Token == token);
        Bro? bro = await userManager.FindByIdAsync(existingRefreshToken?.BroId.ToString() ?? string.Empty);

        if (existingRefreshToken is null || bro is null)
        {
            return Unauthorized();
        }

        if (existingRefreshToken.IsTerminated)
        {
            await revokeAllActiveRefreshTokens.ComputeAsync(broId: existingRefreshToken.BroId);

            return Unauthorized();
        }

        string newRefreshToken = generateRefreshToken.Compute(bro.Id);
        existingRefreshToken.Revoked = DateTimeOffset.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshToken;

        context.RefreshToken.Update(existingRefreshToken);
        context.SaveChanges();

        return Ok(new { Token = Jwt.GenerateToken(For: bro!), RefreshToken = newRefreshToken });
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