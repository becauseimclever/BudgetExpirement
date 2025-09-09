namespace BudgetExperiment.Api.Dtos;

/// <summary>Request to update a bill schedule.</summary>
public sealed class UpdateBillScheduleRequest
{
    /// <summary>Gets or sets the bill name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the currency (3-letter code).</summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>Gets or sets the amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Gets or sets the due date anchor.</summary>
    public DateOnly Anchor { get; set; }

    /// <summary>Gets or sets the recurrence type (0=Weekly, 1=BiWeekly, 2=Monthly, 3=Quarterly, 4=SemiAnnual, 5=Annual).</summary>
    public int Recurrence { get; set; }
}
