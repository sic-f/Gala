using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ui.ViewModels;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionsController(UserManager<Bro> userManager,
                                ApplicationDbContext context,
                                GenerateToken generateToken,
                                GenerateRefreshToken generateRefreshToken) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(LoginViewModel model)
    {
        var user = await ValidateAccount(model);

        if (user is null)
        {
            return BadRequest(new { Message = "Login failed." });
        }

        await context.Entry(user).Reference(u => u.Avatar).LoadAsync();

        return Ok(new { Token = generateToken.Compute(user), RefreshToken = generateRefreshToken.Compute(user.Id) });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] string token,
                                             RevokeAllActiveRefreshTokens revokeAllActiveRefreshTokens,
                                             ApplicationDbContext context)
    {
        RefreshToken? existingRefreshToken = context.RefreshToken.FirstOrDefault(rt => rt.Token == token);

        if (existingRefreshToken is null) return Unauthorized();

        Bro? bro = await context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == existingRefreshToken.BroId);

        if (bro is null) return Unauthorized();

        if (existingRefreshToken.IsUnusable)
        {
            if (existingRefreshToken.HasBeenRevoked)
            {
                await revokeAllActiveRefreshTokens.ComputeAsync(broId: existingRefreshToken.BroId);
            }

            return Unauthorized();
        }

        string newRefreshToken = generateRefreshToken.Compute(bro.Id);
        existingRefreshToken.Revoked = DateTimeOffset.UtcNow;
        existingRefreshToken.ReplacedByToken = newRefreshToken;

        context.RefreshToken.Update(existingRefreshToken);
        context.SaveChanges();

        return Ok(new { Token = generateToken.Compute(bro!), RefreshToken = newRefreshToken });
    }

    [HttpDelete]
    public async Task<IActionResult> Logout([FromBody] string token)
    {
        var refreshToken = context.RefreshToken.FirstOrDefault<RefreshToken>(rt => rt.Token == token);

        if (refreshToken is null || refreshToken.IsUnusable)
        {
            return NotFound("Token not found or already inactive.");
        }

        refreshToken.Revoked = DateTimeOffset.UtcNow;
        await context.SaveChangesAsync();

        return NoContent();
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