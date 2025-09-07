namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Application service for managing pay schedules.
/// </summary>
public interface IPayScheduleService
{
    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateWeeklyAsync(DateOnly anchor, CancellationToken cancellationToken = default);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Identifier of created schedule.</returns>
    Task<Guid> CreateMonthlyAsync(DateOnly anchor, CancellationToken cancellationToken = default);

    /// <summary>Get occurrences for schedule inside inclusive range.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="start">Range start (inclusive).</param>
    /// <param name="end">Range end (inclusive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Occurrences.</returns>
    Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default);
}
