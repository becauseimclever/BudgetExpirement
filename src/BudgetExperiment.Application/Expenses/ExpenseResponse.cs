namespace BudgetExperiment.Application.Expenses;

/// <summary>
/// DTO for expense responses.
/// </summary>
public sealed record ExpenseResponse(
    Guid Id,
    string Description,
    string Currency,
    decimal Amount,
    DateOnly Date,
    string? Category,
    DateTime CreatedUtc,
    DateTime? UpdatedUtc);
