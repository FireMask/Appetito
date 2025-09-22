using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Appetito.Infrastructure;
using Microsoft.EntityFrameworkCore;

public sealed class RefreshTokenRepository(AppetitoDbContext _context) : IRefreshTokenRepository
{
    public Task<IEnumerable<RefreshToken>> GetAll(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetById(Guid id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetByHash(string hash, CancellationToken ct)
    {
        return _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash && r.RevokedAt == null, ct);
    }

    public Task Create(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        return Task.CompletedTask;
    }

    public Task Delete(RefreshToken entity)
    {
        _context.RefreshTokens.Remove(entity);
        return Task.CompletedTask;
    }

    public Task Update(RefreshToken entity)
    {
        throw new NotSupportedException();
    }

    public async Task<int> SaveChanges(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            Console.Error.WriteLine($"An error occurred while updating the database: {ex.Message}");
            throw;
        }
    }
}
