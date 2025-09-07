namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a recurring bill with amount and monthly recurrence (for now).
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
    }

    private BillSchedule(Guid id, string name, MoneyValue amount, DateOnly anchor, RecurrenceKind recurrence)
    {
        this.Id = id;
        this.Name = name;
        this.Amount = amount;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>Recurrence kinds supported.</summary>
    public enum RecurrenceKind
    {
        /// <summary>Monthly recurrence.</summary>
        Monthly,
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

    /// <summary>Create a monthly bill schedule.</summary>
    /// <param name="name">Bill name.</param>
    /// <param name="amount">Bill amount (non-negative).</param>
    /// <param name="anchor">First due date.</param>
    /// <returns>Configured <see cref="BillSchedule"/>.</returns>
    public static BillSchedule CreateMonthly(string name, MoneyValue amount, DateOnly anchor)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Bill name required.");
        }

        if (amount.Amount < 0m)
        {
            throw new DomainException("Bill amount cannot be negative.");
        }

        return new BillSchedule(Guid.NewGuid(), name.Trim(), amount, anchor, RecurrenceKind.Monthly);
    }

    /// <summary>Marks the entity as updated (for future mutable operations).</summary>
    public void MarkUpdated()
    {
        this.UpdatedUtc = DateTime.UtcNow;
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

        if (this.Recurrence == RecurrenceKind.Monthly)
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
    }

    private static DateOnly AddMonthClamped(DateOnly date, int months, int anchorDay)
    {
        var advanced = date.AddMonths(months);
        int daysInMonth = DateTime.DaysInMonth(advanced.Year, advanced.Month);
        int day = Math.Min(anchorDay, daysInMonth);
        return new DateOnly(advanced.Year, advanced.Month, day);
    }
}
