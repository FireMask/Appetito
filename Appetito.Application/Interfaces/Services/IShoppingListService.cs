public interface IShoppingListService
{
    Task<ShoppingListGenerateResponseDto> GenerateShoppingList(ShoppingListGenerateRequestDto request, CancellationToken ct);
}
