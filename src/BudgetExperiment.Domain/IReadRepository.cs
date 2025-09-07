namespace BudgetExperiment.Domain;

/// <summary>
/// Read operations for aggregates.
/// </summary>
/// <typeparam name="T">Aggregate type.</typeparam>
public interface IReadRepository<T>
{
    /// <summary>
    /// Gets an aggregate by id or null if not found.
    /// </summary>
    /// <param name="id">Aggregate identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate or null.</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
