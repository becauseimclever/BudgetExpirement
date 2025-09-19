namespace BudgetExperiment.Domain;

/// <summary>
/// Write repository interface for adhoc transactions.
/// </summary>
public interface IAdhocTransactionWriteRepository
{
    /// <summary>
    /// Adds a new adhoc transaction.
    /// </summary>
    /// <param name="transaction">The transaction to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing adhoc transaction.
    /// </summary>
    /// <param name="transaction">The transaction to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an adhoc transaction.
    /// </summary>
    /// <param name="transaction">The transaction to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default);
}
