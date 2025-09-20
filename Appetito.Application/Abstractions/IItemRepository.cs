using Appetito.Domain.Entities;
namespace Appetito.Application.Abstractions;

public interface IItemRepository
{
    Task<Item?> GetAsync(Guid id, Guid householdId, CancellationToken ct = default);
    Task<IEnumerable<Item>> ListAsync(Guid householdId, string? search, CancellationToken ct = default);
    Task AddAsync(Item item, CancellationToken ct = default);
    Task DeleteAsync(Item item, CancellationToken ct = default);
    Task UpdateAsync(Item item, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    // Task<bool> ExistsAsync(Guid id, Guid householdId, CancellationToken ct = default);
    // Task<int> CountAsync(Guid householdId, CancellationToken ct = default);
    // Task<IReadOnlyList<Item>> ListByCategoryAsync(Guid householdId, string category, CancellationToken ct = default);
    // Task<IReadOnlyList<Item>> ListExpiringItemsAsync(Guid householdId, int days, CancellationToken ct = default);
    // Task<IReadOnlyList<Item>> ListOutOfStockItemsAsync(Guid householdId, CancellationToken ct = default);
}
