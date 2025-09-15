namespace BudgetExperiment.Api.Dtos;

using BudgetExperiment.Domain;

/// <summary>Request to update a recurring schedule.</summary>
public sealed class UpdateRecurringScheduleRequest
{
    /// <summary>Gets or sets the schedule name (can be null for income schedules).</summary>
    public string? Name
    {
        get; set;
    }

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Gets or sets the amount.</summary>
    public decimal Amount
    {
        get; set;
    }

    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }

    /// <summary>Gets or sets the recurrence pattern.</summary>
    public RecurrencePattern Recurrence
    {
        get; set;
    }

    /// <summary>Gets or sets custom interval days (required for Custom pattern).</summary>
    public int? CustomIntervalDays
    {
        get; set;
    }
}
