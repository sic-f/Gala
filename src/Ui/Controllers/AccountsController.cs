using Core.Interfaces;
using Infrastructure.Configuration.Storage.Options;
using Infrastructure.Data;
using Infrastructure.Identity;
using Infrastructure.Services.Images;
using Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Ui.Services;
using Ui.ViewModels;
using static Core.ValueObjects.General;

namespace Ui.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController(UserManager<Bro> userManager, GenerateToken generateToken) : ControllerBase
{
    private Bro? _bro;

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUser(Guid id,
                                             [FromBody] AccountProfileViewModel model,
                                             ApplicationDbContext context)
    {
        _bro = await context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == id);

        if (_bro is null) return NotFound();

        var result = await SetUserAsync(model);

        if (UpdateFailed(result))
        {
            return new BadRequestObjectResult(new
            {
                Message = "Account update failed.",
                result.Errors
            });
        }

        return Ok(new
        {
            Message = "Account details updated successfully.",
            Token = generateToken.Compute(_bro)
        });
    }

    [HttpPost]
    public async Task<ActionResult<Bro>> PostUser(AccountViewModel model,
                                                  GenerateRefreshToken generateRefreshToken)
    {
        var applicationUser = new Bro
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await userManager.CreateAsync(applicationUser, model.Password);

        if (!result.Succeeded)
        {
            var dictionary = new ModelStateDictionary();

            foreach (IdentityError error in result.Errors)
            {
                dictionary.AddModelError(error.Code, error.Description);
            }

            return new BadRequestObjectResult(new
            {
                Message = "Account creation failed",
                Errors = dictionary
            });
        }

        return Ok(new
        {
            Message = "Account creation successful",
            Token = generateToken.Compute(applicationUser),
            RefreshToken = generateRefreshToken.Compute(applicationUser.Id)
        });
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        _bro = await userManager.GetUserAsync(User);

        if (_bro is not null)
        {
            var result = await userManager.DeleteAsync(_bro);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.ToString()!);
                }
            }

            return NoContent();
        }
        else
        {
            return BadRequest(new
            {
                Message = @"Account not found. Please contact the site administrator."
            });
        }
    }

    [Authorize]
    [HttpPut("{id}/Password")]
    public async Task<IActionResult> PasswordAsync(string id, [FromBody] ChangePasswordViewModel model)
    {
        _bro = await userManager.FindByIdAsync(id);

        if (_bro is null) return NotFound();

        var result = await UpdatePasswordAsync(model);

        if (PasswordChangeFailed(result))
        {
            return new BadRequestObjectResult(new { Errors = result.Errors });
        }

        return Ok(new { Message = "Password changed successfully." });
    }

    [Authorize]
    [HttpPut("Update-Avatar")]
    public async Task<IActionResult> UpdateAvatarAsync([FromForm] IFormFile avatar,
                                                       IOptionsSnapshot<AvatarOptions> avatarOptions,
                                                       AttachImages attachImages,
                                                       GenerateToken generateToken,
                                                       ApplicationDbContext context)
    {
        string? broId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (broId is null) return NotFound();

        _bro = await context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(u => u.Id == Guid.Parse(broId));

        if (_bro is null) return NotFound();

        IPathSwapElements paths = avatarOptions.Get(AvatarOptions.Avatar);

        await attachImages.ComputeAsync(_bro,
                                        avatar,
                                        new GetImagesPath(paths),
                                        new UrlSegments(Request.Scheme, Request.Host.ToString()));

        IdentityResult result = await userManager.UpdateAsync(_bro);

        if (result.Errors.Any())
        {
            return new BadRequestObjectResult(new
            {
                Message = "Avatar update failed.",
                result.Errors
            });
        }

        return Ok(new
        {
            Message = "Avatar updated successfully.",
            Token = generateToken.Compute(_bro),
        });
    }


    private async Task<ErrorsParser> SetUserAsync(AccountProfileViewModel model)
    {
        var errorsParser = new ErrorsParser();

        if (_bro is null) return errorsParser;

        _bro.Email = model.Email;
        _bro.UserName = model.Email;
        _bro.FirstName = model.FirstName;
        _bro.LastName = model.LastName;

        var result = await userManager.UpdateAsync(_bro);
        errorsParser.ParseErrors(from: result);

        return errorsParser;
    }

    private async Task<ErrorsParser> UpdatePasswordAsync(ChangePasswordViewModel model)
    {
        var result = await userManager.ChangePasswordAsync(
            user: _bro!,
            currentPassword: model.OldPassword,
            newPassword: model.Password);

        return new ErrorsParser().ParseErrors(from: result);
    }

    private static bool UpdateFailed(ErrorsParser result) => result.ErrorCount > 0;

    private static bool PasswordChangeFailed(ErrorsParser result) => result.ErrorCount > 0;
}