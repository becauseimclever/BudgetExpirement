namespace BudgetExperiment.Client.Api;

/// <summary>
/// Recurrence pattern for pay schedules.
/// </summary>
public enum RecurrenceKind
{
    /// <summary>Weekly recurrence.</summary>
    Weekly,

    /// <summary>Monthly recurrence.</summary>
    Monthly,

    /// <summary>Bi-weekly recurrence.</summary>
    BiWeekly,

    /// <summary>Custom interval recurrence.</summary>
    Custom,
}
