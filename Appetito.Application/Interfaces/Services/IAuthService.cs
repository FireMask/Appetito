public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password, string ip, string ua, CancellationToken ct);
    Task<AuthResult> RefreshAsync(string refreshToken, string ip, string ua, CancellationToken ct);
    Task<AuthResult> RegisterAsync(string email, string password, string ip, string ua, CancellationToken ct);
}
