namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to create a monthly bill schedule.</summary>
public sealed class CreateMonthlyBillScheduleRequest
{
    /// <summary>Gets or sets the bill name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the currency (3-letter code).</summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>Gets or sets the amount.</summary>
    public decimal Amount
    {
        get; set;
    }

    /// <summary>Gets or sets the first due date anchor.</summary>
    public DateOnly Anchor
    {
        get; set;
    }
}
