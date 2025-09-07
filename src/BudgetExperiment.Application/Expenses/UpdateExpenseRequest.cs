namespace BudgetExperiment.Application.Expenses;

/// <summary>
/// DTO for updating an existing expense.
/// </summary>
public sealed record UpdateExpenseRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
