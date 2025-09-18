namespace BudgetExperiment.Client.Api;

/// <summary>
/// Unified recurring schedule item data from API (replaces PayScheduleItem and BillScheduleItem).
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="Name">The schedule name (required for expenses, optional for income).</param>
/// <param name="Anchor">The anchor date.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (positive for income, negative for expenses).</param>
/// <param name="Recurrence">Recurrence pattern.</param>
/// <param name="ScheduleType">Type of schedule (Income or Expense).</param>
/// <param name="DaysInterval">Custom interval days (null if pattern not custom).</param>
/// <param name="CreatedUtc">When this record was created.</param>
/// <param name="UpdatedUtc">When this record was last updated.</param>
public sealed record RecurringScheduleItem(
    Guid Id,
    string? Name,
    DateOnly Anchor,
    string Currency,
    decimal Amount,
    RecurrencePattern Recurrence,
    ScheduleType ScheduleType,
    int? DaysInterval,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc)
{
    /// <summary>Gets a value indicating whether this is an income schedule.</summary>
    public bool IsIncome => this.ScheduleType == ScheduleType.Income;

    /// <summary>Gets a value indicating whether this is an expense schedule.</summary>
    public bool IsExpense => this.ScheduleType == ScheduleType.Expense;

    /// <summary>Gets the display name for this schedule.</summary>
    public string DisplayName => this.IsExpense && !string.IsNullOrWhiteSpace(this.Name)
        ? this.Name
        : $"{this.Recurrence} {this.ScheduleType}";

    /// <summary>Gets the display amount (always positive for display purposes).</summary>
    public decimal DisplayAmount => Math.Abs(this.Amount);

    /// <summary>Gets the amount prefix for display (+ for income, - for expenses).</summary>
    public string AmountPrefix => this.IsIncome ? "+" : "-";

    /// <summary>Gets the formatted amount with prefix for display.</summary>
    public string FormattedAmount => $"{this.AmountPrefix}${this.DisplayAmount:N0}";
}
