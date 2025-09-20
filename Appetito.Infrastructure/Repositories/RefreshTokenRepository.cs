using Appetito.Domain.Entities;
using Appetito.Infrastructure;
using Microsoft.EntityFrameworkCore;

public sealed class RefreshTokenRepository(AppetitoDbContext db) : IRefreshTokenRepository
{
    public Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        db.RefreshTokens.Add(token);
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> FindActiveByHashAsync(string hash, CancellationToken ct)
    {
        return db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash && r.RevokedAt == null, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return db.SaveChangesAsync(ct);
    }
}
