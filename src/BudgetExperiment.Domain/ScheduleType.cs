namespace BudgetExperiment.Domain;

/// <summary>
/// Enum to categorize the type of recurring schedule for UI and reporting purposes.
/// </summary>
public enum ScheduleType
{
    /// <summary>Income schedule (typically positive amounts).</summary>
    Income,

    /// <summary>Expense schedule (typically negative amounts).</summary>
    Expense,
}
