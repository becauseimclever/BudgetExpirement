namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a recurring pay schedule (weekly or monthly) anchored to a specific date.
/// </summary>
public sealed class PaySchedule : ISchedule
{
    private PaySchedule()
    {
        // For EF Core
        this.Id = Guid.Empty;
        this.Anchor = default;
        this.Recurrence = RecurrencePattern.Weekly;
        this.Amount = MoneyValue.Zero("USD");
    }

    private PaySchedule(Guid id, DateOnly anchor, RecurrencePattern recurrence, MoneyValue amount, int? daysInterval)
    {
        this.Id = id;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
        this.Amount = amount;
        this.DaysInterval = daysInterval;
        this.CreatedUtc = DateTime.UtcNow;
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
    public MoneyValue Amount
    {
        get; private set;
    }

    /// <summary>Gets the recurrence pattern.</summary>
    public RecurrencePattern Recurrence
    {
        get; private set;
    }

    /// <summary>Gets custom days interval for recurrence (only when Recurrence == Custom).</summary>
    public int? DaysInterval
    {
        get; private set;
    }

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
    /// <param name="amount">Pay amount.</param>
    /// <returns>Configured schedule.</returns>
    public static PaySchedule CreateWeekly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrencePattern.Weekly, amount, null);

    /// <summary>Create a monthly pay schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Pay amount.</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule CreateMonthly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrencePattern.Monthly, amount, null);

    /// <summary>Create a bi-weekly (14 day) schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Pay amount.</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule CreateBiWeekly(DateOnly anchor, MoneyValue amount) => new(Guid.NewGuid(), anchor, RecurrencePattern.BiWeekly, amount, null);

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

        return new(Guid.NewGuid(), anchor, RecurrencePattern.Custom, amount, intervalDays);
    }

    /// <summary>Create a pay schedule with any supported recurrence pattern.</summary>
    /// <param name="anchor">First occurrence date.</param>
    /// <param name="amount">Pay amount.</param>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <returns>Schedule.</returns>
    public static PaySchedule Create(DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays = null)
    {
        return pattern switch
        {
            RecurrencePattern.Weekly => CreateWeekly(anchor, amount),
            RecurrencePattern.BiWeekly => CreateBiWeekly(anchor, amount),
            RecurrencePattern.Monthly => CreateMonthly(anchor, amount),
            RecurrencePattern.Quarterly => new(Guid.NewGuid(), anchor, pattern, amount, null),
            RecurrencePattern.SemiAnnual => new(Guid.NewGuid(), anchor, pattern, amount, null),
            RecurrencePattern.Annual => new(Guid.NewGuid(), anchor, pattern, amount, null),
            RecurrencePattern.Custom => CreateCustom(anchor, amount, customIntervalDays ?? throw new DomainException("Custom interval days required for Custom pattern.")),
            _ => throw new ArgumentOutOfRangeException(nameof(pattern), pattern, "Unsupported recurrence pattern."),
        };
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
    public IEnumerable<DateOnly> GetOccurrences(DateOnly rangeStart, DateOnly rangeEnd)
    {
        return ScheduleCalculator.CalculateOccurrences(this.Anchor, this.Recurrence, this.DaysInterval, rangeStart, rangeEnd);
    }

    /// <summary>Update the pay amount.</summary>
    /// <param name="amount">New pay amount.</param>
    public void UpdateAmount(MoneyValue amount)
    {
        ArgumentNullException.ThrowIfNull(amount);
        this.Amount = amount;
        this.MarkUpdated();
    }

    /// <summary>Update the anchor date.</summary>
    /// <param name="anchor">New anchor date.</param>
    public void UpdateAnchor(DateOnly anchor)
    {
        this.Anchor = anchor;
        this.MarkUpdated();
    }

    /// <summary>Update the recurrence pattern.</summary>
    /// <param name="pattern">New recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    public void UpdateRecurrence(RecurrencePattern pattern, int? customIntervalDays = null)
    {
        if (pattern == RecurrencePattern.Custom && (!customIntervalDays.HasValue || customIntervalDays.Value <= 0))
        {
            throw new DomainException("Custom recurrence pattern requires a valid interval in days.");
        }

        this.Recurrence = pattern;
        this.DaysInterval = pattern == RecurrencePattern.Custom ? customIntervalDays : null;
        this.MarkUpdated();
    }
}
