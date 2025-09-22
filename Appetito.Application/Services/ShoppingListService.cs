using System.Security.Cryptography;
using System.Text;
using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;

public sealed class ShoppingListService(IItemRepository _itemRepository) : IShoppingListService
{
    public async Task<ShoppingListGenerateResponseDto> GenerateShoppingList(ShoppingListGenerateRequestDto request, CancellationToken ct)
    {
        if (request.HouseholdId == Guid.Empty)
            throw new ArgumentException("HouseholdId is required.", nameof(request.HouseholdId));

        var shoppingList = new ShoppingList
        {
            HouseholdId = request.HouseholdId,
            CreatedAt = DateTime.UtcNow,
            Status = ShoppingListStatus.Draft
        };

        // Fetch items based on the request criteria
        List<ShoppingListItemDto> shoppingListItems = await _itemRepository.ItemsToRestock(request, ct);

        if (shoppingListItems is null || !shoppingListItems.Any())
            return new ShoppingListGenerateResponseDto(
                shoppingList.Id,
                shoppingList.HouseholdId,
                shoppingList.CreatedAt,
                null,
                ShoppingListStatus.Draft,
                new List<ShoppingListItemDto>()
            );

        var shoppingListResult = new ShoppingListGenerateResponseDto(
            shoppingList.Id,
            shoppingList.HouseholdId,
            shoppingList.CreatedAt,
            null,
            ShoppingListStatus.Draft,
            shoppingListItems
        );

        return shoppingListResult;
    }
}