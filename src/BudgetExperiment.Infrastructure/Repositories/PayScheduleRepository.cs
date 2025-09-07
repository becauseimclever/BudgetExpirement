namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// EF Core repository for PaySchedule aggregate.
/// </summary>
public sealed class PayScheduleRepository : IReadRepository<PaySchedule>, IWriteRepository<PaySchedule>
{
    private readonly BudgetDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="PayScheduleRepository"/> class.
    /// </summary>
    /// <param name="db">EF Core DbContext.</param>
    public PayScheduleRepository(BudgetDbContext db) => this._db = db;

    /// <inheritdoc />
    public async Task<PaySchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await this._db.PaySchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task AddAsync(PaySchedule entity, CancellationToken cancellationToken = default)
    {
        await this._db.PaySchedules.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PaySchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        => await this._db.PaySchedules
            .OrderBy(p => p.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await this._db.PaySchedules.LongCountAsync(cancellationToken).ConfigureAwait(false);
}
