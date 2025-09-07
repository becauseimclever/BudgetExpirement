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
}
