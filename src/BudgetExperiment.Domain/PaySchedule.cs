namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a recurring pay schedule (weekly or monthly) anchored to a specific date.
/// </summary>
public sealed class PaySchedule
{
    private PaySchedule(Guid id, DateOnly anchor, RecurrenceKind recurrence)
    {
        this.Id = id;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
    }

    /// <summary>Recurrence kind.</summary>
    public enum RecurrenceKind
    {
        /// <summary>Occurs every 7 days.</summary>
        Weekly,

        /// <summary>Occurs once per calendar month, clamped to month end if needed.</summary>
        Monthly,
    }

    /// <summary>Gets the anchor date (first occurrence).</summary>
    public DateOnly Anchor
    {
        get;
    }

    /// <summary>Gets the unique identifier.</summary>
    public Guid Id { get; }

    /// <summary>Gets the recurrence pattern.</summary>
    public RecurrenceKind Recurrence
    {
        get;
    }

    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <returns>New <see cref="PaySchedule"/>.</returns>
    public static PaySchedule CreateWeekly(DateOnly anchor) => new(Guid.NewGuid(), anchor, RecurrenceKind.Weekly);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <returns>New <see cref="PaySchedule"/>.</returns>
    public static PaySchedule CreateMonthly(DateOnly anchor) => new(Guid.NewGuid(), anchor, RecurrenceKind.Monthly);

    /// <summary>
    /// Gets all occurrence dates within the inclusive range.
    /// </summary>
    /// <param name="rangeStart">Range start date (inclusive).</param>
    /// <param name="rangeEnd">Range end date (inclusive).</param>
    /// <returns>Sequence of occurrence dates.</returns>
    /// <exception cref="DomainException">When rangeStart &gt; rangeEnd.</exception>
    public IEnumerable<DateOnly> GetOccurrences(DateOnly rangeStart, DateOnly rangeEnd)
    {
        if (rangeEnd < rangeStart)
        {
            throw new DomainException("Start date must be before or equal to end date.");
        }

        if (rangeEnd < this.Anchor)
        {
            yield break; // whole range before anchor
        }

        IEnumerable<DateOnly> iterator = this.Recurrence switch
        {
            RecurrenceKind.Weekly => this.EnumerateWeekly(rangeStart, rangeEnd),
            RecurrenceKind.Monthly => this.EnumerateMonthly(rangeStart, rangeEnd),
            _ => throw new DomainException("Unsupported recurrence kind."),
        };

        foreach (var d in iterator)
        {
            yield return d;
        }
    }

    private static DateOnly AddMonthClamped(DateOnly date, int months, int anchorDay)
    {
        var newMonthDate = date.AddMonths(months);
        int daysInMonth = DateTime.DaysInMonth(newMonthDate.Year, newMonthDate.Month);
        int day = Math.Min(anchorDay, daysInMonth);
        return new DateOnly(newMonthDate.Year, newMonthDate.Month, day);
    }

    private IEnumerable<DateOnly> EnumerateWeekly(DateOnly rangeStart, DateOnly rangeEnd)
    {
        DateOnly first = this.Anchor;
        if (rangeStart > first)
        {
            var daysDiff = rangeStart.DayNumber - this.Anchor.DayNumber;
            var remainder = daysDiff % 7;
            if (remainder == 0)
            {
                first = rangeStart;
            }
            else
            {
                var add = 7 - remainder;
                first = rangeStart.AddDays(add);
            }
        }

        for (var current = first; current <= rangeEnd; current = current.AddDays(7))
        {
            if (current < rangeStart)
            {
                continue;
            }

            yield return current;
        }
    }

    private IEnumerable<DateOnly> EnumerateMonthly(DateOnly rangeStart, DateOnly rangeEnd)
    {
        var anchorDay = this.Anchor.Day;
        DateOnly current = this.Anchor;

        while (current < rangeStart)
        {
            current = AddMonthClamped(current, 1, anchorDay);
        }

        while (current <= rangeEnd)
        {
            if (current >= rangeStart)
            {
                yield return current;
            }

            current = AddMonthClamped(current, 1, anchorDay);
        }
    }
}
