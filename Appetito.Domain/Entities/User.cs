namespace Appetito.Domain.Entities;
public sealed class User : BaseEntity
{
    public Guid HouseholdId { get; set; }
    public Household? Household { get; set; } = null;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}