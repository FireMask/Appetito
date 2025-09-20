namespace Appetito.Domain.Entities;
public sealed class StockLot : BaseEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public Guid? StoreId { get; set; }                 // Store entity later
    public DateTime PurchasedAt { get; set; }
    public decimal Quantity { get; set; }              // base unit qty
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public decimal UnitPrice { get; set; }             // per UnitId
    public DateTime? ExpiresAt { get; set; }
    public Guid? SourcePurchaseId { get; set; }
}