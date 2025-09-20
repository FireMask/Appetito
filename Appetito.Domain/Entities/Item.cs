namespace Appetito.Domain.Entities;
public sealed class Item : BaseEntity
{
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid DefaultUnitId { get; set; }
    public Unit DefaultUnit { get; set; } = null!;
    public decimal ReorderPoint { get; set; }
    public decimal TargetQty { get; set; }
    public int? ShelfLifeDays { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}