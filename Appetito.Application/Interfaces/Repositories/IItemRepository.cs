using Appetito.Domain.Entities;

namespace Appetito.Application.Interfaces.Repositories;

public interface IItemRepository : IBaseInterface<Item>
{
    public Task<IEnumerable<Item>> GetAll(Guid householdId, string? search, CancellationToken ct = default);
    public Task<List<ShoppingListItemDto>> ItemsToRestock(ShoppingListGenerateRequestDto request, CancellationToken ct = default);
    public Task<Item?> GetById(Guid id, Guid householdId, CancellationToken ct = default);
}