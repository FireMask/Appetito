using Appetito.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task<RefreshToken?> FindActiveByHashAsync(string hash, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
