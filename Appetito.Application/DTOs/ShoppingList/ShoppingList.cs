using Appetito.Domain.Entities;

public sealed record ShoppingListGenerateRequestDto(
    DateTime? ForDate,          // optional: e.g. "next shopping trip date"
    Guid HouseholdId,           // required
    int ExpiringInDays = 7,     // include items expiring within X days
    bool IncludeInactiveItems = false,    // rarely needed, usually false
    bool IncludeAllLowStockItems = true // whether to include all items below reorder point
);

public sealed record ShoppingListGenerateResponseDto(
    Guid Id,
    Guid HouseholdId,
    DateTime CreatedAt,
    DateTime? ForDate,
    ShoppingListStatus Status,
    IReadOnlyList<ShoppingListItemDto> Items
);

public sealed record ShoppingListItemDto(
    Guid Id,
    Guid ItemId,
    string ItemName,
    decimal SuggestedQty,
    decimal? ConfirmedQty,
    Guid UnitId,
    string UnitName,
    SuggestionFlags ReasonFlags,
    string? Note
);