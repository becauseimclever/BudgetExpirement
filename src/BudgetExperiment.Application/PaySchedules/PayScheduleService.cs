namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Repository-backed implementation of pay schedule service.
/// </summary>
public sealed class PayScheduleService : IPayScheduleService
{
    private readonly IWriteRepository<PaySchedule> _write;
    private readonly IReadRepository<PaySchedule> _read;
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Initializes a new instance of the <see cref="PayScheduleService"/> class.
    /// </summary>
    /// <param name="write">Write repository.</param>
    /// <param name="read">Read repository.</param>
    /// <param name="uow">Unit of work.</param>
    public PayScheduleService(IWriteRepository<PaySchedule> write, IReadRepository<PaySchedule> read, IUnitOfWork uow)
    {
        this._write = write;
        this._read = read;
        this._uow = uow;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = PaySchedule.CreateWeekly(anchor, amount);
        await this._write.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateMonthlyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = PaySchedule.CreateMonthly(anchor, amount);
        await this._write.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateBiWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = PaySchedule.CreateBiWeekly(anchor, amount);
        await this._write.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateCustomAsync(DateOnly anchor, MoneyValue amount, int intervalDays, CancellationToken cancellationToken = default)
    {
        var schedule = PaySchedule.CreateCustom(anchor, amount, intervalDays);
        await this._write.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
    {
        var schedule = await this._read.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            throw new DomainException("Schedule not found.");
        }

        return schedule.GetOccurrences(start, end).ToArray();
    }

    /// <inheritdoc />
    public async Task<PayScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await this._read.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule is null ? null : PayScheduleDto.FromEntity(schedule);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<PayScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        int skip = (page - 1) * pageSize;
        var entities = await this._read.ListAsync(skip, pageSize, cancellationToken).ConfigureAwait(false);
        var total = await this._read.CountAsync(cancellationToken).ConfigureAwait(false);
        return (entities.Select(PayScheduleDto.FromEntity).ToList(), total);
    }
}
