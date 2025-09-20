public interface IPasswordHasher
{
    bool Verify(string password, string passwordHash);
    string Hash(string password);
}
