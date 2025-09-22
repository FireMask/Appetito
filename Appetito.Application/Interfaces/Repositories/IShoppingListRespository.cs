using Appetito.Domain.Entities;

namespace Appetito.Application.Interfaces.Repositories;

public interface IShoppingListRepository : IBaseInterface<ShoppingList>
{
    Task<ShoppingList?> GetByIdWithItems(Guid id, CancellationToken ct);
}
