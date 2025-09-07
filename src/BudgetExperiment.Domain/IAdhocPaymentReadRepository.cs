namespace BudgetExperiment.Domain;

/// <summary>
/// Repository interface for reading AdhocPayment entities.
/// </summary>
public interface IAdhocPaymentReadRepository : IReadRepository<AdhocPayment>
{
    /// <summary>
    /// Gets adhoc payments within a date range.
    /// </summary>
    /// <param name="startDate">Start date (inclusive).</param>
    /// <param name="endDate">End date (inclusive).</param>
    /// <returns>List of adhoc payments in the date range.</returns>
    Task<IReadOnlyList<AdhocPayment>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}
