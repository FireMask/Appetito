namespace Appetito.Domain.Entities;
public enum ShoppingListStatus { Draft=0, Suggested=1, Finalized=2, Cancelled=3 }

[Flags]
public enum SuggestionFlags { None=0, LowStock=1, ExpiringSoon=2, CadenceDue=4, ManualAdd=8 }

public sealed class ShoppingList : BaseEntity
{
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? ForDate { get; set; }
    public ShoppingListStatus Status { get; set; }
    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}

public sealed class ShoppingListItem : BaseEntity
{
    public Guid ShoppingListId { get; set; }
    public ShoppingList ShoppingList { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public decimal SuggestedQty { get; set; }
    public decimal? ConfirmedQty { get; set; }
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public SuggestionFlags ReasonFlags { get; set; }
    public string? Note { get; set; }
}