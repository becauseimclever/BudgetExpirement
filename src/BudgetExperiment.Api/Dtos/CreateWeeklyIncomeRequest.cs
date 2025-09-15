namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a weekly income schedule.</summary>
public sealed class CreateWeeklyIncomeRequest
{
    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = "USD";

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
}
