using Appetito.Domain.Entities;

namespace Appetito.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseInterface<RefreshToken>
{
    Task<RefreshToken?> GetByHash(string hash, CancellationToken ct);
}
