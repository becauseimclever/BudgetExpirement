namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;

/// <summary>
/// Repository for writing expense data.
/// </summary>
public sealed class ExpenseWriteRepository : IExpenseWriteRepository
{
    private readonly BudgetDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseWriteRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ExpenseWriteRepository(BudgetDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task AddAsync(Expense entity, CancellationToken cancellationToken = default)
    {
        await this.context.Expenses.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Adds an expense to the repository.
    /// </summary>
    /// <param name="entity">The expense to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(Expense entity)
    {
        await this.AddAsync(entity, CancellationToken.None);
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Expense entity)
    {
        this.context.Expenses.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Expense entity)
    {
        this.context.Expenses.Remove(entity);
        return Task.CompletedTask;
    }
}
