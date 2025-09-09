namespace BudgetExperiment.Application.BillSchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Application service for managing bill schedules.
/// </summary>
public interface IBillScheduleService
{
    /// <summary>Create a monthly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="anchor">First due date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateMonthlyAsync(string name, MoneyValue amount, DateOnly anchor, CancellationToken cancellationToken = default);

    /// <summary>Create a bill schedule with specified recurrence.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="anchor">First due date.</param>
    /// <param name="recurrence">Recurrence type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateAsync(string name, MoneyValue amount, DateOnly anchor, BillSchedule.RecurrenceKind recurrence, CancellationToken cancellationToken = default);

    /// <summary>Get occurrences for bill schedule.</summary>
    /// <param name="id">Bill schedule id.</param>
    /// <param name="start">Range start.</param>
    /// <param name="end">Range end.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Due dates.</returns>
    Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);

    /// <summary>Get a bill schedule details.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>DTO or null if not found.</returns>
    Task<BillScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>List bill schedules (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<BillScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Update a bill schedule.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="name">Updated bill name.</param>
    /// <param name="amount">Updated amount.</param>
    /// <param name="anchor">Updated anchor date.</param>
    /// <param name="recurrence">Updated recurrence type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if updated, false if not found.</returns>
    Task<bool> UpdateAsync(Guid id, string name, MoneyValue amount, DateOnly anchor, BillSchedule.RecurrenceKind recurrence, CancellationToken cancellationToken = default);

    /// <summary>Delete a bill schedule.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
