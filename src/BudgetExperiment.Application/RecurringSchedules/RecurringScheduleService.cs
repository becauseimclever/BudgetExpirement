using BudgetExperiment.Domain;

namespace BudgetExperiment.Application.RecurringSchedules;

/// <summary>
/// Application service implementation for managing recurring schedules.
/// Replaces both PayScheduleService and BillScheduleService.
/// </summary>
public sealed class RecurringScheduleService : IRecurringScheduleService
{
    private readonly IRecurringScheduleReadRepository _readRepository;
    private readonly IRecurringScheduleWriteRepository _writeRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringScheduleService"/> class.
    /// </summary>
    /// <param name="readRepository">Read repository.</param>
    /// <param name="writeRepository">Write repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public RecurringScheduleService(
        IRecurringScheduleReadRepository readRepository,
        IRecurringScheduleWriteRepository writeRepository,
        IUnitOfWork unitOfWork)
    {
        this._readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        this._writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    // Income schedule creation methods

    /// <inheritdoc />
    public async Task<Guid> CreateWeeklyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateWeeklyIncome(anchor, amount, name);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateMonthlyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateMonthlyIncome(anchor, amount, name);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateBiWeeklyIncomeAsync(DateOnly anchor, MoneyValue amount, string? name = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateBiWeeklyIncome(anchor, amount, name);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateCustomIncomeAsync(DateOnly anchor, MoneyValue amount, int intervalDays, string? name = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateCustomIncome(anchor, amount, intervalDays, name);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateIncomeAsync(DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null, string? name = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateIncome(anchor, amount, recurrence, customIntervalDays, name);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    // Expense schedule creation methods

    /// <inheritdoc />
    public async Task<Guid> CreateWeeklyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateWeeklyExpense(name, anchor, amount);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateMonthlyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateMonthlyExpense(name, anchor, amount);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateBiWeeklyExpenseAsync(string name, DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateBiWeeklyExpense(name, anchor, amount);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateCustomExpenseAsync(string name, DateOnly anchor, MoneyValue amount, int intervalDays, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateCustomExpense(name, anchor, amount, intervalDays);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    /// <inheritdoc />
    public async Task<Guid> CreateExpenseAsync(string name, DateOnly anchor, MoneyValue amount, RecurrencePattern recurrence, int? customIntervalDays = null, CancellationToken cancellationToken = default)
    {
        var schedule = RecurringSchedule.CreateExpense(name, anchor, amount, recurrence, customIntervalDays);
        await this._writeRepository.AddAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return schedule.Id;
    }

    // Query methods

    /// <inheritdoc />
    public async Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
    {
        var schedule = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule?.GetOccurrences(start, end) ?? Enumerable.Empty<DateOnly>();
    }

    /// <inheritdoc />
    public async Task<RecurringScheduleDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return schedule != null ? RecurringScheduleDto.FromEntity(schedule) : null;
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var skip = (page - 1) * pageSize;
        var items = await this._readRepository.ListAsync(skip, pageSize, cancellationToken).ConfigureAwait(false);
        var total = await this._readRepository.CountAsync(cancellationToken).ConfigureAwait(false);
        var dtos = items.Select(RecurringScheduleDto.FromEntity).ToList();
        return (dtos, total);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListIncomeSchedulesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, total) = await this._readRepository.GetIncomeSchedulesAsync(page, pageSize).ConfigureAwait(false);
        var dtos = items.Select(RecurringScheduleDto.FromEntity).ToList();
        return (dtos, (long)total);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<RecurringScheduleDto> Items, long Total)> ListExpenseSchedulesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var (items, total) = await this._readRepository.GetExpenseSchedulesAsync(page, pageSize).ConfigureAwait(false);
        var dtos = items.Select(RecurringScheduleDto.FromEntity).ToList();
        return (dtos, (long)total);
    }

    // Update and delete methods

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Guid id, string? name, MoneyValue amount, DateOnly anchor, RecurrencePattern recurrence, int? customIntervalDays = null, CancellationToken cancellationToken = default)
    {
        var schedule = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule == null)
        {
            return false;
        }

        schedule.UpdateName(name);
        schedule.UpdateAmount(amount);
        schedule.UpdateAnchor(anchor);
        schedule.UpdateRecurrence(recurrence, customIntervalDays);

        // Entity is already being tracked by EF Core, just save changes
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var schedule = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (schedule == null)
        {
            return false;
        }

        await this._writeRepository.RemoveAsync(schedule, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    // Autocomplete support methods

    /// <inheritdoc />
    public async Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(string? searchTerm = null, int maxResults = 10, CancellationToken cancellationToken = default)
    {
        // Enforce max limit
        if (maxResults > 50)
        {
            maxResults = 50;
        }

        if (maxResults < 1)
        {
            maxResults = 10;
        }

        return await this._readRepository.GetDistinctDescriptionsAsync(searchTerm, maxResults, cancellationToken).ConfigureAwait(false);
    }
}

