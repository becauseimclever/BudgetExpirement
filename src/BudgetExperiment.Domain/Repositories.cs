namespace BudgetExperiment.Domain;

/// <summary>
/// Read operations for aggregates.
/// </summary>
/// <typeparam name="T">Aggregate type.</typeparam>
public interface IReadRepository<T>
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Write operations for aggregates.
/// </summary>
/// <typeparam name="T">Aggregate type.</typeparam>
public interface IWriteRepository<T>
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Unit of work abstraction for committing changes.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
