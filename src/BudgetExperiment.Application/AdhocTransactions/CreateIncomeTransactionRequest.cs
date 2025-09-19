namespace BudgetExperiment.Application.AdhocTransactions;

/// <summary>
/// Request to create an income transaction.
/// </summary>
/// <param name="Description">Description of the income.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount (positive).</param>
/// <param name="Date">Date of the income.</param>
/// <param name="Category">Optional category.</param>
public sealed record CreateIncomeTransactionRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
