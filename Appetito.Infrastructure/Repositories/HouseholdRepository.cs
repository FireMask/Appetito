using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class HouseholdRepository(AppetitoDbContext _context) : IHouseholdRepository
{
    public async Task<IEnumerable<Household>> GetAll(CancellationToken ct = default)
    {
        return await _context.Households
            .Include(h => h.Members)
            .ToListAsync(ct);
    }

    public async Task<Household?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Households
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == id, ct);
    }

    public Task Create(Household entity)
    {
        _context.Households.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(Household entity)
    {
        _context.Households.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(Household entity)
    {
        _context.Households.Remove(entity);
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
