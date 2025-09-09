namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;

/// <summary>
/// Repository for writing AdhocPayment entities.
/// </summary>
public sealed class AdhocPaymentWriteRepository : IAdhocPaymentWriteRepository
{
    private readonly BudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPaymentWriteRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AdhocPaymentWriteRepository(BudgetDbContext context)
    {
        this._context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds an adhoc payment to the context.
    /// </summary>
    /// <param name="entity">The adhoc payment entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(AdhocPayment entity, CancellationToken cancellationToken = default)
    {
        await this._context.AdhocPayments.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public Task RemoveAsync(AdhocPayment entity, CancellationToken cancellationToken = default)
    {
        this._context.AdhocPayments.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates an adhoc payment in the context.
    /// </summary>
    /// <param name="entity">The adhoc payment entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateAsync(AdhocPayment entity)
    {
        this._context.AdhocPayments.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes an adhoc payment from the context.
    /// </summary>
    /// <param name="entity">The adhoc payment entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(AdhocPayment entity)
    {
        this._context.AdhocPayments.Remove(entity);
        return Task.CompletedTask;
    }
}
