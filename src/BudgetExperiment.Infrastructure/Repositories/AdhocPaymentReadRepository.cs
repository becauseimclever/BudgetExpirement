namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository for reading AdhocPayment entities.
/// </summary>
public sealed class AdhocPaymentReadRepository : IAdhocPaymentReadRepository
{
    private readonly BudgetDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdhocPaymentReadRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public AdhocPaymentReadRepository(BudgetDbContext context)
    {
        this._context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<AdhocPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this._context.AdhocPayments.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AdhocPayment>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await this._context.AdhocPayments
            .OrderBy(ap => ap.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
    {
        return await this._context.AdhocPayments.LongCountAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all adhoc payments.
    /// </summary>
    /// <returns>All adhoc payments.</returns>
    public async Task<IReadOnlyList<AdhocPayment>> GetAllAsync()
    {
        return await this._context.AdhocPayments.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<AdhocPayment>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        return await this._context.AdhocPayments
            .Where(ap => ap.Date >= startDate && ap.Date <= endDate)
            .OrderBy(ap => ap.Date)
            .ToListAsync();
    }
}
