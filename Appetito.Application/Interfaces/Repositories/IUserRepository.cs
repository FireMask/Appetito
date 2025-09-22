using Appetito.Domain.Entities;

namespace Appetito.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseInterface<User>
{
    Task<User?> GetByEmail(string email, CancellationToken ct = default);
}