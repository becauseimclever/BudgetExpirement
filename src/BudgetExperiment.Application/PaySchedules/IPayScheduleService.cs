namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Application service for managing pay schedules.
/// </summary>
public interface IPayScheduleService
{
    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <returns>Identifier of created schedule.</returns>
    Guid CreateWeekly(DateOnly anchor);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <returns>Identifier of created schedule.</returns>
    Guid CreateMonthly(DateOnly anchor);

    /// <summary>Get occurrences for schedule inside inclusive range.</summary>
    /// <param name="id">Schedule id.</param>
    /// <param name="start">Range start (inclusive).</param>
    /// <param name="end">Range end (inclusive).</param>
    /// <returns>Occurrences.</returns>
    IEnumerable<DateOnly> GetOccurrences(Guid id, DateOnly start, DateOnly end);
}
