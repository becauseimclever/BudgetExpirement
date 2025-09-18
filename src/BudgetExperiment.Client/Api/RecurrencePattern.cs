namespace BudgetExperiment.Client.Api;

/// <summary>
/// Standard recurrence patterns supported across all schedule types.
/// </summary>
public enum RecurrencePattern
{
    /// <summary>Occurs every 7 days.</summary>
    Weekly = 0,

    /// <summary>Occurs every 14 days.</summary>
    BiWeekly = 1,

    /// <summary>Occurs once per calendar month, clamped to month end if needed.</summary>
    Monthly = 2,

    /// <summary>Occurs every 3 months.</summary>
    Quarterly = 3,

    /// <summary>Occurs every 6 months.</summary>
    SemiAnnual = 4,

    /// <summary>Occurs once per year.</summary>
    Annual = 5,

    /// <summary>Custom fixed day interval (uses DaysInterval property).</summary>
    Custom = 6,
}
