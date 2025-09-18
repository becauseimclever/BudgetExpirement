namespace BudgetExperiment.Client.Api;

/// <summary>
/// Request to create an income schedule.
/// </summary>
/// <param name="Anchor">Anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (will be made positive).</param>
/// <param name="Recurrence">Recurrence pattern.</param>
/// <param name="CustomIntervalDays">Custom interval days (required for Custom recurrence).</param>
/// <param name="Name">Optional name for the income schedule.</param>
public sealed record CreateIncomeScheduleRequest(
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    RecurrencePattern Recurrence,
    int? CustomIntervalDays = null,
    string? Name = null);
