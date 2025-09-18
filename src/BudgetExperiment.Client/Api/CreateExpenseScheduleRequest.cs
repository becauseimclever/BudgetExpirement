namespace BudgetExperiment.Client.Api;

/// <summary>
/// Request to create an expense schedule.
/// </summary>
/// <param name="Name">Name for the expense (required).</param>
/// <param name="Anchor">Anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (will be made negative).</param>
/// <param name="Recurrence">Recurrence pattern.</param>
/// <param name="CustomIntervalDays">Custom interval days (required for Custom recurrence).</param>
public sealed record CreateExpenseScheduleRequest(
    string Name,
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    RecurrencePattern Recurrence,
    int? CustomIntervalDays = null);
