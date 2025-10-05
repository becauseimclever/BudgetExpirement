namespace BudgetExperiment.Client.Api;

/// <summary>
/// Running total response for a specific day (client copy).
/// </summary>
public sealed class DailyRunningTotalResponse
{
    /// <summary>Gets or sets the date.</summary>
    public DateOnly Date
    {
        get; set;
    }

    /// <summary>Gets or sets the daily amount (sum of all transactions for this day).</summary>
    public decimal DailyAmount
    {
        get; set;
    }

    /// <summary>Gets or sets the running total up to and including this day.</summary>
    public decimal RunningTotal
    {
        get; set;
    }
}
