namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Application.Schedules;
using BudgetExperiment.Domain;

/// <summary>
/// Modern pay schedule service using the unified schedule service pattern.
/// </summary>
public sealed class ModernPayScheduleService : BaseScheduleService<PaySchedule, PayScheduleDto>, IPayScheduleService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModernPayScheduleService"/> class.
    /// </summary>
    /// <param name="writeRepository">Write repository.</param>
    /// <param name="readRepository">Read repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    public ModernPayScheduleService(
        IWriteRepository<PaySchedule> writeRepository,
        IReadRepository<PaySchedule> readRepository,
        IUnitOfWork unitOfWork)
        : base(writeRepository, readRepository, unitOfWork)
    {
    }

    // Legacy interface support methods - delegate to base unified methods

    /// <inheritdoc />
    public Task<Guid> CreateWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        return this.CreateAsync(anchor, amount, RecurrencePattern.Weekly, null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Guid> CreateMonthlyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        return this.CreateAsync(anchor, amount, RecurrencePattern.Monthly, null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Guid> CreateBiWeeklyAsync(DateOnly anchor, MoneyValue amount, CancellationToken cancellationToken = default)
    {
        return this.CreateAsync(anchor, amount, RecurrencePattern.BiWeekly, null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Guid> CreateCustomAsync(DateOnly anchor, MoneyValue amount, int intervalDays, CancellationToken cancellationToken = default)
    {
        return this.CreateAsync(anchor, amount, RecurrencePattern.Custom, intervalDays, cancellationToken);
    }

    /// <inheritdoc />
    protected override PaySchedule CreateEntity(DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays)
    {
        return PaySchedule.Create(anchor, amount, pattern, customIntervalDays);
    }

    /// <inheritdoc />
    protected override PayScheduleDto EntityToDto(PaySchedule entity)
    {
        return PayScheduleDto.FromEntity(entity);
    }

    /// <inheritdoc />
    protected override void UpdateEntity(PaySchedule entity, DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays)
    {
        entity.UpdateAnchor(anchor);
        entity.UpdateAmount(amount);
        entity.UpdateRecurrence(pattern, customIntervalDays);
    }
}
