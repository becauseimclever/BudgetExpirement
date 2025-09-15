namespace BudgetExperiment.Domain;

/// <summary>
/// Represents a unified recurring schedule that can handle both income (positive amounts) and expenses (negative amounts).
/// Replaces both PaySchedule and BillSchedule entities.
/// </summary>
public sealed class RecurringSchedule : ISchedule
{
    private RecurringSchedule()
    {
        // For EF Core
        this.Id = Guid.Empty;
        this.Name = string.Empty;
        this.Anchor = default;
        this.Recurrence = RecurrencePattern.Weekly;
        this.Amount = MoneyValue.Zero("USD");
        this.ScheduleType = ScheduleType.Income;
    }

    private RecurringSchedule(Guid id, string? name, DateOnly anchor, RecurrencePattern recurrence, MoneyValue amount, ScheduleType scheduleType, int? daysInterval)
    {
        this.Id = id;
        this.Name = name?.Trim() ?? string.Empty;
        this.Anchor = anchor;
        this.Recurrence = recurrence;
        this.Amount = amount;
        this.ScheduleType = scheduleType;
        this.DaysInterval = daysInterval;
        this.CreatedUtc = DateTime.UtcNow;
    }

    /// <summary>Gets the unique identifier.</summary>
    public Guid Id
    {
        get; private set;
    }

    /// <summary>Gets the schedule name (optional, primarily used for expense schedules).</summary>
    public string Name
    {
        get; private set;
    }

    /// <summary>Gets the anchor date (first occurrence).</summary>
    public DateOnly Anchor
    {
        get; private set;
    }

    /// <summary>Gets the recurrence pattern.</summary>
    public RecurrencePattern Recurrence
    {
        get; private set;
    }

    /// <summary>
    /// Gets the schedule amount.
    /// Convention: Positive for income, negative for expenses.
    /// </summary>
    public MoneyValue Amount
    {
        get; private set;
    }

    /// <summary>Gets the schedule type for categorization.</summary>
    public ScheduleType ScheduleType
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

    /// <summary>Create an income schedule (positive amount).</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Income amount (will be made positive if negative).</param>
    /// <param name="recurrence">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <returns>Configured income schedule.</returns>
    public static RecurringSchedule CreateIncome(DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null, string? name = null)
    {
        ValidateRecurrencePattern(recurrence, customIntervalDays);

        // Ensure amount is positive for income
        var positiveAmount = amount.Amount < 0 ? MoneyValue.Create(amount.Currency, Math.Abs(amount.Amount)) : amount;

        return new RecurringSchedule(Guid.NewGuid(), name, anchor, recurrence, positiveAmount, ScheduleType.Income, customIntervalDays);
    }

    /// <summary>Create an expense schedule (negative amount).</summary>
    /// <param name="name">Name for the expense (required for expenses).</param>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Expense amount (will be made negative if positive).</param>
    /// <param name="recurrence">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days (required for Custom pattern).</param>
    /// <returns>Configured expense schedule.</returns>
    public static RecurringSchedule CreateExpense(string name, DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Expense name is required.");
        }

        ValidateRecurrencePattern(recurrence, customIntervalDays);

        // Ensure amount is negative for expenses
        var negativeAmount = amount.Amount > 0 ? MoneyValue.Create(amount.Currency, -amount.Amount) : amount;

        return new RecurringSchedule(Guid.NewGuid(), name, anchor, recurrence, negativeAmount, ScheduleType.Expense, customIntervalDays);
    }

    /// <summary>Create a weekly income schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Income amount.</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <returns>Configured schedule.</returns>
    public static RecurringSchedule CreateWeeklyIncome(DateOnly anchor, MoneyValue amount, string? name = null)
        => CreateIncome(anchor, amount, RecurrencePattern.Weekly, null, name);

    /// <summary>Create a monthly income schedule.</summary>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Income amount.</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateMonthlyIncome(DateOnly anchor, MoneyValue amount, string? name = null)
        => CreateIncome(anchor, amount, RecurrencePattern.Monthly, null, name);

    /// <summary>Create a bi-weekly (14 day) income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount.</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateBiWeeklyIncome(DateOnly anchor, MoneyValue amount, string? name = null)
        => CreateIncome(anchor, amount, RecurrencePattern.BiWeekly, null, name);

    /// <summary>Create a weekly expense schedule.</summary>
    /// <param name="name">Expense name.</param>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Expense amount.</param>
    /// <returns>Configured schedule.</returns>
    public static RecurringSchedule CreateWeeklyExpense(string name, DateOnly anchor, MoneyValue amount)
        => CreateExpense(name, anchor, amount, RecurrencePattern.Weekly);

    /// <summary>Create a monthly expense schedule.</summary>
    /// <param name="name">Expense name.</param>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Expense amount.</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateMonthlyExpense(string name, DateOnly anchor, MoneyValue amount)
        => CreateExpense(name, anchor, amount, RecurrencePattern.Monthly);

    /// <summary>Create a bi-weekly expense schedule.</summary>
    /// <param name="name">Expense name.</param>
    /// <param name="anchor">Anchor (first occurrence) date.</param>
    /// <param name="amount">Expense amount.</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateBiWeeklyExpense(string name, DateOnly anchor, MoneyValue amount)
        => CreateExpense(name, anchor, amount, RecurrencePattern.BiWeekly);

    /// <summary>Create a custom fixed-interval income schedule.</summary>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Income amount.</param>
    /// <param name="intervalDays">Interval in days (>= 1).</param>
    /// <param name="name">Optional name for the income schedule.</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateCustomIncome(DateOnly anchor, MoneyValue amount, int intervalDays, string? name = null)
    {
        if (intervalDays < 1)
        {
            throw new DomainException("Interval must be >= 1 day.");
        }

        if (intervalDays == 7)
        {
            return CreateWeeklyIncome(anchor, amount, name);
        }

        if (intervalDays == 14)
        {
            return CreateBiWeeklyIncome(anchor, amount, name);
        }

        return CreateIncome(anchor, amount, RecurrencePattern.Custom, intervalDays, name);
    }

    /// <summary>Create a custom fixed-interval expense schedule.</summary>
    /// <param name="name">Expense name.</param>
    /// <param name="anchor">First occurrence.</param>
    /// <param name="amount">Expense amount.</param>
    /// <param name="intervalDays">Interval in days (>= 1).</param>
    /// <returns>Schedule.</returns>
    public static RecurringSchedule CreateCustomExpense(string name, DateOnly anchor, MoneyValue amount, int intervalDays)
    {
        if (intervalDays < 1)
        {
            throw new DomainException("Interval must be >= 1 day.");
        }

        if (intervalDays == 7)
        {
            return CreateWeeklyExpense(name, anchor, amount);
        }

        if (intervalDays == 14)
        {
            return CreateBiWeeklyExpense(name, anchor, amount);
        }

        return CreateExpense(name, anchor, amount, RecurrencePattern.Custom, intervalDays);
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

    /// <summary>Update the schedule name.</summary>
    /// <param name="name">New name (can be null or empty for income schedules).</param>
    public void UpdateName(string? name)
    {
        if (this.ScheduleType == ScheduleType.Expense && string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Expense schedules require a name.");
        }

        this.Name = name?.Trim() ?? string.Empty;
        this.MarkUpdated();
    }

    /// <summary>Update the schedule amount.</summary>
    /// <param name="amount">New amount.</param>
    public void UpdateAmount(MoneyValue amount)
    {
        ArgumentNullException.ThrowIfNull(amount);

        // Enforce sign convention based on schedule type
        if (this.ScheduleType == ScheduleType.Income && amount.Amount < 0)
        {
            this.Amount = MoneyValue.Create(amount.Currency, Math.Abs(amount.Amount));
        }
        else if (this.ScheduleType == ScheduleType.Expense && amount.Amount > 0)
        {
            this.Amount = MoneyValue.Create(amount.Currency, -amount.Amount);
        }
        else
        {
            this.Amount = amount;
        }

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
        ValidateRecurrencePattern(pattern, customIntervalDays);

        this.Recurrence = pattern;
        this.DaysInterval = pattern == RecurrencePattern.Custom ? customIntervalDays : null;
        this.MarkUpdated();
    }

    /// <summary>Change the schedule type and adjust amount sign accordingly.</summary>
    /// <param name="newType">New schedule type.</param>
    public void ChangeScheduleType(ScheduleType newType)
    {
        if (newType == ScheduleType.Expense && string.IsNullOrWhiteSpace(this.Name))
        {
            throw new DomainException("Cannot change to expense type without a name. Set a name first.");
        }

        this.ScheduleType = newType;

        // Adjust amount sign based on new type
        if (newType == ScheduleType.Income && this.Amount.Amount < 0)
        {
            this.Amount = MoneyValue.Create(this.Amount.Currency, Math.Abs(this.Amount.Amount));
        }
        else if (newType == ScheduleType.Expense && this.Amount.Amount > 0)
        {
            this.Amount = MoneyValue.Create(this.Amount.Currency, -this.Amount.Amount);
        }

        this.MarkUpdated();
    }

    private static void ValidateRecurrencePattern(RecurrencePattern pattern, int? customIntervalDays)
    {
        if (pattern == RecurrencePattern.Custom && (!customIntervalDays.HasValue || customIntervalDays.Value <= 0))
        {
            throw new DomainException("Custom recurrence pattern requires a valid interval in days.");
        }
    }
}
