namespace BudgetExperiment.Domain;

/// <summary>
/// Repository interface for reading expense data.
/// </summary>
public interface IExpenseReadRepository : IReadRepository<Expense>
{
    /// <summary>
    /// Gets all expenses for a specific date range.
    /// </summary>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    /// <returns>A collection of expenses within the date range.</returns>
    Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);

    /// <summary>
    /// Gets all expenses for a specific date.
    /// </summary>
    /// <param name="date">The date to search for.</param>
    /// <returns>A collection of expenses on the specified date.</returns>
    Task<IReadOnlyList<Expense>> GetByDateAsync(DateOnly date);
}
