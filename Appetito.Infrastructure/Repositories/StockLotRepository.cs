using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class StockLotRepository(AppetitoDbContext _context) : IStockLotRepository
{
    public async Task<IEnumerable<StockLot>> GetAll(CancellationToken ct = default)
    {
        return await _context.StockLots
            .Include(sl => sl.Item)
            .Include(sl => sl.Unit)
            .ToListAsync(ct);
    }

    public async Task<StockLot?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.StockLots
            .Include(sl => sl.Item)
            .Include(sl => sl.Unit)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task Create(StockLot entity)
    {
        _context.StockLots.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(StockLot entity)
    {
        _context.StockLots.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(StockLot entity)
    {
        _context.StockLots.Remove(entity);
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
