namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a recurring pay schedule (weekly or monthly) anchored to a specific date.
/// </summary>
public sealed class PaySchedule
{
    private PaySchedule()
    {
        // For EF Core
        this.Id = Guid.Empty;
        this.Anchor = default;
        this.Recurrence = RecurrenceKind.Weekly;
        this.Amount = MoneyValue.Zero("USD");
    }

    private PaySchedule(Guid id, DateOnly anchor, RecurrenceKind recurrence, MoneyValue amount, int? daysInterval)
    {
        this.Id = id;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
        this.Amount = amount;
        this.DaysInterval = daysInterval;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>Recurrence kind.</summary>
    public enum RecurrenceKind
    {
        /// <summary>Occurs every 7 days.</summary>
        Weekly,

        /// <summary>Occurs once per calendar month, clamped to month end if needed.</summary>
        Monthly,

        /// <summary>Occurs every 14 days.</summary>
        BiWeekly,

        /// <summary>Custom fixed day interval (uses <see cref="DaysInterval"/>).</summary>
        Custom,
    }

    /// <summary>Gets the anchor date (first occurrence).</summary>
    public DateOnly Anchor
    {
        get; private set;
    }

    /// <summary>Gets the unique identifier.</summary>
    public Guid Id
    {
        get; private set;
    }

    /// <summary>Gets the pay amount.</summary>
    public MoneyValue Amount { get; private set; }

    /// <summary>Gets the recurrence pattern.</summary>
    public RecurrenceKind Recurrence
    {
        get; private set;
    }

    /// <summary>Gets custom days interval for recurrence (only when Recurrence == Custom).</summary>
    public int? DaysInterval { get; private set; }

    /// <summary>Gets the UTC creation timestamp.</summary>
    public DateTime CreatedUtc
    {
        get; private set;
    }

    /// <summary>Gets the UTC last update timestamp, null if never updated.</summary>
    public DateTime? UpdatedUtc
    {
        get; private set;
    }

    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <returns>New <see cref="PaySchedule"/>.</returns>
    /// <summary>Create a weekly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <returns>Configured schedule.</returns>
    public static PaySchedule CreateWeekly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrenceKind.Weekly, amount, null);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <returns>New <see cref="PaySchedule"/>.</returns>
    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule CreateMonthly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrenceKind.Monthly, amount, null);

    /// <summary>Create a bi-weekly pay schedule (every 14 days).</summary>
    /// <summary>Create a bi-weekly (14 day) schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule CreateBiWeekly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrenceKind.BiWeekly, amount, null);

    /// <summary>Create a custom fixed-day interval schedule.</summary>
    /// <summary>Create a custom fixed-interval schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="intervalDays">Interval in days (>= 1).</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule CreateCustom(DateOnly anchor, MoneyValue amount, int intervalDays)
    {
        if (intervalDays < 1)
        {
            throw new DomainException("Interval must be >= 1 day.");
        }

        if (intervalDays == 7)
        {
            return CreateWeekly(anchor, amount);
        }

        if (intervalDays == 14)
        {
            return CreateBiWeekly(anchor, amount);
        }

        return new(Guid.NewGuid(), anchor, RecurrenceKind.Custom, amount, intervalDays);
    }

    /// <summary>Marks the entity as updated (for future mutable operations).</summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
    }

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
            RecurrenceKind.Weekly => this.EnumerateFixedInterval(rangeStart, rangeEnd, 7),
            RecurrenceKind.BiWeekly => this.EnumerateFixedInterval(rangeStart, rangeEnd, 14),
            RecurrenceKind.Monthly => this.EnumerateMonthly(rangeStart, rangeEnd),
            RecurrenceKind.Custom when this.DaysInterval.HasValue => this.EnumerateFixedInterval(rangeStart, rangeEnd, this.DaysInterval.Value),
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

    private IEnumerable<DateOnly> EnumerateFixedInterval(DateOnly rangeStart, DateOnly rangeEnd, int interval)
    {
        if (interval < 1)
        {
            yield break;
        }
        DateOnly first = this.Anchor;
        if (rangeStart > first)
        {
            var daysDiff = rangeStart.DayNumber - this.Anchor.DayNumber;
            var remainder = daysDiff % interval;
            first = remainder == 0 ? rangeStart : rangeStart.AddDays(interval - remainder);
        }
    for (var current = first; current <= rangeEnd; current = current.AddDays(interval))
        {
            if (current >= rangeStart)
            {
                yield return current;
            }
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
