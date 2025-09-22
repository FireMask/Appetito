public interface IBaseInterface<T> where T : class
{
    public Task<T?> GetById(Guid id, CancellationToken ct = default);
    public Task<IEnumerable<T>> GetAll(CancellationToken ct = default);
    public Task Create(T entity);
    public Task Update(T entity);
    public Task Delete(T entity);
    public Task<int> SaveChanges(CancellationToken ct = default);
}