using System.Security.Cryptography;
using System.Text;
using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;

public sealed class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    IJwtProvider jwt,
    IRefreshTokenRepository refreshRepo) : IAuthService
{
    public async Task<AuthResult> LoginAsync(string email, string password, string ip, string ua, CancellationToken ct)
    {
        var user = await users.GetByEmail(email) ?? throw new UnauthorizedAccessException();
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
        await refreshRepo.Create(token);
        await refreshRepo.SaveChanges(ct);

        return new AuthResult(access, refreshPlain);
    }

    public async Task<AuthResult> RefreshAsync(string refreshPlain, string ip, string ua, CancellationToken ct)
    {
        var hash = Sha256(refreshPlain);
        var token = await refreshRepo.GetByHash(hash, ct) ?? throw new UnauthorizedAccessException();
        if (token.ExpiresAt <= DateTime.UtcNow) throw new UnauthorizedAccessException();

        token.RevokedAt = DateTime.UtcNow; // rotate family
        var user = token.User; // include user in repo query or re-fetch

        var now = DateTime.UtcNow;
        var access = jwt.GenerateAccessToken(user.Id, user.HouseholdId, user.Email, now);

        var newPlain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var newHash  = Sha256(newPlain);

        var newToken = new RefreshToken {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHash,
            CreatedAt = now,
            ExpiresAt = now.AddDays(30),
            ClientIp = ip,
            UserAgent = ua
        };

        await refreshRepo.Create(newToken);
        await refreshRepo.SaveChanges(ct);

        return new AuthResult(access, newPlain);
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string ip, string ua, CancellationToken ct)
    {
        var existingUser = await users.GetByEmail(email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var passwordHash = hasher.Hash(password);
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash
        };

        await users.Create(newUser);
        await users.SaveChanges(ct);

        // Automatically log in the user after registration
        return await LoginAsync(email, password, ip, ua, ct);
    }

    static string Sha256(string s)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
    }
}
public record AuthResult(string AccessToken, string RefreshToken);
