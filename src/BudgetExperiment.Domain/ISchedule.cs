namespace BudgetExperiment.Domain;

/// <summary>
/// Common interface for all recurring schedule types.
/// </summary>
public interface ISchedule
{
    /// <summary>Gets the unique identifier.</summary>
    Guid Id { get; }

    /// <summary>Gets the anchor date (first occurrence).</summary>
    DateOnly Anchor { get; }

    /// <summary>Gets the amount associated with this schedule.</summary>
    MoneyValue Amount { get; }

    /// <summary>Gets the UTC creation timestamp.</summary>
    DateTime CreatedUtc { get; }

    /// <summary>Gets the UTC last update timestamp, null if never updated.</summary>
    DateTime? UpdatedUtc { get; }

    /// <summary>Generate occurrence dates within the given range.</summary>
    /// <param name="start">Start date (inclusive).</param>
    /// <param name="end">End date (inclusive).</param>
    /// <returns>Sequence of occurrence dates.</returns>
    IEnumerable<DateOnly> GetOccurrences(DateOnly start, DateOnly end);

    /// <summary>Marks the entity as updated.</summary>
    void MarkUpdated();
}
