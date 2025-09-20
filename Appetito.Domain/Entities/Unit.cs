namespace Appetito.Domain.Entities;
public sealed class Unit : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Abbrev { get; set; } = null!;
    public bool IsCountable { get; set; }              // true => pieces
    public Guid? BaseUnitId { get; set; }              // for conversions (e.g., kg -> g)
    public decimal FactorToBase { get; set; } = 1m;
}