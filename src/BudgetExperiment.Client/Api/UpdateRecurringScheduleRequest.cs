namespace BudgetExperiment.Client.Api;

/// <summary>
/// Request to update a recurring schedule.
/// </summary>
/// <param name="Name">Schedule name.</param>
/// <param name="Anchor">Anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount.</param>
/// <param name="Recurrence">Recurrence pattern.</param>
/// <param name="CustomIntervalDays">Custom interval days.</param>
public sealed record UpdateRecurringScheduleRequest(
    string? Name,
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    RecurrencePattern Recurrence,
    int? CustomIntervalDays = null);
