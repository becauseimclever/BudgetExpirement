using BudgetExperiment.Domain;

namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create an income schedule with any recurrence pattern.</summary>
public sealed class CreateIncomeScheduleRequest
{
    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>Gets or sets income amount (must be positive).</summary>
    public decimal Amount
    {
        get; set;
    }

    /// <summary>Gets or sets optional name for the income schedule.</summary>
    public string? Name
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
