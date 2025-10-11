using BudgetExperiment.Domain;

namespace BudgetExperiment.Application.RecurringSchedules;

/// <summary>
/// Application service for managing recurring schedules (both income and expenses).
/// Replaces both IPayScheduleService and IBillScheduleService.
/// </summary>
public interface IRecurringScheduleService
{
    // Income schedule creation methods

    /// <summary>Create a weekly income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount (will be made positive).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateWeeklyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default);

    /// <summary>Create a monthly income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount (will be made positive).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateMonthlyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default);

    /// <summary>Create a bi-weekly (14 day) income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount (will be made positive).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateBiWeeklyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default);

    /// <summary>Create a custom day-interval income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount (will be made positive).</param>
    /// <param name="intervalDays">Interval in days (>=1).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateCustomIncomeAsync(DateOnly anchor, MoneyValue amount, int intervalDays, string? name = null, CancellationToken cancellationToken = default);

    /// <summary>Create an income schedule with any recurrence pattern.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount (will be made positive).</param>
    /// <param name="recurrence">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateIncomeAsync(DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null, string? name = null, CancellationToken cancellationToken = default);

    // Expense schedule creation methods

    /// <summary>Create a weekly expense schedule.</summary>
    /// <param name="name">Expense name (required).</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount (will be made negative).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateWeeklyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a monthly expense schedule.</summary>
    /// <param name="name">Expense name (required).</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount (will be made negative).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateMonthlyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a bi-weekly expense schedule.</summary>
    /// <param name="name">Expense name (required).</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount (will be made negative).</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateBiWeeklyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a custom day-interval expense schedule.</summary>
    /// <param name="name">Expense name (required).</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount (will be made negative).</param>
    /// <param name="intervalDays">Interval in days (>=1).</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateCustomExpenseAsync(string name, DateOnly anchor, MoneyValue amount, int intervalDays, CancellationToken cancellationToken = default);

    /// <summary>Create an expense schedule with any recurrence pattern.</summary>
    /// <param name="name">Expense name (required).</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount (will be made negative).</param>
    /// <param name="recurrence">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateExpenseAsync(string name, DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null, CancellationToken cancellationToken = default);

    // Query methods

    /// <summary>Get occurrences for schedule inside inclusive range.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="start">Range start (inclusive).</param>
    /// <param name="end">Range end (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Occurrences.</returns>
    Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    /// <summary>Get schedule details.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>DTO or null.</returns>
    Task<RecurringScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>List all schedules (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>List income schedules only (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListIncomeSchedulesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>List expense schedules only (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListExpenseSchedulesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    // Update and delete methods

    /// <summary>Update a recurring schedule.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="name">Updated name (can be null for income schedules).</param>
    /// <param name="amount">Updated amount.</param>
    /// <param name="anchor">Updated anchor date.</param>
    /// <param name="recurrence">Updated recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> UpdateAsync(Guid id, string? name, MoneyValue amount, DateOnly anchor, RecurrencePattern recurrence, int? customIntervalDays = null, CancellationToken cancellationToken = default);

    /// <summary>Delete a recurring schedule.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    // Autocomplete support methods

    /// <summary>Get distinct schedule names/descriptions for autocomplete.</summary>
    /// <param name="searchTerm">Optional search term to filter names (case-insensitive prefix match).</param>
    /// <param name="maxResults">Maximum number of results to return (default: 10, max: 50).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of distinct names matching the search criteria.</returns>
    Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(string? searchTerm = null, int maxResults = 10, CancellationToken cancellationToken = default);
}
