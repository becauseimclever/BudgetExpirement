namespace BudgetExperiment.Domain;

/// <summary>
/// Utilities for working with recurrence patterns.
/// </summary>
public static class RecurrencePatternExtensions
{
    /// <summary>
    /// Gets the default interval in days for standard recurrence patterns.
    /// </summary>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <returns>Default interval in days, or null for custom patterns.</returns>
    public static int? GetDefaultIntervalDays(this RecurrencePattern pattern) => pattern switch
    {
        RecurrencePattern.Weekly => 7,
        RecurrencePattern.BiWeekly => 14,
        RecurrencePattern.Monthly => 30, // Approximate for display purposes
        RecurrencePattern.Quarterly => 90, // Approximate for display purposes
        RecurrencePattern.SemiAnnual => 180, // Approximate for display purposes
        RecurrencePattern.Annual => 365, // Approximate for display purposes
        RecurrencePattern.Custom => null,
        _ => throw new ArgumentOutOfRangeException(nameof(pattern), pattern, null),
    };

    /// <summary>
    /// Gets a human-readable display name for the recurrence pattern.
    /// </summary>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <returns>Display name.</returns>
    public static string GetDisplayName(this RecurrencePattern pattern) => pattern switch
    {
        RecurrencePattern.Weekly => "Weekly",
        RecurrencePattern.BiWeekly => "Bi-Weekly",
        RecurrencePattern.Monthly => "Monthly",
        RecurrencePattern.Quarterly => "Quarterly",
        RecurrencePattern.SemiAnnual => "Semi-Annual",
        RecurrencePattern.Annual => "Annual",
        RecurrencePattern.Custom => "Custom",
        _ => pattern.ToString(),
    };
}
