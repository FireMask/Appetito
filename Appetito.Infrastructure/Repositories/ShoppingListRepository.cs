using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class ShoppingListRepository(AppetitoDbContext _context) : IShoppingListRepository
{
    public async Task<IEnumerable<ShoppingList>> GetAll(CancellationToken ct = default)
    {
        return await _context.ShoppingLists.ToListAsync(ct);
    }

    public async Task<ShoppingList?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.ShoppingLists.FirstOrDefaultAsync(item => item.Id == id, ct);
    }

    public Task<ShoppingList?> GetByIdWithItems(Guid id, CancellationToken ct)
    {
        return _context.ShoppingLists
            .Include(sl => sl.Items)
            .FirstOrDefaultAsync(sl => sl.Id == id, ct);
    }

    public Task Create(ShoppingList entity)
    {
        _context.ShoppingLists.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(ShoppingList entity)
    {
        _context.ShoppingLists.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(ShoppingList entity)
    {
        _context.ShoppingLists.Remove(entity);
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