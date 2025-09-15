namespace BudgetExperiment.Infrastructure.Repositories;

using BudgetExperiment.Domain;

/// <summary>
/// EF Core write repository for RecurringSchedule aggregate.
/// </summary>
public sealed class RecurringScheduleWriteRepository : IRecurringScheduleWriteRepository
{
    private readonly BudgetDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringScheduleWriteRepository"/> class.
    /// </summary>
    /// <param name="db">EF Core DbContext.</param>
    public RecurringScheduleWriteRepository(BudgetDbContext db) => this._db = db;

    /// <inheritdoc />
    public async Task AddAsync(RecurringSchedule entity, CancellationToken cancellationToken = default)
    {
        await this._db.RecurringSchedules.AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task RemoveAsync(RecurringSchedule entity, CancellationToken cancellationToken = default)
    {
        this._db.RecurringSchedules.Remove(entity);
        return Task.CompletedTask;
    }
}
