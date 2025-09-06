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
    /// <returns>Identifier.</returns>
    Guid CreateMonthly(string name, MoneyValue amount, DateOnly anchor);

    /// <summary>Get occurrences for bill schedule.</summary>
    /// <param name="id">Bill schedule id.</param>
    /// <param name="start">Range start.</param>
    /// <param name="end">Range end.</param>
    /// <returns>Due dates.</returns>
    IEnumerable<DateOnly> GetOccurrences(Guid id, DateOnly start, DateOnly end);
}
