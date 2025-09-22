using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class UnitRepository(AppetitoDbContext _context) : IUnitRepository
{
    public async Task<IEnumerable<Unit>> GetAll(CancellationToken ct = default)
    {
        return await _context.Units.ToListAsync(ct);
    }

    public async Task<Unit?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Units.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public Task Create(Unit entity)
    {
        _context.Units.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(Unit entity)
    {
        _context.Units.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(Unit entity)
    {
        _context.Units.Remove(entity);
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