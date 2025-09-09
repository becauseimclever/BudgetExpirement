namespace BudgetExperiment.Application.BillSchedules;

using BudgetExperiment.Domain;

/// <summary>
/// Repository-backed implementation of bill schedule service.
/// </summary>
public sealed class BillScheduleService : IBillScheduleService
{
    private readonly IWriteRepository<BillSchedule> _write;
    private readonly IReadRepository<BillSchedule> _read;
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Initializes a new instance of the <see cref="BillScheduleService"/> class.
    /// </summary>
    /// <param name="write">Write repository.</param>
    /// <param name="read">Read repository.</param>
    /// <param name="uow">Unit of work.</param>
    public BillScheduleService(IWriteRepository<BillSchedule> write, IReadRepository<BillSchedule> read, IUnitOfWork uow)
    {
        this._write = write;
        this._read = read;
        this._uow = uow;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateMonthlyAsync(string name, MoneyValue amount, DateOnly anchor, CancellationToken cancellationToken = default)
    {
        var schedule = BillSchedule.CreateMonthly(name, amount, anchor);
        await this._write.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateAsync(string name, MoneyValue amount, DateOnly anchor, BillSchedule.RecurrenceKind recurrence, CancellationToken cancellationToken = default)
    {
        var schedule = recurrence switch
        {
            BillSchedule.RecurrenceKind.Weekly => BillSchedule.CreateWeekly(name, amount, anchor),
            BillSchedule.RecurrenceKind.BiWeekly => BillSchedule.CreateBiWeekly(name, amount, anchor),
            BillSchedule.RecurrenceKind.Monthly => BillSchedule.CreateMonthly(name, amount, anchor),
            BillSchedule.RecurrenceKind.Quarterly => BillSchedule.CreateQuarterly(name, amount, anchor),
            BillSchedule.RecurrenceKind.SemiAnnual => BillSchedule.CreateSemiAnnual(name, amount, anchor),
            BillSchedule.RecurrenceKind.Annual => BillSchedule.CreateAnnual(name, amount, anchor),
            _ => throw new DomainException($"Unsupported recurrence kind: {recurrence}"),
        };

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
            throw new DomainException("Bill schedule not found.");
        }

        return schedule.GetOccurrences(start, end).ToArray();
    }

    /// <inheritdoc />
    public async Task<BillScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await this._read.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule is null ? null : BillScheduleDto.FromEntity(schedule);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<BillScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
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
        return (entities.Select(BillScheduleDto.FromEntity).ToList(), total);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Guid id, string name, MoneyValue amount, DateOnly anchor, BillSchedule.RecurrenceKind recurrence, CancellationToken cancellationToken = default)
    {
        var schedule = await this._read.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return false;
        }

        // Update the entity properties
        schedule.UpdateName(name);
        schedule.UpdateAmount(amount);
        schedule.UpdateAnchor(anchor);
        schedule.UpdateRecurrence(recurrence);

        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await this._read.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule is null)
        {
            return false;
        }

        await this._write.RemoveAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}
