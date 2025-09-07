namespace BudgetExperiment.Application.Expenses;

/// <summary>
/// DTO for creating a new expense.
/// </summary>
public sealed record CreateExpenseRequest(
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category = null);
