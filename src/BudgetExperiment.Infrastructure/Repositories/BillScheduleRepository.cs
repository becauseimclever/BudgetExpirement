namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// EF Core repository for BillSchedule aggregate.
/// </summary>
public sealed class BillScheduleRepository : IReadRepository<BillSchedule>, IWriteRepository<BillSchedule>
{
    private readonly BudgetDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="BillScheduleRepository"/> class.
    /// </summary>
    /// <param name="db">EF Core DbContext.</param>
    public BillScheduleRepository(BudgetDbContext db) => this._db = db;

    /// <inheritdoc />
    public async Task<BillSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await this._db.BillSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task AddAsync(BillSchedule entity, CancellationToken cancellationToken = default)
    {
        await this._db.BillSchedules.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task RemoveAsync(BillSchedule entity, CancellationToken cancellationToken = default)
    {
        this._db.BillSchedules.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BillSchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        => await this._db.BillSchedules
            .OrderBy(b => b.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await this._db.BillSchedules.LongCountAsync(cancellationToken).ConfigureAwait(false);
}
