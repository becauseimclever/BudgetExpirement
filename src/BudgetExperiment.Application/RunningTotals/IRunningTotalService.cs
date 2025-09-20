namespace BudgetExperiment.Application.RunningTotals;

/// <summary>
/// Service for calculating running totals with carryover from previous months.
/// </summary>
public interface IRunningTotalService
{
    /// <summary>
    /// Gets running totals for a specific month, including carryover from previous months.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Dictionary mapping dates to running totals.</returns>
    Task<Dictionary<DateOnly, DailyRunningTotalResponse>> GetRunningTotalsForMonthAsync(int year, int month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the running total at the end of a specific month.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The running total at the end of the month.</returns>
    Task<decimal> GetEndOfMonthTotalAsync(int year, int month, CancellationToken cancellationToken = default);
}
