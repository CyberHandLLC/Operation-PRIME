namespace OperationPrime.Domain.Interfaces;

/// <summary>
/// Generic repository interface providing basic CRUD operations.
/// </summary>
public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
