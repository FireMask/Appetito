public interface IJwtProvider
{
    string GenerateAccessToken(Guid userId, Guid householdId, string email, DateTime nowUtc);
}