using Appetito.Application.Abstractions;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class ItemRepository(AppetitoDbContext _context) : IItemRepository
{
    public Task AddAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();

            _context.Items.Add(item);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task DeleteAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            if (item.Id == Guid.Empty)
                throw new ArgumentException("Item must have a valid ID", nameof(item));

            _context.Items.Remove(item);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task<Item?> GetAsync(Guid id, Guid householdId, CancellationToken ct = default)
    {
        return _context.Items
            .Where(i => i.Id == id && i.HouseholdId == householdId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<Item>> ListAsync(Guid householdId, string? search, CancellationToken ct = default)
    {
        return await _context.Items
            .Where(i => i.HouseholdId == householdId && (string.IsNullOrEmpty(search) || i.Name.Contains(search)))
            .ToListAsync(ct);
    }

    public Task UpdateAsync(Item item, CancellationToken ct = default)
    {
        try
        {
            if (item.Id == Guid.Empty)
                throw new ArgumentException("Item must have a valid ID", nameof(item));

            _context.Items.Update(item);
            return _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }
    
    public Task SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }
}
