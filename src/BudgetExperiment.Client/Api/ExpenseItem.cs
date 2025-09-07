namespace BudgetExperiment.Client.Api
{
    using System;

    /// <summary>
    /// Expense data from API.
    /// </summary>
    public sealed record ExpenseItem(
        Guid Id,
        string Description,
        string Currency,
        decimal Amount,
        DateOnly Date,
        string? Category,
        DateTime CreatedUtc,
        DateTime? UpdatedUtc);
}
