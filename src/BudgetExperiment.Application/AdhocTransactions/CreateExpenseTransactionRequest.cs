namespace BudgetExperiment.Application.AdhocTransactions;

/// <summary>
/// Request to create an expense transaction.
/// </summary>
/// <param name="Description">Description of the expense.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (positive, will be stored as negative).</param>
/// <param name="Date">Date of the expense.</param>
/// <param name="Category">Optional category.</param>
public sealed record CreateExpenseTransactionRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
