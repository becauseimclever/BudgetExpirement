namespace BudgetExperiment.Client.Api;

/// <summary>
/// Request to update an existing expense.
/// </summary>
/// <param name="Description">Description of the expense.</param>
/// <param name="Currency">Currency code.</param>
/// <param name="Amount">Amount of the expense.</param>
/// <param name="Date">Date of the expense.</param>
/// <param name="Category">Optional category.</param>
public sealed record UpdateExpenseRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
