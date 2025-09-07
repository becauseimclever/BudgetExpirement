namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a weekly pay schedule.</summary>
public sealed class CreateWeeklyPayScheduleRequest
{
    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Gets or sets amount.</summary>
    public decimal Amount
    {
        get; set;
    }
}
