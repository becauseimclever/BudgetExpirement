namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a recurring bill with amount and various recurrence options.
/// </summary>
public sealed class BillSchedule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BillSchedule"/> class for EF Core.
    /// </summary>
    private BillSchedule()
    {
        // For EF Core
        this.Id = Guid.Empty;
        this.Name = string.Empty;
        this.Amount = MoneyValue.Create("XXX", 0m);
        this.Anchor = default;
        this.Recurrence = RecurrenceKind.Monthly;
        this.DaysInterval = null;
    }

    private BillSchedule(Guid id, string name, MoneyValue amount, DateOnly anchor, RecurrenceKind recurrence, int? daysInterval = null)
    {
        this.Id = id;
        this.Name = name;
        this.Amount = amount;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
        this.DaysInterval = daysInterval;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>Recurrence kinds supported.</summary>
    public enum RecurrenceKind
    {
        /// <summary>Weekly recurrence (every 7 days).</summary>
        Weekly,

        /// <summary>Bi-weekly recurrence (every 14 days).</summary>
        BiWeekly,

        /// <summary>Monthly recurrence.</summary>
        Monthly,

        /// <summary>Quarterly recurrence (every 3 months).</summary>
        Quarterly,

        /// <summary>Semi-annual recurrence (every 6 months).</summary>
        SemiAnnual,

        /// <summary>Annual recurrence (yearly).</summary>
        Annual,
    }

    /// <summary>Gets the unique identifier.</summary>
    public Guid Id
    {
        get; private set;
    }

    /// <summary>Gets the bill name.</summary>
    public string Name
    {
        get; private set;
    }

    /// <summary>Gets the bill amount.</summary>
    public MoneyValue Amount
    {
        get; private set;
    }

    /// <summary>Gets the anchor (first due date).</summary>
    public DateOnly Anchor
    {
        get; private set;
    }

    /// <summary>Gets the recurrence kind.</summary>
    public RecurrenceKind Recurrence
    {
        get; private set;
    }

    /// <summary>Gets the custom day interval (null unless recurrence is weekly or bi-weekly).</summary>
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

    /// <summary>Create a weekly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateWeekly(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.Weekly, 7);
    }

    /// <summary>Create a bi-weekly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateBiWeekly(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.BiWeekly, 14);
    }

    /// <summary>Create a monthly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateMonthly(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.Monthly);
    }

    /// <summary>Create a quarterly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateQuarterly(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.Quarterly);
    }

    /// <summary>Create a semi-annual bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateSemiAnnual(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.SemiAnnual);
    }

    /// <summary>Create an annual bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateAnnual(string name, MoneyValue amount, DateOnly anchor)
    {
        ValidateCommon(name, amount);
        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.Annual);
    }

    /// <summary>Marks the entity as updated (for future mutable operations).</summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
    }

    /// <summary>Update the bill name.</summary>
    /// <param name="name">New bill name.</param>
    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Bill name required.");
        }

        this.Name = name.Trim();
        this.MarkUpdated();
    }

    /// <summary>Update the bill amount.</summary>
    /// <param name="amount">New bill amount.</param>
    public void UpdateAmount(MoneyValue amount)
    {
        if (amount.Amount < 0m)
        {
            throw new DomainException("Bill amount cannot be negative.");
        }

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

    /// <summary>Update the recurrence type and interval.</summary>
    /// <param name="recurrence">New recurrence type.</param>
    public void UpdateRecurrence(RecurrenceKind recurrence)
    {
        this.Recurrence = recurrence;

        // Update the days interval based on recurrence type
        this.DaysInterval = recurrence switch
        {
            RecurrenceKind.Weekly => 7,
            RecurrenceKind.BiWeekly => 14,
            RecurrenceKind.Monthly => null,
            RecurrenceKind.Quarterly => null,
            RecurrenceKind.SemiAnnual => null,
            RecurrenceKind.Annual => null,
            _ => throw new DomainException($"Unsupported recurrence kind: {recurrence}"),
        };

        this.MarkUpdated();
    }

    /// <summary>Get due dates within an inclusive range.</summary>
    /// <param name="rangeStart">Start (inclusive).</param>
    /// <param name="rangeEnd">End (inclusive).</param>
    /// <returns>Sequence of due dates.</returns>
    public IEnumerable<DateOnly> GetOccurrences(DateOnly rangeStart, DateOnly rangeEnd)
    {
        if (rangeEnd < rangeStart)
        {
            throw new DomainException("Invalid range.");
        }

        if (rangeEnd < this.Anchor)
        {
            yield break;
        }

        var iterator = this.Recurrence switch
        {
            RecurrenceKind.Weekly => this.EnumerateFixedInterval(rangeStart, rangeEnd, 7),
            RecurrenceKind.BiWeekly => this.EnumerateFixedInterval(rangeStart, rangeEnd, 14),
            RecurrenceKind.Monthly => this.EnumerateMonthly(rangeStart, rangeEnd),
            RecurrenceKind.Quarterly => this.EnumerateMultiMonth(rangeStart, rangeEnd, 3),
            RecurrenceKind.SemiAnnual => this.EnumerateMultiMonth(rangeStart, rangeEnd, 6),
            RecurrenceKind.Annual => this.EnumerateMultiMonth(rangeStart, rangeEnd, 12),
            _ => throw new DomainException("Unsupported recurrence kind."),
        };

        foreach (var occurrence in iterator)
        {
            yield return occurrence;
        }
    }

    /// <summary>Common validation for all factory methods.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount.</param>
    /// <exception cref="DomainException">If validation fails.</exception>
    private static void ValidateCommon(string name, MoneyValue amount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Bill name required.");
        }

        if (amount.Amount < 0m)
        {
            throw new DomainException("Bill amount cannot be negative.");
        }
    }

    /// <summary>Advances a date by the specified number of months, clamping the day to the valid range.</summary>
    /// <param name="date">Starting date.</param>
    /// <param name="months">Number of months to advance.</param>
    /// <param name="anchorDay">Target day of month.</param>
    /// <returns>Advanced date, clamped to the valid range.</returns>
    private static DateOnly AddMonthClamped(DateOnly date, int months, int anchorDay)
    {
        var advanced = date.AddMonths(months);
        int daysInMonth = DateTime.DaysInMonth(advanced.Year, advanced.Month);
        int day = Math.Min(anchorDay, daysInMonth);
        return new DateOnly(advanced.Year, advanced.Month, day);
    }

    private IEnumerable<DateOnly> EnumerateFixedInterval(DateOnly rangeStart, DateOnly rangeEnd, int intervalDays)
    {
        var current = this.Anchor;
        while (current < rangeStart)
        {
            current = current.AddDays(intervalDays);
        }

        while (current <= rangeEnd)
        {
            if (current >= rangeStart)
            {
                yield return current;
            }

            current = current.AddDays(intervalDays);
        }
    }

    private IEnumerable<DateOnly> EnumerateMonthly(DateOnly rangeStart, DateOnly rangeEnd)
    {
        var current = this.Anchor;
        var anchorDay = this.Anchor.Day;
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

    private IEnumerable<DateOnly> EnumerateMultiMonth(DateOnly rangeStart, DateOnly rangeEnd, int monthInterval)
    {
        var current = this.Anchor;
        var anchorDay = this.Anchor.Day;
        while (current < rangeStart)
        {
            current = AddMonthClamped(current, monthInterval, anchorDay);
        }

        while (current <= rangeEnd)
        {
            if (current >= rangeStart)
            {
                yield return current;
            }

            current = AddMonthClamped(current, monthInterval, anchorDay);
        }
    }
}
