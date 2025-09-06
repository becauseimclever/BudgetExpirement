namespace BudgetExperiment.Application.PaySchedules;

using BudgetExperiment.Domain;

/// <inheritdoc />
public sealed class InMemoryPayScheduleService : IPayScheduleService
{
    private readonly Dictionary<Guid, PaySchedule> _store = new();

    /// <inheritdoc />
    public Guid CreateWeekly(DateOnly anchor)
    {
        var schedule = PaySchedule.CreateWeekly(anchor);
        var id = Guid.NewGuid();
        this._store[id] = schedule;
        return id;
    }

    /// <inheritdoc />
    public Guid CreateMonthly(DateOnly anchor)
    {
        var schedule = PaySchedule.CreateMonthly(anchor);
        var id = Guid.NewGuid();
        this._store[id] = schedule;
        return id;
    }

    /// <inheritdoc />
    public IEnumerable<DateOnly> GetOccurrences(Guid id, DateOnly start, DateOnly end)
    {
        if (!this._store.TryGetValue(id, out var schedule))
        {
            throw new DomainException("Schedule not found.");
        }

        return schedule.GetOccurrences(start, end);
    }
}
