namespace BudgetExperiment.Client.Api;

/// <summary>
/// Bill schedule item data from API (record version for calendar use).
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="Name">The bill name.</param>
/// <param name="Anchor">The anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the bill.</param>
/// <param name="Recurrence">Recurrence type as integer.</param>
/// <param name="CreatedUtc">When this record was created.</param>
/// <param name="UpdatedUtc">When this record was last updated.</param>
public sealed record BillScheduleItem(
    Guid Id,
    string Name,
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    int Recurrence,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc);
