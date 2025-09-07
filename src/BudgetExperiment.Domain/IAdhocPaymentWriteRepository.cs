namespace BudgetExperiment.Domain;

/// <summary>
/// Repository interface for writing AdhocPayment entities.
/// </summary>
public interface IAdhocPaymentWriteRepository : IWriteRepository<AdhocPayment>
{
    /// <summary>
    /// Deletes an existing adhoc payment.
    /// </summary>
    /// <param name="entity">The adhoc payment to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(AdhocPayment entity);
}
