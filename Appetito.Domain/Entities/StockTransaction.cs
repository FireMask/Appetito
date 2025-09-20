namespace Appetito.Domain.Entities;
public enum StockTxnType { IN=1, OUT=2, ADJUST=3, WASTE=4 }

public sealed class StockTransaction : BaseEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public Guid? LotId { get; set; }
    public StockLot? Lot { get; set; }
    public DateTime OccurredAt { get; set; }
    public StockTxnType Type { get; set; }
    public decimal QuantityDelta { get; set; }         // +IN/-OUT
    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;
    public string? Reason { get; set; }
    public string? RefType { get; set; }
    public Guid? RefId { get; set; }
}