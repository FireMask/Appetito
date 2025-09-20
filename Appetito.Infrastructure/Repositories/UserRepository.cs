using Appetito.Application.Abstractions;
using Appetito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Appetito.Infrastructure.Repositories;

public sealed class UserRepository(AppetitoDbContext _context) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                throw new ArgumentException("User must have a valid ID", nameof(id));
            
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
        }catch(Exception ex)
        {
            return Task.FromException<User?>(ex);
        }
    }

     public Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email must be provided", nameof(email));

            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        catch (Exception ex)
        {
            return Task.FromException<User?>(ex);
        }
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public Task AddAsync(User user)
    {
        try
        {
            if (user.Id == Guid.Empty)
                user.Id = Guid.NewGuid();

            _context.Users.Add(user);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User must have a valid ID", nameof(id));

        var user = new User { Id = id };
        _context.Users.Remove(user);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user)
    {
        try
        {
            if (user.Id == Guid.Empty)
                throw new ArgumentException("User must have a valid ID", nameof(user.Id));

            _context.Users.Update(user);
            
            return Task.CompletedTask;
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