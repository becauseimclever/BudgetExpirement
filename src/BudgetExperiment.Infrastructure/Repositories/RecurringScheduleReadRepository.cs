namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// EF Core read repository for RecurringSchedule aggregate.
/// </summary>
public sealed class RecurringScheduleReadRepository : IRecurringScheduleReadRepository
{
    private readonly BudgetDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringScheduleReadRepository"/> class.
    /// </summary>
    /// <param name="db">EF Core DbContext.</param>
    public RecurringScheduleReadRepository(BudgetDbContext db) => this._db = db;

    /// <inheritdoc />
    public async Task<RecurringSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await this._db.RecurringSchedules.FirstOrDefaultAsync(x => x.Id == id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<IReadOnlyList<RecurringSchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        => await this._db.RecurringSchedules
            .OrderBy(s => s.CreatedUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<long> CountAsync(CancellationToken cancellationToken = default)
        => await this._db.RecurringSchedules.LongCountAsync(cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetIncomeSchedulesAsync(int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = this._db.RecurringSchedules
            .Where(s => s.ScheduleType == ScheduleType.Income)
            .OrderBy(s => s.CreatedUtc);

        var totalCount = await query.CountAsync().ConfigureAwait(false);
        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetExpenseSchedulesAsync(int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = this._db.RecurringSchedules
            .Where(s => s.ScheduleType == ScheduleType.Expense)
            .OrderBy(s => s.CreatedUtc);

        var totalCount = await query.CountAsync().ConfigureAwait(false);
        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<RecurringSchedule> Items, int TotalCount)> GetSchedulesByTypeAsync(ScheduleType scheduleType, int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;

        var query = this._db.RecurringSchedules
            .Where(s => s.ScheduleType == scheduleType)
            .OrderBy(s => s.CreatedUtc);

        var totalCount = await query.CountAsync().ConfigureAwait(false);
        var items = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync()
            .ConfigureAwait(false);

        return (items, totalCount);
    }
}
