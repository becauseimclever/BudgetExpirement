namespace BudgetExperiment.Infrastructure.Data.Repositories;

using BudgetExperiment.Domain;

/// <summary>
/// EF Core implementation of adhoc transaction write repository.
/// </summary>
public sealed class AdhocTransactionWriteRepository : IAdhocTransactionWriteRepository
{
    private readonly BudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocTransactionWriteRepository"/> class.
    /// </summary>
    /// <param name="context">Database context.</param>
    public AdhocTransactionWriteRepository(BudgetDbContext context)
    {
        this._context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task AddAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        await this._context.AdhocTransactions.AddAsync(transaction, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task UpdateAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        this._context.AdhocTransactions.Update(transaction);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        this._context.AdhocTransactions.Remove(transaction);
        return Task.CompletedTask;
    }
}
