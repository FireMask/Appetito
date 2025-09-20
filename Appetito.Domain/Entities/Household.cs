namespace Appetito.Domain.Entities;
public sealed class Household : BaseEntity
{
    public string Name { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}