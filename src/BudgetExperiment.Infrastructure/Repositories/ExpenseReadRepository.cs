namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository for reading expense data.
/// </summary>
public sealed class ExpenseReadRepository : IExpenseReadRepository
{
    private readonly BudgetDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpenseReadRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ExpenseReadRepository(BudgetDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc/>
    public async Task<Expense?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Expenses.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Expense>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await this.context.Expenses
            .OrderBy(e => e.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await this.context.Expenses.CountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets an expense by its ID with no cancellation token.
    /// </summary>
    /// <param name="id">The expense ID.</param>
    /// <returns>The expense if found, otherwise null.</returns>
    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await this.GetByIdAsync(id, CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Expense>> GetByDateAsync(DateOnly date)
    {
        return await this.context.Expenses
            .Where(e => e.Date == date)
            .OrderBy(e => e.CreatedUtc)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Expense>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await this.context.Expenses
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.CreatedUtc)
            .ToListAsync();
    }
}
