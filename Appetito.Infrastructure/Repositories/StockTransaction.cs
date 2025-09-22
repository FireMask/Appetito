using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class StockTransactionRepository(AppetitoDbContext _context) : IStockTransactionRepository
{
    public async Task<IEnumerable<StockTransaction>> GetAll(CancellationToken ct = default)
    {
        return await _context.StockTransactions
            .Include(st => st.Item)
            .Include(st => st.Lot)
            .Include(st => st.Unit)
            .ToListAsync(ct);
    }

    public async Task<StockTransaction?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.StockTransactions
            .Include(st => st.Item)
            .Include(st => st.Lot)
            .Include(st => st.Unit)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task Create(StockTransaction entity)
    {
        _context.StockTransactions.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(StockTransaction entity)
    {
        _context.StockTransactions.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(StockTransaction entity)
    {
        _context.StockTransactions.Remove(entity);
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
