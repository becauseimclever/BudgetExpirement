namespace BudgetExperiment.Application.AdhocTransactions;

/// <summary>
/// Request to update an adhoc transaction.
/// </summary>
/// <param name="Description">Updated description.</param>
/// <param name="Currency">Updated currency code.</param>
/// <param name="Amount">Updated amount.</param>
/// <param name="Date">Updated date.</param>
/// <param name="Category">Updated category.</param>
public sealed record UpdateAdhocTransactionRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
