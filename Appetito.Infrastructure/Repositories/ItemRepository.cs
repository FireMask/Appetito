using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class ItemRepository(AppetitoDbContext _context) : IItemRepository
{
    public async Task<Item?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Items
            .Where(i => i.Id == id)
            .Include(i => i.Household)
            .Include(i => i.Category)
            .Include(i => i.DefaultUnit)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Item>> GetAll(CancellationToken ct = default)
    {
        return await _context.Items
            .Include(i => i.Household)
            .Include(i => i.Category)
            .Include(i => i.DefaultUnit)
            .ToListAsync(ct);
    }

    public async Task<List<ShoppingListItemDto>> ItemsToRestock(ShoppingListGenerateRequestDto request, CancellationToken ct = default)
    {
        var expiringThreshold = DateTime.UtcNow.AddDays(request.ExpiringInDays);

        // Get all items for the household
        var items = await _context.Items
            .Where(i => i.HouseholdId == request.HouseholdId)
            .Include(i => i.DefaultUnit)
            .ToListAsync(ct);

        // Get all stock lots for those items
        var stockLots = await _context.StockLots
            .Where(s => s.Item.HouseholdId == request.HouseholdId)
            .ToListAsync(ct);

        // Prepare a lookup for stock by item
        var stockByItem = stockLots
            .GroupBy(s => s.ItemId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var itemsToRestock = new List<ShoppingListItemDto>();

        foreach (var item in items)
        {
            var unit = item.DefaultUnit;
            var hasStock = stockByItem.TryGetValue(item.Id, out var _lots) && _lots.Any();
            var lots = _lots ?? new List<StockLot>();

            // If no stock, suggest target qty
            if (!hasStock)
            {
                if ((request.IncludeInactiveItems || item.IsActive) &&
                    (request.IncludeAllLowStockItems || item.TargetQty > 0) &&
                    item.ReorderPoint > 0)
                {
                    itemsToRestock.Add(new ShoppingListItemDto(
                        Guid.NewGuid(),
                        item.Id,
                        item.Name,
                        item.TargetQty,
                        null,
                        item.DefaultUnitId,
                        unit?.Name ?? "",
                        SuggestionFlags.LowStock | SuggestionFlags.ExpiringSoon,
                        "No stock available"
                    ));
                }
                continue;
            }

            // Otherwise, check for expiring/low stock lots
            foreach (var stock in lots)
            {
                if ((request.IncludeInactiveItems || item.IsActive) &&
                    (request.IncludeAllLowStockItems || item.TargetQty > 0) &&
                    item.ReorderPoint > 0 &&
                    stock.ExpiresAt <= expiringThreshold &&
                    stock.ExpiresAt > DateTime.UtcNow &&
                    stock.Quantity <= item.ReorderPoint)
                {
                    itemsToRestock.Add(new ShoppingListItemDto(
                        Guid.NewGuid(),
                        item.Id,
                        item.Name,
                        item.TargetQty - stock.Quantity,
                        null,
                        item.DefaultUnitId,
                        unit?.Name ?? "",
                        SuggestionFlags.LowStock | SuggestionFlags.ExpiringSoon,
                        "Auto-generated suggestion"
                    ));
                }
            }
        }

        itemsToRestock = itemsToRestock
            .DistinctBy(i => i.ItemId)
            .ToList();

        return itemsToRestock;
    }

    public Task<Item?> GetById(Guid id, Guid householdId, CancellationToken ct = default)
    {
        return _context.Items
            .Where(i => i.Id == id && i.HouseholdId == householdId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Item>> GetAll(Guid householdId, string? search, CancellationToken ct = default)
    {
        return await _context.Items
            .Where(i => i.HouseholdId == householdId && (string.IsNullOrEmpty(search) || i.Name.Contains(search)))
            .ToListAsync(ct);
    }

    public Task Create(Item item)
    {
        _context.Items.Add(item);
        return Task.CompletedTask;
    }

    public Task Update(Item item)
    {
        _context.Items.Update(item);
        return Task.CompletedTask;
    }

    public Task Delete(Item item)
    {
        _context.Items.Remove(item);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChanges(CancellationToken ct = default)
    {
        try
        {
            return await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            Console.Error.WriteLine($"An error occurred while updating the database: {ex.Message}");
            throw;
        }
    }
}
