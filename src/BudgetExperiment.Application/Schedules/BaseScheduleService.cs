namespace BudgetExperiment.Application.Schedules;

using BudgetExperiment.Domain;

/// <summary>
/// Base implementation for schedule services providing common functionality.
/// </summary>
/// <typeparam name="TEntity">The schedule entity type.</typeparam>
/// <typeparam name="TDto">The DTO type.</typeparam>
public abstract class BaseScheduleService<TEntity, TDto> : IScheduleService<TEntity, TDto>
    where TEntity : class, ISchedule
    where TDto : class
{
    private readonly IWriteRepository<TEntity> _writeRepository;
    private readonly IReadRepository<TEntity> _readRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseScheduleService{TEntity, TDto}"/> class.
    /// </summary>
    /// <param name="writeRepository">Write repository.</param>
    /// <param name="readRepository">Read repository.</param>
    /// <param name="unitOfWork">Unit of work.</param>
    protected BaseScheduleService(
        IWriteRepository<TEntity> writeRepository,
        IReadRepository<TEntity> readRepository,
        IUnitOfWork unitOfWork)
    {
        this._writeRepository = writeRepository ?? throw new ArgumentNullException(nameof(writeRepository));
        this._readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
        this._unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc />
    public virtual async Task<Guid> CreateAsync(DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays = null, CancellationToken cancellationToken = default)
    {
        var entity = this.CreateEntity(anchor, amount, pattern, customIntervalDays);
        await this._writeRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return entity.Id;
    }

    /// <inheritdoc />
    public virtual async Task<TDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return entity is null ? null : this.EntityToDto(entity);
    }

    /// <inheritdoc />
    public virtual async Task<(IReadOnlyList<TDto> Items, long Total)> ListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
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
        var entities = await this._readRepository.ListAsync(skip, pageSize, cancellationToken).ConfigureAwait(false);
        var total = await this._readRepository.CountAsync(cancellationToken).ConfigureAwait(false);
        return (entities.Select(this.EntityToDto).ToList(), total);
    }

    /// <inheritdoc />
    public virtual async Task<IEnumerable<DateOnly>> GetOccurrencesAsync(Guid id, DateOnly start, DateOnly end, CancellationToken cancellationToken = default)
    {
        var entity = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            throw new DomainException("Schedule not found.");
        }

        return entity.GetOccurrences(start, end).ToArray();
    }

    /// <inheritdoc />
    public virtual async Task<bool> UpdateAsync(Guid id, DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays = null, CancellationToken cancellationToken = default)
    {
        var entity = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return false;
        }

        this.UpdateEntity(entity, anchor, amount, pattern, customIntervalDays);
        entity.MarkUpdated();

        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await this._readRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return false;
        }

        await this._writeRepository.RemoveAsync(entity, cancellationToken).ConfigureAwait(false);
        await this._unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }

    /// <summary>
    /// Create a new entity instance. Derived classes must implement this factory method.
    /// </summary>
    /// <param name="anchor">Anchor date.</param>
    /// <param name="amount">Amount.</param>
    /// <param name="pattern">Recurrence pattern.</param>
    /// <param name="customIntervalDays">Custom interval days.</param>
    /// <returns>New entity instance.</returns>
    protected abstract TEntity CreateEntity(DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays);

    /// <summary>
    /// Convert entity to DTO. Derived classes must implement this conversion method.
    /// </summary>
    /// <param name="entity">Entity to convert.</param>
    /// <returns>DTO representation.</returns>
    protected abstract TDto EntityToDto(TEntity entity);

    /// <summary>
    /// Update an entity with new values. Derived classes must implement this update method.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <param name="anchor">New anchor date.</param>
    /// <param name="amount">New amount.</param>
    /// <param name="pattern">New recurrence pattern.</param>
    /// <param name="customIntervalDays">New custom interval days.</param>
    protected abstract void UpdateEntity(TEntity entity, DateOnly anchor, MoneyValue amount, RecurrencePattern pattern, int? customIntervalDays);
}
