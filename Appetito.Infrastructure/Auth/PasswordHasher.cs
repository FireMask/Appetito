public sealed class PasswordHasher : IPasswordHasher
{
    public bool Verify(string password, string hash)
    {
        return true; // TOOD: temporary disable password hashing
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}