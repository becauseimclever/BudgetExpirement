namespace BudgetExperiment.Domain;

/// <summary>
/// Repository interface for writing expense data.
/// </summary>
public interface IExpenseWriteRepository : IWriteRepository<Expense>
{
    /// <summary>
    /// Updates an existing expense.
    /// </summary>
    /// <param name="entity">The expense to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Expense entity);

    /// <summary>
    /// Deletes an existing expense.
    /// </summary>
    /// <param name="entity">The expense to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Expense entity);
}
