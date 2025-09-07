namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Application service for managing pay schedules.
/// </summary>
public interface IPayScheduleService
{
    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateMonthlyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a bi-weekly (14 day) schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateBiWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default);

    /// <summary>Create a custom day-interval schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="intervalDays">Interval in days (>=1).</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Identifier.</returns>
    Task<Guid> CreateCustomAsync(DateOnly anchor, MoneyValue amount, int intervalDays, CancellationToken cancellationToken = default);

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
    Task<PayScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>List schedules (paged).</summary>
    /// <param name="page">1-based page.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Token.</param>
    /// <returns>Items + total count.</returns>
    Task<(IReadOnlyList<PayScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}
