using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class CategoryRepository(AppetitoDbContext _context) : ICategoryRepository
{
    public async Task<IEnumerable<Category>> GetAll(CancellationToken ct = default)
    {
        return await _context.Categories.ToListAsync(ct);
    }

    public async Task<Category?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task Create(Category entity)
    {
        _context.Categories.Add(entity);
        return Task.CompletedTask;
    }

    public Task Update(Category entity)
    {
        _context.Categories.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(Category entity)
    {
        _context.Categories.Remove(entity);
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
