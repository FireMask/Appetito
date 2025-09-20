// Pantry.Application/Auth/AuthService.cs
using System.Security.Cryptography;
using System.Text;
using Appetito.Application.Abstractions;
using Appetito.Domain.Entities;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    IJwtProvider jwt,
    IRefreshTokenRepository refreshRepo) : IAuthService
{
    public async Task<AuthResult> LoginAsync(string email, string password, string ip, string ua, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(email) ?? throw new UnauthorizedAccessException();
        if (!hasher.Verify(password, user.PasswordHash)) throw new UnauthorizedAccessException();

        var now = DateTime.UtcNow;
        var access = jwt.GenerateAccessToken(user.Id, user.HouseholdId, user.Email, now);

        var refreshPlain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshHash  = Sha256(refreshPlain);
        var token = new RefreshToken {
            Id = Guid.NewGuid(), UserId = user.Id,
            TokenHash = refreshHash, CreatedAt = now,
            ExpiresAt = now.AddDays(30), ClientIp = ip, UserAgent = ua
        };
        await refreshRepo.AddAsync(token, ct);
        await refreshRepo.SaveChangesAsync(ct);

        return new AuthResult(access, refreshPlain);
    }

    public async Task<AuthResult> RefreshAsync(string refreshPlain, string ip, string ua, CancellationToken ct)
    {
        var hash = Sha256(refreshPlain);
        var token = await refreshRepo.FindActiveByHashAsync(hash, ct) ?? throw new UnauthorizedAccessException();
        if (token.ExpiresAt <= DateTime.UtcNow) throw new UnauthorizedAccessException();

        token.RevokedAt = DateTime.UtcNow; // rotate family
        var user = token.User; // include user in repo query or re-fetch

        var now = DateTime.UtcNow;
        var access = jwt.GenerateAccessToken(user.Id, user.HouseholdId, user.Email, now);

        var newPlain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newHash  = Sha256(newPlain);
        await refreshRepo.AddAsync(new RefreshToken {
            Id = Guid.NewGuid(), UserId = user.Id,
            TokenHash = newHash, CreatedAt = now, ExpiresAt = now.AddDays(30),
            ClientIp = ip, UserAgent = ua
        }, ct);
        await refreshRepo.SaveChangesAsync(ct);

        return new AuthResult(access, newPlain);
    }

    static string Sha256(string s)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
    }
}

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password, string ip, string ua, CancellationToken ct);
    Task<AuthResult> RefreshAsync(string refreshToken, string ip, string ua, CancellationToken ct);
}
public record AuthResult(string AccessToken, string RefreshToken);
