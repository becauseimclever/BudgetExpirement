namespace BudgetExperiment.Domain;

/// <summary>
/// Read-only repository for recurring schedules.
/// </summary>
public interface IRecurringScheduleReadRepository : IReadRepository<RecurringSchedule>
{
    /// <summary>Get income schedules (positive amounts).</summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paged income schedules.</returns>
    Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetIncomeSchedulesAsync(int pageNumber, int pageSize);

    /// <summary>Get expense schedules (negative amounts).</summary>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paged expense schedules.</returns>
    Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetExpenseSchedulesAsync(int pageNumber, int pageSize);

    /// <summary>Get schedules by type.</summary>
    /// <param name="scheduleType">Type of schedule to retrieve.</param>
    /// <param name="pageNumber">Page number (1-based).</param>
    /// <param name="pageSize">Page size.</param>
    /// <returns>Paged schedules of the specified type.</returns>
    Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetSchedulesByTypeAsync(ScheduleType scheduleType, int pageNumber, int pageSize);

    /// <summary>
    /// Gets distinct schedule names for autocomplete.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter names (case-insensitive prefix match).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 10).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of distinct names matching the search criteria.</returns>
    Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(string? searchTerm = null, int maxResults = 10, CancellationToken cancellationToken = default);
}
