using Infrastructure.Data;

namespace Ui.Services.Jwt;

public class RevokeAllActiveRefreshTokens(ApplicationDbContext context)
{
    public async Task ComputeAsync(Guid broId)
    {
        var activeTokens = context.RefreshToken.Where(rt => rt.BroId == broId && rt.Revoked == null);

        foreach (var token in activeTokens)
        {
            token.Revoked = DateTimeOffset.UtcNow;
        }

        await context.SaveChangesAsync();
    }
}
