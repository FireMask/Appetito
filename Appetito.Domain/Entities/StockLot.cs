namespace Appetito.Domain.Entities;
public sealed class StockLot : BaseEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public Guid? StoreId { get; set; } //TODO: add Store entity
    // public Store? Store { get; set; } = null!;
    public DateTime PurchasedAt { get; set; }
    public decimal Quantity { get; set; }
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid? SourcePurchaseId { get; set; }
}