using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace Ui.Services;

public static class Extensions
{
    public static Guid BroId(this ControllerBase _, ClaimsPrincipal user)
    {
        string? broId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        return broId is null ? throw new AuthenticationException("User not authenticated.") : Guid.Parse(broId);
    }
}
