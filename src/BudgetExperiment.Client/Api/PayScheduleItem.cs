namespace BudgetExperiment.Client.Api;

/// <summary>
/// Pay schedule item data from API (record version for calendar use).
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="Anchor">The anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the payment.</param>
/// <param name="DaysInterval">Custom interval days (null if pattern not custom).</param>
/// <param name="CreatedUtc">When this record was created.</param>
/// <param name="UpdatedUtc">When this record was last updated.</param>
/// <param name="Recurrence">Recurrence kind.</param>
public sealed record PayScheduleItem(
    Guid Id,
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    int? DaysInterval,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc,
    RecurrenceKind Recurrence);
