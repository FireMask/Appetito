using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class ShoppingListItemRepository(AppetitoDbContext _context) : IShoppingListItemRepository
{
    public async Task<IEnumerable<ShoppingListItem>> GetAll(CancellationToken ct = default)
    {
        return await _context.ShoppingListItems.ToListAsync(ct);
    }

    public async Task<ShoppingListItem?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.ShoppingListItems.FirstOrDefaultAsync(item => item.Id == id, ct);
    }

    public async Task<ICollection<ShoppingListItem>> GetByShoppingListId(Guid shoppingListId, CancellationToken ct)
    {
        return await _context.ShoppingListItems
            .Where(item => item.ShoppingListId == shoppingListId)
            .ToListAsync(ct);
    }

    public Task Create(ShoppingListItem entity)
    {
        _context.ShoppingListItems.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(ShoppingListItem entity)
    {
        _context.ShoppingListItems.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(ShoppingListItem entity)
    {
        _context.ShoppingListItems.Remove(entity);
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