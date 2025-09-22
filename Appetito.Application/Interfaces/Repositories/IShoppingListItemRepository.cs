using Appetito.Domain.Entities;

namespace Appetito.Application.Interfaces.Repositories;

public interface IShoppingListItemRepository : IBaseInterface<ShoppingListItem>
{
    Task<ICollection<ShoppingListItem>> GetByShoppingListId(Guid shoppingListId, CancellationToken ct);
}
