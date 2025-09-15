namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a weekly expense schedule.</summary>
public sealed class CreateWeeklyExpenseRequest
{
    /// <summary>Gets or sets the expense name (required).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the anchor (first occurrence) date.</summary>
    public DateOnly Anchor
    {
        get; set;
    }

    /// <summary>Gets or sets ISO currency code.</summary>
    public string Currency { get; set; } = "USD";

    /// <summary>Gets or sets expense amount (must be positive, will be stored as negative).</summary>
    public decimal Amount
    {
        get; set;
    }
}
