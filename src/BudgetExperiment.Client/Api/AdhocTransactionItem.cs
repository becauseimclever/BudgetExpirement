namespace BudgetExperiment.Client.Api;

using TransactionType = BudgetExperiment.Domain.TransactionType;

/// <summary>
/// Unified adhoc transaction data from API (replaces ExpenseItem and AdhocPaymentItem).
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="Description">Description of the transaction.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (positive for income, negative for expenses).</param>
/// <param name="Date">Date of the transaction.</param>
/// <param name="Category">Optional category.</param>
/// <param name="TransactionType">Type of transaction (Income or Expense).</param>
/// <param name="CreatedUtc">When this record was created.</param>
/// <param name="UpdatedUtc">When this record was last updated.</param>
public sealed record AdhocTransactionItem(
    Guid Id,
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category,
    TransactionType TransactionType,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc)
{
    /// <summary>Gets a value indicating whether this is an income transaction.</summary>
    public bool IsIncome => this.TransactionType == TransactionType.Income;

    /// <summary>Gets a value indicating whether this is an expense transaction.</summary>
    public bool IsExpense => this.TransactionType == TransactionType.Expense;

    /// <summary>Gets the display amount (always positive for display purposes).</summary>
    public decimal DisplayAmount => Math.Abs(this.Amount);

    /// <summary>Gets the amount prefix for display (+ for income, - for expenses).</summary>
    public string AmountPrefix => this.IsIncome ? "+" : "-";

    /// <summary>Gets the formatted amount with prefix for display.</summary>
    public string FormattedAmount => $"{this.AmountPrefix}${this.DisplayAmount:N0}";
}
