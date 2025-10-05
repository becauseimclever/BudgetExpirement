using BudgetExperiment.Domain;

namespace BudgetExperiment.Application.Schedules;

/// <summary>
/// Unified service interface for managing recurring schedules (pay schedules and bill schedules).
/// </summary>
/// <typeparam name="TEntity">The schedule entity type.</typeparam>
/// <typeparam name="TDto">The DTO type for the schedule.</typeparam>
public interface IScheduleService<TEntity, TDto>
    where TEntity : class, ISchedule
    where TDto : class
{
    /// <summary>Create a schedule with specified recurrence pattern.</summary>
    /// <param name="anchor">First occurrence date.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval in days (required for Custom pattern).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateAsync(DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays = null, CancellationToken cancellationToken = default);

    /// <summary>Get a schedule by identifier.</summary>
    /// <param name="id">Schedule identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Schedule DTO or null if not found.</returns>
    Task<TDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>List schedules with pagination.</summary>
    /// <param name="page">1-based page number.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Items and total count.</returns>
    Task<(IReadOnlyList<TDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Get occurrence dates for a schedule within a date range.</summary>
    /// <param name="id">Schedule identifier.</param>
    /// <param name="start">Range start (inclusive).</param>
    /// <param name="end">Range end (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Occurrence dates.</returns>
    Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    /// <summary>Update a schedule.</summary>
    /// <param name="id">Schedule identifier.</param>
    /// <param name="anchor">Updated anchor date.</param>
    /// <param name="amount">Updated amount.</param>
    /// <param name="pattern">Updated recurrence pattern.</param>
    /// <param name="customIntervalDays">Updated custom interval days.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> UpdateAsync(Guid id, DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays = null, CancellationToken cancellationToken = default);

    /// <summary>Delete a schedule.</summary>
    /// <param name="id">Schedule identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
