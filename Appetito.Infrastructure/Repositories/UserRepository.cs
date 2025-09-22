using Appetito.Application.Interfaces.Repositories;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class UserRepository(AppetitoDbContext _context, IPasswordHasher _passwordHasher) : IUserRepository
{
    

    public async Task<User?> GetById(Guid id, CancellationToken ct = default)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.Household)
            .FirstOrDefaultAsync(ct);
    }

     public Task<User?> GetByEmail(string email, CancellationToken ct = default)
    {
        return _context.Users
            .Where(u => u.Email == email)
            .Include(u => u.Household)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IEnumerable<User>> GetAll(CancellationToken ct = default)
    {
        return await _context.Users
            .Include(u => u.Household)
            .ToListAsync(ct);
    }

    public Task Create(User user)
    {
        user.PasswordHash = _passwordHasher.Hash(user.PasswordHash);
        _context.Users.Add(user);
        return Task.CompletedTask;
    }

    public Task Update(User user)
    {
        _context.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task Delete(User user)
    {
        _context.Users.Remove(user);
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