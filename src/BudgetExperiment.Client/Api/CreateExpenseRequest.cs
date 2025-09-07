namespace BudgetExperiment.Client.Api
{
    using System;

    /// <summary>
    /// Request to create a new expense.
    /// </summary>
    public sealed record CreateExpenseRequest(
        string Description,
        string Currency,
        decimal Amount,
        DateOnly Date,
        string? Category = null);
}
